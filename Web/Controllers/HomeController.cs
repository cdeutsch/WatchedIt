using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Reporting;
using Web.Models;
using Web.Infrastructure.Session;
using TvdbLib.Data;

namespace Web.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        SiteDB _db;
        IUserSession _userSession;

        public HomeController(IUserSession UserSession)
        {
            _db = new SiteDB();
            _userSession = UserSession;
        }

        public ActionResult Index()
        {
            WatchedList model = new WatchedList(_db, _userSession.GetCurrentUserId());

            return View(model);
        }

        [HttpPost]
        public ActionResult Search(string searchSeriesName)
        {
            //search for series.
            WatchedList model = new WatchedList(_db, _userSession.GetCurrentUserId());
            model.SearchResults = TVDBRepository.GetTvdbHandler().SearchSeries(searchSeriesName);
            
            //System.Web.Mvc.Html.InputExtensions.CheckBoxF

            return View("Index", model);
        }

        [HttpPost]
        public ActionResult WatchSeries(string SeriesIds)
        {
            long userId = _userSession.GetCurrentUserId();
            
            //add list of seriesIds to Watch list.
            foreach (string sId in SeriesIds.Split(','))
            {
                int iId;
                if (int.TryParse(sId, out iId))
                {
                    //make sure each Series is in our local db.
                    Series series = SeriesRepository.AddSeries(_db, userId, iId);
                    //add this series to this user's watch list.
                    WatchedRepository.WatchSeries(_db, userId, series.SeriesId);
                }
            }

            //load up watched Series.
            WatchedList model = new WatchedList(_db, userId);
            
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult WatchEpisodes(string EpisodeIds)
        {
            long userId = _userSession.GetCurrentUserId();

            WatchedRepository.SetWatched(_db, EpisodeIds, userId);

            //load up watched Series.
            WatchedList model = new WatchedList(_db, userId);

            return View("Index", model);
        }


        public ActionResult Series(long Id)
        {
            WatchedList model = new WatchedList(_db, _userSession.GetCurrentUserId(), Id);

            return View("Index", model);
        }

        public ActionResult About()
        {
            return View();
        }
    }

    public class WatchedList
    {
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
    }
}
