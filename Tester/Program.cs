using System;
using System.Threading;
using ULog;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new Logger("Main", new TraceControl());
            logger.Info("=====================================");
            logger.Trace(() => "running...");
            logger.Info("=====================================");

            while (true)
            {
                logger.Info("Test");
                logger.Trace(() => "running...");
                logger.Verbose(() => "running and running...");
                Thread.Sleep(1000);
            }
        }
    }
}
