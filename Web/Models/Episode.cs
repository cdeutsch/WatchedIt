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
        //public User() 
        //{
        //    //ID = Guid.NewGuid();
        //}

        [Key]
        public long EpisodeId { get; set; }
        public long SeriesId { get; set; }
        public Series Series { get; set; }
        public int Season { get; set; }
        public int EpisodeNumber { get; set; }
        [StringLength(500)]
        public string EpisodeTitle { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Created { get; set; }
        

        //public void JustLoggedIn()
        //{
        //    Updated = DateTime.Now;
        //    LastLogin = DateTime.Now;
        //}

        ////overrides basic equality. By overriding this
        ////you're telling the container how to find this object
        //public override bool Equals(object obj)
        //{
        //    if (obj.GetType() == typeof(User))
        //    {
        //        var comp = (User)obj;
        //        return comp.UserID.Equals(this.UserID);
        //    }
        //    else
        //    {
        //        return base.Equals(obj);
        //    }
        //}

        //public override string ToString()
        //{
        //    return this.UserID.ToString();
        //}

    }

    //public class UserMetaData
    //{
    //    [Required(ErrorMessage="Username is required.")]
    //    public object Username { get; set; }


    //}

}
