using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using WebCrawlerCore;

namespace SyncCrawler
{
    static class Program
    {
        private static readonly TimeSpan crawlDelay = TimeSpan.FromSeconds(15);

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                var exeName = Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                Console.WriteLine($"Usage: {exeName} url ...");
                return;
            }

            var programLog = ConsoleLog.Create(typeof(Program));

            string resultsArchive = $"crawler-{DateTime.Now:yyyyMMdd}.zip";
            File.Delete(resultsArchive);

            using (var outZip = ZipFile.Open(resultsArchive, ZipArchiveMode.Create))
            using (var pageStore = new ZipArchiveWebPageStore(outZip, ConsoleLog.Create<ZipArchiveWebPageStore>(cid: "zip")))
            {
                programLog.WriteEntry("Created new archive {0}", resultsArchive);

                var uris = args.Select(x => new Uri(x)).ToArray();
                var logs = uris.Select(x => ConsoleLog.Create(typeof(Program), cid: x.Host)).ToArray();
                var crawlers = uris.Select((x, i) => new WebCrawler(new HttpClient(), logs[i].CreateSubLog<WebCrawler>(), x)).ToArray();
                var nextCrawlTimes = crawlers.Select(x => DateTime.MinValue).ToArray();

                foreach (var i in Enumerable.Range(0, uris.Length))
                    programLog.WriteEntry("Beginning crawl through {0}", uris[i]);

                int crawlerIndex = 0;
                while(crawlers.Any(x => !x.Done))
                {
                    WebCrawler crawler;
                    int currentIndex;
                    do
                    {
                        currentIndex = crawlerIndex % crawlers.Length;
                        crawler = crawlers[currentIndex];
                        crawlerIndex += 1;
                    }
                    while (crawler.Done && nextCrawlTimes[currentIndex] > DateTime.Now);

                    nextCrawlTimes[currentIndex] = DateTime.Now.Add(crawlDelay);
                    var page = crawler.CrawlNext();
                    if (page == null)
                        continue;

                    pageStore.StorePage(page);
                    programLog.WriteEntry("Pushed result page");

                    if (crawler.Done)
                    {
                        nextCrawlTimes[currentIndex] = DateTime.MaxValue;
                        Uri uri = uris[currentIndex];
                        programLog.WriteEntry("Finished crawling {0}", uri);
                    }

                    var now = DateTime.Now;
                    var nextCrawl = nextCrawlTimes.Min();
                    if (now < nextCrawl && nextCrawl < DateTime.MaxValue)
                    {
                        var diff = nextCrawl - now;
                        programLog.WriteEntry("Waiting for {0:N} milliseconds", diff.TotalMilliseconds);

                        Thread.Sleep(diff);
                    }
                }
            }

            Console.ReadKey(intercept: true);
        }
    }
}
