using GhostRunner.Server.Models.Migrations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;

namespace GhostRunner.Server.Models
{
    public class GhostRunnerContext : DbContext
    {
        public GhostRunnerContext(string connectionString)
            : base(connectionString)
        {
            Configuration.AutoDetectChangesEnabled = true;
            Database.SetInitializer<GhostRunnerContext>(new MigrateDatabaseToLatestVersion<GhostRunnerContext, Configuration>(connectionString));
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Script> Scripts { get; set; }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<TaskParameter> TaskParameters { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
