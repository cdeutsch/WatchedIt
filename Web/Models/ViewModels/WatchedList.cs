using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TvdbLib.Data;

namespace Web.Models
{
    public class WatchedList
    {
        public bool EditMode { get; set; }
        public long UserId { get; set; }
        public long SelectedSeriesId { get; set; }
        public List<WatchedSeries> WatchedSerieses { get; set; }
        public List<WatchedEpisodeStatus> WatchedEpisodes { get; set; }
        public List<TvdbSearchResult> SearchResults { get; set; }

        public WatchedList(SiteDB db, long UserId)
        {
            this.UserId = UserId;
            WatchedSerieses = WatchedRepository.GetAllSeries(db, UserId).ToList();
        }

        public WatchedList(SiteDB db, long UserId, long SeriesId)
        {
            SelectedSeriesId = SeriesId;
            WatchedSerieses = WatchedRepository.GetAllSeries(db, UserId).ToList();
            WatchedEpisodes = WatchedRepository.GetAllEpisodes(db, UserId, SeriesId);
        }

        public string SelectedSeriesFriendlyName
        {
            get
            {
                WatchedSeries ws = WatchedSerieses.SingleOrDefault(oo => oo.SeriesId == SelectedSeriesId);
                if (ws != null)
                {
                    return ws.Series.SeriesName;
                }
                else
                {
                    return "";
                }
            }
        }

        public string GetSelectedSeriesFriendlyNameOrDefault(string Default)
        {
            string seriesName = SelectedSeriesFriendlyName;
            if (!string.IsNullOrEmpty(seriesName))
            {
                return seriesName;
            }
            else
            {
                return Default;
            }
        }
    }
}