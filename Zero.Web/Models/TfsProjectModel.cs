using System.Collections.Generic;

namespace Zero.Web.Models
{
	public class TfsProjectModel
	{
		public TfsProjectModel()
		{
			Iterations = new List<string>();
		}
		
		public ICollection<string> Iterations { get; set; }
	}
}