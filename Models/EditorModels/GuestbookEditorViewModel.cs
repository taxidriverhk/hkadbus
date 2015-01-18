using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HKAdBus.Models.DomainModels;

namespace HKAdBus.Models.EditorModels
{
    public class GuestbookEditorViewModel
    {
        public List<GuestBookEntry> GuestBookEntries { get; set; }
    }
}