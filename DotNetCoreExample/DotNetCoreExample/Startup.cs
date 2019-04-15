using System;
using DotNetCoreExample.BackgroundTasks;
using DotNetCoreExample.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetCoreExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //WebHost.Creat
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            /* Session */
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(ProjectVariables.sessionTimeLimitMinute);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        
            services.AddMemoryCache();

            services.AddSingleton<IHostedService, MovieDataRefreshService>();

            /* Init Project Variables */
            /* Log Burak: Configuration returns null, control this later */
            //Configuration.GetSection("ConnectionStrings").Bind(ProjectVariables.dbConnectionString);
            //ProjectVariables.applicationLogPath = Configuration["ApplicationLogPath"];
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();            
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Login}");

                routes.MapRoute(
                   name: "Home",
                   template: "Home/{action=MainPage}");
            });            
        }
    }
}
