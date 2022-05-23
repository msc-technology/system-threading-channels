namespace WebCrawlerCore
{
    public interface ILog
    {
        void WriteEntry(string format, params object[] values);

        ILog CreateSubLog<TOwner>();
    }
}
