using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TvdbLib;
using TvdbLib.Cache;
using System.IO;
using System.Configuration;

namespace Web.Models
{
    public class TVDBRepository
    {
        public static TvdbHandler m_tvdbHandler = null;
        public static ICacheProvider m_cacheProvider = null;

        public static TvdbHandler GetTvdbHandler()
        {
            if (m_tvdbHandler == null)
            {
                //using caching.
                if (m_cacheProvider == null)
                {
                    //setup caching folder in App_Data.
                    string rootFolder = HttpContext.Current.Server.MapPath("~/App_Data/TVDB");
                    if (!Directory.Exists(rootFolder)) Directory.CreateDirectory(rootFolder);
                    m_cacheProvider = new XmlCacheProvider(rootFolder);
                }

                //create new tvdbHandler.
                m_tvdbHandler = new TvdbHandler(m_cacheProvider, ConfigurationManager.AppSettings["TVDB_API_KEY"]);
            }
            return m_tvdbHandler;
        }
    }
}