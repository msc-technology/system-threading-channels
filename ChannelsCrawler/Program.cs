using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Channels;
using System.Threading.Tasks;
using WebCrawlerCore;

namespace ChannelsCrawler
{
    static class Program
    {
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
            var channel = Channel.CreateUnbounded<WebPage>();

            var writingTask = StorePagesAsync(log, channel.Reader).ConfigureAwait(true);
            try
            {
                var uris = args.Select(x => new Uri(x)).ToArray();
                var logs = uris.Select(x => ConsoleLog.Create(typeof(Program), cid: x.Host)).ToArray();
                var crawlingTasks = uris
                    .Select((x, i) => CrawlOneAsync(x, channel.Writer, logs[i], i))
                    .ToArray();

                await Task.WhenAll(crawlingTasks).ConfigureAwait(false);
            }
            finally
            {
                channel.Writer.Complete();
            }
            await writingTask;

            Console.ReadKey(intercept: true);
        }

        static async Task CrawlOneAsync(Uri url, ChannelWriter<WebPage> output, ILog log, int index)
        {
            var crawler = new WebCrawler(new HttpClient(), log.CreateSubLog<WebCrawler>(), url);

            log.WriteEntry("Beginning crawl through {0}", url);
            while (!crawler.Done)
            {
                WebPage page = await crawler.CrawlNextAsync().ConfigureAwait(false);
                if (page != null)
                {
                    await output.WriteAsync(page).ConfigureAwait(false);
                    log.WriteEntry("Pushed result page");
                }

                await Task.Delay(crawlDelay).ConfigureAwait(false);
            }
            log.WriteEntry("Finished crawling {0}", index, url);
        }

        static async Task StorePagesAsync(ILog log, ChannelReader<WebPage> input)
        {
            string resultsArchive = $"crawler-{DateTime.Now:yyyyMMdd}.zip";
            File.Delete(resultsArchive);

            using (var outZip = ZipFile.Open(resultsArchive, ZipArchiveMode.Create))
            using (var pageStore = new ZipArchiveWebPageStore(outZip, ConsoleLog.Create<ZipArchiveWebPageStore>(cid: "zip")))
            {
                log.WriteEntry("Created new archive {0}", resultsArchive);

                while (await input.WaitToReadAsync().ConfigureAwait(false))
                {
                    while (input.TryRead(out WebPage nextPage))
                    {
                        await pageStore.StorePageAsync(nextPage);
                    }
                }
            }

            log.WriteEntry("Channel completed");
        }
    }
}
