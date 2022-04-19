using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebCrawlerCore;

namespace TaskCrawler
{
    static class Program
    {
        // Task that never completes
        private static readonly Task<WebPage> nullTask = new TaskCompletionSource<WebPage>().Task;
        private static readonly TimeSpan crawlDelay = TimeSpan.FromSeconds(15);

        static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                var exeName = Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                Console.WriteLine($"Usage: {exeName} url ...");
                return;
            }

            var log = ConsoleLog.Create(typeof(Program));

            string resultsArchive = $"crawler-{DateTime.Now:yyyyMMdd}.zip";
            File.Delete(resultsArchive);

            using (var outZip = ZipFile.Open(resultsArchive, ZipArchiveMode.Create))
            using (var pageStore = new ZipArchiveWebPageStore(outZip, ConsoleLog.Create<ZipArchiveWebPageStore>()))
            {
                log.WriteEntry("Created new archive {0}", resultsArchive);

                var uris = args.Select(x => new Uri(x)).ToArray();
                var crawlers = uris.Select(x => new WebCrawler(new HttpClient(), ConsoleLog.Create<WebCrawler>(), x)).ToArray();

                foreach (var i in Enumerable.Range(0, uris.Length))
                    log.WriteEntry("[crawlerIx:{0}]Beginning crawl through {1}", i, uris[i]);

                var crawlTasks = crawlers.Select(x => x.CrawlNextAsync()).ToArray();
                do
                {
                    var finishedTask = await Task.WhenAny(crawlTasks).ConfigureAwait(false);
                    var finishedIndex = Array.IndexOf(crawlTasks, finishedTask);

                    var crawler = crawlers[finishedIndex];
                    crawlTasks[finishedIndex] = crawler.Done ? nullTask : CreateNextTask(crawler);

                    await pageStore.StorePageAsync(await finishedTask).ConfigureAwait(false);
                    log.WriteEntry("[crawlerIx:{0}]Pushed result page", finishedIndex);

                    if (crawler.Done)
                        log.WriteEntry("[crawlerIx:{0}]Finished crawling {1}", finishedIndex, uris[finishedIndex]);
                }
                while (crawlers.Any(x => !x.Done));
            }

            Console.ReadKey(intercept: true);
        }

        private static async Task<WebPage> CreateNextTask(WebCrawler crawler)
        {
            await Task.Delay(crawlDelay);
            return await crawler.CrawlNextAsync();
        }
    }
}
