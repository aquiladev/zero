using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.ProcessConfiguration.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Zero.Core.Mvc;
using Zero.Web.Models;

namespace Zero.Web.Controllers
{
	public class TfsController : ZApiController
	{
		public IEnumerable<CollectionModel> GetCollections()
		{
			using (var server = GetServer())
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
			using (var server = GetServer())
			{
				TfsTeamProjectCollection collection = server.GetTeamProjectCollection(collectionId);
				WorkItemStore wiStore = collection.GetService<WorkItemStore>();
				return Map(wiStore.Projects);
			}
		}

		public IEnumerable<GroupModel> GetGroups(Guid collectionId, int projectId)
		{
			using (var server = GetServer())
			{
				TfsTeamProjectCollection collection = server.GetTeamProjectCollection(collectionId);
				WorkItemStore wiStore = collection.GetService<WorkItemStore>();
				Project project = wiStore.Projects.GetById(projectId);
				IIdentityManagementService identityManagement = collection.GetService<IIdentityManagementService>();
				TeamFoundationIdentity[] identities = identityManagement.ListApplicationGroups(project.Uri.ToString(), ReadIdentityOptions.None);
				return Map(identities);
			}
		}

		public TeamModel GetTeam(Guid collectionId, int projectId)
		{
			using (var server = GetServer())
			{
				TfsTeamProjectCollection collection = server.GetTeamProjectCollection(collectionId);
				WorkItemStore wiStore = collection.GetService<WorkItemStore>();
				Project project = wiStore.Projects.GetById(projectId);
				TeamSettingsConfigurationService configurationService = collection.GetService<TeamSettingsConfigurationService>();
				TeamConfiguration config = configurationService.GetTeamConfigurationsForUser(new[] { project.Uri.ToString() }).FirstOrDefault();
				//TODO config can be null
				return Map(config);
			}
		}

		public IEnumerable<AreaModel> GetAreas(Guid collectionId, int projectId)
		{
			using (var server = GetServer())
			{
				TfsTeamProjectCollection collection = server.GetTeamProjectCollection(collectionId);
				WorkItemStore wiStore = collection.GetService<WorkItemStore>();
				Project project = wiStore.Projects.GetById(projectId);
				return Map(project.AreaRootNodes);
			}
		}

		public IterationModel GetIteration(Guid collectionId, int projectId, string iterationPath)
		{
			using (var server = GetServer())
			{
				TfsTeamProjectCollection collection = server.GetTeamProjectCollection(collectionId);
				WorkItemStore wiStore = collection.GetService<WorkItemStore>();
				string query = @"SELECT * " +
					"FROM WorkItems " +
					"WHERE ([System.WorkItemType] = 'Bug' OR [System.WorkItemType] = 'Product Backlog Item' OR [System.WorkItemType] = 'Task') " +
					"AND [System.IterationPath] = '@IterationPath' " +
					"ORDER BY [System.WorkItemType]";
				query = query.Replace("@IterationPath", iterationPath);
				var items = wiStore.Query(query);

				return Map(items);
			}
		}

		#region Mappers
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
			return
				from Project i in projects
				select new ProjectModel
				{
					Id = i.Id,
					Name = i.Name
				};
		}

		private IEnumerable<AreaModel> Map(NodeCollection areas)
		{
			return
				from Node area in areas
				from Node item in area.ChildNodes
				select new AreaModel
				{
					Name = item.Name
				};
		}

		private IEnumerable<GroupModel> Map(TeamFoundationIdentity[] identities)
		{
			return identities.Select(x => new GroupModel
			{
				Id = x.TeamFoundationId,
				Name = x.DisplayName
			});
		}

		private TeamModel Map(TeamConfiguration teamConfiguration)
		{
			return new TeamModel
			{
				Id = teamConfiguration.TeamId,
				Name = teamConfiguration.TeamName,
				CurrentIterationPath = teamConfiguration.TeamSettings.CurrentIterationPath,
				IterationPaths = teamConfiguration.TeamSettings.IterationPaths
			};
		}

		private IterationModel Map(WorkItemCollection workItems)
		{
			var model = new IterationModel();

			IDictionary<int, WorkItemModel> itemDictionary = (
				from WorkItem item in workItems
				where item.Type.Name != "Task"
				select new KeyValuePair<int, WorkItemModel>(item.Id, Map(item)))
				.ToDictionary(x => x.Key, x => x.Value);

			IEnumerable<WorkItem> tasks =
				from WorkItem item in workItems
				where item.Type.Name == "Task"
				select item;

			foreach (var task in tasks)
			{
				int targetId = (
					from WorkItemLink link in task.WorkItemLinks
					where link.LinkTypeEnd.ImmutableName == "System.LinkTypes.Hierarchy-Reverse"
					select link.TargetId)
					.FirstOrDefault();

				if (targetId != default(int) && itemDictionary.ContainsKey(targetId))
				{
					var revisions = GetTaskRevisions(task);
					itemDictionary[targetId].Revisions.AddRange(revisions);
				}
			}

			model.WorkItems = itemDictionary.Select(x => x.Value).ToList();
			return model;
		}

		private WorkItemModel Map(WorkItem item)
		{
			return new WorkItemModel
			{
				Id = item.Id,
				Name = item.Title,
				Type = item.Type.Name,
				CreatedDate = item.CreatedDate,
				//Revisions = GetRevisions(item).ToList()
			};
		}
		#endregion

		private IEnumerable<RevisionModel> GetRevisions(WorkItem item)
		{
			return
				from Revision rev in item.Revisions
				where (string)rev.Fields["State"].OriginalValue == "Done"
					|| (string)rev.Fields["State"].Value == "Done"
					|| rev.Fields["Effort"].OriginalValue != rev.Fields["Effort"].Value
				select new RevisionModel
				{
					WorkItemId = rev.WorkItem.Id,
					State = (string)rev.Fields["State"].Value,
					Effort = (double)rev.Fields["Effort"].Value,
					ChangedDate = (DateTime)rev.Fields["Changed Date"].Value
				};
		}

		private IEnumerable<RevisionModel> GetTaskRevisions(WorkItem item) //TODO DRY
		{
			return
				from Revision rev in item.Revisions
				where (string)rev.Fields["State"].OriginalValue == "Done"
					|| (string)rev.Fields["State"].Value == "Done"
				select new RevisionModel
				{
					WorkItemId = rev.WorkItem.Id,
					State = (string)rev.Fields["State"].Value,
					//RemainingWork = Convert.ToDouble(item.Revisions[0].Fields["Remaining Work"].Value), //TODO Convert.ToDouble
					ChangedDate = (DateTime)rev.Fields["Changed Date"].Value
				};
		}
	}
}
