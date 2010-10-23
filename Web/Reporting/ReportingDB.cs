using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Objects;
using System.Data.Entity.ModelConfiguration;

namespace Web.Reporting
{
    public class ReportingDB : DbContext
    {

        public DbSet<UserActivity> UserActivitys { get; set; }

        public ObjectContext Context
        {
            get
            {
                return this.ObjectContext;
            }
        }
    }
}