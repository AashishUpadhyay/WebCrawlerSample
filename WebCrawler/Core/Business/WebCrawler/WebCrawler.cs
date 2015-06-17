using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public class WebCrawler : IWebCrawler
    {
        private IConcurrentQueueRepository<PageToBeCrawled> crawlingRepository = new ConcurrentQueueRepository<PageToBeCrawled>(new ConcurrentQueue<PageToBeCrawled>());
        private List<Task> parallelTasks = new List<Task>();
        int parallelTaskCounter = 0;
        private List<Uri> allLinksVisitedCollection = new List<Uri>();
        private static readonly object _syncObject = new object();
        private bool _terminateCrawling;
        public void Crawl(string urlToCrawl)
        {
            Uri uriResult;
            if (Utilities.IsValidURL(urlToCrawl, out uriResult))
            {
                AddtoVisitedLinkCollection(uriResult);
                Crawl();
            }
            else
            {
                Utilities.LogExceptions("Invalid URL");
            }
            LogAllVistedLinks();
        }

        #region Private Methods
        private void Crawl()
        {
            while (!_terminateCrawling)
            {
                var currentURL = string.Empty;
                try
                {
                    PageToBeCrawled nextPageToBeCrawled;
                    string maximumNoOfLinksToBeCrawled = ConfigurationManager.AppSettings["maximumNoOfLinksToBeCrawled"];
                    if (parallelTasks.Count < Convert.ToInt32(ConfigManager.maximumNoOfThreads))
                    {
                        if (crawlingRepository.GetNext(out nextPageToBeCrawled))
                        {
                            parallelTaskCounter = parallelTaskCounter + 1;
                            currentURL = nextPageToBeCrawled.pageURI.AbsoluteUri;
                            parallelTasks.Add(Task.Factory.StartNew(() => CrawlPage(nextPageToBeCrawled)));
                        }
                    }

                    if (parallelTaskCounter == 0)
                    {
                        _terminateCrawling = true;
                    }
                }
                catch (Exception)
                {
                    var message = "Failure experienced while crawling.";
                    if (!string.IsNullOrEmpty(currentURL))
                    {
                        message += " Link Name :" + currentURL;
                    }
                    Utilities.LogExceptions(message);
                }
            }
            Task.WaitAll(parallelTasks.ToArray());
        }
        private void CrawlPage(PageToBeCrawled pageToBeCrawled)
        {
            var links = GetLinks(pageToBeCrawled);
            foreach (var link in links)
            {
                lock (_syncObject)
                {
                    if ((allLinksVisitedCollection.Count < Convert.ToInt32(ConfigManager.maximumNoOfLinksToBeCrawled)))
                    {
                        if (!allLinksVisitedCollection.Contains(link))
                        {
                            AddtoVisitedLinkCollection(link);
                        }
                    }
                    else
                    {
                        _terminateCrawling = true;
                        break;
                    }
                }
            }
            lock (_syncObject)
            {
                parallelTaskCounter = parallelTaskCounter - 1;
            }
        }
        private void AddtoVisitedLinkCollection(Uri inputLink)
        {
            var response = Utilities.GetHttpResponse(inputLink.AbsoluteUri);
            var newpageToBeCrawled = new PageToBeCrawled(inputLink, response);
            crawlingRepository.Add(newpageToBeCrawled);
            allLinksVisitedCollection.Add(newpageToBeCrawled.pageURI);
        }
        private List<Uri> GetLinks(PageToBeCrawled pageToBeCrawled)
        {
            var baseUri = pageToBeCrawled.pageURI;
            List<Uri> returnValue = new List<Uri>();
            HtmlNodeCollection anchorTags = pageToBeCrawled.htmlDocument.DocumentNode.SelectNodes("//a[@href]");

            if (anchorTags != null)
            {
                foreach (HtmlNode anchorTag in anchorTags)
                {
                    var href = anchorTag.Attributes["href"].Value;
                    var link = GetValidLink(href);
                    if (!string.IsNullOrEmpty(link))
                    {
                        Uri linkedUri;
                        if (Utilities.IsValidURL(link, out linkedUri))
                        {
                            returnValue.Add(linkedUri);
                        }
                        else
                        {
                            linkedUri = new Uri(baseUri, link);
                            returnValue.Add(linkedUri);
                        }
                    }
                }
            }
            return returnValue;
        }
        private string GetValidLink(string href)
        {
            var hrefSplitArray = href.Split('#');
            var link = hrefSplitArray[0];
            if (!ConfigManager.linksToBeIgnored.Contains(link))
            {
                return link;
            }
            return string.Empty;
        }
        private void LogAllVistedLinks()
        {
            var loggingPath = ConfigManager.visitedLinkLog;
            if (!Directory.Exists(loggingPath))  // if it doesn't exist, create
                Directory.CreateDirectory(loggingPath);

            var logMessageBuilder = new StringBuilder();

            allLinksVisitedCollection.ForEach(u =>
            {
                logMessageBuilder.AppendLine(u.AbsoluteUri);
            });

            if (!(logMessageBuilder.ToString().Trim().Length > 0))
            {
                logMessageBuilder.AppendLine("Nothing to log!!");
            }
            File.WriteAllText(Path.Combine(loggingPath, "links_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".txt"), logMessageBuilder.ToString());
        }
        #endregion Private Methods
    }
}
