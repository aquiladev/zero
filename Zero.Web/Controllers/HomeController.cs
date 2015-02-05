using System.Web.Mvc;

namespace Zero.Web.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			if (Request.IsAuthenticated)
			{
				return RedirectToAction("Index", "Tfs");
			}
			return View();
		}
	}
}
