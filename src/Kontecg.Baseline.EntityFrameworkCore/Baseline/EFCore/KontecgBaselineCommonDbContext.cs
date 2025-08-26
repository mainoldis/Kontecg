using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kontecg.Auditing;
using Kontecg.Authorization;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.Configuration;
using Kontecg.DynamicEntityProperties;
using Kontecg.EFCore;
using Kontecg.EntityHistory;
using Kontecg.Localization;
using Kontecg.Notifications;
using Kontecg.Organizations;
using Kontecg.Webhooks;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Baseline.EFCore
{
    public abstract class KontecgBaselineCommonDbContext<TRole, TUser, TSelf> : KontecgDbContext
        where TRole : KontecgRole<TUser>
        where TUser : KontecgUser<TUser>
        where TSelf : KontecgBaselineCommonDbContext<TRole, TUser, TSelf>
    {
        protected KontecgBaselineCommonDbContext(DbContextOptions<TSelf> options)
            : base(options)
        {
        }

        /// <summary>
        ///     Roles.
        /// </summary>
        public virtual DbSet<TRole> Roles { get; set; }

        /// <summary>
        ///     Users.
        /// </summary>
        public virtual DbSet<TUser> Users { get; set; }

        /// <summary>
        ///     User logins.
        /// </summary>
        public virtual DbSet<UserLogin> UserLogins { get; set; }

        /// <summary>
        ///     User login attempts.
        /// </summary>
        public virtual DbSet<UserLoginAttempt> UserLoginAttempts { get; set; }

        /// <summary>
        ///     User roles.
        /// </summary>
        public virtual DbSet<UserRole> UserRoles { get; set; }

        /// <summary>
        ///     User claims.
        /// </summary>
        public virtual DbSet<UserClaim> UserClaims { get; set; }

        /// <summary>
        ///     User tokens.
        /// </summary>
        public virtual DbSet<UserToken> UserTokens { get; set; }

        /// <summary>
        ///     Role claims.
        /// </summary>
        public virtual DbSet<RoleClaim> RoleClaims { get; set; }

        /// <summary>
        ///     Permissions.
        /// </summary>
        public virtual DbSet<PermissionSetting> Permissions { get; set; }

        /// <summary>
        ///     Role permissions.
        /// </summary>
        public virtual DbSet<RolePermissionSetting> RolePermissions { get; set; }

        /// <summary>
        ///     User permissions.
        /// </summary>
        public virtual DbSet<UserPermissionSetting> UserPermissions { get; set; }

        /// <summary>
        ///     Settings.
        /// </summary>
        public virtual DbSet<Setting> Settings { get; set; }

        /// <summary>
        ///     Audit logs.
        /// </summary>
        public virtual DbSet<AuditLog> AuditLogs { get; set; }

        /// <summary>
        ///     Languages.
        /// </summary>
        public virtual DbSet<ApplicationLanguage> Languages { get; set; }

        /// <summary>
        ///     LanguageTexts.
        /// </summary>
        public virtual DbSet<ApplicationLanguageText> LanguageTexts { get; set; }

        /// <summary>
        ///     OrganizationUnits.
        /// </summary>
        public virtual DbSet<OrganizationUnit> OrganizationUnits { get; set; }

        /// <summary>
        ///     UserOrganizationUnits.
        /// </summary>
        public virtual DbSet<UserOrganizationUnit> UserOrganizationUnits { get; set; }

        /// <summary>
        ///     OrganizationUnitRoles.
        /// </summary>
        public virtual DbSet<OrganizationUnitRole> OrganizationUnitRoles { get; set; }

        /// <summary>
        ///     Company notifications.
        /// </summary>
        public virtual DbSet<CompanyNotificationInfo> CompanyNotifications { get; set; }

        /// <summary>
        ///     User notifications.
        /// </summary>
        public virtual DbSet<UserNotificationInfo> UserNotifications { get; set; }

        /// <summary>
        ///     Notification subscriptions.
        /// </summary>
        public virtual DbSet<NotificationSubscriptionInfo> NotificationSubscriptions { get; set; }

        /// <summary>
        ///     Entity changes.
        /// </summary>
        public virtual DbSet<EntityChange> EntityChanges { get; set; }

        /// <summary>
        ///     Entity change sets.
        /// </summary>
        public virtual DbSet<EntityChangeSet> EntityChangeSets { get; set; }

        /// <summary>
        ///     Entity property changes.
        /// </summary>
        public virtual DbSet<EntityPropertyChange> EntityPropertyChanges { get; set; }

        /// <summary>
        /// Webhook information
        /// </summary>
        public virtual DbSet<WebhookEvent> WebhookEvents { get; set; }

        /// <summary>
        /// Web subscriptions
        /// </summary>
        public virtual DbSet<WebhookSubscriptionInfo> WebhookSubscriptions { get; set; }

        /// <summary>
        /// Webhook work items
        /// </summary>
        public virtual DbSet<WebhookSendAttempt> WebhookSendAttempts { get; set; }

        /// <summary>
        ///     DynamicProperties
        /// </summary>
        public virtual DbSet<DynamicProperty> DynamicProperties { get; set; }

        /// <summary>
        ///     DynamicProperty selectable values
        /// </summary>
        public virtual DbSet<DynamicPropertyValue> DynamicPropertyValues { get; set; }

        /// <summary>
        ///     Entities dynamic properties. Which property that entity has
        /// </summary>
        public virtual DbSet<DynamicEntityProperty> DynamicEntityProperties { get; set; }

        /// <summary>
        ///     Entities dynamic properties values
        /// </summary>
        public virtual DbSet<DynamicEntityPropertyValue> DynamicEntityPropertyValues { get; set; }

        public IEntityHistoryHelper EntityHistoryHelper { get; set; }

        public override int SaveChanges()
        {
            EntityChangeSet changeSet = EntityHistoryHelper?.CreateEntityChangeSet(ChangeTracker.Entries().ToList());

            int result = base.SaveChanges();

            EntityHistoryHelper?.Save(changeSet);

            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            EntityChangeSet changeSet = EntityHistoryHelper?.CreateEntityChangeSet(ChangeTracker.Entries().ToList());

            int result = await base.SaveChangesAsync(cancellationToken);

            if (EntityHistoryHelper != null)
            {
                await EntityHistoryHelper.SaveAsync(changeSet);
            }

            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TUser>(b =>
            {
                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                b.HasOne(p => p.DeleterUser)
                    .WithMany()
                    .HasForeignKey(p => p.DeleterUserId);

                b.HasOne(p => p.CreatorUser)
                    .WithMany()
                    .HasForeignKey(p => p.CreatorUserId);

                b.HasOne(p => p.LastModifierUser)
                    .WithMany()
                    .HasForeignKey(p => p.LastModifierUserId);
            });

            modelBuilder.Entity<TRole>(b => { b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken(); });

            modelBuilder.Entity<AuditLog>(b =>
            {
                b.HasIndex(e => new {e.CompanyId, e.UserId});
                b.HasIndex(e => new {e.CompanyId, e.ExecutionTime});
                b.HasIndex(e => new {e.CompanyId, e.ExecutionDuration});
            });

            modelBuilder.Entity<ApplicationLanguage>(b => { b.HasIndex(e => new {e.CompanyId, e.Name}); });

            modelBuilder.Entity<ApplicationLanguageText>(b =>
            {
                b.HasIndex(e => new {e.CompanyId, e.Source, e.LanguageName, e.Key});
            });

            modelBuilder.Entity<EntityChange>(b =>
            {
                b.HasMany(p => p.PropertyChanges)
                    .WithOne()
                    .HasForeignKey(p => p.EntityChangeId);

                b.HasIndex(e => new {e.EntityChangeSetId});
                b.HasIndex(e => new {e.EntityTypeFullName, e.EntityId});
            });

            modelBuilder.Entity<EntityChangeSet>(b =>
            {
                b.HasMany(p => p.EntityChanges)
                    .WithOne()
                    .HasForeignKey(p => p.EntityChangeSetId);

                b.HasIndex(e => new {e.CompanyId, e.UserId});
                b.HasIndex(e => new {e.CompanyId, e.CreationTime});
                b.HasIndex(e => new {e.CompanyId, e.Reason});
            });

            modelBuilder.Entity<EntityPropertyChange>(b => { b.HasIndex(e => e.EntityChangeId); });

            modelBuilder.Entity<NotificationSubscriptionInfo>(b =>
            {
                b.HasIndex(e => new {e.NotificationName, e.EntityTypeName, e.EntityId, e.UserId});
                b.HasIndex(e => new {e.CompanyId, e.NotificationName, e.EntityTypeName, e.EntityId, e.UserId});
            });

            modelBuilder.Entity<OrganizationUnit>(b => { b.HasIndex(e => new {e.CompanyId, e.Code}).IsUnique(false); });

            modelBuilder.Entity<PermissionSetting>(b => { b.HasIndex(e => new {e.CompanyId, e.Name}); });

            modelBuilder.Entity<RoleClaim>(b =>
            {
                b.HasIndex(e => new {e.RoleId});
                b.HasIndex(e => new {e.CompanyId, e.ClaimType});
            });

            modelBuilder.Entity<TRole>(b => { b.HasIndex(e => new {e.CompanyId, e.NormalizedName}); });

            modelBuilder.Entity<Setting>(b =>
            {
                b.HasIndex(e => new {e.CompanyId, e.Name, e.UserId}).IsUnique().HasFilter(null);
            });

            modelBuilder.Entity<CompanyNotificationInfo>(b => { b.HasIndex(e => new {e.CompanyId}); });

            modelBuilder.Entity<UserClaim>(b => { b.HasIndex(e => new {e.CompanyId, e.ClaimType}); });

            modelBuilder.Entity<UserLoginAttempt>(b =>
            {
                b.HasIndex(e => new {e.CompanyName, e.UserNameOrEmailAddress, e.Result});
                b.HasIndex(ula => new {ula.UserId, ula.CompanyId});
            });

            modelBuilder.Entity<UserLogin>(b =>
            {
                b.HasIndex(e => new {e.CompanyId, e.LoginProvider, e.ProviderKey});
                b.HasIndex(e => new {e.CompanyId, e.UserId});
            });

            modelBuilder.Entity<UserNotificationInfo>(b =>
            {
                b.HasIndex(e => new {e.UserId, e.State, e.CreationTime});
            });

            modelBuilder.Entity<UserOrganizationUnit>(b =>
            {
                b.HasIndex(e => new {e.CompanyId, e.UserId});
                b.HasIndex(e => new {e.CompanyId, e.OrganizationUnitId});
            });

            modelBuilder.Entity<OrganizationUnitRole>(b =>
            {
                b.HasIndex(e => new {e.CompanyId, e.RoleId});
                b.HasIndex(e => new {e.CompanyId, e.OrganizationUnitId});
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.HasIndex(e => new {e.CompanyId, e.UserId});
                b.HasIndex(e => new {e.CompanyId, e.RoleId});
            });

            modelBuilder.Entity<TUser>(b =>
            {
                b.HasIndex(e => new {e.CompanyId, e.NormalizedUserName});
                b.HasIndex(e => new {e.CompanyId, e.NormalizedEmailAddress});
            });

            modelBuilder.Entity<UserToken>(b => { b.HasIndex(e => new {e.CompanyId, e.UserId}); });

            modelBuilder.Entity<DynamicProperty>(b =>
            {
                b.HasIndex(e => new {e.PropertyName, e.CompanyId}).IsUnique();
            });

            modelBuilder.Entity<DynamicEntityProperty>(b =>
            {
                b.HasIndex(e => new {e.EntityFullName, e.DynamicPropertyId, e.CompanyId}).IsUnique();
            });

            #region UserLogin.ProviderKey_CompanyId

            modelBuilder.Entity<UserLogin>(b => { b.HasIndex(e => new {e.ProviderKey, e.CompanyId}).IsUnique(); });

            #endregion
        }
    }
}
