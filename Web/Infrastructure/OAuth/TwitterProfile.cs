using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Infrastructure.OAuth
{
    public class TwitterProfile
    {
        public long id { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
    }
}