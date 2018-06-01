using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;


namespace MvcCookieAuthSample.Data
{
    public class ApplicationDbContextSeed
    {
        private  UserManager<Models.ApplicationUser> _userManager;
        public async Task SeedAsync(ApplicationDbContext context, IServiceProvider servers)
        {
            if (!context.Roles.Any())
            {
                var _roleManager = servers.GetRequiredService<RoleManager<Models.ApplicationRole>>();
                var defaultRole = new Models.ApplicationRole
                {
                    Name= "Administrators",
                    NormalizedName= "Administrators",
                };
                var result= await _roleManager.CreateAsync(defaultRole);
                if (!result.Succeeded)
                {
                    throw new Exception("角色创建失败"+ result.Errors.SelectMany(e => e.Description));
                }
            }
            

            if (!context.Users.Any())
            {
                _userManager = servers.GetRequiredService<UserManager<Models.ApplicationUser>>();
                var defaultUser = new Models.ApplicationUser
                {
                    UserName = "Admin",
                    Email="793087382@qq.com",
                    NormalizedUserName="admin",
                    //SecurityStamp="admin",
                    Avatar= "https://b-ssl.duitang.com/uploads/item/201707/11/20170711135214_53FnY.thumb.224_0.jpeg"
                };
                var result = await _userManager.CreateAsync(defaultUser, "123456");
                await _userManager.AddToRoleAsync(defaultUser, "Administrators");
                if (!result.Succeeded)
                {
                    throw new Exception("初始化默认用户失败！");
                }
               
            }
        }
    }
}
