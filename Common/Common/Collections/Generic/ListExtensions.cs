using OculiService.Common;

namespace System.Collections.Generic
{
    public static class ListExtensions
    {
        public static void AddRange<TItem>(this ICollection<TItem> self, IEnumerable<TItem> items)
        {
            List<TItem> objList = self as List<TItem>;
            if (objList != null)
            {
                objList.AddRange(items);
            }
            else
            {
                foreach (TItem obj in items)
                    self.Add(obj);
            }
        }

        public static void InsertRange<TItem>(this IList<TItem> self, int index, IEnumerable<TItem> items)
        {
            Invariant.ArgumentIsInRange<int>(index, "index", new Range<int>(0, self.Count));
            List<TItem> objList = self as List<TItem>;
            if (objList != null)
            {
                objList.InsertRange(index, items);
            }
            else
            {
                foreach (TItem obj in items)
                    self.Insert(index++, obj);
            }
        }

        public static int BinarySearch<TItem>(this IList<TItem> self, TItem item, IComparer<TItem> comparer)
        {
            return self.BinarySearch<TItem>(0, self.Count, item, comparer);
        }

        public static int BinarySearch<TItem>(this IList<TItem> self, int index, int count, TItem item, IComparer<TItem> comparer)
        {
            if (self == null)
                throw new ArgumentNullException("self");
            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException(index < 0 ? "index" : "count");
            if (self.Count - index < count)
                throw new ArgumentException("Invalid offset length.");
            comparer = comparer ?? (IComparer<TItem>)Comparer<TItem>.Default;
            int num1 = index;
            int num2 = index + count - 1;
            while (num1 <= num2)
            {
                int index1 = num1 + (num2 - num1 >> 1);
                int num3;
                try
                {
                    num3 = comparer.Compare(self[index1], item);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("IComparer failed.", ex);
                }
                if (num3 == 0)
                    return index1;
                if (num3 < 0)
                    num1 = index1 + 1;
                else
                    num2 = index1 - 1;
            }
            return ~num1;
        }

        public static int BinarySearch<TKey, TItem>(this IList<TItem> self, TKey key, Func<TItem, TKey> keySelector)
        {
            return self.BinarySearch<TKey, TItem>(0, self.Count, key, keySelector, (IComparer<TKey>)null);
        }

        public static int BinarySearch<TKey, TItem>(this IList<TItem> self, TKey key, Func<TItem, TKey> keySelector, IComparer<TKey> comparer)
        {
            return self.BinarySearch<TKey, TItem>(0, self.Count, key, keySelector, comparer);
        }

        public static int BinarySearch<TKey, TItem>(this IList<TItem> self, int index, int count, TKey key, Func<TItem, TKey> keySelector, IComparer<TKey> comparer)
        {
            if (self == null)
                throw new ArgumentNullException("self");
            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException(index < 0 ? "index" : "count");
            if (self.Count - index < count)
                throw new ArgumentException("Invalid offset length.");
            if (keySelector == null)
                throw new ArgumentNullException("keySelector");
            comparer = comparer ?? (IComparer<TKey>)Comparer<TKey>.Default;
            int num1 = index;
            int num2 = index + count - 1;
            while (num1 <= num2)
            {
                int index1 = num1 + (num2 - num1 >> 1);
                int num3;
                try
                {
                    TKey x = keySelector(self[index1]);
                    num3 = comparer.Compare(x, key);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("IComparer failed.", ex);
                }
                if (num3 == 0)
                    return index1;
                if (num3 < 0)
                    num1 = index1 + 1;
                else
                    num2 = index1 - 1;
            }
            return ~num1;
        }
    }
}
