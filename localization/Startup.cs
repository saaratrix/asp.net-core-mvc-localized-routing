using System.Collections.Generic;
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
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;

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
            ConfigureDatabase(services);

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            // Set up cultures
            LocalizationRouteDataHandler.DefaultCulture = "en";
            LocalizationRouteDataHandler.SupportedCultures = new Dictionary<string, string>()
            {
                { "en", "English" },
                { "fi", "Suomeksi" },
                { "sv", "Svenska" }
            };

            // Add the LocalizationRouteConvention to MVC Conventions.
            // Without this the localized routes won't work because this is what initializes all the routes!
            IMvcBuilder mvcBuilder = services.AddMvc(options =>
            {
                options.Conventions.Add(new LocalizationRouteConvention());
            });

            // Set up the request localization options which is what sets the culture for every http request.
            // Meaning if you visit /controller the culture is LocalizationRouteDataHandler.DefaultCulture.
            // And if you visit /fi/controller the culture is fi.            
            services.Configure<RequestLocalizationOptions>(options =>
            {               
                options.DefaultRequestCulture = new RequestCulture(LocalizationRouteDataHandler.DefaultCulture, LocalizationRouteDataHandler.DefaultCulture);

                foreach (var kvp in LocalizationRouteDataHandler.SupportedCultures)
                {
                    CultureInfo culture = new CultureInfo(kvp.Key);
                    options.SupportedCultures.Add(culture);
                    options.SupportedUICultures.Add(culture);
                }
                
                options.RequestCultureProviders = new List<IRequestCultureProvider>()
                {
                    new UrlCultureProvider(options.SupportedCultures)
                };
            });

            // Setup resource file usage and IViewLocalizer
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            // Views.Shared._Layout is for the /Views/Shared/_Layout.cshtml file
            mvcBuilder.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            // Add support for localizing strings in data annotations (e.g. validation messages) via the
            // IStringLocalizer abstractions.
            .AddDataAnnotationsLocalization();

        }

        // This is so we can override the database configuration for tests
        public virtual void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizationOptions.Value);            

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");                           
            }
            // This is to catch 404s e.t.c.
            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            app.UseStaticFiles();            

            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}",
                    defaults: new { culture = LocalizationRouteDataHandler.DefaultCulture }
                );
            });
        }
    }
}
