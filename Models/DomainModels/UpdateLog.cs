using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HKAdBus.Models.DomainModels
{
    public class UpdateLog
    {
        public int UpdateLogID { get; set; }
        public DateTime LogDate { get; set; }
        public string Description { get; set; }
    }
}