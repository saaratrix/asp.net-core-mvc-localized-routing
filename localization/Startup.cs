using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using localization.Data;
using localization.Models;
using localization.Services;
using localization.Localization;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace localization
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            // Set up cultures
            LocalizationDataHandler.DefaultCulture = "en";
            LocalizationDataHandler.SupportedCultures = new List<string>()
            {
                "en",
                "fi",
                "sv"
            };
           
            services.AddMvc(options =>
            {
                options.Conventions.Add(new LocalizedRouteConvention());
            });
            services.AddLocalization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            RequestCulture defaultCulture = new RequestCulture(LocalizationDataHandler.DefaultCulture);
            RequestLocalizationOptions requestLocalizationOptions = new RequestLocalizationOptions();            

            requestLocalizationOptions.SupportedCultures = new List<CultureInfo>();
            foreach (string culture in LocalizationDataHandler.SupportedCultures)
            {
                requestLocalizationOptions.SupportedCultures.Add(new CultureInfo(culture));
            }

            requestLocalizationOptions.DefaultRequestCulture = defaultCulture;

            requestLocalizationOptions.RequestCultureProviders = new List<IRequestCultureProvider>()
            {
                new UrlCultureProvider(  requestLocalizationOptions.SupportedCultures )
            };            

            app.UseRequestLocalization(requestLocalizationOptions);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
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
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
