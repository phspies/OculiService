using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace OculiService.Common.Logging
{
    public sealed class LogicalOperation : IDisposable
    {
        private readonly static string CallContextDataSlotName;

        private Guid previousActivityId;

        private bool disposed;

        private object data;

        private readonly static bool IsRunningInAdapter;

        public static IEnumerable<object> LogicalOperationStack
        {
            get
            {
                return LogicalOperation.GetLogicalOperationStack();
            }
        }

        static LogicalOperation()
        {
            LogicalOperation.CallContextDataSlotName = "Omoxi.Common.Logging.LogicalOperation";
            LogicalOperation.IsRunningInAdapter = AppDomain.CurrentDomain.GetAssemblies().Any<Assembly>((Assembly a) =>
            {
                if (a.FullName.Contains("Microsoft.VisualStudio.TestPlatform") || a.FullName.Contains("Microsoft.VisualStudio.QualityTools"))
                {
                    return true;
                }
                return a.FullName.Contains("LINQPad");
            });
        }

        private LogicalOperation(object data)
        {
            this.data = data;
            LogicalOperation.StartLogicalOperation(data);
            this.previousActivityId = Trace.CorrelationManager.ActivityId;
            if (Trace.CorrelationManager.ActivityId == Guid.Empty)
            {
                Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            }
        }

        private LogicalOperation(object data, Guid activityId)
        {
            this.data = data;
            LogicalOperation.StartLogicalOperation(this.data);
            this.previousActivityId = Trace.CorrelationManager.ActivityId;
            Trace.CorrelationManager.ActivityId = activityId;
        }

        public static LogicalOperation Create(string activityNameFormat, params object[] parameters)
        {
            Invariant.ArgumentNotNull(activityNameFormat, "activityNameFormat");
            return new LogicalOperation(string.Format(CultureInfo.InvariantCulture, activityNameFormat, parameters));
        }

        public static LogicalOperation Create(Guid activityId, string activityNameFormat, params object[] parameters)
        {
            Invariant.ArgumentNotNull(activityNameFormat, "activityNameFormat");
            if (activityId == Guid.Empty)
            {
                throw new ArgumentException(string.Concat("ActivityId may not be set to ", activityId.ToString()), "activityId");
            }
            return new LogicalOperation(string.Format(CultureInfo.InvariantCulture, activityNameFormat, parameters), activityId);
        }

        public static LogicalOperation Create(object data)
        {
            Invariant.ArgumentNotNull(data, "data");
            return new LogicalOperation(data);
        }

        public static LogicalOperation Create(Guid activityId, object data)
        {
            Invariant.ArgumentNotNull(data, "data");
            if (activityId == Guid.Empty)
            {
                throw new ArgumentException(string.Concat("ActivityId may not be set to ", activityId.ToString()), "activityId");
            }
            return new LogicalOperation(data, activityId);
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                this.Dispose(true);
                this.disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!LogicalOperation.IsRunningInAdapter && LogicalOperation.GetLogicalOperationStack().Peek() != this.data)
                {
                    throw new InvalidOperationException("Logical operation stack has been corrupted");
                }
                LogicalOperation.StopLogicalOperation();
                Trace.CorrelationManager.ActivityId = this.previousActivityId;
            }
        }

        private static IImmutableStack<object> GetLogicalOperationStack()
        {
            IImmutableStack<object> empty = (IImmutableStack<object>)CallContext.LogicalGetData(LogicalOperation.CallContextDataSlotName);
            if (empty == null)
            {
                empty = ImmutableLogicalOperationStack<object>.Empty;
                if (LogicalOperation.IsRunningInAdapter)
                {
                    empty = empty.Push(new object());
                }
                LogicalOperation.UpdateImmutableStack(empty);
            }
            return empty;
        }

        private static void StartLogicalOperation(object data)
        {
            LogicalOperation.UpdateImmutableStack(LogicalOperation.GetLogicalOperationStack().Push(data));
        }

        private static void StopLogicalOperation()
        {
            LogicalOperation.UpdateImmutableStack(LogicalOperation.GetLogicalOperationStack().Pop());
        }

        private static void UpdateImmutableStack(IImmutableStack<object> stack)
        {
            if (!LogicalOperation.IsRunningInAdapter)
            {
                CallContext.LogicalSetData(LogicalOperation.CallContextDataSlotName, stack);
            }
        }
    }
}