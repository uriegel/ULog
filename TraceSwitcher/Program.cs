using System;
using ULog;

namespace TraceSwitcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var control = new TraceControl();
            while (true)
            {
                Console.WriteLine("type 'trace' or 'verbose' or 'none'");
                var kind = Console.ReadLine();
                switch (kind)
                {
                    case "trace":
                        control.SetKind(TraceKind.Trace);
                        break;
                    case "verbose":
                        control.SetKind(TraceKind.Verbose);
                        break;
                    default:
                        control.SetKind(TraceKind.None);
                        break;
                }
            }
        }
    }
}
