using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Web.Infrastructure;

namespace Web.Models
{

    public class WatchedEpisode
    {
        
        [Key]
        public long WatchedEpisodeId { get; set; }
        public long EpisodeId { get; set; }
        public Episode Episode { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        //public DateTime Updated { get; set; }
        public DateTime Created { get; set; }


        //overrides basic equality. By overriding this
        //you're telling the container how to find this object
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(WatchedEpisode))
            {
                var comp = (WatchedEpisode)obj;
                return comp.WatchedEpisodeId.Equals(this.WatchedEpisodeId);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override string ToString()
        {
            return this.WatchedEpisodeId.ToString();
        }

    }

}
