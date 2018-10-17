using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using VTG.Models;

[assembly: OwinStartupAttribute(typeof(VTG.Startup))]
namespace VTG
{
    public partial class Startup
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateDefaultRoles();
            CreateDefaultUsers();
        }
        public void CreateDefaultUsers()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser user = new ApplicationUser();
            user.Email = "aboghazi1997@gmail.com";
            user.UserName = "AbdelHady";
            user.ProfileImage = "~/profile images/profileImage.jpg";
            var check = userManager.Create(user, "@bdelHady231");
            if (check.Succeeded)
            {
                userManager.AddToRole(user.Id, "Admins");
            }
            
        }
        public void CreateDefaultRoles()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            IdentityRole role;
            if (!roleManager.RoleExists("Admins"))
            {
                role = new IdentityRole();
                role.Name = "Admins";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Tourists"))
            {
                role = new IdentityRole();
                role.Name = "Tourists";
                roleManager.Create(role);
            }
        }
    }
}
