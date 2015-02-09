using System;

namespace Zero.Web.Models
{
	public class RevisionModel
	{
		public int WorkItemId { get; set; }

		public string State { get; set; }

		public double Effort { get; set; }

		public double RemainingWork { get; set; }

		public DateTime ChangedDate { get; set; }
	}
}