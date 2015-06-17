using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    interface IWebCrawler
    {
        void Crawl(string urlToCrawl);
    }
}
