using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreExample.Utils
{
    public class ProjectVariables
    {
        public static string applicationLogPath = "D:\\Projects\\DotNetCoreExample\\Logs";
        public static string dbConnectionString = "User ID=merlin; Password=excalibur; Initial Catalog=MOVIEDB;Data Source=DESKTOP-KR5BURO\\SQLEXPRESS";

        public static int sessionTimeLimitMinute = 20;
        public static int cachingTimeLimitMinute = 12;
        public static int serviceSleepTimeMinute = 10;

        /** 
         * Log Burak: 
         *  URL should be finished with '/'
         **/
        public static string omdbURL= "http://www.omdbapi.com/";
        public static string omdbApiKey = "8ba264ca";

        /* constants */
        public static string MOVIE_CACHE_TAG = "MOVIE";
    }
}
