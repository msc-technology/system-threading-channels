using System.Threading.Tasks;

namespace WebCrawlerCore
{
    public interface IWebPageStore
    {
        void StorePage(WebPage page);
        Task StorePageAsync(WebPage page);
    }
}
