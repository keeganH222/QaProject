namespace QaProject.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using QaProject.Models;
    internal sealed class Configuration : DbMigrationsConfiguration<QaProject.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(QaProject.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            context.Tags.AddOrUpdate(
                new Tag { Name = "Java"},
                new Tag { Name = "JavaScript"},
                new Tag { Name = "C#"},
                new Tag { Name = "C++"},
                new Tag { Name = "Python"},
                new Tag { Name = "PHP"}
                );
        }
    }
}
