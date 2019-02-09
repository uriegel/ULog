﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ULog
{
    public class Logger : IDisposable
    {
        // TODO: Control Windows Event Log (FATAL ERROR WARNING INFO)
        // TODO: Log to Console (FATAL ERROR WARNING INFO)
        // TODO: Control TRACE and VERBOSE
        // TODO: Blocking Collection
        public Logger(string category, TraceControl traceControl)
        {
            this.category = category;
            this.traceControl = traceControl;
            ThreadPool.RegisterWaitForSingleObject(traceControl.Event, (s, b) => TraceChanged(), null, -1, false);
        }

        public void Fatal(string text) { Console.WriteLine($"{DateTime.Now} FATAL {category}-{text}"); }
        public void Error(string text) { Console.WriteLine($"{DateTime.Now} ERROR {category}-{text}"); }
        public void Warning(string text) { Console.WriteLine($"{DateTime.Now} WARN  {category}-{text}"); }
        public void Info(string text) { Console.WriteLine($"{DateTime.Now} INFO  {category}-{text}"); }
        public void Trace(Func<string> getText)
        {
            if (tracing)
                Console.WriteLine($"{DateTime.Now} TRACE {category}-{getText()}");
        }
        public void Verbose(Func<string> getText)
        {
            if (tracing && verbose)
                Console.WriteLine($"{DateTime.Now} VERB  {category}-{getText()}");
        }

        void TraceChanged()
        {
            try
            {
                var list = traceControl.GetFilterList();
                tracing = list != null;
            }
            catch { }
        }

        readonly string category;
        readonly TraceControl traceControl;
        bool tracing;
        bool verbose;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
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
