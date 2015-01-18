using HKAdBus.Models;
using HKAdBus.Models.DomainModels;
using HKAdBus.Models.EditorModels;
using HKAdBus.Others;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HKAdBus.Controllers
{
    public class GuestbookController : Controller
    {
        private HKAdBusDBContext db = new HKAdBusDBContext();

        //
        // GET: /Guestbook/
        public ActionResult Index()
        {
            return View();
        }

        //
        // POST: /Guestbook/GuestBookEntryList
        public ActionResult GuestBookEntryList(string mode)
        {
            Dictionary<string, object> queries = QueryStringConverter.GetQueries(Request.Form);
            if (mode == "add" && queries.ContainsKey("__RequestVerificationToken"))
            {
                if (Request.HttpMethod == "POST" && Request.Cookies.AllKeys.Contains("postTimeLimitFlag"))
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                GuestBookEntry gbe = new GuestBookEntry();
                gbe.Name = queries["name"].ToString();
                gbe.Email = queries["email"].ToString();
                gbe.Website = queries["website"].ToString();
                gbe.CreationTime = DateTime.Now;
                gbe.Message = WebUtility.HtmlEncode(queries["message"].ToString()).Replace(Environment.NewLine, "<br />");
                gbe.Reply = null;
                gbe.ReplyTime = DateTime.Now;

                db.GuestBookEntries.Add(gbe);
                db.SaveChanges();

                // Set time limit to avoid posting attack
                if (!Request.Cookies.AllKeys.Contains("postTimeLimitFlag"))
                {
                    HttpCookie cookie = new HttpCookie("postTimeLimitFlag");
                    cookie.Expires = DateTime.Now.AddHours(24);
                    Response.Cookies.Add(cookie);
                }
            }

            return View(new GuestbookEditorViewModel { GuestBookEntries = db.GuestBookEntries.OrderByDescending(g => g.ID).ToList() });
        }
	}
}