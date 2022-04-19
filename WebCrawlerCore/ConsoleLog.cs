using System;
using System.Threading;

namespace WebCrawlerCore
{
    public class ConsoleLog : ILog
    {
        private readonly string ownerName;

        public static ILog Create<TOwner>() => new ConsoleLog(typeof(TOwner).Name);
        public static ILog Create(Type owner) => new ConsoleLog(owner.Name);
        public static ILog Create(string ownerName) => new ConsoleLog(ownerName);

        private ConsoleLog(string ownerName)
        {
            this.ownerName = ownerName;
        }

        public void WriteEntry(string format, params object[] values)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            Console.Write("[thread:{0}]", threadId);
            Console.Write("[owner:{0}]", ownerName);
            Console.WriteLine(format, values);
        }
    }
}
