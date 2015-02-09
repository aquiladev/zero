using System;

namespace Zero.Web.Models
{
	public class TeamModel
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string CurrentIterationPath { get; set; }

		public string[] IterationPaths { get; set; }
	}
}