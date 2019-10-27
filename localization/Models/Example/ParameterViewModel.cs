using System.ComponentModel.DataAnnotations;

namespace localization.Models.Example
{
	public class ParameterViewModel
	{
		public int Index { get; set; }

		[Display(Name = "Test parameter!")]
		public string Test { get; set; }
	}
}