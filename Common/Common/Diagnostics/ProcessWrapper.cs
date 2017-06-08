using System;using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace OculiService.Common.Diagnostics
{
  internal class ProcessWrapper : IProcess
  {
    private readonly Process process;

    public int BasePriority
    {
      get
      {
        return this.process.BasePriority;
      }
    }

    public int ExitCode
    {
      get
      {
        return this.process.ExitCode;
      }
    }

    public DateTime ExitTime
    {
      get
      {
        return this.process.ExitTime;
      }
    }

    public IntPtr Handle
    {
      get
      {
        return this.process.Handle;
      }
    }

    public int HandleCount
    {
      get
      {
        return this.process.HandleCount;
      }
    }

    public bool HasExited
    {
      get
      {
        return this.process.HasExited;
      }
    }

    public int Id
    {
      get
      {
        return this.process.Id;
      }
    }

    public string MachineName
    {
      get
      {
        return this.process.MachineName;
      }
    }

    public ProcessModule MainModule
    {
      get
      {
        return this.process.MainModule;
      }
    }

    public IntPtr MainWindowHandle
    {
      get
      {
        return this.process.MainWindowHandle;
      }
    }

    public string MainWindowTitle
    {
      get
      {
        return this.process.MainWindowTitle;
      }
    }

    public IntPtr MaxWorkingSet
    {
      get
      {
        return this.process.MaxWorkingSet;
      }
      set
      {
        this.process.MaxWorkingSet = value;
      }
    }

    public IntPtr MinWorkingSet
    {
      get
      {
        return this.process.MinWorkingSet;
      }
      set
      {
        this.process.MinWorkingSet = value;
      }
    }

    public ProcessModuleCollection Modules
    {
      get
      {
        return this.process.Modules;
      }
    }

    public long NonpagedSystemMemorySize
    {
      get
      {
        return this.process.NonpagedSystemMemorySize64;
      }
    }

    public long PagedMemorySize
    {
      get
      {
        return this.process.PagedMemorySize64;
      }
    }

    public long PagedSystemMemorySize
    {
      get
      {
        return this.process.PagedSystemMemorySize64;
      }
    }

    public long PeakPagedMemorySize
    {
      get
      {
        return this.process.PeakPagedMemorySize64;
      }
    }

    public long PeakVirtualMemorySize
    {
      get
      {
        return this.process.PeakVirtualMemorySize64;
      }
    }

    public long PeakWorkingSet
    {
      get
      {
        return this.process.PeakWorkingSet64;
      }
    }

    public bool PriorityBoostEnabled
    {
      get
      {
        return this.process.PriorityBoostEnabled;
      }
    }

    public ProcessPriorityClass PriorityClass
    {
      get
      {
        return this.process.PriorityClass;
      }
      set
      {
        this.process.PriorityClass = value;
      }
    }

    public long PrivateMemorySize
    {
      get
      {
        return this.process.PrivateMemorySize64;
      }
    }

    public TimeSpan PrivilegedProcessorTime
    {
      get
      {
        return this.process.PrivilegedProcessorTime;
      }
    }

    public string ProcessName
    {
      get
      {
        return this.process.ProcessName;
      }
    }

    public IntPtr ProcessorAffinity
    {
      get
      {
        return this.process.ProcessorAffinity;
      }
      set
      {
        this.process.ProcessorAffinity = value;
      }
    }

    public bool Responding
    {
      get
      {
        return this.process.Responding;
      }
    }

    public int SessionId
    {
      get
      {
        return this.process.HandleCount;
      }
    }

    public StreamReader StandardError
    {
      get
      {
        return this.process.StandardError;
      }
    }

    public StreamWriter StandardInput
    {
      get
      {
        return this.process.StandardInput;
      }
    }

    public StreamReader StandardOutput
    {
      get
      {
        return this.process.StandardOutput;
      }
    }

    public ProcessStartInfo StartInfo
    {
      get
      {
        return this.process.StartInfo;
      }
      set
      {
        this.process.StartInfo = value;
      }
    }

    public DateTime StartTime
    {
      get
      {
        return this.process.StartTime;
      }
    }

    public ISynchronizeInvoke SynchronizingObject
    {
      get
      {
        return this.process.SynchronizingObject;
      }
      set
      {
        this.process.SynchronizingObject = value;
      }
    }

    public ProcessThreadCollection Threads
    {
      get
      {
        return this.process.Threads;
      }
    }

    public TimeSpan TotalProcessorTime
    {
      get
      {
        return this.process.TotalProcessorTime;
      }
    }

    public TimeSpan UserProcessorTime
    {
      get
      {
        return this.process.UserProcessorTime;
      }
    }

    public long VirtualMemorySize
    {
      get
      {
        return this.process.VirtualMemorySize64;
      }
    }

    public long WorkingSet
    {
      get
      {
        return this.process.WorkingSet64;
      }
    }

    internal ProcessWrapper(Process process)
    {
      this.process = process;
    }

    public void Close()
    {
      this.process.Close();
    }

    public bool CloseMainWindow()
    {
      return this.process.CloseMainWindow();
    }

    public void Kill()
    {
      this.process.Kill();
    }

    public void Refresh()
    {
      this.process.Refresh();
    }

    public bool Start()
    {
      return this.process.Start();
    }

    public bool WaitForExit(TimeSpan timeout)
    {
      return this.process.WaitForExit((int) timeout.TotalMilliseconds);
    }

    public bool WaitForInputIdle(TimeSpan timeout)
    {
      return this.process.WaitForInputIdle((int) timeout.TotalMilliseconds);
    }
  }
}
