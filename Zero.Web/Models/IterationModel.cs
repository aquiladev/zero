using System.Collections.Generic;

namespace Zero.Web.Models
{
	public class IterationModel
	{
		public IterationModel()
		{
			WorkItems = new List<WorkItemModel>();
		}

		public IEnumerable<WorkItemModel> WorkItems { get; set; }
	}
}