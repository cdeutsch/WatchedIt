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

    public class Series : IAuditable
    {
        //public User() 
        //{
        //    //ID = Guid.NewGuid();
        //}

        [Key]
        public long SeriesId { get; set; }
        [StringLength(500)]
        public string SeriesName { get; set; }
        [StringLength(50)]
        public DateTime Updated { get; set; }
        public DateTime Created { get; set; }

        //relationships:
        public ICollection<Episode> Episodes { get; set; }

        //public void JustLoggedIn()
        //{
        //    Updated = DateTime.Now;
        //    LastLogin = DateTime.Now;
        //}

        //overrides basic equality. By overriding this
        //you're telling the container how to find this object
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(User))
            {
                var comp = (Series)obj;
                return comp.SeriesId.Equals(this.SeriesId);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override string ToString()
        {
            return this.SeriesName;
        }

    }

    //public class UserMetaData
    //{
    //    [Required(ErrorMessage="Username is required.")]
    //    public object Username { get; set; }


    //}

}
