using System;

namespace Zero.Api.Models
{
	public class RevisionModel
	{
		public int WorkItemId { get; set; }

		public string State { get; set; }

		public double RemainingWork { get; set; }

		public DateTime Changed { get; set; }
	}
}