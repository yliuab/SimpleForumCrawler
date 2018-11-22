using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Logic
{
    public class AbotRepository
    {
        private PoliteWebCrawler _crawler;
        private CancellationTokenSource _cancellationTokenSource;

        private OnCompleteCallbackDelegate _onCompleteCallback;
        private decimal _callbackId;

        public AbotRepository()
        {
        }

        public void CrawWebPage(string url, OnCompleteCallbackDelegate onCompleteCallback, decimal callbackId = -1)
        {
            _callbackId = callbackId;
            _onCompleteCallback = onCompleteCallback;
            Initialize();

            CrawlResult result = _crawler.Crawl(new Uri(url), _cancellationTokenSource);
            
            if (result.ErrorOccurred)
            {
                Console.WriteLine(string.Format("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message));
            }
            else
            {
                Console.WriteLine(string.Format("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri));
            }
        }

        private void Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _crawler = new PoliteWebCrawler();
            _crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
            _crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
            _crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
            _crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;
        }

        private void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
        }

        private void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
            else
                Console.WriteLine("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);

            _onCompleteCallback.Invoke(_callbackId, crawledPage.Content.Text);
        }

        private void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
        }

        private void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
        }
    }
}
