using System.ComponentModel.DataAnnotations;

namespace Zero.Web.Models
{
	public class LoginViewModel
	{
		[Required]
		[DataType(DataType.Url)]
		[Display(Name = "Url")]
		public string Url { get; set; }

		[Required]
		[Display(Name = "Login")]
		public string Login { get; set; }

		[Required]
		[Display(Name = "Domain")]
		public string Domain { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }
	}
}