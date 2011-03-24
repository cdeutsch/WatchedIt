using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure;
using Web.Infrastructure;
using System.Data.Entity.Database;
using System.Data.Entity.Validation;

namespace Web.Models
{

    public class SiteDBInitializer : DropCreateDatabaseIfModelChanges<SiteDB>
    {
        public IMembershipService MembershipService { get; set; }

        public SiteDBInitializer(IMembershipService MembershipService)
        {
            this.MembershipService = MembershipService;
        }

        /// <summary>
        /// Seed database with defaults.
        /// </summary>
        /// <param name="db"></param>
        /// <remarks>WARNING: Does not run until you try to access the DB.</remarks>
        protected override void Seed(SiteDB db)
        {
            try
            {
                ////add a User.
                MembershipService.CreateUser("cdeutsch", "Mvc3333", "cd@cdeutsch.com");
                //finish the registration that the MembershipProvider did not handle.
                User user = UserRepository.CompleteRegistration(db, "cdeutsch", "Christopher", "Deutsch");

                ////populate Series table:
                var series1 = new Series { SeriesName = "Friends" };
                AuditableRepository.DefaultAuditableToNow(series1);
                db.Serieses.Add(series1);

                var series2 = new Series { SeriesName = "Batman" };
                AuditableRepository.DefaultAuditableToNow(series2);
                db.Serieses.Add(series2);

                var series3 = new Series { SeriesName = "Bing Bang Theory" };
                AuditableRepository.DefaultAuditableToNow(series3);
                db.Serieses.Add(series3);

                var series4 = new Series { SeriesName = "Mad Men" };
                AuditableRepository.DefaultAuditableToNow(series4);
                db.Serieses.Add(series4);

                //////populate Episodes table:
                ////Series 1 
                //Season 1 Episodes:
                var s01e01 = new Episode { EpisodeTitle = "Friends S01 Episode One", Season = 1, EpisodeNumber = 1 };
                AuditableRepository.DefaultAuditableToNow(s01e01);
                series1.Episodes.Add(s01e01);
                var s01e02 = new Episode { EpisodeTitle = "Friends S01 Episode Two", Season = 1, EpisodeNumber = 2 };
                AuditableRepository.DefaultAuditableToNow(s01e02);
                series1.Episodes.Add(s01e02);
                var s01e03 = new Episode { EpisodeTitle = "Friends S01 Episode Three", Season = 1, EpisodeNumber = 3 };
                AuditableRepository.DefaultAuditableToNow(s01e03);
                series1.Episodes.Add(s01e03);
                var s01e04 = new Episode { EpisodeTitle = "Friends S01 Episode Four", Season = 1, EpisodeNumber = 4 };
                AuditableRepository.DefaultAuditableToNow(s01e04);
                series1.Episodes.Add(s01e04);
                //Season 2 Episodes:
                var s02e01 = new Episode { EpisodeTitle = "Friends S02 Episode One", Season = 2, EpisodeNumber = 1 };
                AuditableRepository.DefaultAuditableToNow(s02e01);
                series1.Episodes.Add(s02e01);
                var s02e02 = new Episode { EpisodeTitle = "Friends S02 Episode Two", Season = 2, EpisodeNumber = 2 };
                AuditableRepository.DefaultAuditableToNow(s02e02);
                series1.Episodes.Add(s02e02);
                var s02e03 = new Episode { EpisodeTitle = "Friends S02 Episode Three", Season = 2, EpisodeNumber = 3 };
                AuditableRepository.DefaultAuditableToNow(s02e03);
                series1.Episodes.Add(s02e03);
                var s02e04 = new Episode { EpisodeTitle = "Friends S02 Episode Four", Season = 2, EpisodeNumber = 4 };
                AuditableRepository.DefaultAuditableToNow(s02e04);
                series1.Episodes.Add(s02e04);

                db.SaveChanges();

                //////populate WatchedSeries and WatchedEpisodes table:
                WatchedRepository.WatchSeries(db, user.UserId, series1.SeriesId);
            }
            catch (DbEntityValidationException exp)
            {
                //do something.
                throw;
            }
            catch (Exception exp)
            {
                //do something.
                throw;
            }
        }
    }

}