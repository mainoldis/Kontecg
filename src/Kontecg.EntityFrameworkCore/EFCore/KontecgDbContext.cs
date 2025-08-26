using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Kontecg.Collections.Extensions;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.EFCore.Configuration;
using Kontecg.EFCore.Extensions;
using Kontecg.EFCore.Utils;
using Kontecg.EFCore.ValueConverters;
using Kontecg.Events.Bus;
using Kontecg.Events.Bus.Entities;
using Kontecg.Extensions;
using Kontecg.Linq.Expressions;
using Kontecg.Runtime.Session;
using Kontecg.Timing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kontecg.EFCore
{
    /// <summary>
    ///     Base class for all DbContext classes in the application.
    /// </summary>
    public abstract class KontecgDbContext : DbContext, ITransientDependency, IShouldInitializeDbContext
    {
        /// <summary>
        /// Used to get current session values.
        /// </summary>
        public IKontecgSession KontecgSession { get; set; }

        /// <summary>
        /// Used to trigger entity change events.
        /// </summary>
        public IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

        /// <summary>
        /// Reference to the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Reference to the event bus.
        /// </summary>
        public IEventBus EventBus { get; set; }

        /// <summary>
        /// Reference to GUID generator.
        /// </summary>
        public IGuidGenerator GuidGenerator { get; set; }

        /// <summary>
        /// Reference to the current UOW provider.
        /// </summary>
        public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }

        /// <summary>
        /// Reference to multi tenancy configuration.
        /// </summary>
        public IMultiCompanyConfig MultiCompanyConfig { get; set; }

        /// <summary>
        /// Reference to the KONTECG entity configuration.
        /// </summary>
        public IKontecgEfCoreConfiguration KontecgEfCoreConfiguration { get; set; }

        /// <summary>
        /// Can be used to suppress automatically setting CompanyId on SaveChanges.
        /// Default: false.
        /// </summary>
        public virtual bool SuppressAutoSetCompanyId { get; set; }

        public virtual int? CurrentCompanyId => GetCurrentCompanyIdOrNull();

        public virtual bool IsSoftDeleteFilterEnabled => CurrentUnitOfWorkProvider?.Current?.IsFilterEnabled(KontecgDataFilters.SoftDelete) == true;

        public virtual bool IsMayHaveCompanyFilterEnabled => CurrentUnitOfWorkProvider?.Current?.IsFilterEnabled(KontecgDataFilters.MayHaveCompany) == true;

        public virtual bool IsMustHaveCompanyFilterEnabled => CurrentCompanyId != null && CurrentUnitOfWorkProvider?.Current?.IsFilterEnabled(KontecgDataFilters.MustHaveCompany) == true;

        private static readonly MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(KontecgDbContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo ConfigureGlobalValueConverterMethodInfo = typeof(KontecgDbContext).GetMethod(nameof(ConfigureGlobalValueConverter), BindingFlags.Instance | BindingFlags.NonPublic);

        protected readonly DbContextOptions DbContextOptions;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected KontecgDbContext(DbContextOptions options)
            : base(options)
        {
            DbContextOptions = options;
            InitializeDbContext();
        }

        private void InitializeDbContext()
        {
            SetNullsForInjectedProperties();
        }

        private void SetNullsForInjectedProperties()
        {
            Logger = NullLogger.Instance;
            KontecgSession = NullKontecgSession.Instance;
            EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
            GuidGenerator = UuidGenerator.Instance;
            EventBus = NullEventBus.Instance;
            KontecgEfCoreConfiguration = NullKontecgEfCoreConfiguration.Instance;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ConfigureGlobalFiltersMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] { modelBuilder, entityType });

                ConfigureGlobalValueConverterMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] { modelBuilder, entityType });
            }
        }

        protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
            where TEntity : class
        {
            if (entityType.BaseType == null && ShouldFilterEntity<TEntity>(entityType))
            {
                var filterExpression = CreateFilterExpression<TEntity>(modelBuilder);
                if (filterExpression != null)
                {
                    modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
                }
            }
        }

        protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }

            if (typeof(IMayHaveCompany).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }

            if (typeof(IMustHaveCompany).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }

            return false;
        }

        protected virtual Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>(ModelBuilder modelBuilder)
            where TEntity : class
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> softDeleteFilter = e => !IsSoftDeleteFilterEnabled || !((ISoftDelete)e).IsDeleted;
                if (UseKontecgQueryCompiler())
                {
                    softDeleteFilter = e => SoftDeleteFilter(((ISoftDelete)e).IsDeleted, true);
                    modelBuilder.ConfigureSoftDeleteDbFunction(typeof(KontecgDbContext).GetMethod(nameof(SoftDeleteFilter), new[] { typeof(bool), typeof(bool) })!, this.GetService<KontecgEfCoreCurrentDbContext>());
                }
                expression = expression == null ? softDeleteFilter : CombineExpressions(expression, softDeleteFilter);
            }

            if (typeof(IMayHaveCompany).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> mayHaveCompanyFilter = e => !IsMayHaveCompanyFilterEnabled || ((IMayHaveCompany)e).CompanyId == CurrentCompanyId;
                if (UseKontecgQueryCompiler())
                {
                    mayHaveCompanyFilter = e => MayHaveCompanyFilter(((IMayHaveCompany)e).CompanyId, CurrentCompanyId, true);
                    modelBuilder.ConfigureMayHaveCompanyDbFunction(typeof(KontecgDbContext).GetMethod(nameof(MayHaveCompanyFilter), new[] { typeof(int?), typeof(int?), typeof(bool) })!, this.GetService<KontecgEfCoreCurrentDbContext>());
                }
                expression = expression == null ? mayHaveCompanyFilter : CombineExpressions(expression, mayHaveCompanyFilter);
            }

            if (typeof(IMustHaveCompany).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> mustHaveCompanyFilter = e => !IsMustHaveCompanyFilterEnabled || ((IMustHaveCompany)e).CompanyId == CurrentCompanyId;
                if (UseKontecgQueryCompiler())
                {
                    mustHaveCompanyFilter = e => MustHaveCompanyFilter(((IMustHaveCompany)e).CompanyId, CurrentCompanyId, true);
                    modelBuilder.ConfigureMustHaveCompanyDbFunction(typeof(KontecgDbContext).GetMethod(nameof(MustHaveCompanyFilter), new[] { typeof(int), typeof(int?), typeof(bool) })!, this.GetService<KontecgEfCoreCurrentDbContext>());
                }
                expression = expression == null ? mustHaveCompanyFilter : CombineExpressions(expression, mustHaveCompanyFilter);
            }

            return expression;
        }

        protected virtual bool UseKontecgQueryCompiler()
        {
            return DbContextOptions?.FindExtension<KontecgDbContextOptionsExtension>() != null && KontecgEfCoreConfiguration.UseKontecgQueryCompiler;
        }

        public virtual string GetCompiledQueryCacheKey()
        {
            return $"{CurrentCompanyId?.ToString() ?? "Null"}:{IsSoftDeleteFilterEnabled}:{IsMayHaveCompanyFilterEnabled}:{IsMustHaveCompanyFilterEnabled}";
        }

        protected const string DbFunctionNotSupportedExceptionMessage = "Your EF Core database provider does not support 'User-defined function mapping'." +
                                                            "Please set 'UseKontecgQueryCompiler' of 'IKontecgEfCoreConfiguration' to false to disable it." +
                                                            "See https://learn.microsoft.com/en-us/ef/core/querying/user-defined-function-mapping for more information.";

        public static bool SoftDeleteFilter(bool isDeleted, bool boolParam)
        {
            throw new NotSupportedException(DbFunctionNotSupportedExceptionMessage);
        }

        public static bool MustHaveCompanyFilter(int companyId, int? currentCompanyId, bool boolParam)
        {
            throw new NotSupportedException(DbFunctionNotSupportedExceptionMessage);
        }

        public static bool MayHaveCompanyFilter(int? companyId, int? currentCompanyId, bool boolParam)
        {
            throw new NotSupportedException(DbFunctionNotSupportedExceptionMessage);
        }

        protected void ConfigureGlobalValueConverter<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
            where TEntity : class
        {
            if (entityType.BaseType == null &&
                !typeof(TEntity).IsDefined(typeof(DisableDateTimeNormalizationAttribute), true) &&
                !typeof(TEntity).IsDefined(typeof(OwnedAttribute), true) &&
                !entityType.IsOwned())
            {
                var dateTimeValueConverter = new KontecgDateTimeValueConverter();
                var dateTimePropertyInfos = DateTimePropertyInfoHelper.GetDatePropertyInfos(typeof(TEntity));
                dateTimePropertyInfos.DateTimePropertyInfos.ForEach(property =>
                {
                    modelBuilder
                        .Entity<TEntity>()
                        .Property(property.Name)
                        .HasConversion(dateTimeValueConverter);
                });
            }
        }

        public override int SaveChanges()
        {
            try
            {
                var changeReport = ApplyKontecgConcepts();
                var result = base.SaveChanges();
                EntityChangeEventHelper.TriggerEvents(changeReport);
                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new KontecgDbConcurrencyException(ex.Message, ex);
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var changeReport = ApplyKontecgConcepts();
                var result = await base.SaveChangesAsync(cancellationToken);
                await EntityChangeEventHelper.TriggerEventsAsync(changeReport);
                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new KontecgDbConcurrencyException(ex.Message, ex);
            }
        }

        public virtual void Initialize(KontecgEfDbContextInitializationContext initializationContext)
        {
            var uowOptions = initializationContext.UnitOfWork.Options;
            if (uowOptions.Timeout.HasValue &&
                Database.IsRelational() &&
                !Database.GetCommandTimeout().HasValue)
            {
                Database.SetCommandTimeout(uowOptions.Timeout.Value.TotalSeconds.To<int>());
            }

            ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;
        }

        protected virtual EntityChangeReport ApplyKontecgConcepts()
        {
            var changeReport = new EntityChangeReport();

            var userId = GetAuditUserId();

            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                if (entry.State != EntityState.Modified && entry.CheckOwnedEntityChange())
                {
                    Entry(entry.Entity).State = EntityState.Modified;
                }

                ApplyKontecgConcepts(entry, userId, changeReport);
            }

            return changeReport;
        }

        protected virtual void ApplyKontecgConcepts(EntityEntry entry, long? userId, EntityChangeReport changeReport)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ApplyKontecgConceptsForAddedEntity(entry, userId, changeReport);
                    break;
                case EntityState.Modified:
                    ApplyKontecgConceptsForModifiedEntity(entry, userId, changeReport);
                    break;
                case EntityState.Deleted:
                    ApplyKontecgConceptsForDeletedEntity(entry, userId, changeReport);
                    break;
            }

            AddDomainEvents(changeReport.DomainEvents, entry.Entity);
        }

        protected virtual void ApplyKontecgConceptsForAddedEntity(EntityEntry entry, long? userId, EntityChangeReport changeReport)
        {
            CheckAndSetId(entry);
            CheckAndSetMustHaveCompanyIdProperty(entry.Entity);
            CheckAndSetMayHaveCompanyIdProperty(entry.Entity);
            SetCreationAuditProperties(entry.Entity, userId);
            changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Created));
        }

        protected virtual void ApplyKontecgConceptsForModifiedEntity(EntityEntry entry, long? userId, EntityChangeReport changeReport)
        {
            SetModificationAuditProperties(entry.Entity, userId);
            if (entry.Entity is ISoftDelete && entry.Entity.As<ISoftDelete>().IsDeleted)
            {
                SetDeletionAuditProperties(entry.Entity, userId);
                changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Deleted));
            }
            else
            {
                changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Updated));
            }
        }

        protected virtual void ApplyKontecgConceptsForDeletedEntity(EntityEntry entry, long? userId, EntityChangeReport changeReport)
        {
            if (IsHardDeleteEntity(entry))
            {
                changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Deleted));
                return;
            }

            CancelDeletionForSoftDelete(entry);
            SetDeletionAuditProperties(entry.Entity, userId);
            changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Deleted));
        }

        protected virtual bool IsHardDeleteEntity(EntityEntry entry)
        {
            if (!EntityHelper.IsEntity(entry.Entity.GetType()))
            {
                return false;
            }

            if (CurrentUnitOfWorkProvider?.Current?.Items == null)
            {
                return false;
            }

            if (!CurrentUnitOfWorkProvider.Current.Items.ContainsKey(UnitOfWorkExtensionDataTypes.HardDelete))
            {
                return false;
            }

            var hardDeleteItems = CurrentUnitOfWorkProvider.Current.Items[UnitOfWorkExtensionDataTypes.HardDelete];
            if (!(hardDeleteItems is HashSet<string> objects))
            {
                return false;
            }

            var currentCompanyId = GetCurrentCompanyIdOrNull();
            var hardDeleteKey = EntityHelper.GetHardDeleteKey(entry.Entity, currentCompanyId);
            return objects.Contains(hardDeleteKey);
        }

        protected virtual void AddDomainEvents(List<DomainEventEntry> domainEvents, object entityAsObj)
        {
            var generatesDomainEventsEntity = entityAsObj as IGeneratesDomainEvents;
            if (generatesDomainEventsEntity == null)
            {
                return;
            }

            if (generatesDomainEventsEntity.DomainEvents.IsNullOrEmpty())
            {
                return;
            }

            domainEvents.AddRange(generatesDomainEventsEntity.DomainEvents.Select(eventData => new DomainEventEntry(entityAsObj, eventData)));
            generatesDomainEventsEntity.DomainEvents.Clear();
        }

        protected virtual void CheckAndSetId(EntityEntry entry)
        {
            //Set GUID Ids
            var entity = entry.Entity as IEntity<Guid>;
            if (entity != null && entity.Id == Guid.Empty)
            {
                var idPropertyEntry = entry.Property("Id");

                if (idPropertyEntry != null && idPropertyEntry.Metadata.ValueGenerated == ValueGenerated.Never)
                {
                    entity.Id = GuidGenerator.Create();
                }
            }
        }

        protected virtual void CheckAndSetMustHaveCompanyIdProperty(object entityAsObj)
        {
            if (SuppressAutoSetCompanyId)
            {
                return;
            }

            //Only set IMustHaveCompany entities
            if (!(entityAsObj is IMustHaveCompany))
            {
                return;
            }

            var entity = entityAsObj.As<IMustHaveCompany>();

            //Don't set if it's already set
            if (entity.CompanyId != 0)
            {
                return;
            }

            var currentCompanyId = GetCurrentCompanyIdOrNull();

            if (currentCompanyId != null)
            {
                entity.CompanyId = currentCompanyId.Value;
            }
            else
            {
                throw new KontecgException("Can not set CompanyId to 0 for IMustHaveCompany entities!");
            }
        }

        protected virtual void CheckAndSetMayHaveCompanyIdProperty(object entityAsObj)
        {
            if (SuppressAutoSetCompanyId)
            {
                return;
            }

            //Only works for single tenant applications
            if (MultiCompanyConfig?.IsEnabled ?? false)
            {
                return;
            }

            //Only set IMayHaveCompany entities
            if (!(entityAsObj is IMayHaveCompany))
            {
                return;
            }

            var entity = entityAsObj.As<IMayHaveCompany>();

            //Don't set if it's already set
            if (entity.CompanyId != null)
            {
                return;
            }

            entity.CompanyId = GetCurrentCompanyIdOrNull();
        }

        protected virtual void SetCreationAuditProperties(object entityAsObj, long? userId)
        {
            EntityAuditingHelper.SetCreationAuditProperties(
                MultiCompanyConfig,
                entityAsObj,
                KontecgSession.CompanyId,
                userId,
                CurrentUnitOfWorkProvider?.Current?.AuditFieldConfiguration
            );
        }

        protected virtual void SetModificationAuditProperties(object entityAsObj, long? userId)
        {
            EntityAuditingHelper.SetModificationAuditProperties(
                MultiCompanyConfig,
                entityAsObj,
                KontecgSession.CompanyId,
                userId,
                CurrentUnitOfWorkProvider?.Current?.AuditFieldConfiguration
            );
        }

        protected virtual void CancelDeletionForSoftDelete(EntityEntry entry)
        {
            if (!(entry.Entity is ISoftDelete))
            {
                return;
            }

            entry.Reload();
            entry.State = EntityState.Modified;
            entry.Entity.As<ISoftDelete>().IsDeleted = true;
        }

        protected virtual void SetDeletionAuditProperties(object entityAsObj, long? userId)
        {
            EntityAuditingHelper.SetDeletionAuditProperties(
                MultiCompanyConfig,
                entityAsObj,
                KontecgSession.CompanyId,
                userId,
                CurrentUnitOfWorkProvider?.Current?.AuditFieldConfiguration
            );
        }

        protected virtual long? GetAuditUserId()
        {
            if (KontecgSession.UserId.HasValue &&
                CurrentUnitOfWorkProvider != null &&
                CurrentUnitOfWorkProvider.Current != null &&
                CurrentUnitOfWorkProvider.Current.GetCompanyId() == KontecgSession.CompanyId)
            {
                return KontecgSession.UserId;
            }

            return null;
        }

        protected virtual int? GetCurrentCompanyIdOrNull()
        {
            if (CurrentUnitOfWorkProvider != null &&
                CurrentUnitOfWorkProvider.Current != null)
            {
                return CurrentUnitOfWorkProvider.Current.GetCompanyId();
            }

            return KontecgSession.CompanyId;
        }

        protected virtual Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return ExpressionCombiner.Combine(expression1, expression2);
        }
    }
}
