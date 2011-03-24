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

    public class Episode : IAuditable
    {

        [Key]
        public long EpisodeId { get; set; }
        public long SeriesId { get; set; }
        public int TVDBEpisodeId { get; set; }
        public Series Series { get; set; }
        public int Season { get; set; }
        public int EpisodeNumber { get; set; }
        [StringLength(500)]
        public string EpisodeTitle { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Created { get; set; }


        //overrides basic equality. By overriding this
        //you're telling the container how to find this object
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Episode))
            {
                var comp = (Episode)obj;
                return comp.EpisodeId.Equals(this.EpisodeId);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override string ToString()
        {
            return this.EpisodeTitle;
        }

    }


}
