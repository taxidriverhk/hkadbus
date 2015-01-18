using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HKAdBus.Models.DomainModels
{
    public enum BusCompany
    { 
        KMB, CTB, CMB, NWFB 
    }

    public class Photo
    {
        public int PhotoID { get; set; }
        public string Image { get; set; }
        public DateTime CreationDate { get; set; }
        public BusCompany BusCompany { get; set; }
        public string BusRoute { get; set; }
        public string FleetNumber { get; set; }
        public string LicencePlateNumber { get; set; }
        public string Provider { get; set; }
        public string Tags { get; set; }
        public int AdvertisementID { get; set; }
        public int BusModelID { get; set; }
        public int FleetPrefixIndex { get; set; }

        [ForeignKey("AdvertisementID")]
        public virtual Advertisement Advertisement { get; set; }
        [ForeignKey("BusModelID")]
        public virtual BusModel BusModel { get; set; }
    }
}