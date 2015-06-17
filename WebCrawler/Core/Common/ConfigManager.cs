using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ConfigManager
    {
        public static string urlToCrawl = GetConfigValue("urlToCrawl");
        public static List<string> linksToBeIgnored = GetConfigValue("linksToBeIgnored").Split(new string[] { "||" }, StringSplitOptions.None).ToList();
        public static string maximumNoOfThreads = GetConfigValue("maximumNoOfThreads");
        public static string maximumNoOfLinksToBeCrawled = GetConfigValue("maximumNoOfLinksToBeCrawled");
        public static string errorLogPath = GetConfigValue("errorLogPath");
        public static string visitedLinkLog = GetConfigValue("VisitedLinkLog");

        private static string GetConfigValue(string key)
        {
            return (ConfigurationManager.AppSettings[key]);
        }
    }
}
