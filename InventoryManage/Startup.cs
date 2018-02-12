using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using InventoryManage.Data;
using InventoryManage.Models;
using InventoryManage.Authorization;

namespace InventoryManage
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<CenterContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<Worker, CenterRole>().AddEntityFrameworkStores<CenterContext>().AddDefaultTokenProviders();
            services.AddMvc();

            // Configure Identity
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.AllowedUserNameCharacters = null;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Home/Login";
                options.LogoutPath = "/Home/LogOff";
            });
            // Configure Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanViewEquipment", policyBuilder => policyBuilder.RequireClaim(Constant.EquipmentViewClaim));
                options.AddPolicy("CanEditEquipment", policyBuilder => policyBuilder.RequireClaim(Constant.EquipmentEditClaim));
                options.AddPolicy("CanAddEquipment", policyBuilder => policyBuilder.RequireClaim(Constant.EquipmentAddClaim));
                options.AddPolicy("CanDeleteEquipment", policyBuilder => policyBuilder.RequireClaim(Constant.EquipmentDeleteClaim));
                options.AddPolicy("CanViewInvoice", policyBuilder => policyBuilder.RequireClaim(Constant.InvoiceViewClaim));
                options.AddPolicy("CanEditInvoice", policyBuilder => policyBuilder.RequireClaim(Constant.InvoiceEditClaim));
                options.AddPolicy("CanAddInvoice", policyBuilder => policyBuilder.RequireClaim(Constant.InvoiceAddClaim));
                options.AddPolicy("CanDeleteInvoice", policyBuilder => policyBuilder.RequireClaim(Constant.InvoiceDeleteClaim));
                options.AddPolicy("CanViewWorker", policyBuilder => policyBuilder.RequireClaim(Constant.WorkerViewClaim));
                options.AddPolicy("CanEditWorker", policyBuilder => policyBuilder.RequireClaim(Constant.WorkerEditClaim));
                options.AddPolicy("CanAddWorker", policyBuilder => policyBuilder.RequireClaim(Constant.WorkerAddClaim));
                options.AddPolicy("CanDeleteWorker", policyBuilder => policyBuilder.RequireClaim(Constant.WorkerDeleteClaim));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Login}/{id?}");
            });

            DbInitializer.Initialize(app.ApplicationServices).Wait();
        }
    }
}
