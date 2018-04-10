using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using localization;
using localization.Data;
using Microsoft.EntityFrameworkCore;

namespace localization.tests.TestClasses
{
    // Inherits from the Startup class in the main project
    // but with some things changed such as memory database instead.
    class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration)
            : base(configuration)
        {
            
        }

        public override void ConfigureDatabase(IServiceCollection services)
        {
            // Replace default database connection with In-Memory database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("localization_test_db"));            

            // Based on this repository the DatabaseSeeder sets up the predefined data
            // https://github.com/DaniJG/BlogPlayground/tree/master/BlogPlayground.IntegrationTest
            //services.AddTransient<DatabaseSeeder>();
        }
    }
}
