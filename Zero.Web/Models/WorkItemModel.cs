using System;
using System.Collections.Generic;

namespace Zero.Web.Models
{
	public class WorkItemModel
	{
		public WorkItemModel()
		{
			Revisions = new List<RevisionModel>();
		}

		public int Id { get; set; }

		public string Name { get; set; }

		public string Type { get; set; }

		public DateTime CreatedDate { get; set; }

		public List<RevisionModel> Revisions { get; set; }
	}
}