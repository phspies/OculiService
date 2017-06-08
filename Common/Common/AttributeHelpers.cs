using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OculiService.Common
{
    public static class AttributeHelpers
    {
        public static T GetAssemblyAttribute<T>(Assembly assembly) where T : Attribute
        {
            Invariant.ArgumentNotNull((object)assembly, "assembly");
            return Attribute.GetCustomAttribute(assembly, typeof(T)) as T;
        }
        public static TValue GetPropertyAttributeValue<T, TOut, TAttribute, TValue>(Expression<Func<T, TOut>> propertyExpression,Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            var expression = (MemberExpression)propertyExpression.Body;
            var propertyInfo = (PropertyInfo)expression.Member;
            var attr = propertyInfo.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
            return attr != null ? valueSelector(attr) : default(TValue);
        }
        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        public static bool IsPropValueEmpty(object src, string propName)
        {
            return String.IsNullOrEmpty((string)src.GetType().GetProperty(propName).GetValue(src, null));
        }
        public static TValue GetAttributValue<TAttribute, TValue>(this PropertyInfo prop, Func<TAttribute, TValue> value) where TAttribute : Attribute
        {
            var att = prop.GetCustomAttributes(
                typeof(TAttribute), true
                ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return value(att);
            }
            return default(TValue);
        }
        public static string GetAttributeName<T>(object _property)
        {
            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static);
            // sort properties by name
            Array.Sort(propertyInfos, delegate (PropertyInfo propertyInfo1, PropertyInfo propertyInfo2) { return propertyInfo1.Name.CompareTo(propertyInfo2.Name); });

            return "";
        }
    }
}
