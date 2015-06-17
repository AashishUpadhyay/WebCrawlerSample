using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class PageToBeCrawled
    {
        public PageToBeCrawled()
        {
        }
        public PageToBeCrawled(Uri pageURI, string textContent)
        {
            this.pageURI = pageURI;
            this.textContent = textContent;
        }
        public Uri pageURI { get; set; }
        public string textContent { get; set; }
        public HtmlDocument htmlDocument
        {
            get
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(textContent);
                return doc;
            }
        }
    }
}
