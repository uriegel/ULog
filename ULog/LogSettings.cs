using System;
using System.Collections.Generic;
using System.Text;

namespace ULog
{
    public struct LogSettings
    {
        public TraceKind TraceKindConsole { get; set; }
        public string Category { get; set; }
    }
}
