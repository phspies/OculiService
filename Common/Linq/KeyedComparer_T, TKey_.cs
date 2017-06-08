using OculiService.Common;
using System;
using System.Collections.Generic;

namespace System.Linq
{
    public sealed class KeyedComparer<T, TKey> : Comparer<T>
    {
        private Func<T, TKey> keySelector;

        private IComparer<TKey> keyComparer;

        public KeyedComparer(Func<T, TKey> keySelector) : this(keySelector, Comparer<TKey>.Default)
        {
        }

        public KeyedComparer(Func<T, TKey> keySelector, IComparer<TKey> keyComparer)
        {
            Invariant.ArgumentNotNull(keySelector, "keySelector");
            Invariant.ArgumentNotNull(keyComparer, "keyComparer");
            this.keySelector = keySelector;
            this.keyComparer = keyComparer;
        }

        public override int Compare(T x, T y)
        {
            return this.keyComparer.Compare(this.keySelector(x), this.keySelector(y));
        }
    }
}