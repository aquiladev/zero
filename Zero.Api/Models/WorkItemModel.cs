using System;
using System.Collections.Generic;

namespace Zero.Api.Models
{
	public class WorkItemModel
	{
		public WorkItemModel()
		{
			Tasks = new List<TaskModel>();
		}

		public int Id { get; set; }

		public string Title { get; set; }

		public string Type { get; set; }

		public double Effort { get; set; }

		public DateTime Created { get; set; }

		public IList<TaskModel> Tasks { get; set; }
	}
}