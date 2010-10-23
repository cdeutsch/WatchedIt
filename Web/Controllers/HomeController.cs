using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Reporting;
using Web.Models;
using Web.Infrastructure.Session;

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
        public long SelectedSeriesId { get; set; }
        public List<WatchedSeries> WatchedSerieses { get; set; }
        public List<WatchedEpisode> WatchedEpisodes { get; set; }

        public WatchedList(SiteDB db, long UserId)
        {
            WatchedSerieses = WatchedRepository.GetAllSeries(db, UserId).ToList();
        }

        public WatchedList(SiteDB db, long UserId, long SeriesId)
        {
            SelectedSeriesId = SeriesId;
            WatchedSerieses = WatchedRepository.GetAllSeries(db, UserId).ToList();
            WatchedEpisodes = WatchedRepository.GetAllEpisodes(db, UserId, SeriesId).ToList();
        }
    }
}
