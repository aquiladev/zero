using System.Collections.Generic;
using System.Web.Mvc;

namespace Zero.Web.Models
{
	public class TfsOverviewModel
	{
		public TfsOverviewModel()
		{
			Projects = new List<SelectListItem>();
		}

		public int ProjectId { get; set; }

		public ICollection<SelectListItem> Projects { get; set; }
	}
}