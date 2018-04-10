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
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

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
            LocalizationDataHandler.DefaultCulture = "en";
            LocalizationDataHandler.SupportedCultures = new HashSet<string>()
            {
                "en",
                "fi",
                "sv"
            };

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc(options =>
            {
                options.Conventions.Add(new LocalizedRouteConvention());
            })
            // Views.Shared._Layout is for the /Views/Shared/_Layout.cshtml file
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            // Add support for localizing strings in data annotations (e.g. validation messages) via the
            // IStringLocalizer abstractions.
            .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(LocalizationDataHandler.DefaultCulture, LocalizationDataHandler.DefaultCulture);

                foreach (string name in LocalizationDataHandler.SupportedCultures)
                {
                    CultureInfo culture = new CultureInfo(name);
                    options.SupportedCultures.Add(culture);
                    options.SupportedUICultures.Add(culture);
                }

                options.RequestCultureProviders = new List<IRequestCultureProvider>()
                {
                    new UrlCultureProvider(options.SupportedCultures)
                };
            });
            
            //services.AddSingleton<IHtmlGenerator, LocalizedHtmlGenerator>();    
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
                    defaults: new { culture = LocalizationDataHandler.DefaultCulture }
                );
            });
        }
    }
}
