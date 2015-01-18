using HKAdBus.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HKAdBus.Models.DomainModels
{
    public class HKAdBusDBInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<HKAdBusDBContext>
    {
        // Add test data if the database is empty
        protected override void Seed(HKAdBusDBContext context)
        {
            var buses = new List<BusModel> 
            {
                new BusModel { Name = "丹尼士祖比倫", FleetPrefix = "N,DS" },
                new BusModel { Name = "丹尼士三叉戟12米", FleetPrefix = "10,11,ATR" }
            };
            buses.ForEach(b => context.BusModels.AddOrUpdate(b));
            context.SaveChanges();

            var categories = new List<Category> 
            {
                new Category { Name = "家居用品" },
                new Category { Name = "食品/飲品" }
            };
            categories.ForEach(c => context.Categories.AddOrUpdate(c));
            context.SaveChanges();

            var advertisements = new List<Advertisement> 
            {
                new Advertisement { Name = "紅A", CategoryID = 1 },
                new Advertisement { Name = "紅圈牌", CategoryID = 2 },
            };
            advertisements.ForEach(a => context.Advertisements.AddOrUpdate(a));
            context.SaveChanges();

            var photos = new List<Photo>
            {
                new Photo { AdvertisementID = 1, BusModelID = 1, Image = "/img/photos/606/scan0036.jpg", BusCompany = BusCompany.KMB, BusRoute = "73A", FleetNumber = "196", 
                            LicencePlateNumber = "CM1385", CreationDate = DateTime.Parse("2014-12-14"), Provider = "606", Tags = "紅A,Red A", FleetPrefixIndex = 0 },
                new Photo { AdvertisementID = 2, BusModelID = 2, Image = "/img/photos/606/scan0075.jpg", BusCompany = BusCompany.NWFB, BusRoute = "101", FleetNumber = "09", 
                            LicencePlateNumber = "HY2877", CreationDate = DateTime.Parse("2014-12-19"), Provider = "606", Tags = "紅圈牌,罐頭,Red Maruchan Brand", FleetPrefixIndex = 1 },
            };
            photos.ForEach(p => context.Photos.AddOrUpdate(p));
            context.SaveChanges();
        }
    }
}