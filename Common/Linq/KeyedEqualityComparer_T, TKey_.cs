using OculiService.Common;
using System;
using System.Collections.Generic;

namespace System.Linq
{
    public sealed class KeyedEqualityComparer<T, TKey> : EqualityComparer<T>
    {
        private Func<T, TKey> keySelector;

        private IEqualityComparer<TKey> keyComparer;

        public KeyedEqualityComparer(Func<T, TKey> keySelector) : this(keySelector, EqualityComparer<TKey>.Default)
        {
        }

        public KeyedEqualityComparer(Func<T, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
        {
            Invariant.ArgumentNotNull(keySelector, "keySelector");
            Invariant.ArgumentNotNull(keyComparer, "keyComparer");
            this.keySelector = keySelector;
            this.keyComparer = keyComparer;
        }

        public override bool Equals(T x, T y)
        {
            return this.keyComparer.Equals(this.keySelector(x), this.keySelector(y));
        }

        public override int GetHashCode(T obj)
        {
            return this.keyComparer.GetHashCode(this.keySelector(obj));
        }
    }
}