using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HockeySignups.Web.Models
{
    public class HistoryViewModel
    {
        public IEnumerable<EventWithCount> Events { get; set; }
    }
}