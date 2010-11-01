using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography;
using System.Data.Entity;

namespace Web.Models
{
    public class WatchedRepository
    {

        public static IQueryable<WatchedSeries> GetAllSeries(SiteDB db, long UserId)
        {
            return db.WatchedSerieses.Include("Series").Where(oo => oo.UserId == UserId);
        }

        public static List<WatchedEpisodeStatus> GetAllEpisodes(SiteDB db, long UserId, long SeriesId)
        {
            //due to some LINQ to Entities limitations we have to generate an anonymous type and then transform into into a WatchedEpisodeStatus.
            //http://samuelmueller.com/2009/11/working-with-projections-and-dtos-in-wcf-data-services/
            return (from ee in db.Episodes
                    from we in db.WatchedEpisodes.Where(oo => oo.EpisodeId == ee.EpisodeId && oo.UserId == UserId).DefaultIfEmpty()
                    where ee.SeriesId == SeriesId
                    select new { Created = ee.Created, EpisodeId = ee.EpisodeId, EpisodeNumber = ee.EpisodeNumber, EpisodeTitle = ee.EpisodeTitle, Season = ee.Season, SeriesId = ee.SeriesId, TVDBEpisodeId = ee.TVDBEpisodeId, Updated = ee.Updated, UserId = UserId, Watched = (we.WatchedEpisodeId != null) }).ToList()
                    .Select(ee => new WatchedEpisodeStatus { Created = ee.Created, EpisodeId = ee.EpisodeId, EpisodeNumber = ee.EpisodeNumber, EpisodeTitle = ee.EpisodeTitle, Season = ee.Season, SeriesId = ee.SeriesId, TVDBEpisodeId = ee.TVDBEpisodeId, Updated = ee.Updated, UserId = UserId, Watched = ee.Watched }).ToList();

            //return db.WatchedEpisodes.Include("Episode").Where(oo => oo.UserId == UserId && oo.Episode.SeriesId == SeriesId).OrderBy(oo => oo.Episode.Season).OrderBy(oo => oo.Episode.EpisodeNumber);
        }

        public static void SetWatched(SiteDB db, long EpisodeId, long UserId, bool Watched)
        {
            //find the WatchedEpisode record.
            WatchedEpisode watchedEpisode = db.WatchedEpisodes.SingleOrDefault(oo => oo.EpisodeId == EpisodeId && oo.UserId == UserId);
            if (watchedEpisode != null)
            {
                //chech if we should mark watched or unwatched.
                if (!Watched)
                {
                    //mark episode unwatched by deleting the WatchedEpisode record.
                    db.WatchedEpisodes.Remove(watchedEpisode);
                }
            }
            else if (Watched)
            {
                //mark episode watched by adding a WatchedEpisode record.
                watchedEpisode = new WatchedEpisode();
                watchedEpisode.EpisodeId = EpisodeId;
                watchedEpisode.UserId = UserId;
                watchedEpisode.Created = DateTime.Now;
            }
            //save changes.
            db.SaveChanges();
        }

        public static void SetWatched(SiteDB db, string EpisodeIds, long UserId)
        {
            //take the comma sperated list of Ids and mark them as watched. Any WatchedEpisodes not in the list should be marked as unwatched by removing them.

        }

        public static void WatchSeries(SiteDB db, long UserId, long SeriesId)
        {
            ////add WatchedSeries record.
            //check if we're watching it yet.
            WatchedSeries watchedSeries = db.WatchedSerieses.SingleOrDefault(oo => oo.SeriesId == SeriesId && oo.UserId == UserId);
            if (watchedSeries == null)
            {
                watchedSeries = new WatchedSeries { UserId = UserId, SeriesId = SeriesId };
                AuditableRepository.DefaultAuditableToNow(watchedSeries);
                db.WatchedSerieses.Add(watchedSeries);
                db.SaveChanges();
            }
        }

        //public static void WatchSeries(SiteDB db, long UserId, long SeriesId)
        //{
        //    ////add all Series and Episdoes to the Series.
        //    //check if we're watching it yet.
        //    WatchedSeries watchedSeries = db.WatchedSerieses.Include("WatchedEpisodes").SingleOrDefault(oo => oo.SeriesId == SeriesId && oo.UserId == UserId);
        //    if (watchedSeries == null)
        //    {
        //        watchedSeries = new WatchedSeries { UserId = UserId, SeriesId = SeriesId };
        //        AuditableRepository.DefaultAuditableToNow(watchedSeries);
        //        db.WatchedSerieses.Add(watchedSeries);
        //    }

        //    //add all Episodes for this Series if they don't exist.
        //    foreach (Episode episode in db.Episodes.Where(oo => oo.SeriesId == SeriesId))
        //    {
        //        //check if episode already exits.
        //        if (watchedSeries.WatchedEpisodes == null || watchedSeries.WatchedEpisodes.SingleOrDefault(oo => oo.EpisodeId == episode.EpisodeId) == null)
        //        {
        //            //doesn't exist so add.
        //            WatchedEpisode watchedEpisode = new WatchedEpisode { EpisodeId = episode.EpisodeId, UserId = UserId };
        //            AuditableRepository.DefaultAuditableToNow(watchedEpisode);
        //            db.WatchedEpisodes.Add(watchedEpisode);
        //        }
        //    }

        //    //TODO:delete any WatchedEpisodes that are no longer in the Series.Episodes list.


        //    db.SaveChanges();
        //}
    }
}