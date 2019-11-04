namespace localization.Localization.CultureRouteData
{
	public class LocalizationRouteData
	{
		public string Area { get; set; }
		
		public string Controller { get; set; }
		
		public string? Action { get; set; }

		public LocalizationRouteData(string? area, string controller, string action)
		{
			Area = area;
			Controller = controller;
			Action = action;
		}
	}
}