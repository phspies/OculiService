using System;using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace OculiService.Common
{
  public static class CloneUtility
  {
    private static Dictionary<Type, Delegate> cachedIL = new Dictionary<Type, Delegate>();
    private static Dictionary<Type, Delegate> cachedILDeep = new Dictionary<Type, Delegate>();

    public static T CloneShallow<T>(T source)
    {
      Delegate @delegate = (Delegate) null;
      if (!CloneUtility.cachedIL.TryGetValue(typeof (T), out @delegate))
      {
        DynamicMethod dynamicMethod = new DynamicMethod("DoClone", typeof (T), new Type[1]{ typeof (T) }, Assembly.GetExecutingAssembly().ManifestModule, 1 != 0);
        ConstructorInfo constructor = source.GetType().GetConstructor(new Type[0]);
        ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
        ilGenerator.DeclareLocal(typeof (T));
        ilGenerator.Emit(OpCodes.Newobj, constructor);
        ilGenerator.Emit(OpCodes.Stloc_0);
        foreach (FieldInfo field in source.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
          ilGenerator.Emit(OpCodes.Ldloc_0);
          ilGenerator.Emit(OpCodes.Ldarg_0);
          ilGenerator.Emit(OpCodes.Ldfld, field);
          ilGenerator.Emit(OpCodes.Stfld, field);
        }
        ilGenerator.Emit(OpCodes.Ldloc_0);
        ilGenerator.Emit(OpCodes.Ret);
        @delegate = dynamicMethod.CreateDelegate(typeof (Func<T, T>));
        CloneUtility.cachedIL.Add(typeof (T), @delegate);
      }
      return ((Func<T, T>) @delegate)(source);
    }

    public static T CloneDeep<T>(T source)
    {
      Delegate @delegate = (Delegate) null;
      if (!CloneUtility.cachedILDeep.TryGetValue(typeof (T), out @delegate))
      {
        DynamicMethod dynamicMethod = new DynamicMethod("DoClone", typeof (T), new Type[1]{ typeof (T) }, Assembly.GetExecutingAssembly().ManifestModule, 1 != 0);
        ConstructorInfo constructor = source.GetType().GetConstructor(new Type[0]);
        ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
        ilGenerator.DeclareLocal(typeof (T));
        ilGenerator.Emit(OpCodes.Newobj, constructor);
        ilGenerator.Emit(OpCodes.Stloc_0);
        foreach (FieldInfo field in typeof (T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
          if (field.FieldType.IsValueType || field.FieldType == typeof (string))
            CloneUtility.CopyValueType(ilGenerator, field);
          else if (field.FieldType.IsClass)
            CloneUtility.CopyReferenceType(ilGenerator, field);
        }
        ilGenerator.Emit(OpCodes.Ldloc_0);
        ilGenerator.Emit(OpCodes.Ret);
        @delegate = dynamicMethod.CreateDelegate(typeof (Func<T, T>));
        CloneUtility.cachedILDeep.Add(typeof (T), @delegate);
      }
      return ((Func<T, T>) @delegate)(source);
    }

    private static void CreateNewTempObject(ILGenerator generator, Type type, LocalBuilder localBuilder)
    {
      ConstructorInfo constructor = type.GetConstructor(new Type[0]);
      generator.Emit(OpCodes.Newobj, constructor);
      generator.Emit(OpCodes.Stloc, localBuilder);
    }

    private static void PlaceNewTempObjInClone(ILGenerator generator, FieldInfo field, LocalBuilder localBuilder)
    {
      generator.Emit(OpCodes.Ldloc_0);
      generator.Emit(OpCodes.Ldloc, localBuilder);
      generator.Emit(OpCodes.Stfld, field);
    }

    private static void CopyValueType(ILGenerator generator, FieldInfo field)
    {
      generator.Emit(OpCodes.Ldloc_0);
      generator.Emit(OpCodes.Ldarg_0);
      generator.Emit(OpCodes.Ldfld, field);
      generator.Emit(OpCodes.Stfld, field);
    }

    private static void CopyValueTypeTemp(ILGenerator generator, FieldInfo fieldParent, FieldInfo fieldDetail)
    {
      generator.Emit(OpCodes.Ldloc_1);
      generator.Emit(OpCodes.Ldarg_0);
      generator.Emit(OpCodes.Ldfld, fieldParent);
      generator.Emit(OpCodes.Ldfld, fieldDetail);
      generator.Emit(OpCodes.Stfld, fieldDetail);
    }

    private static void CopyReferenceType(ILGenerator generator, FieldInfo field)
    {
      LocalBuilder localBuilder = generator.DeclareLocal(field.FieldType);
      if (field.FieldType.GetInterface("IEnumerable") != (Type) null)
      {
        if (!field.FieldType.IsGenericType)
          return;
        Type type = Type.GetType("System.Collections.Generic.IEnumerable`1[" + field.FieldType.GetGenericArguments()[0].FullName + "]");
        ConstructorInfo constructor = field.FieldType.GetConstructor(new Type[1]{ type });
        if (!(constructor != (ConstructorInfo) null))
          return;
        generator.Emit(OpCodes.Ldarg_0);
        generator.Emit(OpCodes.Ldfld, field);
        generator.Emit(OpCodes.Newobj, constructor);
        generator.Emit(OpCodes.Stloc, localBuilder);
        CloneUtility.PlaceNewTempObjInClone(generator, field, localBuilder);
      }
      else
      {
        CloneUtility.CreateNewTempObject(generator, field.FieldType, localBuilder);
        CloneUtility.PlaceNewTempObjInClone(generator, field, localBuilder);
        foreach (FieldInfo field1 in field.FieldType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
          if (field1.FieldType.IsValueType || field1.FieldType == typeof (string))
            CloneUtility.CopyValueTypeTemp(generator, field, field1);
          else if (field1.FieldType.IsClass)
            CloneUtility.CopyReferenceType(generator, field1);
        }
      }
    }
  }
}
