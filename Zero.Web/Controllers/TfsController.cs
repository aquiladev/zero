using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Linq;
using System.Web.Mvc;
using Zero.Core.Domain;
using Zero.Web.Models;

namespace Zero.Web.Controllers
{
	public class TfsController : Controller
	{
		public ActionResult Index()
		{
			var model = new TfsOverviewModel();

			var user = HttpContext.User as ZeroPrincipal;
			//TODO user == null

			var uri = new Uri(user.Url);
			using (var server = new TfsConfigurationServer(uri))
			{
				CatalogNode configurationServerNode = server.CatalogNode;
				// Query the children of the configuration server node for all of the team project collection nodes
				CatalogNode collection = configurationServerNode.QueryChildren(
					new[] { CatalogResourceTypes.ProjectCollection },
					false, CatalogQueryOptions.None)
					.FirstOrDefault(x => x.Resource.DisplayName == "DefaultCollection");

				if (collection == null)
				{
					return RedirectToAction("Index", "Home");
				}

				Guid tpcId = new Guid(collection.Resource.Properties["InstanceId"]);
				TfsTeamProjectCollection tpc = server.GetTeamProjectCollection(tpcId);
				WorkItemStore wiStore = tpc.GetService<WorkItemStore>();

				foreach (Project project in wiStore.Projects)
				{
					model.Projects.Add(new SelectListItem
					{
						Value = project.Id.ToString(),
						Text = project.Name
					});
				}
			}

			return View(model);
		}

		public ActionResult GetProject(TfsOverviewModel model)
		{
			var mod = new TfsProjectModel();
			var user = HttpContext.User as ZeroPrincipal;
			//TODO user == null

			var uri = new Uri(user.Url);
			using (var server = new TfsConfigurationServer(uri))
			{
				CatalogNode configurationServerNode = server.CatalogNode;
				// Query the children of the configuration server node for all of the team project collection nodes
				CatalogNode collection = configurationServerNode.QueryChildren(
					new[] { CatalogResourceTypes.ProjectCollection },
					false, CatalogQueryOptions.None)
					.FirstOrDefault(x => x.Resource.DisplayName == "DefaultCollection");

				if (collection == null)
				{
					return RedirectToAction("Index", "Home");
				}

				Guid tpcId = new Guid(collection.Resource.Properties["InstanceId"]);
				TfsTeamProjectCollection tpc = server.GetTeamProjectCollection(tpcId);
				WorkItemStore wiStore = tpc.GetService<WorkItemStore>();
				Project project = wiStore.Projects.GetById(model.ProjectId);

				foreach (Node node in project.IterationRootNodes)
				{
					foreach (Node item in node.ChildNodes)
					{
						mod.Iterations.Add(item.Name);
					}
				}
			}

			return View(mod);
		}
	}
}
