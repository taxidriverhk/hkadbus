using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HKAdBus.Models.DomainModels;
using HKAdBus.Models.EditorModels;
using HKAdBus.Models;
using HKAdBus.Others;

namespace HKAdBus.Controllers
{
    public class PhotoController : Controller
    {
        private const string IMAGE_NOT_AVAILABLE = "~/img/image_not_available.jpg";

        private HKAdBusDBContext db = new HKAdBusDBContext();

        private class TagComparator : IEqualityComparer<string>
        {
            public bool Equals(string a, string b)
            {
                return a.Contains(b);
            }

            public int GetHashCode(string s)
            {
                return 0;
            }
        }

        // GET: /Photo/Buses
        public ActionResult Buses()
        {
            var buses = db.BusModels.Include(b => b.Photos).OrderBy(b => b.Name);
            Dictionary<BusModel, CountThumbnailPair> pairs = new Dictionary<BusModel, CountThumbnailPair>();

            foreach(BusModel b in buses)
            {
                var photos = b.Photos.ToList();
                int index = new Random().Next(0, photos.Count);
                if (photos != null && photos.Count != 0)
                    pairs.Add(b, new CountThumbnailPair(photos.Count, photos[index].Image));
                else
                    pairs.Add(b, new CountThumbnailPair(0, IMAGE_NOT_AVAILABLE));
            }

            return View(new BusesViewModel { BusImagePairs = pairs });
        }

        public ActionResult Bus(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var bus = db.BusModels.Where(b => b.BusModelID == id).
                                   Include(b => b.Photos).
                                   OrderBy(b => b.Name).SingleOrDefault();
            if (bus == null)
                return HttpNotFound();

            return View(new BusViewModel { Bus = bus });
        }

        // GET: /Photo/Categories
        public ActionResult Categories()
        {
            var categories = db.Categories.OrderBy(c => c.Name);
            var photos = db.Photos;
            Dictionary<Category, CountThumbnailPair> pairs = new Dictionary<Category, CountThumbnailPair>();

            foreach(Category c in categories)
            {
                var query = from p in photos
                            where p.Advertisement.CategoryID == c.CategoryID
                            select p;
                List<Photo> result = query.ToList();
                int index = new Random().Next(0, result.Count);
                if(result != null && result.Count != 0)
                    pairs.Add(c, new CountThumbnailPair(result.Count, result[index].Image));
                else
                    pairs.Add(c, new CountThumbnailPair(0, IMAGE_NOT_AVAILABLE));
            }

            return View(new CategoriesViewModel { Pairs = pairs });
        }

        // GET: /Photo/Category/5
        public ActionResult Category(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var category = db.Categories.Where(c => c.CategoryID == id).
                                         Include(c => c.Advertisements).SingleOrDefault();
            if (category == null)
                return HttpNotFound();

            var photos = db.Photos;
            Dictionary<Advertisement, CountThumbnailPair> pairs = new Dictionary<Advertisement, CountThumbnailPair>();

            foreach(Advertisement a in category.Advertisements.OrderBy(a => a.Name))
            {
                var query = from p in photos
                            where p.AdvertisementID == a.AdvertisementID
                            select p;
                List<Photo> result = query.ToList();
                int index = new Random().Next(0, result.Count);
                if (result != null && result.Count != 0)
                    pairs.Add(a, new CountThumbnailPair(result.Count, result[index].Image));
                else
                    pairs.Add(a, new CountThumbnailPair(0, IMAGE_NOT_AVAILABLE));
            }

            return View(new CategoryViewModel { CategoryName = category.Name, Pairs = pairs });
        }

        // GET: /Photo/Advertisement/5
        public ActionResult Advertisement(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Advertisement advertisement = db.Advertisements.
                                             Where(a => a.AdvertisementID == id).
                                             Include(a => a.Photos).SingleOrDefault();
            if (advertisement == null)
                return HttpNotFound();

            return View(new AdvertisementViewModel { Advertisement = advertisement });
        }

        // GET: /Photo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Where(p => p.PhotoID == id).
                                    Include(p => p.Advertisement).
                                    Include(p => p.Advertisement.Category).SingleOrDefault();
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(new PhotoDetailsViewModel { Photo = photo });
        }

        // GET: /Search?key1=value1&key2=value2
        public ActionResult Search()
        {
            // Avoid request attack on the server side
            if (Request.HttpMethod == "POST" && Request.Cookies.AllKeys.Contains("searchTimeLimitFlag"))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Get queries
            Dictionary<string, object> queries = Request.HttpMethod == "GET" ? 
                QueryStringConverter.GetQueries(Request.QueryString) : QueryStringConverter.GetQueries(Request.Form);
            string queriesJson = "{";
            foreach(string key in queries.Keys)
                queriesJson += '"' + key + "\" : \"" + queries[key] + "\",";
            queriesJson = (queriesJson == "{" ? 
                queriesJson : queriesJson.Substring(0, queriesJson.Length-1)) + "}";
            if (queries.Count == 0)
                return View(new SearchViewModel { ParametersJson = queriesJson, Photos = new List<Photo>() });

            List<string> tags = new List<string>();
            List<string> includes = new List<string>();
            // Get filters to apply
            if (queries.ContainsKey("includeTag") && queries["includeTag"].ToString() == "on")
                if (queries.ContainsKey("tag") && queries["tag"].ToString() != "")
                {
                    includes.Add("includeTag");
                    tags = queries["tag"].ToString().Split(',').ToList();
                }
            if (queries.ContainsKey("includeFleet") && queries["includeFleet"].ToString() == "on")
            {
                if (queries.ContainsKey("fleetNumberPrefix") && queries["fleetNumberPrefix"].ToString() != "")
                    includes.Add("includeFleet");
            }
            if (queries.ContainsKey("includeProvider") && queries["includeProvider"].ToString() == "on")
                includes.Add("includeProvider");
            if (queries.ContainsKey("includeRoute") && queries["includeRoute"].ToString() == "on")
                includes.Add("includeRoute");
            if (queries.ContainsKey("includeLicence") && queries["includeLicence"].ToString() == "on")
                includes.Add("includeLicence");
            if (queries.ContainsKey("includeCreationDate") && queries["includeCreationDate"].ToString() == "on")
                includes.Add("includeCreationDate");

            // Perform search by different filters
            int satisfied = 0;
            bool noResult = false;
            List<Photo> photos = db.Photos.Include(p => p.Advertisement).
                                           Include(p => p.BusModel).
                                           Include(p => p.Advertisement.Category).ToList();
            List<Photo> resultPhotos = new List<Photo>();
            foreach (Photo p in photos)
            {
                // Tag filter
                if(includes.Contains("includeTag"))
                {
                    List<string> compareTags = p.Tags.Split(',').ToList();
                    bool result;
                    if (queries.ContainsKey("exactTagMatch") && queries["exactTagMatch"].ToString() == "on")
                        result = tags.Intersect(compareTags).Any();
                    else
                        result = tags.Intersect(compareTags, new TagComparator()).Any();

                    if (result)
                        satisfied++;
                }
                // Fleet number filter
                if(includes.Contains("includeFleet"))
                {
                    if (p.BusModel.FleetPrefix.Split(',')[p.FleetPrefixIndex] == queries["fleetNumberPrefix"].ToString()
                        && (!queries.ContainsKey("fleetNumber") || queries["fleetNumber"].ToString() == ""))
                        satisfied++;
                    else if (queries.ContainsKey("fleetNumber") && queries["fleetNumber"].ToString() != "" 
                        && p.BusModel.FleetPrefix.Split(',')[p.FleetPrefixIndex] == queries["fleetNumberPrefix"].ToString()
                        && p.FleetNumber == queries["fleetNumber"].ToString())
                        satisfied++;
                }
                // Provider filter
                if(includes.Contains("includeProvider"))
                    if (p.Provider == queries["provider"].ToString())
                        satisfied++;
                // Route filter
                if (includes.Contains("includeRoute"))
                    if (p.BusRoute == queries["route"].ToString())
                        satisfied++;
                // Licence filter
                if (includes.Contains("includeLicence"))
                    if (p.Provider == queries["licencePlate"].ToString())
                        satisfied++;
                // Date range filter (date format has been validated in its view page)
                if (includes.Contains("includeCreationDate"))
                {
                    DateTime? start = null, end = null;
                    if (queries.ContainsKey("startDate") && queries["startDate"].ToString() != "")
                        start = DateTime.Parse(queries["startDate"].ToString());
                    if (queries.ContainsKey("endDate") && queries["endDate"].ToString() != "")
                        end = DateTime.Parse(queries["endDate"].ToString() + " 23:59:59");

                    if((start == null || (start != null && p.CreationDate >= start))
                        && (end == null || (end != null && p.CreationDate <= end)))
                        satisfied++;
                }

                if (satisfied == includes.Count) resultPhotos.Add(p);
                satisfied = 0;
            }
            if (!resultPhotos.Any())
                noResult = true;

            // Set time limit
            if(!Request.Cookies.AllKeys.Contains("searchTimeLimitFlag"))
            {
                HttpCookie cookie = new HttpCookie("searchTimeLimitFlag");
                cookie.Expires = DateTime.Now.AddSeconds(29);
                Response.Cookies.Add(cookie);
            }

            return View(new SearchViewModel { ParametersJson = queriesJson, Photos = resultPhotos, NoResult = noResult  });
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
