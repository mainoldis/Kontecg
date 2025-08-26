using Kontecg.Application.Clients;
using Kontecg.Application.Features;
using Kontecg.Auditing;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.BackgroundJobs;
using Kontecg.MultiCompany;
using Kontecg.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Baseline.EFCore
{
    /// <summary>
    ///     Base DbContext for Kontecg Baseline.
    ///     Derive your DbContext from this class to have base entities.
    /// </summary>
    public abstract class
        KontecgBaselineDbContext<TCompany, TRole, TUser, TSelf> : KontecgBaselineCommonDbContext<TRole, TUser, TSelf>
        where TCompany : KontecgCompany<TUser>
        where TRole : KontecgRole<TUser>
        where TUser : KontecgUser<TUser>
        where TSelf : KontecgBaselineDbContext<TCompany, TRole, TUser, TSelf>
    {
        protected KontecgBaselineDbContext(DbContextOptions<TSelf> options)
            : base(options)
        {
        }

        /// <summary>
        ///     Companies
        /// </summary>
        public virtual DbSet<TCompany> Companies { get; set; }

        /// <summary>
        ///     Companies
        /// </summary>
        public virtual DbSet<ClientInfo> Clients { get; set; }

        /// <summary>
        ///     FeatureSettings.
        /// </summary>
        public virtual DbSet<FeatureSetting> FeatureSettings { get; set; }

        /// <summary>
        ///     CompanyFeatureSetting.
        /// </summary>
        public virtual DbSet<CompanyFeatureSetting> CompanyFeatureSettings { get; set; }

        /// <summary>
        ///     ClientFeatureSetting.
        /// </summary>
        public virtual DbSet<ClientFeatureSetting> ClientFeatureSettings { get; set; }

        /// <summary>
        ///     Background jobs.
        /// </summary>
        public virtual DbSet<BackgroundJobInfo> BackgroundJobs { get; set; }

        /// <summary>
        ///     User accounts
        /// </summary>
        public virtual DbSet<UserAccount> UserAccounts { get; set; }

        /// <summary>
        ///     Notifications.
        /// </summary>
        public virtual DbSet<NotificationInfo> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BackgroundJobInfo>(b => { b.HasIndex(e => new {e.IsAbandoned, e.NextTryTime}); });

            modelBuilder.Entity<CompanyFeatureSetting>(b => { b.HasIndex(e => new {e.CompanyId, e.Name}); });

            modelBuilder.Entity<ClientFeatureSetting>(b => { b.HasIndex(e => new { e.ClientId, e.Name }); });

            modelBuilder.Entity<TCompany>(b =>
            {
                b.HasOne(p => p.DeleterUser)
                    .WithMany()
                    .HasForeignKey(p => p.DeleterUserId);

                b.HasOne(p => p.CreatorUser)
                    .WithMany()
                    .HasForeignKey(p => p.CreatorUserId);

                b.HasOne(p => p.LastModifierUser)
                    .WithMany()
                    .HasForeignKey(p => p.LastModifierUserId);

                b.HasIndex(e => e.CompanyName);
            });

            modelBuilder.Entity<UserAccount>(b =>
            {
                b.HasIndex(e => new {e.CompanyId, e.UserId});
                b.HasIndex(e => new {e.CompanyId, e.UserName});
                b.HasIndex(e => new {e.CompanyId, e.EmailAddress});
                b.HasIndex(e => new {e.UserName});
                b.HasIndex(e => new {e.EmailAddress});
            });

            modelBuilder.Entity<ClientInfo>(b =>
            {
                b.Property(e => e.Id).HasMaxLength(128);
                b.Property(e => e.Name).HasMaxLength(ClientInfo.MaxClientNameLength);
                b.Property(e => e.IpAddress).HasMaxLength(ClientInfo.MaxClientIpAddressLength);
                b.Property(e => e.Info).HasMaxLength(ClientInfo.MaxClientInfoLength);
                b.Property(e => e.Version).HasMaxLength(ClientInfo.MaxClientIpAddressLength);
                b.HasIndex(e => new { e.Id, e.CompanyId });
            });

            #region AuditLog.Set_MaxLengths

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.ServiceName)
                .HasMaxLength(AuditLog.MaxServiceNameLength);

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.MethodName)
                .HasMaxLength(AuditLog.MaxMethodNameLength);

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.Parameters)
                .HasMaxLength(AuditLog.MaxParametersLength);

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.ClientIpAddress)
                .HasMaxLength(AuditLog.MaxClientIpAddressLength);

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.ClientName)
                .HasMaxLength(AuditLog.MaxClientNameLength);

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.ClientInfo)
                .HasMaxLength(AuditLog.MaxClientInfoLength);

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.ExceptionMessage)
                .HasMaxLength(AuditLog.MaxExceptionMessageLength);

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.Exception)
                .HasMaxLength(AuditLog.MaxExceptionLength);

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.CustomData)
                .HasMaxLength(AuditLog.MaxCustomDataLength);

            #endregion
        }
    }
}
