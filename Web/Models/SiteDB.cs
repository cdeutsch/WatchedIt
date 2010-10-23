﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Objects;
using System.Data.Entity.ModelConfiguration;

namespace Web.Models
{
    public class SiteDB : DbContext
    {

        public SiteDB()
        {
            
        }

        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Series> Serieses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WatchedEpisode> WatchedEpisodes { get; set; }
        public DbSet<WatchedSeries> WatchedSerieses { get; set; }
        
        public ObjectContext Context
        {
            get
            {
                return this.ObjectContext;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}