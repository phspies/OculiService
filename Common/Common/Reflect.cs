using System;
using System.Linq.Expressions;
using System.Reflection;

namespace OculiService.Common
{
    public static class Reflect
    {
        public static FieldInfo GetField<T>(Expression<Func<T>> fieldExpression)
        {
            Invariant.ArgumentNotNull((object)fieldExpression, "fieldExpression");
            FieldInfo memberInfo = Reflect.GetMemberInfo((LambdaExpression)fieldExpression) as FieldInfo;

            object local = null;
            if (!(memberInfo == (FieldInfo)local))
                return memberInfo;
            throw new ArgumentException("Expression is not a field access expression.", "memberExpression");
        }

        public static FieldInfo GetField<TSource, T>(Expression<Func<TSource, T>> fieldExpression)
        {
            Invariant.ArgumentNotNull((object)fieldExpression, "fieldExpression");
            FieldInfo memberInfo = Reflect.GetMemberInfo((LambdaExpression)fieldExpression) as FieldInfo;

            object local = null;
            if (!(memberInfo == (FieldInfo)local))
                return memberInfo;
            throw new ArgumentException("Expression is not a field access expression.", "memberExpression");
        }

        public static MemberInfo GetMember(Expression<Action> memberExpression)
        {
            Invariant.ArgumentNotNull((object)memberExpression, "memberExpression");
            MemberInfo memberInfo = Reflect.GetMemberInfo((LambdaExpression)memberExpression);

            object local = null;
            if (!(memberInfo == (MemberInfo)local))
                return memberInfo;
            throw new ArgumentException("Expression is not a member expression.", "memberExpression");
        }

        public static MemberInfo GetMember<T>(Expression<Func<T>> memberExpression)
        {
            Invariant.ArgumentNotNull((object)memberExpression, "memberExpression");
            MemberInfo memberInfo = Reflect.GetMemberInfo((LambdaExpression)memberExpression);

            object local = null;
            if (!(memberInfo == (MemberInfo)local))
                return memberInfo;
            throw new ArgumentException("Expression is not a member expression.", "memberExpression");
        }

        public static MemberInfo GetMember<TSource, T>(Expression<Func<TSource, T>> memberExpression)
        {
            Invariant.ArgumentNotNull((object)memberExpression, "memberExpression");
            MemberInfo memberInfo = Reflect.GetMemberInfo((LambdaExpression)memberExpression);

            object local = null;
            if (!(memberInfo == (MemberInfo)local))
                return memberInfo;
            throw new ArgumentException("Expression is not a member expression.", "memberExpression");
        }

        public static MethodInfo GetMethod(Expression<Action> methodExpression)
        {
            Invariant.ArgumentNotNull((object)methodExpression, "methodExpression");
            MethodInfo memberInfo = Reflect.GetMemberInfo((LambdaExpression)methodExpression) as MethodInfo;

            object local = null;
            if (!(memberInfo == (MethodInfo)local))
                return memberInfo;
            throw new ArgumentException("Expression is not a method call expression.", "memberExpression");
        }

        public static MethodInfo GetMethod<T>(Expression<Func<T>> methodExpression)
        {
            Invariant.ArgumentNotNull((object)methodExpression, "methodExpression");
            MethodInfo memberInfo = Reflect.GetMemberInfo((LambdaExpression)methodExpression) as MethodInfo;

            object local = null;
            if (!(memberInfo == (MethodInfo)local))
                return memberInfo;
            throw new ArgumentException("Expression is not a method call expression.", "memberExpression");
        }

        public static MethodInfo GetMethod<TSource, T>(Expression<Func<TSource, T>> methodExpression)
        {
            Invariant.ArgumentNotNull((object)methodExpression, "methodExpression");
            MethodInfo memberInfo = Reflect.GetMemberInfo((LambdaExpression)methodExpression) as MethodInfo;

            object local = null;
            if (!(memberInfo == (MethodInfo)local))
                return memberInfo;
            throw new ArgumentException("Expression is not a method call expression.", "memberExpression");
        }

        public static string GetName(Expression<Action> memberExpression)
        {
            return Reflect.GetMember(memberExpression).Name;
        }

        public static string GetName<T>(Expression<Func<T>> memberExpression)
        {
            return Reflect.GetMember<T>(memberExpression).Name;
        }

        public static string GetName<TSource, T>(Expression<Func<TSource, T>> memberExpression)
        {
            return Reflect.GetMember<TSource, T>(memberExpression).Name;
        }

        public static PropertyInfo GetProperty<T>(Expression<Func<T>> propertyExpression)
        {
            Invariant.ArgumentNotNull((object)propertyExpression, "propertyExpression");
            PropertyInfo memberInfo = Reflect.GetMemberInfo((LambdaExpression)propertyExpression) as PropertyInfo;

            object local = null;
            if (!(memberInfo == (PropertyInfo)local))
                return memberInfo;
            throw new ArgumentException("Expression is not a property access expression.", "memberExpression");
        }

        public static PropertyInfo GetProperty<TSource, T>(Expression<Func<TSource, T>> propertyExpression)
        {
            Invariant.ArgumentNotNull((object)propertyExpression, "propertyExpression");
            PropertyInfo memberInfo = Reflect.GetMemberInfo((LambdaExpression)propertyExpression) as PropertyInfo;

            object local = null;
            if (!(memberInfo == (PropertyInfo)local))
                return memberInfo;
            throw new ArgumentException("Expression is not a property access expression.", "memberExpression");
        }

        private static MemberInfo GetMemberInfo(LambdaExpression lambda)
        {
            MemberExpression memberExpression = (MemberExpression)null;
            if (lambda.Body.NodeType == ExpressionType.Convert)
                memberExpression = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
                memberExpression = lambda.Body as MemberExpression;
            else if (lambda.Body.NodeType == ExpressionType.Call)
                return (MemberInfo)((MethodCallExpression)lambda.Body).Method;
            if (memberExpression == null)
                return (MemberInfo)null;
            return memberExpression.Member;
        }
    }
}
