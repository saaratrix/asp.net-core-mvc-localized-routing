using localization.Localization;

namespace localization.tests.TestClasses
{
	public static class TestLocalizationRouteDataHandlerHelper
	{
		public static void SetUpLocalizationRouteDataHandlerHelper(string culture)
		{
			LocalizationRouteDataHandler.DefaultCulture = culture;
		}
		
		public static void TearDownLocalizationRouteDataHandlerHelper()
		{
			LocalizationRouteDataHandler.DefaultCulture = "";
			LocalizationRouteDataHandler.ControllerRoutes.Clear();
			LocalizationRouteDataHandler.LocalizedControllerNames.Clear();
		}
	}
}