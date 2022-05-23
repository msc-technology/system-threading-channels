using System;
using System.Threading;

namespace WebCrawlerCore
{
    public class ConsoleLog : ILog
    {
        private readonly string ownerName;
        private readonly string cid;

        public static ILog Create<TOwner>(string cid = null) => new ConsoleLog(typeof(TOwner).Name, cid);
        public static ILog Create(Type owner, string cid = null) => new ConsoleLog(owner.Name, cid);
        public static ILog Create(string ownerName, string cid = null) => new ConsoleLog(ownerName, cid);

        private ConsoleLog(string ownerName, string cid)
        {
            this.ownerName = ownerName;
            this.cid = cid;
        }

        public void WriteEntry(string format, params object[] values)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            Console.Write("[thread:{0}]", threadId);
            Console.Write("[owner:{0}]", ownerName);
            Console.Write("[cid:{0}]", cid ?? " ");
            Console.WriteLine(format, values);
        }

        public ILog CreateSubLog<TOwner>()
        {
            return new ConsoleLog(typeof(TOwner).Name, cid);
        }
    }
}
