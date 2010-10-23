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

        public static IQueryable<WatchedEpisode> GetAllEpisodes(SiteDB db, long UserId, long SeriesId)
        {
            return db.WatchedEpisodes.Include("Episode").Where(oo => oo.UserId == UserId && oo.Episode.SeriesId == SeriesId).OrderBy(oo => oo.Episode.Season).OrderBy(oo => oo.Episode.EpisodeNumber);
        }

        public static void WatchSeries(SiteDB db, long UserId, long SeriesId)
        {
            ////add all Series and Episdoes to the Series.
            //check if we're watching it yet.
            WatchedSeries watchedSeries = db.WatchedSerieses.Include("WatchedEpisodes").SingleOrDefault(oo => oo.SeriesId == SeriesId && oo.UserId == UserId);
            if (watchedSeries == null)
            {
                watchedSeries = new WatchedSeries { UserId = UserId, SeriesId = SeriesId };
                AuditableRepository.DefaultAuditableToNow(watchedSeries);
                db.WatchedSerieses.Add(watchedSeries);
            }

            //add all Episodes for this Series if they don't exist.
            foreach (Episode episode in db.Episodes.Where(oo => oo.SeriesId == SeriesId))
            {
                //check if episode already exits.
                if (watchedSeries.WatchedEpisodes == null || watchedSeries.WatchedEpisodes.SingleOrDefault(oo => oo.EpisodeId == episode.EpisodeId) == null)
                {
                    //doesn't exist so add.
                    WatchedEpisode watchedEpisode = new WatchedEpisode { EpisodeId = episode.EpisodeId, UserId = UserId };
                    AuditableRepository.DefaultAuditableToNow(watchedEpisode);
                    db.WatchedEpisodes.Add(watchedEpisode);
                }
            }

            //TODO:delete any WatchedEpisodes that are no longer in the Series.Episodes list.


            db.SaveChanges();
        }
    }
}