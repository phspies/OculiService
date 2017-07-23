using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Oculi.Jobs.Context
{
    public class ContextCleanup : IContextCleanup, IDisposable
    {
        private ContextCleanup.CleanupActionsComparer _CleanupActionsComparer = new ContextCleanup.CleanupActionsComparer();
        private List<KeyValuePair<string, Action>> _CleanupActions = new List<KeyValuePair<string, Action>>();

        ~ContextCleanup()
        {
            this.Dispose(false);
        }

        public void Add(string role, Action action)
        {
            KeyValuePair<string, Action> keyValuePair = new KeyValuePair<string, Action>(role, action);
            if (this._CleanupActions.Contains<KeyValuePair<string, Action>>(keyValuePair, (IEqualityComparer<KeyValuePair<string, Action>>)this._CleanupActionsComparer))
                return;
            this._CleanupActions.Insert(0, keyValuePair);
        }

        public bool RoleAdded(string role)
        {
            return this._CleanupActions.Contains<KeyValuePair<string, Action>>(new KeyValuePair<string, Action>(role, (Action)null), (IEqualityComparer<KeyValuePair<string, Action>>)this._CleanupActionsComparer);
        }

        public void Remove(string role)
        {
            for (int index = 0; index < this._CleanupActions.Count; ++index)
            {
                if (string.Compare(this._CleanupActions[index].Key, role, true) == 0)
                {
                    this._CleanupActions.RemoveAt(index);
                    break;
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            foreach (KeyValuePair<string, Action> cleanupAction in this._CleanupActions)
            {
                Trace.WriteLine("Executing cleanup action " + cleanupAction.Key);
                cleanupAction.Value();
            }
            this._CleanupActions.Clear();
        }

        public class CleanupActionsComparer : IEqualityComparer<KeyValuePair<string, Action>>
        {
            public bool Equals(KeyValuePair<string, Action> x, KeyValuePair<string, Action> y)
            {
                return string.Compare(x.Key, y.Key, true) == 0;
            }

            public int GetHashCode(KeyValuePair<string, Action> obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
