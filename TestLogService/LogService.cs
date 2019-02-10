using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ULog;
using ULogWin;

namespace TestLogService
{
    public partial class LogService : ServiceBase
    {
        public LogService() => InitializeComponent();

        protected override void OnStart(string[] args) => (new Thread(Run)).Start();

        protected override void OnStop() => stopEvent.Set();

        void Run()
        {
            using (var logger = new Logger(new LogSettings
            {
                Category = "Main",
                LogFile = Path.Combine(@"C:\ProgramData", "LogTest", "test.log")
            }, new WindowsTraceControl()))
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
