using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HKAdBus.Models.DomainModels;

namespace HKAdBus.Models.EditorModels
{
    public class CountThumbnailPair
    {
        public CountThumbnailPair(int count, string thumbnail)
        {
            Count = count;
            Thumbnail = thumbnail;
        }

        public int Count { get; set; }
        public string Thumbnail { get; set; }
    }

    public class CategoriesViewModel
    {
        public Dictionary<Category, CountThumbnailPair> Pairs { get; set; }
    }

    public class CategoryViewModel
    {
        public string CategoryName { get; set; }
        public Dictionary<Advertisement, CountThumbnailPair> Pairs { get; set; }
    }

    public class AdvertisementViewModel
    {
        public Advertisement Advertisement { get; set; }
    }

    public class PhotoDetailsViewModel
    {
        public static string GetBusCompany(BusCompany company)
        {
            switch (company)
            {
                case BusCompany.CMB:
                    return "城巴";
                case BusCompany.CTB:
                    return "中華巴士";
                case BusCompany.KMB:
                    return "九龍巴士";
                case BusCompany.NWFB:
                    return "新世界第一巴士";
                default:
                    return "九龍巴士";
            }
        }

        public Photo Photo { get; set; }
    }

    public class BusesViewModel
    {
        public Dictionary<BusModel, CountThumbnailPair> BusImagePairs { get; set; }
    }

    public class BusViewModel
    {
        public BusModel Bus { get; set; }
    }

    public class SearchViewModel
    {
        public List<Photo> Photos { get; set; }
        public string ParametersJson { get; set; }
        public bool NoResult { get; set; }
    }

    public class PhotoEditorViewModel
    {

    }
}