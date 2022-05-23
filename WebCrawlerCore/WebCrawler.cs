using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace WebCrawlerCore
{

    public class WebCrawler
    {
        private static readonly Regex absoluteUrlRegex = new(@"\bhttp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*?(\/[a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?", RegexOptions.Compiled);
        private static readonly Regex anchorUrlRegex = new(@"href=""([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)""", RegexOptions.Compiled);

        private readonly HttpClient httpClient;
        private readonly ILog log;
        private readonly string crawlScope;

        private readonly HashSet<Uri> urlsToFetch;
        private readonly HashSet<Uri> urlsFetched;

        public bool Done => !urlsToFetch.Any() || urlsFetched.Count >= 10;

        public WebCrawler(HttpClient httpClient, ILog log, Uri firstPage)
        {
            this.httpClient = httpClient;
            this.log = log;
            this.crawlScope = firstPage.Authority;
            this.urlsToFetch = new HashSet<Uri> { firstPage };
            this.urlsFetched = new HashSet<Uri>();
        }

        public async Task<WebPage> CrawlNextAsync()
        {
            try
            {
                if (Done)
                    return null;

                var url = urlsToFetch.First();
                urlsToFetch.Remove(url);

                var notAlreadyDownloaded = urlsFetched.Add(url);

                if (notAlreadyDownloaded)
                    log.WriteEntry("Next url: {0}", url);
                else
                {
                    log.WriteEntry("Skipped, already downloaded");
                    return null;
                }

                var httpResponse = await httpClient.GetAsync(url).ConfigureAwait(false);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    log.WriteEntry("No response");
                    return null;
                }

                var httpResponseBody = await httpResponse.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                var page = new WebPage(url, httpResponseBody);

                using (var pageBody = page.OpenBody())
                using (var pageReader = new StreamReader(pageBody))
                {
                    log.WriteEntry("Matching urls in page");

                    string line;
                    int absoluteUrlCount = 0;
                    int relativeUrlCount = 0;
                    while ((line = await pageReader.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        absoluteUrlCount += FindNewAbsoluteUrls(line);
                        relativeUrlCount += FindNewRelativeUrls(page.PageUrl, line);
                    }
                    log.WriteEntry("Found {0} absolute and {1} relative new urls", absoluteUrlCount, relativeUrlCount);
                }

                return page;
            }
            catch (HttpRequestException ex)
            {
                log.WriteEntry("Request failed: {0}", ex.Message);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                log.WriteEntry("Request timeout: {0}", ex.Message);
                return null;
            }
        }

        public WebPage CrawlNext()
        {
            try
            {
                var url = urlsToFetch.First();
                urlsToFetch.Remove(url);

                var notAlreadyDownloaded = urlsFetched.Add(url);
                if (notAlreadyDownloaded)
                    log.WriteEntry("Next url: {0}", url);
                else
                {
                    log.WriteEntry("Skipped, already downloaded");
                    return null;
                }

                var httpResponse = httpClient.Get(url);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    log.WriteEntry("No response");
                    return null;
                }

                var httpResponseBody = httpResponse.Content.ReadAsByteArray();
                var page = new WebPage(url, httpResponseBody);

                using (var pageBody = page.OpenBody())
                using (var pageReader = new StreamReader(pageBody))
                {
                    log.WriteEntry("Matching urls in page");

                    string line;
                    int absoluteUrlCount = 0;
                    int relativeUrlCount = 0;
                    while ((line = pageReader.ReadLine()) != null)
                    {
                        absoluteUrlCount += FindNewAbsoluteUrls(line);
                        relativeUrlCount += FindNewRelativeUrls(page.PageUrl, line);
                    }
                    log.WriteEntry("Found {0} absolute and {1} relative new urls", absoluteUrlCount, relativeUrlCount);
                }

                return page;
            }
            catch (HttpRequestException ex)
            {
                log.WriteEntry("Request failed: {0}", ex.Message);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                log.WriteEntry("Request timeout: {0}", ex.Message);
                return null;
            }
        }

        private int FindNewAbsoluteUrls(string line)
        {
            var absoluteUris = 0;
            foreach (Match match in absoluteUrlRegex.Matches(line))
            {
                var newUrl = new Uri(match.Value, UriKind.Absolute);
                var sameAuthority = newUrl.Authority == crawlScope;
                var alreadyDownloaded = urlsFetched.Contains(newUrl);

                if (!sameAuthority || alreadyDownloaded)
                    continue;

                var notAlreadyQueued = urlsToFetch.Add(newUrl);
                if (notAlreadyQueued)
                    absoluteUris += 1;
            }

            return absoluteUris;
        }

        private int FindNewRelativeUrls(Uri currentPageUrl, string line)
        {
            var relativeUris = 0;
            foreach (Match match in anchorUrlRegex.Matches(line))
            {
                var newUrl = new Uri(currentPageUrl, match.Groups[1].Value);
                var alreadyDownloaded = urlsFetched.Contains(newUrl);

                if (alreadyDownloaded)
                    continue;

                var notAlreadyQueued = urlsToFetch.Add(newUrl);
                if (notAlreadyQueued)
                    relativeUris += 1;
            }

            return relativeUris;
        }
    }
}
