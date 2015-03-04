using System;
using System.Collections.Generic;

namespace Zero.Api.Models
{
	public class TaskModel
	{
		public TaskModel()
		{
			Revisions = new List<RevisionModel>();
		}

		public int Id { get; set; }

		public string Title { get; set; }

		public DateTime Created { get; set; }

		public IEnumerable<RevisionModel> Revisions { get; set; }
	}
}