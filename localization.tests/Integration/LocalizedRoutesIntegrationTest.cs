﻿using localization.tests.TestClasses;
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
                new TestInputExpected("/exampleFi/param/1337/english", null),
            };

            // Also test some failed routes
            List<string> failedRoutes = new List<string>()
            {                
                "/Home/somewrongaction"
            };

            List<(string Href, string Link)> navigationUrls = new List<(string Href, string Text)>()
            {
                ( "/", "Home"),
                ( "/Home/About", "About"),
                ( "/Home/Contact", "Contact"),
                ( "/Example", "Example test"),
                ( "/Example/Parameter/5/en", "example param"),
                ( "/Example/Parameter/5/en", "example param 2")
            };

            await TestCulture(inputsAndExpectations, failedRoutes, "/", navigationUrls);
        }

        [TestMethod]
        public async Task TestFinnishCultures()
        {
            // Test HomeController (default), ExampleController and Error routes
            List<TestInputExpected> inputsAndExpectations = new List<TestInputExpected>()
            {
                new TestInputExpected("/fi", null),
                new TestInputExpected("/fi/koti/mIEStä", null),
                new TestInputExpected("/fi/KoTi/ota_yhteyttä", null),
                new TestInputExpected("/fi/exampleFi", null),
                new TestInputExpected("/fi/exampleFi/param/1337/finnish", null),
            };

            // Also test some failed routes
            List<string> failedRoutes = new List<string>()
            {
                "/fi/koti/somewrongaction"
            };

            List<(string Href, string Link)> navigationUrls = new List<(string Href, string Text)>()
            {
                ( "/fi", "Koti"),
                ( "/fi/koti/miestä", "Miestä"),
                ( "/fi/koti/ota_yhteyttä", "Ota Yhteyttä"),
                ( "/fi/exampleFi", "Example test"),
                ( "/fi/exampleFi/param/5/fi", "example param"),
                ( "/fi/exampleFi/param/5/fi", "example param 2")
            };

            await TestCulture(inputsAndExpectations, failedRoutes, "/fi", navigationUrls);
        }

        [TestMethod]
        public async Task TestSwedishCultures()
        {
            // Test HomeController (default), ExampleController and Error routes
            List<TestInputExpected> inputsAndExpectations = new List<TestInputExpected>()
            {
                new TestInputExpected("/sv", null),
                new TestInputExpected("/sv/Home/om", null),
                new TestInputExpected("/sv/Home/kontakta-oss/", null),
                new TestInputExpected("/sv/example", null),
                new TestInputExpected("/sv/example/param/1337/swedish", null),
            };

            // Also test some failed routes
            List<string> failedRoutes = new List<string>()
            {
                "/sv/hem/somewrongaction"
            };

            List<(string Href, string Link)> navigationUrls = new List<(string Href, string Text)>()
            {
                ( "/sv", "Home"),
                ( "/sv/Home/om", "Om"),
                ( "/sv/Home/kontakta-oss", "Kontakta Oss"),
                ( "/sv/example", "Example test"),
                ( "/sv/example/param/5/sv", "example param"),
                ( "/sv/example/param/5/sv", "example param 2")
            };

            await TestCulture(inputsAndExpectations, failedRoutes, "/sv", navigationUrls);
        }

        private async Task TestCulture(List<TestInputExpected> a_inputsAndExpectations, List<string> a_failedRoutes, string a_navigationUrl, List<(string Href, string Link)> a_navLinksExpected)
        {
            TestHTMLHelper testHTMLHelper = new TestHTMLHelper();
            HttpResponseMessage response;
            string content;

            /* Check ok routes! */
            foreach (var test in a_inputsAndExpectations)
            {
                string inputUrl = test.Input as string;

                response = await _client.GetAsync(inputUrl);
                content = await response.Content.ReadAsStringAsync();

                Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Status code for " + inputUrl);
            }

            /* Check bad routes */
            foreach (var failUrl in a_failedRoutes)
            {
                response = await _client.GetAsync(failUrl);
                content = await response.Content.ReadAsStringAsync();

                Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode, "Status code for " + failUrl);
            }

            /* Check the Nav Links */
            response = await _client.GetAsync(a_navigationUrl);
            content = await response.Content.ReadAsStringAsync();

            var navLinks = testHTMLHelper.GetNavLinks(content);

            for (int i = 0; i < navLinks.Count; i++)
            {
                string expectedHref = a_navLinksExpected[i].Href;
                string expectedLink = a_navLinksExpected[i].Link;

                string responseHref = navLinks[i].Href;
                string responseLink = navLinks[i].Link;

                Assert.AreEqual(expectedHref, responseHref);
                Assert.AreEqual(expectedLink, responseLink);
            }
        }
    }
}