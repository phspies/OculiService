using OculiService.Common;
using OculiService.Common.Diagnostics;
using OculiService.Common.IO;
using OculiService.Common.Logging;
using OculiService.Internal.Service;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace OculiService.Internal
{
    internal class OculiServiceLogger : ILogger
    {
        public static int MaxLogFileSizeBytes = 1048576;
        private readonly CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
        public const int MinLogFileSizeBytes = 1048576;
        private const string TraceSourceName = "OculiService";
        private readonly IFileSystem fileSystem;
        private readonly TraceSource traceSource;
        private readonly TraceSourceLogger logger;
        private readonly IEventLog eventLog;

        public CultureInfo Culture
        {
            get
            {
                return this.culture;
            }
        }
        public OculiServiceLogger()
        {
            this.fileSystem = (IFileSystem)new FileSystem();
            this.traceSource = new TraceSource("OculiService", SourceLevels.All);
            this.logger = new TraceSourceLogger(this.traceSource);
            this.eventLog = EventLogExtensions.CreateEventLog("Application", ".", "Oculi Service");
            if (EventLog.SourceExists("Oculi Service"))
            {
                EventLog.DeleteEventSource("Oculi Service");
            }
            EventLog.CreateEventSource(new EventSourceCreationData("Oculi Service", "Application")
            {
                //MessageResourceFile = PathHelpers.GetFullPath("DoubleTake.ManagementService.EventMessages.dll", RelativeFolder.Application)
            });
            this.ConfigureListeners();
            Tracer.ConfigurationChanged += new EventHandler(this.Tracer_ConfigurationChanged);
        }

        public void WriteEntry(LogEntry entry)
        {
            this.logger.WriteEntry(entry);
        }

        private void Tracer_ConfigurationChanged(object sender, EventArgs e)
        {
            this.ConfigureListeners();
        }

        private void ConfigureListeners()
        {
            if (!this.traceSource.Listeners.OfType<OculiServiceLogTraceListener>().Any<OculiServiceLogTraceListener>())
                this.traceSource.Listeners.Add((TraceListener)new OculiServiceLogTraceListener(this.eventLog, this.fileSystem, Math.Max(1048576, OculiServiceLogger.MaxLogFileSizeBytes)));
            this.traceSource.Switch.Level = SourceLevels.All;
        }
    }
}
