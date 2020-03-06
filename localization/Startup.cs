using System.Collections.Generic;
using localization.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
			LocalizationRouteDataHandler.DefaultCulture = "en";
			LocalizationRouteDataHandler.SupportedCultures = new Dictionary<string, string>()
			{
					{"en", "English"},
					{"fi", "Suomeksi"},
					{"sv", "Svenska"}
			};
			
			services.AddControllersWithViews(options =>
			{
				options.Conventions.Add(new LocalizationRouteConvention());
			});
			
			//Replace DefaultHtmlGenerator with CustomHtmlGenerator
			services.AddTransient<IHtmlGenerator, LocalizationHtmlGenerator>();
			services.AddSingleton<LocalizationTransformer>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				// Shorten the variable name for the dynamic controller route :)
				string cultureRegex = @"[a-z]{{2}}(-[A-Z]{{2}})?";
				string culture = $"{{culture={LocalizationRouteDataHandler.DefaultCulture}}}";
				// {culture}/... creates incorrect default routes like en/home/privacy, which could be a feature! :)
				// Without the MapControllerRoute the HtmlGenerator doesn't work, all URL's end up being "/"
				// But now the transformer has culture = controller, controller = action, action = first param.
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
				endpoints.MapDynamicControllerRoute<LocalizationTransformer>(culture + "/{controller=Home}/{action=Index}/{*params}");
				});
		}
	}
}