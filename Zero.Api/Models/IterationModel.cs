using System;
using System.Collections.Generic;

namespace Zero.Api.Models
{
	public class IterationModel
	{
		public IterationModel()
		{
			WorkItems = new List<WorkItemModel>();
		}

		public string Uri { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? FinishDate { get; set; }

		public IEnumerable<WorkItemModel> WorkItems { get; set; }
	}
}