using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpeakerManagement.Entities;

namespace SpeakerManagement.DatabaseContext
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        #region Constructor
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
                
        }
        #endregion

        #region Entities
        // DbSet properties for your entities
        public DbSet<Events> Events { get; set; }
        public DbSet<EventTasks> EventTasks { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<SpeakerEvents> SpeakerEvents { get; set; }
        public DbSet<SpeakerTasks> SpeakerTasks { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        #endregion

        #region Protected
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Id property as primary key
            modelBuilder.Entity<Events>().HasKey(t => t.Id);
            modelBuilder.Entity<Tasks>().HasKey(t => t.Id);
            modelBuilder.Entity<Organization>().HasKey(t => t.Id);

            base.OnModelCreating(modelBuilder);

            // MSSQL uses the dbo schema by default - not public.
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "User");
            });

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable(name: "RoleClaim");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable(name: "UserClaim");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable(name: "UserLogin");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable(name: "UserRole");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable(name: "UserToken");
            });

        }
        #endregion
    }
}
