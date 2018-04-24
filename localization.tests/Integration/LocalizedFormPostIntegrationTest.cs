using localization.Models.Example;
using localization.tests.TestClasses;
using localization.tests.UnitTests.Localization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace localization.tests.Integration
{
    [TestClass]
    public class LocalizedFormPostIntegrationTest
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
        public async Task POST_FormLocalizedEnglish()
        {
            List<TestInputExpected> postRequests = new List<TestInputExpected>()
            {
                new TestInputExpected(new FormPostExampleInput("/Example/Parameter", new ParameterViewModel()
                    { Index = 1337, Test = "Batman" }), "/Example/Parameter"
                ),
                new TestInputExpected(new FormPostExampleInput("/Example/Parameter", new ParameterViewModel()
                    { Index = 1234, Test = "Moo" }), "/Example/Parameter"
                )
            };

            await TestCulture(postRequests);
        }

        [TestMethod]
        public async Task POST_FormLocalizedFinnish()
        {
            List<TestInputExpected> postRequests = new List<TestInputExpected>()
            {
                new TestInputExpected(new FormPostExampleInput("/fi/ExampleFi/param", new ParameterViewModel()                    
                    { Index = 100, Test = "Vesityttö" }), "/Example/Parameter"
                ),
                new TestInputExpected(new FormPostExampleInput("/fi/ExampleFi/PaRam", new ParameterViewModel()
                    { Index = 9999, Test = "Moomin" }), "/Example/Parameter"
                )
            };

            await TestCulture(postRequests);
        }

        [TestMethod]
        public async Task POST_FormLocalizedSwedish()
        {
            List<TestInputExpected> postRequests = new List<TestInputExpected>()
            {
                new TestInputExpected(new FormPostExampleInput("/sv/Example/Parameter", new ParameterViewModel()
                    { Index = 12, Test = "Ko" }), "/Example/Parameter"
                ),
                new TestInputExpected(new FormPostExampleInput("/sv/Example/Parameter", new ParameterViewModel()
                    { Index = 34, Test = "Mjölk" }), "/Example/Parameter"
                )
            };

            await TestCulture(postRequests);
        }

        private async Task TestCulture (List<TestInputExpected> tests)
        {
            TestHTMLHelper testHTMLHelper = new TestHTMLHelper();

            foreach (var test in tests)
            {
                var input = test.Input as FormPostExampleInput;
                var expected = test.Expected as string;

                var pairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>( "Index", input.Model.Index.ToString() ),
                    new KeyValuePair<string, string>( "Test", input.Model.Test )
                };

                FormUrlEncodedContent postContent = new FormUrlEncodedContent(pairs);

                var result = await _client.PostAsync(input.Route, postContent);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

                string content = await result.Content.ReadAsStringAsync();

                // Validate the form action url

                // Make sure that the inputs are correct data
                var inputs = testHTMLHelper.GetInputsForForm(content, false);

                Assert.IsTrue(inputs.ContainsKey("Index"));
                Assert.IsTrue(inputs.ContainsKey("Test"));

                string inputIndex = HttpUtility.HtmlDecode(inputs["Index"]);
                string inputTest = HttpUtility.HtmlDecode(inputs["Test"]);

                Assert.AreEqual(input.Model.Index, int.Parse(inputIndex));
                Assert.AreEqual(input.Model.Test, inputTest);
                // Validate the <span> elements too
                var elements = testHTMLHelper.GetElements(content, new List<string>() { "parameter_index", "parameter_test" });
                Assert.IsTrue(elements.ContainsKey("parameter_index"));
                Assert.IsTrue(elements.ContainsKey("parameter_test"));

                string elementIndex = HttpUtility.HtmlDecode(elements["parameter_index"]);
                string elementTest = HttpUtility.HtmlDecode(elements["parameter_test"]);

                Assert.AreEqual(input.Model.Index, int.Parse(elementIndex));
                Assert.AreEqual(input.Model.Test, elementTest);
            }
        }

        private class FormPostExampleInput
        {
            public string Route { get; set; }
            public ParameterViewModel Model { get; set; }

            public FormPostExampleInput(string route, ParameterViewModel model)
            {
                Route = route;
                Model = model;
            }
        }
    }
}
