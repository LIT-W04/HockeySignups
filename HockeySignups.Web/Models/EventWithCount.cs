using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HockeySignups.Data;

namespace HockeySignups.Web.Models
{
    public class EventWithCount
    {
        public Event Event { get; set; }
        public int SignupCount { get; set; }
    }
}