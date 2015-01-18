using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HKAdBus.Models.DomainModels
{
    public class GuestBookEntry
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public DateTime CreationTime { get; set; }
        public string Message { get; set; }
        public string Reply { get; set; }
        public DateTime ReplyTime { get; set; }
    }
}