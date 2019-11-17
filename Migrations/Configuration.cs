namespace QaProject.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
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
            

            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!RoleManager.RoleExists("Admin"))
            {
                RoleManager.Create(new IdentityRole("Admin"));
            }
            if (!context.Users.Any(u => u.UserName == "Admin@test.com"))
            {
                ApplicationUser Admin = new ApplicationUser { UserName = "Admin@test.com", Email = "Admin@test.com" };
                UserManager.Create(Admin, "EntityFr@mew0rk");
                UserManager.AddToRole(Admin.Id, "Admin");
            }
            if(context.Tags.Count() == 0)
            {
                context.Tags.AddOrUpdate(
                new Tag { Name = "Java" },
                new Tag { Name = "JavaScript" },
                new Tag { Name = "C#" },
                new Tag { Name = "C++" },
                new Tag { Name = "Python" },
                new Tag { Name = "PHP" }
                );
            }
        }
    }
}
