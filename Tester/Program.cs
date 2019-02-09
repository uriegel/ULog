using System;
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

            Console.WriteLine("Hello World!");
        }
    }
}
