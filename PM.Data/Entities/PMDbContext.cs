using PM.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Data.Entities
{
    public class PMDbContext : DbContext
    {
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Projects> Projects { get; set; }

        public PMDbContext() : base(ConfigurationManager.ConnectionStrings["ProjectManDb"].ConnectionString)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<PMDbContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder
               .Entity<Projects>()
               .HasRequired(mgr => mgr.Manager)
               .WithMany()
               .HasForeignKey(fk => fk.ManagerId)
               .WillCascadeOnDelete(false);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
