using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HKAdBus.Models.DomainModels
{
    public class Advertisement
    {
        public int AdvertisementID { get; set; }

        [Display(Name = "AdvertisementName")]
        public string Name { get; set; }
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }
        public ICollection<Photo> Photos { get; set; }
    }
}