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

            //can't seem to get outer joins to default values properly so get all watched episodes for this series/user.
            List<long> lstWatchedEpisodeIds = db.WatchedEpisodes.Where(oo => oo.Episode.SeriesId == SeriesId && oo.UserId == UserId).Select(oo => oo.EpisodeId).ToList();


            //due to some LINQ to Entities limitations we have to generate an anonymous type and then transform into into a WatchedEpisodeStatus.
            //http://samuelmueller.com/2009/11/working-with-projections-and-dtos-in-wcf-data-services/
            return (from ee in db.Episodes
                    where ee.SeriesId == SeriesId
                    select ee).ToList()
                    .Select(ee => new WatchedEpisodeStatus { Created = ee.Created, EpisodeId = ee.EpisodeId, EpisodeNumber = ee.EpisodeNumber, EpisodeTitle = ee.EpisodeTitle, Season = ee.Season, SeriesId = ee.SeriesId, TVDBEpisodeId = ee.TVDBEpisodeId, Updated = ee.Updated, UserId = UserId, Watched = (lstWatchedEpisodeIds.Contains(ee.EpisodeId)) }).ToList();

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
                db.WatchedEpisodes.Add(watchedEpisode);
            }
            //save changes.
            db.SaveChanges();
        }

        public static void SetWatched(SiteDB db, long SeriesId, long[] EpisodeIds, long UserId)
        {
            ////take the array of Ids and mark them as watched. Any WatchedEpisodes not in the list should be marked as unwatched by removing them.

            //grab all watched episodes for this series & user.
            List<WatchedEpisode> lstWatchedEpisodes = db.WatchedEpisodes.Where(oo => oo.Episode.SeriesId == SeriesId && oo.UserId == UserId).ToList();

            if (EpisodeIds != null)
            {
                //loop threw new list of watched Episodes
                foreach (long id in EpisodeIds)
                {
                    if (lstWatchedEpisodes.Count(oo => oo.EpisodeId == id) == 0)
                    {
                        //add new record.
                        WatchedEpisode newWatchedEpisode = new WatchedEpisode();
                        newWatchedEpisode.EpisodeId = id;
                        newWatchedEpisode.UserId = UserId;
                        newWatchedEpisode.Created = DateTime.Now;

                        db.WatchedEpisodes.Add(newWatchedEpisode);
                    }
                }
            }

            //delete all records not in new list of watched Episodes
            foreach (WatchedEpisode we in lstWatchedEpisodes.Where(oo => EpisodeIds == null || !EpisodeIds.Contains(oo.EpisodeId)))
            {
                db.WatchedEpisodes.Remove(we);
            }
            
            //save.
            db.SaveChanges();
        }

        public static void SetWatched(SiteDB db, long SeriesId, int Season, long UserId, bool Watched)
        {
            ////take the array of Ids and mark them as watched. Any WatchedEpisodes not in the list should be marked as unwatched by removing them.

            //grab all watched episodes for this series, user, and Season.
            List<WatchedEpisode> lstWatchedEpisodes = db.WatchedEpisodes.Where(oo => oo.Episode.SeriesId == SeriesId && oo.UserId == UserId && oo.Episode.Season == Season).ToList();

            //get list of EpisodeIds based on Season.
            List<long> lstEpisodeIds = new List<long>();

            //get list of EpisodeIds based on Season if we're marking as Watched.
            if (Watched)
            {
                lstEpisodeIds = db.Episodes.Where(oo => oo.SeriesId == SeriesId && oo.Season == Season).Select(oo => oo.EpisodeId).ToList();
            }

            //loop threw new list of watched Episodes
            foreach (long id in lstEpisodeIds)
            {
                if (lstWatchedEpisodes.Count(oo => oo.EpisodeId == id) == 0)
                {
                    //add new record.
                    WatchedEpisode newWatchedEpisode = new WatchedEpisode();
                    newWatchedEpisode.EpisodeId = id;
                    newWatchedEpisode.UserId = UserId;
                    newWatchedEpisode.Created = DateTime.Now;

                    db.WatchedEpisodes.Add(newWatchedEpisode);
                }
            }
            
            //delete all records not in new list of watched Episodes
            foreach (WatchedEpisode we in lstWatchedEpisodes.Where(oo => !lstEpisodeIds.Contains(oo.EpisodeId)))
            {
                db.WatchedEpisodes.Remove(we);
            }

            //save.
            db.SaveChanges();
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

        public static void StopWatchingSeries(SiteDB db, long UserId, long SeriesId)
        {
            ////delete all watched episodes.
            //grab all watched episodes for this series & user.
            List<WatchedEpisode> lstWatchedEpisodes = db.WatchedEpisodes.Where(oo => oo.Episode.SeriesId == SeriesId && oo.UserId == UserId).ToList();

            foreach (WatchedEpisode we in lstWatchedEpisodes)
            {
                db.WatchedEpisodes.Remove(we);
            }

            ////remove WatchedSeries record.
            WatchedSeries watchedSeries = db.WatchedSerieses.SingleOrDefault(oo => oo.SeriesId == SeriesId && oo.UserId == UserId);
            if (watchedSeries != null)
            {
                db.WatchedSerieses.Remove(watchedSeries);
            }
            db.SaveChanges();
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