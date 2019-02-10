using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ULog
{
    public class Logger : IDisposable
    {
        #region Constructor

        public Logger(LogSettings logSettings, TraceControl traceControl)
        {
            category = logSettings.Category;
            traceKindConsole = logSettings.TraceKindConsole;
            if (!string.IsNullOrEmpty(logSettings.LogFile))
            {
                var fi = new FileInfo(logSettings.LogFile);
                Directory.CreateDirectory(fi.DirectoryName);
                logFile = logSettings.LogFile;
            }
            this.traceControl = traceControl;
            ThreadPool.RegisterWaitForSingleObject(traceControl.Event, (s, b) => TraceChanged(), null, -1, false);
            (new Thread(RunLogging)
            {
                IsBackground = false
            }).Start();
        }

        #endregion

        #region Methods

        public void Fatal(string text) { queue.TryAdd(new LogItem(TraceKind.None, $"{DateTime.Now} FATAL {category}-{text}")); }

        public void Error(string text) { queue.TryAdd(new LogItem(TraceKind.None, $"{DateTime.Now} ERROR {category}-{text}")); }

        public void Warning(string text) { queue.TryAdd(new LogItem(TraceKind.None, $"{DateTime.Now} WARN  {category}-{text}")); }

        public void Info(string text) { queue.TryAdd(new LogItem(TraceKind.None, $"{DateTime.Now} INFO  {category}-{text}")); }

        public void Trace(Func<string> getText)
        {
            if (traceKind != TraceKind.None)
                queue.TryAdd(new LogItem(TraceKind.Trace, $"{DateTime.Now} TRACE {category}-{getText()}"));
        }

        public void Verbose(Func<string> getText)
        {
            if (traceKind == TraceKind.Verbose)
                queue.TryAdd(new LogItem(TraceKind.Verbose, $"{DateTime.Now} VERB  {category}-{getText()}"));
        }

        void TraceChanged()
        {
            try
            {
                traceKind = traceControl.GetKind();
            }
            catch { }
        }

        void RunLogging()
        {
            while (queue.TryTake(out var logItem, -1))
            {
                if (logItem.TraceKind == TraceKind.None 
                    || (traceKindConsole == TraceKind.Trace && (logItem.TraceKind == TraceKind.Trace))
                    || (traceKindConsole == TraceKind.Verbose && (logItem.TraceKind == TraceKind.Trace || logItem.TraceKind == TraceKind.Verbose)))
                    Console.WriteLine(logItem.Text);
                if (!string.IsNullOrEmpty(logFile))
                    using (var sw = File.AppendText(logFile))
                        sw.WriteLine(logItem.Text);
            }
            queue.Dispose();
        }

        #endregion

        #region Types

        struct LogItem
        {
            public LogItem(TraceKind traceKind, string text)
            {
                Text = text;
                TraceKind = traceKind;
            }
            public TraceKind TraceKind { get; }
            public string Text { get; }
        }

        #endregion

        #region Fields

        readonly string category;
        readonly TraceKind traceKindConsole;
        readonly TraceControl traceControl;
        readonly BlockingCollection<LogItem> queue = new BlockingCollection<LogItem>();
        readonly string logFile;
        TraceKind traceKind;

        #endregion

        #region IDisposable Support

        bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    queue.CompleteAdding();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Logger() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
