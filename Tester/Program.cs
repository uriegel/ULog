using System;
using System.Threading;
using ULog;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            (new Thread(() =>
            {
                Console.ReadLine();
                stopEvent.Set();
            })).Start();

            using (var logger = new Logger(new LogSettings
            {
                Category = "Main",
                TraceKindConsole = TraceKind.Verbose
            }, new TraceControl()))
            {
                logger.Info("=====================================");
                logger.Trace(() => "running...");
                logger.Info("=====================================");

                while (true)
                {
                    logger.Info("Test");
                    logger.Trace(() => "running...");
                    logger.Verbose(() => "running and running...");
                    if (stopEvent.WaitOne(1000))
                        break;
                }
            }

        }
        static ManualResetEvent stopEvent = new ManualResetEvent(false);
    }
}
