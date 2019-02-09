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

        public string[] GetFilterList()
        {
            if (traceForceFilterFile == null)
                return null;
            lock (locker)
            {
                var bytes = new byte[size];
                using (var accessor = traceForceFilterFile.CreateViewAccessor())
                    accessor.ReadArray(0, bytes, 0, size);
                var list = Encoding.UTF8.GetString(bytes).TrimEnd(new[] { '\0' });
                return list.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public void AddFilter(string item)
        {
            if (traceForceFilterFile == null)
                return;
            lock (locker)
            {
                var list = GetFilterList();
                list = list.Concat(new[] { item }).Distinct().ToArray();
                var listText = string.Join(";", list);
                var bytesText = Encoding.UTF8.GetBytes(listText);
                using (var accessor = traceForceFilterFile.CreateViewAccessor())
                    accessor.WriteArray(0, bytesText, 0, Math.Min(size, bytesText.Length));
                traceChangedEvent.Set();
            }
        }

        public void RemoveFilter(string item)
        {
            if (traceForceFilterFile == null)
                return;
            lock (locker)
            {
                var list = GetFilterList();
                list = list.Where(n => n != item).ToArray();
                var listText = string.Join(";", list);
                var bytesText = Encoding.UTF8.GetBytes(listText);
                using (var accessor = traceForceFilterFile.CreateViewAccessor())
                {
                    var nill = new byte[size];
                    accessor.WriteArray(0, nill, 0, nill.Length);
                    accessor.WriteArray(0, bytesText, 0, Math.Min(size, bytesText.Length));
                }
                traceChangedEvent.Set();
            }
        }

        protected const int size = 1000;
        object locker = new object();
        EventWaitHandle traceChangedEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "ULog.TraceSwitcherChanged");

        MemoryMappedFile traceForceFilterFile;
    }
}
