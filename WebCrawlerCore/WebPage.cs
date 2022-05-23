using System;
using System.IO;

namespace WebCrawlerCore
{
    public class WebPage
    {
        public Uri PageUrl { get; }

        private readonly byte[] bodyBuffer;

        public WebPage(Uri pageUrl, byte[] bodyBuffer)
        {
            PageUrl = pageUrl;

            this.bodyBuffer = bodyBuffer;
        }

        public Stream OpenBody()
            => new MemoryStream(bodyBuffer, writable: false);
    }
}
