using OculiService.Common;
using OculiService.Common.Diagnostics;
using OculiService.Common.IO;
using OculiService.Common.Logging.EventLog;
using OculiService.Common.Logging.EventLog.Interface;
using OculiService.Common.Logging.File;
using OculiService.Common.Logging.File.Interface;
using OculiService.Common.Logging.Listener;
using System;
using System.Diagnostics;

namespace OculiService.Internal.Service
{
    public sealed class OculiServiceLogTraceListener : TraceListener
    {
        private const string LogFileName = "OculiService.log";
        private readonly ArchivingTraceListener oculiServiceTraceListener;
        private readonly IFileSystem fileSystem;
        private readonly IEventLog eventLog;
        private readonly int maxLogFileSizeBytes;

        public OculiServiceLogTraceListener(IEventLog eventLog, IFileSystem fileSystem, int maxLogFileSizeBytes)
        {
            this.fileSystem = fileSystem;
            this.eventLog = eventLog;
            this.maxLogFileSizeBytes = maxLogFileSizeBytes;
            this.oculiServiceTraceListener = this.GetListener("OculiService.log");
        }

        private ArchivingTraceListener GetListener(string fileName)
        {
            ArchivingTextWriter archivingTextWriter = new ArchivingTextWriter(this.fileSystem, (ILogArchiver)new LogArchiver((ILogArchive)new LogArchive(this.fileSystem), this.fileSystem), OculiServiceLogTraceListener.GetLogFilePath(fileName), this.maxLogFileSizeBytes);
            return new ArchivingTraceListener((IArchivingTextWriter)archivingTextWriter, (IReportingEventLogWriter)new ReportingEventLogWriter(this.eventLog, (IArchivingTextWriter)archivingTextWriter, (ILogEntryConverter)new LogEntryConverter()));
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            this.GetTraceListener().TraceData(eventCache, source, eventType, id, data);
        }

        public override void Write(string message)
        {
            this.GetTraceListener().Write(message);
        }

        public override void WriteLine(string message)
        {
            this.GetTraceListener().Write(message);
        }

        private static string GetLogFilePath(string name)
        {
            return PathHelpers.GetFullPath(name, RelativeFolder.Application, RelativeSubfolder.Logs);
        }

        private TraceListener GetTraceListener()
        {
            try
            {
                return (TraceListener)this.oculiServiceTraceListener;
            }
            catch (Exception ex)
            {
                this.oculiServiceTraceListener.TraceEvent((TraceEventCache)null, (string)null, TraceEventType.Error, 0, ex.ToString());
                return (TraceListener)this.oculiServiceTraceListener;
            }
        }
    }
}
