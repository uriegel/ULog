using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ULog
{
    public class TraceControl
    {
        public EventWaitHandle Event { get => traceChangedEvent; }

        public TraceControl()
        {
            try
            {
                traceForceFilterFile = Create();
            }
            catch { }
        }

        protected virtual MemoryMappedFile Create()
            => MemoryMappedFile.CreateOrOpen("ULog.TraceSwitcherChangedFile", size);

        public TraceKind GetKind()
        {
            if (traceForceFilterFile == null)
                return TraceKind.None;
            lock (locker)
            {
                var bytes = new byte[size];
                using (var accessor = traceForceFilterFile.CreateViewAccessor())
                    accessor.ReadArray(0, bytes, 0, size);
                var str = Encoding.UTF8.GetString(bytes).Trim(new[] { '\0' });
                switch (str)
                {
                    case "1":
                        return TraceKind.Trace;
                    case "2":
                        return TraceKind.Verbose;
                    default:
                        return TraceKind.None;
                }
            }
        }

        public void SetKind(TraceKind kind)
        {
            if (traceForceFilterFile == null)
                return;
            lock (locker)
            {
                string str;
                switch (kind)
                {
                    case TraceKind.Trace:
                        str = "1";
                        break;
                    case TraceKind.Verbose:
                        str = "2";
                        break;
                    default:
                        str = "0";
                        break;
                }
                var bytesText = Encoding.UTF8.GetBytes(str);
                using (var accessor = traceForceFilterFile.CreateViewAccessor())
                    accessor.WriteArray(0, bytesText, 0, Math.Min(size, bytesText.Length));
                traceChangedEvent.Set();
            }
        }

        protected const int size = 1;
        object locker = new object();
        // Not supported on Linux
        //EventWaitHandle traceChangedEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "ULog.TraceSwitcherChanged");
        EventWaitHandle traceChangedEvent = new EventWaitHandle(false, EventResetMode.AutoReset);

        MemoryMappedFile traceForceFilterFile;
    }
}
