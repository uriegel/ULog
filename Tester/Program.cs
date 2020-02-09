using System;
using System.IO;
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

            var logfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "LogTest", "test.log");
            using (var logger = new Logger(new LogSettings
            {
                Category = "Main",
                LogFile = logfile,
                TraceKindConsole = TraceKind.Verbose
            }, new TraceControl()))
            {
                logger.Info("=====================================");
                logger.Trace(() => "running...");
                logger.Info("=====================================");
                logger.Info($"Logfile: {logfile}");

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
