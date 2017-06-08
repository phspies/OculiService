using OculiService.Common.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace OculiService.Common.Diagnostics
{
  public static class MiniDumpProvider
  {
    private static Tracer _tracer = Tracer.GetTracer(typeof (MiniDumpProvider));

    public static int Write(string filepath)
    {
      return MiniDumpProvider.Write(filepath, MiniDumpType.WithDataSegs | MiniDumpType.WithFullMemory | MiniDumpType.WithHandleData);
    }

    public static void ClearArchivedDumpFiles(string path)
    {
      if (!Directory.Exists(path))
      {
        MiniDumpProvider._tracer.TraceInformation("No dump file directory");
      }
      else
      {
        foreach (string file in Directory.GetFiles(path, "*.dmp", SearchOption.TopDirectoryOnly))
        {
          using (LogicalOperation.Create(string.Format("Deleting {0}", (object) file), new object[0]))
          {
            try
            {
              File.Delete(file);
              MiniDumpProvider._tracer.TraceInformation(string.Format("Removed"));
            }
            catch (Exception ex)
            {
              MiniDumpProvider._tracer.TraceInformation(string.Format("Error: {0}", (object) ex));
            }
          }
        }
      }
    }

    public static int Write(string filepath, MiniDumpType dumpType)
    {
      MiniDumpProvider.MinidumpExceptionInfo structure = new MiniDumpProvider.MinidumpExceptionInfo() { ThreadId = MiniDumpProvider.GetCurrentThreadId(), ExceptionPointers = Marshal.GetExceptionPointers(), ClientPointers = false };
      IntPtr num = Marshal.AllocHGlobal(Marshal.SizeOf<MiniDumpProvider.MinidumpExceptionInfo>(structure));
      try
      {
        using (FileStream fileStream = new FileStream(filepath, FileMode.Create))
        {
          Process currentProcess = Process.GetCurrentProcess();
          Marshal.StructureToPtr<MiniDumpProvider.MinidumpExceptionInfo>(structure, num, false);
          return MiniDumpProvider.MiniDumpWriteDump(currentProcess.Handle, currentProcess.Id, fileStream.SafeFileHandle.DangerousGetHandle(), dumpType, structure.ClientPointers ? num : IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) ? 0 : Marshal.GetLastWin32Error();
        }
      }
      finally
      {
        Marshal.FreeHGlobal(num);
      }
    }

    [DllImport("DbgHelp.dll", SetLastError = true)]
    private static extern bool MiniDumpWriteDump(IntPtr hProcess, int processId, IntPtr fileHandle, MiniDumpType dumpType, IntPtr excepInfo, IntPtr userInfo, IntPtr extInfo);

    [DllImport("kernel32.dll")]
    private static extern int GetCurrentThreadId();

    private struct MinidumpExceptionInfo
    {
      public int ThreadId;
      public IntPtr ExceptionPointers;
      public bool ClientPointers;
    }
  }
}
