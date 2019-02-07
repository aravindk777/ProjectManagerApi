using PM.Models.DataModel;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace PM.Data.Entities
{
    public class PMDbContext : DbContext
    {
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<Task> Task { get; set; }

        public PMDbContext() : base(ConfigurationManager.ConnectionStrings["ProjectManDb"].ConnectionString)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<PMDbContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder
               .Entity<Project>()
               .HasRequired(mgr => mgr.Manager)
               .WithMany()
               .HasForeignKey(fk => fk.ManagerId)
               .WillCascadeOnDelete(false);

            modelBuilder
                .Entity<Task>()
                .HasRequired(t => t.TaskOwner)
                .WithMany()
                .HasForeignKey(fk => fk.TaskOwnerId)
                .WillCascadeOnDelete(false);

            modelBuilder
                .Entity<Task>()
                .HasRequired(t => t.Project)
                .WithMany()
                .HasForeignKey(fk => fk.ProjectId)
                .WillCascadeOnDelete(false);

            modelBuilder
                .Entity<Task>()
                .HasOptional(parent => parent.ParentTask)
                .WithMany()
                .HasForeignKey(fk => fk.ParentTaskId)
                .WillCascadeOnDelete(false);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
