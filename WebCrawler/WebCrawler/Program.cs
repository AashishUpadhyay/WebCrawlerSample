using Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            var urlToCrawl = ConfigManager.urlToCrawl;

            if (string.IsNullOrEmpty(urlToCrawl))
            {
                Console.WriteLine("Please enter a URL to crawl");
                urlToCrawl = Console.ReadLine();
            }

            var crawlerIns = new Core.WebCrawler();
            crawlerIns.Crawl(urlToCrawl);
        }
    }
}
