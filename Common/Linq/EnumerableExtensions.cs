using OculiService.Common;
using OculiService.Common.ExceptionHandling;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> enumerable, T value)
        {
            foreach (T obj in enumerable)
                yield return obj;
            yield return value;
        }

        public static IEnumerable<T> DefaultIfNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable ?? Enumerable.Empty<T>();
        }
        public static IEnumerable<TSource> Do<TSource>(this IEnumerable<TSource> source, Action<TSource> onNext, Action<Exception> onError)
        {
            Invariant.ArgumentNotNull((object)source, "source");
            Invariant.ArgumentNotNull((object)onNext, "onNext");
            Invariant.ArgumentNotNull((object)onError, "onError");
            return source.DoHelper<TSource>(onNext, onError, (Action)(() => { }));
        }

        public static IEnumerable<TSource> Do<TSource>(this IEnumerable<TSource> source, Action<TSource> onNext, Action onCompleted)
        {
            Invariant.ArgumentNotNull((object)source, "source");
            Invariant.ArgumentNotNull((object)onNext, "onNext");
            Invariant.ArgumentNotNull((object)onCompleted, "onCompleted");
            return source.DoHelper<TSource>(onNext, (Action<Exception>)(error => { }), onCompleted);
        }

        public static IEnumerable<TSource> Do<TSource>(this IEnumerable<TSource> source, Action<TSource> onNext, Action<Exception> onError, Action onCompleted)
        {
            Invariant.ArgumentNotNull((object)source, "source");
            Invariant.ArgumentNotNull((object)onNext, "onNext");
            Invariant.ArgumentNotNull((object)onError, "onError");
            Invariant.ArgumentNotNull((object)onCompleted, "onCompleted");
            return source.DoHelper<TSource>(onNext, onError, onCompleted);
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T obj in enumerable)
                action(obj);
        }

        public static bool IsEquivalentTo<T>(this IEnumerable<T> left, IEnumerable<T> right)
        {
            Invariant.ArgumentNotNull((object)left, "left");
            Invariant.ArgumentNotNull((object)right, "right");
            if (left == right)
                return true;
            bool? nullable = EnumerableExtensions.TryVerifyCollectionCountsAreEqual<T>(left, right);
            if (nullable.HasValue && !nullable.Value)
                return false;
            int nullCount1;
            Dictionary<T, int> itemCounts1 = EnumerableExtensions.GetItemCounts<T>(left, out nullCount1);
            int nullCount2;
            Dictionary<T, int> itemCounts2 = EnumerableExtensions.GetItemCounts<T>(right, out nullCount2);
            if (nullCount1 != nullCount2 || itemCounts1.Count != itemCounts2.Count)
                return false;
            foreach (KeyValuePair<T, int> keyValuePair in itemCounts1)
            {
                int num;
                if (!itemCounts2.TryGetValue(keyValuePair.Key, out num) || num != keyValuePair.Value)
                    return false;
            }
            return true;
        }
        private static IEnumerable<T> SplitInner<T>(IEnumerator<T> enumerator, int chunkSize)
        {
            yield return enumerator.Current;
            for (int i = 1; i < chunkSize && enumerator.MoveNext(); ++i)
                yield return enumerator.Current;
        }

        public static IEnumerable<T> IfEmptyThen<T>(this IEnumerable<T> source, IEnumerable<T> data)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (data == null)
                throw new ArgumentNullException("data");
            return EnumerableExtensions.IfEmptyThenListImpl<T>(source, data);
        }

        private static IEnumerable<T> IfEmptyThenListImpl<T>(IEnumerable<T> source, IEnumerable<T> data)
        {
            IEnumerator<T> e = source.GetEnumerator();
            try
            {
                if (e.MoveNext())
                {
                    do
                    {
                        yield return e.Current;
                    }
                    while (e.MoveNext());
                }
                else
                {
                    foreach (T obj in data)
                        yield return obj;
                }
            }
            finally
            {
                if (e != null)
                    e.Dispose();
            }
            e = (IEnumerator<T>)null;
        }

        public static IEnumerable<T> IfEmptyThen<T>(this IEnumerable<T> source, T item)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            return EnumerableExtensions.IfEmptyThenImpl<T>(source, item);
        }

        private static IEnumerable<T> IfEmptyThenImpl<T>(IEnumerable<T> source, T item)
        {
            IEnumerator<T> e = source.GetEnumerator();
            try
            {
                if (e.MoveNext())
                {
                    do
                    {
                        yield return e.Current;
                    }
                    while (e.MoveNext());
                }
                else
                    yield return item;
            }
            finally
            {
                if (e != null)
                    e.Dispose();
            }
            e = (IEnumerator<T>)null;
        }

        public static IEnumerable<T> RecurseThrough<T>(this IEnumerable<T> enumerable, Func<T, IEnumerable<T>> descendBy)
        {
            foreach (T obj1 in enumerable)
            {
                T value = obj1;
                yield return value;
                foreach (T obj2 in descendBy(value).RecurseThrough<T>(descendBy))
                    yield return obj2;
                value = default(T);
            }
        }

        public static string StringJoin(this IEnumerable<string> enumerable, string separator)
        {
            Invariant.ArgumentNotNull((object)enumerable, "enumerable");
            Invariant.ArgumentNotNullOrEmpty(separator, "separator");
            StringBuilder stringBuilder = new StringBuilder();
            using (IEnumerator<string> enumerator = enumerable.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    stringBuilder.Append(enumerator.Current ?? "");
                    while (enumerator.MoveNext())
                    {
                        stringBuilder.Append(separator);
                        stringBuilder.Append(enumerator.Current ?? "");
                    }
                }
            }
            return stringBuilder.ToString();
        }

        private static IEnumerable<TSource> DoHelper<TSource>(this IEnumerable<TSource> source, Action<TSource> onNext, Action<Exception> onError, Action onCompleted)
        {
            IEnumerator<TSource> enumerator = source.GetEnumerator();
            try
            {
                TSource current = default(TSource);
                while (true)
                {
                    try
                    {
                        if (enumerator.MoveNext())
                            current = enumerator.Current;
                        else
                            break;
                    }
                    catch (Exception ex)
                    {
                        onError(ex);
                        throw ex.PrepareForRethrow();
                    }
                    onNext(current);
                    yield return current;
                }
                onCompleted();
                current = default(TSource);
            }
            finally
            {
                if (enumerator != null)
                    enumerator.Dispose();
            }
            enumerator = (IEnumerator<TSource>)null;
        }

        private static Dictionary<T, int> GetItemCounts<T>(IEnumerable<T> items, out int nullCount)
        {
            nullCount = 0;
            bool isValueType = typeof(T).IsValueType;
            Dictionary<T, int> dictionary = new Dictionary<T, int>();
            foreach (T key in items)
            {
                if (!isValueType && (object)key == null)
                {
                    nullCount = nullCount + 1;
                }
                else
                {
                    int num;
                    dictionary[key] = dictionary.TryGetValue(key, out num) ? ++num : 0;
                }
            }
            return dictionary;
        }

        private static bool? TryVerifyCollectionCountsAreEqual<T>(IEnumerable<T> left, IEnumerable<T> right)
        {
            ICollection<T> objs1 = left as ICollection<T>;
            ICollection<T> objs2 = right as ICollection<T>;
            if (objs1 == null || objs2 == null)
                return new bool?();
            return new bool?(objs1.Count == objs2.Count);
        }
    }
}
