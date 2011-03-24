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

    public class WatchedSeries : IAuditable
    {
        [Key]
        public long WatchedSeriesId { get; set; }
        public long SeriesId { get; set; }
        public Series Series { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Created { get; set; }

        //relationships:
        public ICollection<WatchedEpisode> WatchedEpisodes { get; set; }

        //overrides basic equality. By overriding this
        //you're telling the container how to find this object
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(WatchedSeries))
            {
                var comp = (WatchedSeries)obj;
                return comp.WatchedSeriesId.Equals(this.WatchedSeriesId);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override string ToString()
        {
            return this.WatchedSeriesId.ToString();
        }

    }


}
