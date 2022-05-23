using System;
using System.Text;
using System.Threading;

namespace WebCrawlerCore
{
    public class ConsoleLog : ILog
    {
        private readonly string ownerName;
        private readonly string cid;
        private readonly StringBuilder messageBuilder = new StringBuilder();

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

            messageBuilder.AppendFormat("[thread:{0:D2}]", threadId);
            messageBuilder.AppendFormat("[cid:{0}]", cid ?? " ");
            messageBuilder.AppendFormat("[owner:{0}]", ownerName);
            messageBuilder.AppendFormat(format, values);

            var msg = messageBuilder.ToString();
            messageBuilder.Clear();

            Console.WriteLine(msg);
        }

        public ILog CreateSubLog<TOwner>()
        {
            return new ConsoleLog(typeof(TOwner).Name, cid);
        }
    }
}
