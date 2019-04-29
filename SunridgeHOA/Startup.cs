﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SunridgeHOA.Data;
using SunridgeHOA.Models;

namespace SunridgeHOA
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public object UIFramework { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {

                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller:exists}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                //routes.MapRoute(
                //  name: "areas",
                //  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            });

            SeedData.EnsurePopulated(app, serviceProvider);
            CreateRoles(serviceProvider).Wait();
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            string[] roles = { "SuperAdmin", "Admin", "Owner" };

            foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (!context.Owner.Any())
            {
                var owner = new Owner
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    Address = new Address
                    {
                        StreetAddress = "123 Testing",
                        City = "Testing",
                        State = "UT",
                        Zip = "84317"
                    }
                };
                context.Add(owner);
                context.SaveChanges();

                var superAdmin = new ApplicationUser
                {
                    UserName = "admin",
                    //Email = "admin@email.com",
                    OwnerId = owner.OwnerId
                };

                var pass = "Password123$";

                var result = await userManager.CreateAsync(superAdmin, pass);
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(superAdmin, new List<string> { "SuperAdmin", "Admin", "Owner" });
                }

                owner.ApplicationUserId = superAdmin.Id;
                context.SaveChanges();
            }
        }
    }
}