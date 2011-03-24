using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Infrastructure.Session;

namespace Web.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        SiteDB _db;
        IUserSession _userSession;

        protected long CurrentUserId { get; set; }

        public HomeController(IUserSession UserSession)
        {
            _db = new SiteDB();
            _userSession = UserSession;

            //since CurrentUserId is used a lot save it in a variable right away for easier to read code.
            if (System.Web.HttpContext.Current.Request.IsAuthenticated)
            {
                CurrentUserId = _userSession.GetCurrentUserId();
            }
        }

        public ActionResult Index()
        {
            WatchedList model = new WatchedList(_db, CurrentUserId);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string searchSeriesName)
        {
            //search for series.
            WatchedList model = new WatchedList(_db, CurrentUserId);
            model.SearchResults = TVDBRepository.GetTvdbHandler().SearchSeries(searchSeriesName);

            //System.Web.Mvc.Html.LinkExtensions.ActionLink(

            return View(model);
        }

        public ActionResult Edit()
        {
            WatchedList model = new WatchedList(_db, CurrentUserId);
            model.EditMode = true;

            return View("Index", model);
        }

        [HttpPost]
        public ActionResult DeleteSeries(long Id)
        {
            //delete the series.
            WatchedRepository.StopWatchingSeries(_db, CurrentUserId, Id);

            return RedirectToAction("Edit");
        }

        [HttpPost]
        public ActionResult WatchSeries(int[] SeriesIds)
        {
            long firstId = 0;

            //add list of seriesIds to Watch list.
            foreach (int iId in SeriesIds)
            {
                //make sure each Series is in our local db.
                Series series = SeriesRepository.AddSeries(_db, CurrentUserId, iId);
                //add this series to this user's watch list.
                WatchedRepository.WatchSeries(_db, CurrentUserId, series.SeriesId);

                //set first series if necessary.
                if (firstId == 0)
                {
                    firstId = series.SeriesId;
                }
            }

            //redirect to the first series.
            if (firstId > 0)
            {
                return RedirectToAction("Series", new { id = firstId });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult WatchEpisodes(long SelectedSeriesId, long[] EpisodeIds)
        {
            WatchedRepository.SetWatched(_db, SelectedSeriesId, EpisodeIds, CurrentUserId);

            return RedirectToAction("Series", new { Id = SelectedSeriesId });
        }

        /// <summary>
        /// Make watched using Ajax.
        /// </summary>
        /// <param name="EpisodeId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WatchEpisode(long EpisodeId, bool Watched)
        {
            WatchedRepository.SetWatched(_db, EpisodeId, CurrentUserId, Watched);

            return null;
        }

        /// <summary>
        /// Make watched using Ajax.
        /// </summary>
        /// <param name="EpisodeId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WatchSeason(long SeriesId, int Season, bool Watched)
        {
            WatchedRepository.SetWatched(_db, SeriesId, Season, CurrentUserId, Watched);

            return null;
        }

        public ActionResult Series(long Id)
        {
            WatchedList model = new WatchedList(_db, CurrentUserId, Id);

            return View("Index", model);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }
    }

}
