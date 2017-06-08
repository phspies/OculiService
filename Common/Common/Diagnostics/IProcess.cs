using System;using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace OculiService.Common.Diagnostics
{
  public interface IProcess
  {
    int BasePriority { get; }

    int ExitCode { get; }

    DateTime ExitTime { get; }

    IntPtr Handle { get; }

    int HandleCount { get; }

    bool HasExited { get; }

    int Id { get; }

    string MachineName { get; }

    ProcessModule MainModule { get; }

    IntPtr MainWindowHandle { get; }

    string MainWindowTitle { get; }

    IntPtr MaxWorkingSet { get; set; }

    IntPtr MinWorkingSet { get; set; }

    ProcessModuleCollection Modules { get; }

    long NonpagedSystemMemorySize { get; }

    long PagedMemorySize { get; }

    long PagedSystemMemorySize { get; }

    long PeakPagedMemorySize { get; }

    long PeakVirtualMemorySize { get; }

    long PeakWorkingSet { get; }

    bool PriorityBoostEnabled { get; }

    ProcessPriorityClass PriorityClass { get; set; }

    long PrivateMemorySize { get; }

    TimeSpan PrivilegedProcessorTime { get; }

    string ProcessName { get; }

    IntPtr ProcessorAffinity { get; set; }

    bool Responding { get; }

    int SessionId { get; }

    StreamReader StandardError { get; }

    StreamWriter StandardInput { get; }

    StreamReader StandardOutput { get; }

    ProcessStartInfo StartInfo { get; set; }

    DateTime StartTime { get; }

    ISynchronizeInvoke SynchronizingObject { get; set; }

    ProcessThreadCollection Threads { get; }

    TimeSpan TotalProcessorTime { get; }

    TimeSpan UserProcessorTime { get; }

    long VirtualMemorySize { get; }

    long WorkingSet { get; }

    void Close();

    bool CloseMainWindow();

    void Kill();

    void Refresh();

    bool Start();

    bool WaitForExit(TimeSpan timeout);

    bool WaitForInputIdle(TimeSpan timeout);
  }
}
