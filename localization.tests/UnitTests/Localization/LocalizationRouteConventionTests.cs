
using localization.Localization;

namespace localization.tests.UnitTests.Localization
{
	using NUnit.Framework;

	[TestFixture]
	public class LocalizationRouteConventionTests
	{
		private LocalizationRouteConvention routeConvention;

		[Test]
		public void AddControllerRoutes_Test()
		{
			routeConvention.AddControllerRoutes(null);
			
			Assert.Fail("Not implemented");
		}
		
		[SetUp]
		public void SetUp()
		{
			routeConvention = new LocalizationRouteConvention();
		}
	}
}