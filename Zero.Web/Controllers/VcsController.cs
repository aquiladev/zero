using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Zero.Core.Domain;
using Zero.Web.Models;

namespace Zero.Web.Controllers
{
	public class VcsController : ApiController
	{
		public IEnumerable<CollectionModel> GetCollections()
		{
			var user = (ZeroPrincipal)HttpContext.Current.User;
			var uri = new Uri(user.Url);
			using (var server = new TfsConfigurationServer(uri))
			{
				CatalogNode configurationServerNode = server.CatalogNode;
				IReadOnlyCollection<CatalogNode> collections = configurationServerNode.QueryChildren(
					new[] { CatalogResourceTypes.ProjectCollection },
					false, CatalogQueryOptions.None);
				return Map(collections);
			}
		}

		public IEnumerable<ProjectModel> GetProjects(Guid collectionId)
		{
			var user = (ZeroPrincipal)HttpContext.Current.User;
			var uri = new Uri(user.Url);
			using (var server = new TfsConfigurationServer(uri))
			{
				TfsTeamProjectCollection collection = server.GetTeamProjectCollection(collectionId);
				WorkItemStore wiStore = collection.GetService<WorkItemStore>();
				return Map(wiStore.Projects);
			}
		}

		public IEnumerable<AreaModel> GetAreas(Guid collectionId, int projectId)
		{
			var user = (ZeroPrincipal)HttpContext.Current.User;
			var uri = new Uri(user.Url);
			using (var server = new TfsConfigurationServer(uri))
			{
				TfsTeamProjectCollection collection = server.GetTeamProjectCollection(collectionId);
				WorkItemStore wiStore = collection.GetService<WorkItemStore>();
				Project project = wiStore.Projects.GetById(projectId);
				return Map(project.AreaRootNodes);
			}
		}

		private IEnumerable<CollectionModel> Map(IReadOnlyCollection<CatalogNode> collections)
		{
			return collections.Select(i => new CollectionModel
			{
				Name = i.Resource.DisplayName,
				InstanceId = i.Resource.Properties["InstanceId"]
			});
		}

		private IEnumerable<ProjectModel> Map(ProjectCollection projects)
		{
			return from Project i in projects
				select new ProjectModel
				{
					Id = i.Id,
					Name = i.Name
				};
		}

		private IEnumerable<AreaModel> Map(NodeCollection areas)
		{
			return from Node area in areas
				from Node item in area.ChildNodes
				select new AreaModel
				{
					Name = item.Name
				};
		}
	}
}
