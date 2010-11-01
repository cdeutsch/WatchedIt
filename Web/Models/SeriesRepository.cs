using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography;
using System.Data.Entity;
using TvdbLib.Data;

namespace Web.Models
{
    public class SeriesRepository
    {

        public static Series AddSeries(SiteDB db, long UserId, int TVDBSeriesId)
        {
             TvdbSeries tvdbSeries = TVDBRepository.GetTvdbHandler().GetSeries(TVDBSeriesId, TvdbLanguage.DefaultLanguage, true, false, false);
             if (tvdbSeries == null)
             {
                 throw new ApplicationException("Could not find Series.");
             }
            
            //add the series from TVDB if it doesn't exist.
            Series series = db.Serieses.Include("Episodes").SingleOrDefault(oo => oo.TVDBSeriesId == TVDBSeriesId);
            if (series == null)
            {  
                //create series if it doesn't exist in local db.
                if (series == null)
                {
                    //add the series to our DB.
                    series = new Series();
                    series.TVDBSeriesId = tvdbSeries.Id;
                    series.Created = DateTime.Now;
                    db.Serieses.Add(series);
                    series.Episodes = new List<Episode>();
                }
            }
            //check for changes.
            if (series.SeriesName != tvdbSeries.SeriesName)
            {
                //update values.
                series.SeriesName = tvdbSeries.SeriesName;
                series.Updated = DateTime.Now;
            }

            //update/add epsidoes.
            List<Episode> seriesEpisodes = series.Episodes.ToList();
            //if (series.Episodes != null)
            //{
            //    seriesEpisodes = series.Episodes.ToList();
            //}
            //else
            //{
            //    seriesEpisodes = new List<Episode>();
            //}
            foreach (TvdbEpisode tvdpEpisode in tvdbSeries.Episodes)
            {
                Episode episode = seriesEpisodes.SingleOrDefault(oo => oo.TVDBEpisodeId == tvdpEpisode.Id);
                if (episode == null)
                {
                    episode = new Episode();
                    episode.TVDBEpisodeId = tvdpEpisode.Id;
                    episode.Created = DateTime.Now;
                    series.Episodes.Add(episode);
                    //episode.SeriesId = series.SeriesId;
                    //db.Episodes.Add(episode);
                }
                //check for changes.
                if (episode.EpisodeTitle != tvdpEpisode.EpisodeName
                    || episode.EpisodeNumber != tvdpEpisode.EpisodeNumber
                    || episode.Season != tvdpEpisode.SeasonNumber)
                {
                    //at least one change so update values.
                    episode.EpisodeTitle = tvdpEpisode.EpisodeName;
                    episode.EpisodeNumber = tvdpEpisode.EpisodeNumber;
                    episode.Season = tvdpEpisode.SeasonNumber;
                    episode.Updated = DateTime.Now;
                }
            }

            db.SaveChanges();

            return series;
        }
    }
}