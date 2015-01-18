using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HKAdBus.Models.DomainModels
{
    public class BusModel
    {
        public int BusModelID { get; set; }

        [Display(Name = "BusModelName")]
        public string Name { get; set; }
        public string FleetPrefix { get; set; }

        public virtual ICollection<Photo> Photos { get; set; }
    }
}