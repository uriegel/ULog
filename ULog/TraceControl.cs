using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ULog
{
    public static class TraceControl
    {
        public static EventWaitHandle Event { get => traceChangedEvent; }

        public static string[] GetFilterList()
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

        public static void AddFilter(string item)
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

        public static void RemoveFilter(string item)
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

        static TraceControl()
        {
            try
            {
                var customSecurity = new MemoryMappedFileSecurity();
                customSecurity.AddAccessRule(new System.Security.AccessControl.AccessRule<MemoryMappedFileRights>("everyone", 
                    MemoryMappedFileRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));
                traceForceFilterFile = MemoryMappedFile.CreateOrOpen("ULog.TraceSwitcherChangedFile", size, MemoryMappedFileAccess.ReadWriteExecute,
                    MemoryMappedFileOptions.None, CustomSecurity, System.IO.HandleInheritability.Inheritable);
            }
            catch { }
        }

        const int size = 1000;
        static object locker = new object();
        static EventWaitHandle traceChangedEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "ULog.TraceSwitcherChanged");

        static MemoryMappedFile traceForceFilterFile;
    }
}
