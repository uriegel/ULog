using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULog;
using ULogWin;

namespace WindowsTraceSwitcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var control = new WindowsTraceControl();
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
