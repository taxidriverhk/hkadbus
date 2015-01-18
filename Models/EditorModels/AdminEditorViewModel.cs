using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HKAdBus.Models.DomainModels;

namespace HKAdBus.Models.EditorModels
{
    public class AdminEditorViewModel
    {
        public static List<Object> BusCompanies = new List<Object>() 
        {
            new { Text = "中華巴士 (CMB)", Value = BusCompany.CMB.ToString() },
            new { Text = "城巴 (CTB)", Value = BusCompany.CTB.ToString() },
            new { Text = "九龍巴士 (KMB)", Value = BusCompany.KMB.ToString() },
            new { Text = "新世界第一巴士 (NWFB)", Value = BusCompany.NWFB.ToString() }
        };

        public static List<Object> BusModels { get; set; }

        public Dictionary<string, List<Advertisement>> Categories { get; set; }

        public string FleetPrefixesJSON { get; set; }

        public Photo Photo { get; set; }
    }

    public class AdvertisementListViewModel
    {
        public List<Category> Categories { get; set; }

        public Dictionary<int, int> PhotoCounts { get; set; }
    }

    public class BusListViewModel
    {
        public List<BusModel> Buses { get; set; }
        public Dictionary<int, int> PhotoCounts { get; set; }
    }
}