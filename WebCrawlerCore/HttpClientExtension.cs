using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace WebCrawlerCore
{
    static class HttpClientExtension
    {
        public static HttpResponseMessage Get(this HttpClient client, Uri url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            return client.Send(request);
        }

        public static byte[] ReadAsByteArray(this HttpContent content)
        {
            var contentLength = content.Headers.ContentLength;
            if(contentLength.HasValue)
            {
                var buffer = new byte[contentLength.Value];
                using var outStream = new MemoryStream(buffer, writable: true);
                content.CopyTo(outStream, null, CancellationToken.None);

                return buffer;
            }
            else
            {
                using var outStream = new MemoryStream();
                content.CopyTo(outStream, null, CancellationToken.None);
                return outStream.ToArray();
            }
        }
    }
}
