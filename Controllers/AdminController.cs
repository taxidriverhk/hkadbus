using HKAdBus.Models;
using HKAdBus.Models.DomainModels;
using HKAdBus.Models.EditorModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace HKAdBus.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private HKAdBusDBContext db = new HKAdBusDBContext();

        private void ConstructViewModel(ref Dictionary<string, List<Advertisement>> categories, ref List<List<string>> fleetPrefixes)
        {
            // Get list of bus models
            AdminEditorViewModel.BusModels = new List<Object>();
            foreach (BusModel b in db.BusModels.OrderBy(b => b.Name))
                AdminEditorViewModel.BusModels.Add(new { Text = b.Name, Value = b.BusModelID.ToString() });

            // Get list of advertisements with categories
            foreach (Category c in db.Categories.Include(c => c.Advertisements))
                categories.Add(c.Name, c.Advertisements.ToList());

            // Get list of fleet prefixes for each bus model
            foreach (BusModel b in db.BusModels.OrderBy(b => b.Name))
                fleetPrefixes.Add(b.FleetPrefix.Split(',').ToList());
        }

        // GET: /Admin/
        public ActionResult Index()
        {
            var photos = db.Photos.Include(p => p.Advertisement).Include(p => p.BusModel).Include(p => p.Advertisement.Category);
            return View(photos.ToList());
        }

        // GET: /Admin/Details/5
        public ActionResult Details(int? id)
        {
            return RedirectToAction("Details", "Photo", new { id = id });
        }

        // GET: /Admin/Create
        public ActionResult Create()
        {
            AdminEditorViewModel.BusModels = new List<Object>();
            Dictionary<string, List<Advertisement>> categories = new Dictionary<string, List<Advertisement>>();
            List<List<string>> fleetPrefixes = new List<List<string>>();
            ConstructViewModel(ref categories, ref fleetPrefixes);

            return View(new AdminEditorViewModel { Categories = categories, Photo = new Photo(), FleetPrefixesJSON = new JavaScriptSerializer().Serialize(fleetPrefixes) });
        }

        // POST: /Admin/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="PhotoID,Image,CreationDate,BusCompany,BusRoute,FleetNumber,LicencePlateNumber,Provider,Tags,AdvertisementID,BusModelID,FleetPrefixIndex")] Photo photo)
        {
            if (ModelState.IsValid)
            {
                photo.CreationDate = DateTime.Now;
                db.Photos.Add(photo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdvertisementID = new SelectList(db.Advertisements, "AdvertisementID", "Name", photo.AdvertisementID);
            ViewBag.BusModelID = new SelectList(db.BusModels, "BusModelID", "Name", photo.BusModelID);
            return View(photo);
        }

        // GET: /Admin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Photo photo = db.Photos.Where(p => p.PhotoID == id)
                                   .Include(p => p.BusModel)
                                   .SingleOrDefault();
            if (photo == null)
                return HttpNotFound();

            AdminEditorViewModel.BusModels = new List<Object>();
            Dictionary<string, List<Advertisement>> categories = new Dictionary<string, List<Advertisement>>();
            List<List<string>> fleetPrefixes = new List<List<string>>();
            ConstructViewModel(ref categories, ref fleetPrefixes);

            return View(new AdminEditorViewModel { Photo = photo, FleetPrefixesJSON = new JavaScriptSerializer().Serialize(fleetPrefixes), Categories = categories });
        }

        // POST: /Admin/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="PhotoID,Image,CreationDate,BusCompany,BusRoute,FleetNumber,LicencePlateNumber,Provider,Tags,AdvertisementID,BusModelID,FleetPrefixIndex")] Photo photo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(photo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdvertisementID = new SelectList(db.Advertisements, "AdvertisementID", "Name", photo.AdvertisementID);
            ViewBag.BusModelID = new SelectList(db.BusModels, "BusModelID", "Name", photo.BusModelID);
            return View(photo);
        }

        // GET: /Admin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // POST: /Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Photo photo = db.Photos.Find(id);
            db.Photos.Remove(photo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: /Admin/Advertisements
        public ActionResult Advertisements()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AdvertisementList(string mode, int? cateId, int? adId, string newCateName, string newAdName)
        {
            if (mode == "view")
            {
                Dictionary<int, int> photoCounts = new Dictionary<int, int>();
                foreach(Category c in db.Categories)
                    foreach(Advertisement a in c.Advertisements)
                    {
                        var query = db.Photos.Where(p => p.AdvertisementID == a.AdvertisementID);
                        photoCounts.Add(a.AdvertisementID, query.ToList().Count);
                    }

                return PartialView("AdvertisementList",
                                   new AdvertisementListViewModel { Categories = db.Categories.Include(c => c.Advertisements).ToList(), PhotoCounts = photoCounts });
            }
            else if(mode == "addCate" && !String.IsNullOrEmpty(newCateName))
            {
                Category category = new Category();
                category.Name = newCateName;
                db.Categories.Add(category);
                db.SaveChanges();
                return new EmptyResult();
            }
            else if(mode == "addAd" && cateId != null && !String.IsNullOrEmpty(newAdName))
            {
                Advertisement advertisement = new Advertisement();
                advertisement.Name = newAdName;
                advertisement.CategoryID = cateId.Value;
                db.Advertisements.Add(advertisement);
                db.SaveChanges();
                return new EmptyResult();
            }
            else if(mode == "editCate" && cateId != null && !String.IsNullOrEmpty(newCateName))
            {
                Category category = db.Categories.Find(cateId);
                category.Name = newCateName;
                db.Categories.AddOrUpdate(category);
                db.SaveChanges();
                return new EmptyResult();
            }
            else if (mode == "editAd" && adId != null && !String.IsNullOrEmpty(newAdName))
            {
                Advertisement advertisement = db.Advertisements.Find(adId);
                advertisement.Name = newAdName;
                db.Advertisements.AddOrUpdate(advertisement);
                db.SaveChanges();
                return new EmptyResult();
            }
            else if(mode == "deleteCate" && cateId != null)
            {
                Category category = db.Categories.Where(c => c.CategoryID == cateId)
                                                 .Include(c => c.Advertisements)
                                                 .SingleOrDefault();
                foreach(Advertisement a in category.Advertisements)
                {
                    var photos = db.Photos.Where(p => p.AdvertisementID == a.AdvertisementID);
                    db.Photos.RemoveRange(photos);
                    db.Advertisements.Remove(a);
                    if (category.Advertisements.Count == 0) break;
                }
                db.Categories.Remove(category);
                db.SaveChanges();
                return new EmptyResult();
            }
            else if (mode == "deleteAd" && adId != null)
            {
                Advertisement advertisement = db.Advertisements.Find(adId);
                var photos = db.Photos.Where(p => p.AdvertisementID == adId);
                db.Photos.RemoveRange(photos);
                db.Advertisements.Remove(advertisement);
                db.SaveChanges();
                return new EmptyResult();
            }
            else
                return new EmptyResult();
        }

        // GET: /Admin/Buses
        public ActionResult Buses(string mode)
        {
            return View();
        }

        [HttpGet]
        public ActionResult BusList(string mode, int? busId, string newBusName, string newFleetPrefixes)
        {
            if(mode == "view")
            {
                Dictionary<int, int> photoCounts = new Dictionary<int, int>();
                foreach(BusModel b in db.BusModels.Include(b => b.Photos))
                    photoCounts.Add(b.BusModelID, b.Photos.Count);

                return PartialView("BusList",
                                   new BusListViewModel { Buses = db.BusModels.OrderBy(b => b.Name).ToList(), PhotoCounts = photoCounts });
            }
            else if(mode == "addBus" && !String.IsNullOrEmpty(newBusName) && !String.IsNullOrEmpty(newFleetPrefixes))
            {
                BusModel bus = new BusModel();
                bus.Name = newBusName;
                bus.FleetPrefix = newFleetPrefixes;
                db.BusModels.Add(bus);
                db.SaveChanges();
                return new EmptyResult();
            }
            else if (mode == "editBus" && busId != null && !String.IsNullOrEmpty(newBusName) && !String.IsNullOrEmpty(newFleetPrefixes))
            {
                BusModel bus = db.BusModels.Find(busId);
                bus.Name = newBusName;
                bus.FleetPrefix = newFleetPrefixes;
                db.BusModels.AddOrUpdate(bus);
                db.SaveChanges();
                return new EmptyResult();
            }
            else if (mode == "deleteBus" && busId != null)
            {
                BusModel bus = db.BusModels.Where(b => b.BusModelID == busId)
                                           .Include(b => b.Photos).SingleOrDefault();
                foreach (Photo p in bus.Photos)
                    db.Photos.Remove(p);
                db.BusModels.Remove(bus);
                db.SaveChanges();
                return new EmptyResult();
            }
            else
                return new EmptyResult();
        }

        public ActionResult Guestbook()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GuestbookList(string mode, int? postId, string reply)
        {
            if(mode == "delete" && postId != null)
            {
                GuestBookEntry gbe = db.GuestBookEntries.Find(postId);
                db.GuestBookEntries.Remove(gbe);
                db.SaveChanges();
            }
            else if(mode == "reply" && postId != null && !String.IsNullOrEmpty(reply))
            {
                GuestBookEntry gbe = db.GuestBookEntries.Find(postId);
                gbe.Reply = reply;
                gbe.ReplyTime = DateTime.Now;
                db.GuestBookEntries.AddOrUpdate(gbe);
                db.SaveChanges();
            }

            return View(new GuestbookEditorViewModel { GuestBookEntries = db.GuestBookEntries.ToList() });
        }

        public ActionResult UpdateLogs(int? logId, string mode, string date, string description)
        {
            if (mode == "add" && !String.IsNullOrEmpty(date) && !String.IsNullOrEmpty(description))
            {
                UpdateLog log = new UpdateLog();
                log.LogDate = DateTime.Parse(date);
                log.Description = description;
                db.Updates.Add(log);
                db.SaveChanges();
            }
            else if (mode == "edit" && logId != null && !String.IsNullOrEmpty(date) && !String.IsNullOrEmpty(description))
            {
                UpdateLog log = db.Updates.Find(logId);
                log.LogDate = DateTime.Parse(date);
                log.Description = description;
                db.Updates.AddOrUpdate(log);
                db.SaveChanges();
            }
            else if(mode == "delete" && logId != null)
            {
                UpdateLog log = db.Updates.Find(logId);
                db.Updates.Remove(log);
                db.SaveChanges();
            }

            return View(db.Updates);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
