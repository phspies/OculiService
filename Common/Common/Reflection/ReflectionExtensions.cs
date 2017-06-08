using System;using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OculiService.Common.Reflection
{
  public static class ReflectionExtensions
  {
    public static IEnumerable<Type> GetTypes(this AppDomain appdomain)
    {
      return ((IEnumerable<Assembly>) appdomain.GetAssemblies()).SelectMany<Assembly, Type>((Func<Assembly, IEnumerable<Type>>) (assembly => (IEnumerable<Type>) assembly.GetTypesSafely()));
    }

    public static Type[] GetTypesSafely(this Assembly self)
    {
      try
      {
        return self.GetTypes();
      }
      catch (ReflectionTypeLoadException ex)
      {
        return new Type[0];
      }
    }

    public static TAttribute[] GetCustomAttributes<TAttribute>(this MemberInfo self, bool inherit)
    {
      return self.GetCustomAttributes(typeof (TAttribute), inherit).Cast<TAttribute>().ToArray<TAttribute>();
    }

    public static IEnumerable<T> WithAttribute<T>(this IEnumerable<T> self, Type type, bool inherit) where T : MemberInfo
    {
      return self.Where<T>((Func<T, bool>) (mi => ((IEnumerable<object>) mi.GetCustomAttributes(type, inherit)).Any<object>()));
    }
  }
}
