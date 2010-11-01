using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class WatchedEpisodeStatus : Episode
    {
        public long UserId { get; set; }
        public bool Watched { get; set; }

        //update watched status.
        public void SetWatched(SiteDB db, bool Watched)
        {
            WatchedRepository.SetWatched(db, EpisodeId, UserId, Watched);
        }

        public long FriendlySortOrder
        {
            get
            {
                return (Season * 10000) + EpisodeNumber;
            }
        }
    }
}