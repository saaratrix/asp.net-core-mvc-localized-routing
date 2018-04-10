using localization.tests.TestClasses;
using localization.tests.UnitTests.Localization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace localization.tests.Integration
{
    [TestClass]
    public class LocalizedRoutesIntegrationTest
    {
        private TestServer _server;
        private HttpClient _client;

        [TestInitialize]
        public void TestInitialize()
        {
            // Source: http://www.dotnetcurry.com/aspnet-core/1420/integration-testing-aspnet-core
            // I was using Environment.CurrentDirectory but this feels like a better solution.
            // C://..../"asp.net-mvc-core-1.0-localized-routing\\localization.tests\\bin\\Debug\\netcoreapp2.0\\"            
            string testingApplicationPath = PlatformServices.Default.Application.ApplicationBasePath;
            string contentRoot = Path.GetFullPath(Path.Combine(testingApplicationPath, "../../../../localization"));

            IWebHostBuilder builder = new WebHostBuilder()
                // Without the ContentRoot the Views are not found
                .UseContentRoot(contentRoot)
                .UseStartup<TestStartup>()
                .UseEnvironment("Development");

            // Arrange
            _server = new TestServer(builder);
            _client = _server.CreateClient();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _server?.Dispose();
            _client?.Dispose();
            // Need to reset the data handler
            LocalizationDataHandlerTest.ResetDataHandler();
        }

        [TestMethod]
        public async Task TestEnglishCultures()
        {
            // Test HomeController (default), ExampleController and Error routes
            List<TestInputExpected> inputsAndExpectations = new List<TestInputExpected>()
            {
                new TestInputExpected("/", null),
                new TestInputExpected("/Home/About", null),
                new TestInputExpected("/hOmE/cOntAcT/", null),
                new TestInputExpected("/example", null),
                new TestInputExpected("/example/", null),

            };

            // Also test some failed routes
            List<string> failedRoutes = new List<string>()
            {                
                "/Home/somewrongaction"
            };

            List<(string Href, string Text)> navigationUrls = new List<(string Href, string Text)>()
            {
                ( "/", ""),
                ( "/Home/About", ""),
                ( "/Home/Contact", ""),
                ( "/Example", ""),
                ( "/Example/Parameter/5/en", ""),
                ( "/Example/Parameter/5/en", "")
            };

            TestHTMLHelper testHTMLHelper = new TestHTMLHelper();
            HttpResponseMessage response;
            string content;

            /* Check ok routes! */
            foreach (var test in inputsAndExpectations)
            {
                string inputUrl = test.Input as string;

                response = await _client.GetAsync(inputUrl);
                content = await response.Content.ReadAsStringAsync();

                Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Status code for " + inputUrl);
            }

            /* Check bad routes */
            foreach (var failUrl in failedRoutes)
            {
                response = await _client.GetAsync(failUrl);
                content = await response.Content.ReadAsStringAsync();

                Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode, "Status code for " + failUrl);
            }

            /* Check the Nav Links */
            response = await _client.GetAsync("/");
            content = await response.Content.ReadAsStringAsync();

            var navLinks = testHTMLHelper.GetNavLinks(content);

            for (int i = 0; i < navLinks.Count; i++)
            {
                string expectedHref = navigationUrls[i].Href;
                string inputHref = navLinks[i].Href;

                Assert.AreEqual(expectedHref, inputHref);
            }

            Assert.Fail();
        }

        [TestMethod]
        public async Task TestFinnishCultures()
        {
            Assert.Fail();
        }

        [TestMethod]
        public async Task TestSwedishCultures()
        {
            Assert.Fail();
        }
    }
}
