using System;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawlerCore
{
    public sealed class ZipArchiveWebPageStore : IWebPageStore, IDisposable
    {
        private readonly ZipArchive archive;
        private readonly ILog log;

        public ZipArchiveWebPageStore(ZipArchive archive, ILog log)
        {
            this.archive = archive;
            this.log = log;
        }

        public void Dispose()
        {
            archive.Dispose();
        }

        public void StorePage(WebPage page)
        {
            log.WriteEntry("Received contents of page {0}", page.PageUrl);

            var entryName = ReplaceFileSystemForbiddenChars(page.PageUrl.AbsoluteUri);
            log.WriteEntry("Saving as {0}", entryName);

            using var pageBody = page.OpenBody();
            using var newEntry = archive.CreateEntry(entryName).Open();
            pageBody.CopyTo(newEntry);
        }

        public async Task StorePageAsync(WebPage page)
        {
            log.WriteEntry("Received contents of page {0}", page.PageUrl);

            var entryName = ReplaceFileSystemForbiddenChars(page.PageUrl.AbsoluteUri);
            log.WriteEntry("Saving as {0}", entryName);

            using var pageBody = page.OpenBody();
            using var newEntry = archive.CreateEntry(entryName).Open();
            await pageBody.CopyToAsync(newEntry).ConfigureAwait(false);
        }

        private static string ReplaceFileSystemForbiddenChars(string sourceString)
        {
            var entryName = new StringBuilder(sourceString);
            entryName.Replace("\\", "%5C");
            entryName.Replace("/", "%2F");
            entryName.Replace(":", "%3A");
            entryName.Replace("*", "%2A");
            entryName.Replace("?", "%3F");
            entryName.Replace("\"", "%22");
            entryName.Replace("<", "%3C");
            entryName.Replace(">", "%3E");
            entryName.Replace("|", "%7C");

            return entryName.ToString();
        }
    }
}
