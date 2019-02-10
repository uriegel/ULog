using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULogWin
{
    // TODO: Windows Event Log (FATAL ERROR WARNING INFO)
    public class WindowsTraceControl : ULog.TraceControl
    {
        protected override MemoryMappedFile Create()
        {
            var customSecurity = new MemoryMappedFileSecurity();
            customSecurity.AddAccessRule(new System.Security.AccessControl.AccessRule<MemoryMappedFileRights>("everyone",
                MemoryMappedFileRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));
            return MemoryMappedFile.CreateOrOpen("ULog.TraceSwitcherChangedFile", size, MemoryMappedFileAccess.ReadWriteExecute,
                MemoryMappedFileOptions.None, customSecurity, System.IO.HandleInheritability.Inheritable);
        }
    }
}
