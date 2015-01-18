using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HKAdBus.Models;
using HKAdBus.Models.EditorModels;

namespace HKAdBus.Controllers
{
    public class HomeController : Controller
    {
        private HKAdBusDBContext db = new HKAdBusDBContext();

        public ActionResult Index()
        {
            return View(new HomeEditorViewModel { UpdateLogs = db.Updates.OrderByDescending(u => u.LogDate).ToList() });
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}