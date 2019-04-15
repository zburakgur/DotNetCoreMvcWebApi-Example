using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreExample.Utils
{
    public class Functions
    {
        public static List<string> stringToList(string value, string splitValue)
        {
            List<string> returnList = new List<string>();

            string[] values = value.Split(splitValue);
            for (int i = 0; i < values.Length; i++)
                returnList.Add(values[i]);

            return returnList;
        }

        public static void EnsureNotNullCredentials(string user, string pass)
        {
            if (user.Trim() == "")
                throw new Exception("Credentials error. User name is null!");

            if (pass.Trim() == "")
                throw new Exception("Credentials error. Password is null!");
        }

        /* cache functions */

        public static void setCache<T>(IMemoryCache cache, string tag, T value)
        {
            MemoryCacheEntryOptions cacheExpirationOptions = new MemoryCacheEntryOptions();
            cacheExpirationOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(ProjectVariables.cachingTimeLimitMinute);
            cacheExpirationOptions.Priority = CacheItemPriority.Normal;
            cache.Set<T>(tag, value, cacheExpirationOptions);
        }

        public static bool getCache<T>(IMemoryCache cache, string tag, out T value)
        {
            return cache.TryGetValue<T>(tag, out value);
        }

        /* session functions */

        public static void setSession(ISession session, string tag, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            session.Set(tag, bytes);
        }

        public static string getSession(ISession session, string tag)
        {
            var bytes = default(byte[]);
            session.TryGetValue(tag, out bytes);

            if (bytes == null)
                return null;

            string value = Encoding.UTF8.GetString(bytes);     
            return value;            
        }
        
        /* log functions */

        public static void LogWebMethodError(string className, string methodName, Exception pEx)
        {
            string errorText = "Web service error!!\r\n  Error occured on web method: " + className + "::" + methodName +
                "\r\n  Exception: " + pEx.Message + "\r\n  Stack Trace: " + pEx.StackTrace + "\r\n";
            DebugOutput(DebugType.debugFile, errorText);
        }

        public enum DebugType { debugNone, debugConsole, debugFile };

        public static void DebugOutput(DebugType debugType, string str)
        {
            switch (debugType)
            {
                case DebugType.debugConsole:
                    DebugOutputConsole(str);
                    break;
                case DebugType.debugFile:
                    DebugOutputFile(str);
                    break;
            }
        }

        private static void DebugOutputConsole(string str)
        {
            Console.WriteLine(str);
        }

        private static DateTime DebugFileClearTime = DateTime.MinValue;
        private static object objDebugOutputFile = new object();
        private static void DebugOutputFile(string str)
        {
            lock (objDebugOutputFile) /* Perform thread-safe write */
            {
                string directory = ProjectVariables.applicationLogPath;

                DateTime currentTime = DateTime.Now;
                string fileName = String.Format("{0:0000}{1:00}{2:00}.txt",
                    currentTime.Year, currentTime.Month, currentTime.Day);

                string logStr = str;
                if (str != "")
                {
                    logStr = currentTime.ToString();
                    logStr += ": ";
                    logStr += str;
                }

                fileName = directory + @"\" + fileName;
                System.IO.StreamWriter writer = System.IO.File.AppendText(fileName);
                writer.WriteLine(logStr);
                writer.Close();

                TimeSpan span = currentTime - DebugFileClearTime;
                if (span.TotalMinutes > (24 * 60))
                {
                    /* Keep last 7 days */
                    DateTime minTime = currentTime - new TimeSpan(7, 0, 0, 0);
                    string minFileName = String.Format("{0:0000}{1:00}{2:00}",
                                    minTime.Year, minTime.Month, minTime.Day);
                    System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(directory);
                    System.IO.FileInfo[] dirFiles = dirInfo.GetFiles("*.TXT");
                    foreach (System.IO.FileInfo file in dirFiles)
                    {
                        string checkFileName = System.IO.Path.GetFileNameWithoutExtension(file.Name);
                        if (string.Compare(checkFileName, minFileName) < 0)
                            System.IO.File.Delete(file.FullName);
                    }

                    DebugFileClearTime = currentTime;
                }
            }
        }
    }
}
