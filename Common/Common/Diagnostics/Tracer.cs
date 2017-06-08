using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace OculiService.Common.Diagnostics
{
    public class Tracer
    {
        private static readonly object syncRoot = new object();
        private static readonly List<WeakReference> sources = new List<WeakReference>();
        private static readonly List<WeakReference> tracers = new List<WeakReference>();
        private static HashSet<string> configuredSources = (HashSet<string>)null;
        private readonly string name;
        private static int lastCollectionCount;
        private volatile TraceSource traceSource;

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public TraceSource Source
        {
            get
            {
                return this.traceSource;
            }
        }

        public static event EventHandler ConfigurationChanged;

        static Tracer()
        {
            ConfigurationMonitor.ConfigurationChanged += new EventHandler(CountEvent);
        }

        static private void CountEvent(object sender, EventArgs e)
        {
            int count = 0;
        }

        private Tracer(string name, TraceSource traceSource)
        {
            this.name = name;
            this.traceSource = traceSource;
        }

        public static Tracer GetTracer(Type type)
        {
            return Tracer.GetTracer(type.FullName, SourceLevels.Off);
        }

        public static Tracer GetTracer(Type type, SourceLevels defaultLevel)
        {
            return Tracer.GetTracer(type.FullName, defaultLevel);
        }

        public static Tracer GetTracer(string name)
        {
            return Tracer.GetTracer(name, SourceLevels.Off);
        }

        public static Tracer GetTracer(string name, SourceLevels defaultLevel)
        {
            lock (Tracer.syncRoot)
            {
                Tracer.Prune(false);
                Tracer local_3 = Tracer.tracers.Select<WeakReference, Tracer>((Func<WeakReference, Tracer>)(t => (Tracer)t.Target)).FirstOrDefault<Tracer>((Func<Tracer, bool>)(t =>
             {
                 if (t != null)
                     return t.Name == name;
                 return false;
             }));
                if (local_3 == null)
                {
                    local_3 = new Tracer(name, Tracer.GetTraceSource(name, defaultLevel));
                    Tracer.tracers.Add(new WeakReference((object)local_3));
                }
                return local_3;
            }
        }

        public static void RefreshAll()
        {
            lock (Tracer.syncRoot)
            {
                Trace.Refresh();
                Tracer.configuredSources = (HashSet<string>)null;
                Tracer.Prune(true);
                foreach (Tracer item_0 in Tracer.tracers.Select<WeakReference, Tracer>((Func<WeakReference, Tracer>)(t => (Tracer)t.Target)).Where<Tracer>((Func<Tracer, bool>)(t => t != null)))
                {
                    TraceSource temp_18 = Tracer.GetTraceSource(item_0.Name, SourceLevels.Off);
                    item_0.traceSource = temp_18;
                }
            }
        }

        public void Flush()
        {
            this.traceSource.Flush();
        }

        [Conditional("TRACE")]
        public void TraceData(TraceEventType eventType, int id, object data)
        {
            this.traceSource.TraceData(eventType, id, data);
        }

        [Conditional("TRACE")]
        public void TraceData(TraceEventType eventType, int id, params object[] data)
        {
            this.traceSource.TraceData(eventType, id, data);
        }

        [Conditional("TRACE")]
        public void TraceEvent(TraceEventType eventType, int id)
        {
            this.traceSource.TraceEvent(eventType, id);
        }

        [Conditional("TRACE")]
        public void TraceEvent(TraceEventType eventType, int id, string message)
        {
            this.traceSource.TraceEvent(eventType, id, message);
        }

        [Conditional("TRACE")]
        public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
        {
            this.traceSource.TraceEvent(eventType, id, format, args);
        }

        [Conditional("TRACE")]
        public void TraceInformation(string message)
        {
            this.traceSource.TraceInformation(message);
        }

        [Conditional("TRACE")]
        public void TraceInformation(string format, params object[] args)
        {
            this.traceSource.TraceInformation(format, args);
        }

        [Conditional("TRACE")]
        public void TraceTransfer(int id, string message, Guid relatedActivityId)
        {
            this.traceSource.TraceTransfer(id, message, relatedActivityId);
        }

        private static TraceSource GetTraceSource(string name, SourceLevels defaultLevel)
        {
            TraceSource traceSource = Tracer.sources.Select<WeakReference, TraceSource>((Func<WeakReference, TraceSource>)(s => (TraceSource)s.Target)).FirstOrDefault<TraceSource>((Func<TraceSource, bool>)(s =>
         {
             if (s != null)
                 return s.Name == name;
             return false;
         }));
            if (traceSource == null)
            {
                string name1 = Tracer.ShortenName(name);
                if (string.IsNullOrEmpty(name1))
                {
                    traceSource = new TraceSource(name, defaultLevel);
                    Tracer.sources.Add(new WeakReference((object)traceSource));
                }
                else
                    traceSource = Tracer.GetConfiguredTraceSource(name);
                if (traceSource == null)
                    traceSource = Tracer.GetTraceSource(name1, defaultLevel);
            }
            return traceSource;
        }

        private static string ShortenName(string name)
        {
            int length = name.LastIndexOf('.');
            if (length >= 0)
                return name.Substring(0, length);
            return (string)null;
        }

        private static TraceSource GetConfiguredTraceSource(string name)
        {
            if (Tracer.configuredSources == null)
                Tracer.GetConfiguredSources();
            if (Tracer.configuredSources.Contains(name))
                return new TraceSource(name);
            return (TraceSource)null;
        }

        private static void GetConfiguredSources()
        {
            Tracer.configuredSources = new HashSet<string>();
            foreach (ConfigurationElement configurationElement in ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).GetSection("system.diagnostics").ElementInformation.Properties["sources"].Value as ConfigurationElementCollection)
            {
                string str = configurationElement.ElementInformation.Properties["name"].Value.ToString();
                Tracer.configuredSources.Add(str);
            }
        }

        private static void Prune(bool refreshing)
        {
            if (!refreshing && Tracer.lastCollectionCount == GC.CollectionCount(2))
                return;
            Tracer.Prune(Tracer.tracers, (Func<WeakReference, bool>)(t => t.IsAlive));
            Tracer.Prune(Tracer.sources, (Func<WeakReference, bool>)(t =>
           {
               if (!refreshing)
                   return t.IsAlive;
               return Tracer.IsConfigured((TraceSource)t.Target);
           }));
            Tracer.lastCollectionCount = GC.CollectionCount(2);
        }

        private static void Prune(List<WeakReference> list, Func<WeakReference, bool> predicate)
        {
            List<WeakReference> weakReferenceList = new List<WeakReference>(list.Count);
            weakReferenceList.AddRange(list.Where<WeakReference>((Func<WeakReference, bool>)(r => predicate(r))));
            list.Clear();
            list.AddRange((IEnumerable<WeakReference>)weakReferenceList);
            list.TrimExcess();
        }

        private static bool IsConfigured(TraceSource source)
        {
            Tracer.GetConfiguredSources();
            return Tracer.configuredSources.Contains(source.Name);
        }
    }
}
