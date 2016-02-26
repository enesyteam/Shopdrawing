// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleDataDesignTimeTypeGenerator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SampleDataDesignTimeTypeGenerator
  {
    private Dictionary<SampleNonBasicType, SampleDataDesignTimeTypeGenerator.TypeBuildState> buildStates = new Dictionary<SampleNonBasicType, SampleDataDesignTimeTypeGenerator.TypeBuildState>();
    private static int uniqueVersion;
    private SampleDataSet dataSet;
    private string designTimeClrNamespace;

    private SampleDataDesignTimeTypeGenerator(SampleDataSet dataSet)
    {
      this.dataSet = dataSet;
    }

    public static void RebuildDesignTimeTypes(SampleDataSet dataSet)
    {
      new SampleDataDesignTimeTypeGenerator(dataSet).RebuildDesignTimeTypesInternal();
    }

    public static string SampleDataSetGuidFromType(Type type)
    {
      if (type == (Type) null || type.FullName == null || !DataSetContext.DesignTimeClrNamespacePrefixMatchesDataSetContext(type.FullName))
        return (string) null;
      return ((SampleDataSetAttribute) type.GetCustomAttributes(typeof (SampleDataSetAttribute), false)[0]).Guid;
    }

    private void RebuildDesignTimeTypesInternal()
    {
      this.ResetDesignTimeTypes();
      this.designTimeClrNamespace = this.dataSet.Context.DesignTimeClrNamespacePrefix + (object) SampleDataDesignTimeTypeGenerator.uniqueVersion++ + "." + this.dataSet.Name + ".";
      foreach (SampleType sampleType in this.dataSet.Types)
        this.GetOrBeginDesignTimeType(sampleType);
      foreach (SampleType sampleType in this.dataSet.Types)
        this.GetOrGenerateDesignTimeType(sampleType);
      this.buildStates.Clear();
    }

    private void ResetDesignTimeTypes()
    {
      foreach (SampleNonBasicType sampleType in this.dataSet.Types)
        this.ResetDesignTimeType(sampleType);
    }

    private void ResetDesignTimeType(SampleNonBasicType sampleType)
    {
      if (sampleType == null)
        return;
      sampleType.DesignTimeType = (Type) null;
      SampleCompositeType sampleCompositeType = sampleType as SampleCompositeType;
      if (sampleCompositeType == null)
        return;
      foreach (SampleProperty sampleProperty in (IEnumerable<SampleProperty>) sampleCompositeType.SampleProperties)
        sampleProperty.ResetPropertyInfo();
    }

    private Type GetOrBeginDesignTimeType(SampleType sampleType)
    {
      Type type;
      if (!sampleType.IsBasicType)
      {
        SampleNonBasicType key = (SampleNonBasicType) sampleType;
        SampleDataDesignTimeTypeGenerator.TypeBuildState typeBuildState = (SampleDataDesignTimeTypeGenerator.TypeBuildState) null;
        if (!this.buildStates.TryGetValue(key, out typeBuildState))
        {
          typeBuildState = !sampleType.IsCollection ? this.BeginCompositeType((SampleCompositeType) sampleType) : this.BeginCollectionType((SampleCollectionType) sampleType);
          this.buildStates[key] = typeBuildState;
        }
        type = typeBuildState.TypeBuilder.UnderlyingSystemType;
      }
      else
        type = this.dataSet.ResolveSampleType(sampleType).RuntimeType;
      return type;
    }

    private Type GetOrGenerateDesignTimeType(SampleType sampleType)
    {
      Type type;
      if (!sampleType.IsBasicType)
      {
        SampleNonBasicType index = (SampleNonBasicType) sampleType;
        SampleDataDesignTimeTypeGenerator.TypeBuildState buildState = this.buildStates[index];
        if (!buildState.IsGenerated)
        {
          buildState.IsGenerated = true;
          index.DesignTimeType = !sampleType.IsCollection ? this.GenerateCompositeType((SampleCompositeType) sampleType, buildState) : this.GenerateCollectionType((SampleCollectionType) sampleType, buildState);
        }
        type = index.DesignTimeType;
      }
      else
        type = this.dataSet.ResolveSampleType(sampleType).RuntimeType;
      return type;
    }

    private SampleDataDesignTimeTypeGenerator.TypeBuildState BeginCollectionType(SampleCollectionType collectionType)
    {
      Type parent = this.dataSet.ProjectContext.ResolveType(PlatformTypes.ObservableCollection).RuntimeType.MakeGenericType(this.GetOrBeginDesignTimeType(collectionType.ItemSampleType));
      TypeBuilder typeBuilder = RuntimeGeneratedTypesHelper.RuntimeGeneratedTypesAssembly.DefineType(this.designTimeClrNamespace + collectionType.Name, TypeAttributes.Public | TypeAttributes.Sealed, parent);
      ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
      return new SampleDataDesignTimeTypeGenerator.TypeBuildState(typeBuilder, constructorBuilder);
    }

    private Type GenerateCollectionType(SampleCollectionType collectionType, SampleDataDesignTimeTypeGenerator.TypeBuildState buildState)
    {
      collectionType.DesignTimeType = buildState.TypeBuilder.UnderlyingSystemType;
      Type type = this.dataSet.ProjectContext.ResolveType(PlatformTypes.ObservableCollection).RuntimeType.MakeGenericType(this.GetOrGenerateDesignTimeType(collectionType.ItemSampleType));
      ILGenerator ilGenerator = buildState.ConstructorBuilder.GetILGenerator();
      ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Call, constructor);
      ilGenerator.Emit(OpCodes.Ret);
      return this.GenerateDesignTimeType(buildState.TypeBuilder);
    }

    private SampleDataDesignTimeTypeGenerator.TypeBuildState BeginCompositeType(SampleCompositeType compositeType)
    {
      Type parent = compositeType == this.dataSet.RootType ? typeof (DesignTimeSampleRootType) : typeof (DesignTimeSampleCompositeType);
      TypeBuilder typeBuilder = RuntimeGeneratedTypesHelper.RuntimeGeneratedTypesAssembly.DefineType(this.designTimeClrNamespace + compositeType.Name, TypeAttributes.Public | TypeAttributes.Sealed, parent);
      ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
      return new SampleDataDesignTimeTypeGenerator.TypeBuildState(typeBuilder, constructorBuilder);
    }

    private Type GenerateCompositeType(SampleCompositeType compositeType, SampleDataDesignTimeTypeGenerator.TypeBuildState buildState)
    {
      compositeType.DesignTimeType = buildState.TypeBuilder.UnderlyingSystemType;
      ILGenerator ilGenerator = buildState.ConstructorBuilder.GetILGenerator();
      ConstructorInfo constructor = compositeType.DesignTimeType.BaseType.GetConstructor(Type.EmptyTypes);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Call, constructor);
      foreach (SampleProperty sampleProperty in (IEnumerable<SampleProperty>) compositeType.SampleProperties)
        this.GenerateProperty(buildState.TypeBuilder, sampleProperty, ilGenerator);
      ilGenerator.Emit(OpCodes.Ret);
      return this.GenerateDesignTimeType(buildState.TypeBuilder);
    }

    private Type GenerateDesignTimeType(TypeBuilder typeBuilder)
    {
      CustomAttributeBuilder customBuilder = new CustomAttributeBuilder(typeof (SampleDataSetAttribute).GetConstructor(new Type[2]
      {
        typeof (string),
        typeof (string)
      }), new object[2]
      {
        (object) this.dataSet.Name,
        (object) this.dataSet.Guid.ToString()
      });
      typeBuilder.SetCustomAttribute(customBuilder);
      return typeBuilder.CreateType();
    }

    private Type GenerateProperty(TypeBuilder typeBuilder, SampleProperty sampleProperty, ILGenerator ilDefaultConstructor)
    {
      Type beginDesignTimeType = this.GetOrBeginDesignTimeType(sampleProperty.PropertySampleType);
      FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + sampleProperty.Name, beginDesignTimeType, FieldAttributes.Private);
      PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(sampleProperty.Name, PropertyAttributes.HasDefault, fieldBuilder.FieldType, (Type[]) null);
      if (sampleProperty.IsBasicType || sampleProperty.IsCollection)
        this.InitializeField(fieldBuilder, ilDefaultConstructor, sampleProperty);
      MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
      MethodBuilder mdBuilder1 = typeBuilder.DefineMethod("get_" + sampleProperty.Name, attributes, fieldBuilder.FieldType, Type.EmptyTypes);
      ILGenerator ilGenerator1 = mdBuilder1.GetILGenerator();
      ilGenerator1.Emit(OpCodes.Ldarg_0);
      ilGenerator1.Emit(OpCodes.Ldfld, (FieldInfo) fieldBuilder);
      ilGenerator1.Emit(OpCodes.Ret);
      propertyBuilder.SetGetMethod(mdBuilder1);
      if (!sampleProperty.IsCollection)
      {
        MethodBuilder mdBuilder2 = typeBuilder.DefineMethod("set_" + sampleProperty.Name, attributes, (Type) null, new Type[1]
        {
          fieldBuilder.FieldType
        });
        ILGenerator ilGenerator2 = mdBuilder2.GetILGenerator();
        ilGenerator2.DeclareLocal(typeof (bool));
        MethodInfo method1 = typeof (object).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public);
        if (beginDesignTimeType.IsValueType)
        {
          ilGenerator2.Emit(OpCodes.Ldarg_0);
          ilGenerator2.Emit(OpCodes.Ldfld, (FieldInfo) fieldBuilder);
          ilGenerator2.Emit(OpCodes.Box, beginDesignTimeType);
          ilGenerator2.Emit(OpCodes.Ldarg_1);
          ilGenerator2.Emit(OpCodes.Box, beginDesignTimeType);
        }
        else
        {
          ilGenerator2.Emit(OpCodes.Ldarg_0);
          ilGenerator2.Emit(OpCodes.Ldfld, (FieldInfo) fieldBuilder);
          ilGenerator2.Emit(OpCodes.Ldarg_1);
        }
        ilGenerator2.Emit(OpCodes.Call, method1);
        Label label = ilGenerator2.DefineLabel();
        ilGenerator2.Emit(OpCodes.Stloc_0);
        ilGenerator2.Emit(OpCodes.Ldloc_0);
        ilGenerator2.Emit(OpCodes.Brtrue_S, label);
        ilGenerator2.Emit(OpCodes.Ldarg_0);
        ilGenerator2.Emit(OpCodes.Ldarg_1);
        ilGenerator2.Emit(OpCodes.Stfld, (FieldInfo) fieldBuilder);
        MethodInfo method2 = typeBuilder.BaseType.GetMethod("OnPropertyChanged");
        ilGenerator2.Emit(OpCodes.Ldarg_0);
        ilGenerator2.Emit(OpCodes.Ldstr, sampleProperty.Name);
        ilGenerator2.Emit(OpCodes.Call, method2);
        ilGenerator2.MarkLabel(label);
        ilGenerator2.Emit(OpCodes.Ret);
        propertyBuilder.SetSetMethod(mdBuilder2);
      }
      return beginDesignTimeType;
    }

    private void InitializeField(FieldBuilder fieldBuilder, ILGenerator ilConstructor, SampleProperty sampleProperty)
    {
      if (ilConstructor == null)
        return;
      if (!sampleProperty.IsBasicType)
      {
        SampleDataDesignTimeTypeGenerator.TypeBuildState typeBuildState = this.buildStates[(SampleNonBasicType) sampleProperty.PropertySampleType];
        ilConstructor.Emit(OpCodes.Ldarg_0);
        ilConstructor.Emit(OpCodes.Newobj, (ConstructorInfo) typeBuildState.ConstructorBuilder);
        ilConstructor.Emit(OpCodes.Stfld, (FieldInfo) fieldBuilder);
      }
      else
      {
        SampleBasicType sampleBasicType = (SampleBasicType) sampleProperty.PropertySampleType;
        if (sampleBasicType == SampleBasicType.String)
        {
          FieldInfo field = typeof (string).GetField("Empty", BindingFlags.Static | BindingFlags.Public);
          ilConstructor.Emit(OpCodes.Ldarg_0);
          ilConstructor.Emit(OpCodes.Ldsfld, field);
          ilConstructor.Emit(OpCodes.Stfld, (FieldInfo) fieldBuilder);
        }
        else if (sampleBasicType == SampleBasicType.Date)
        {
          PropertyInfo property = typeof (DateTime).GetProperty("Today", BindingFlags.Static | BindingFlags.Public);
          ilConstructor.Emit(OpCodes.Ldarg_0);
          ilConstructor.Emit(OpCodes.Call, property.GetGetMethod());
          ilConstructor.Emit(OpCodes.Stfld, (FieldInfo) fieldBuilder);
        }
        else if (sampleBasicType == SampleBasicType.Number)
        {
          ilConstructor.Emit(OpCodes.Ldarg_0);
          ilConstructor.Emit(OpCodes.Ldc_R8, 0.0);
          ilConstructor.Emit(OpCodes.Stfld, (FieldInfo) fieldBuilder);
        }
        else if (sampleBasicType == SampleBasicType.Boolean)
        {
          ilConstructor.Emit(OpCodes.Ldarg_0);
          ilConstructor.Emit(OpCodes.Ldc_I4, 0);
          ilConstructor.Emit(OpCodes.Stfld, (FieldInfo) fieldBuilder);
        }
        else
        {
          if (sampleBasicType != SampleBasicType.Image)
            return;
          ilConstructor.Emit(OpCodes.Ldarg_0);
          ilConstructor.Emit(OpCodes.Ldnull);
          ilConstructor.Emit(OpCodes.Stfld, (FieldInfo) fieldBuilder);
        }
      }
    }

    private class TypeBuildState
    {
      public TypeBuilder TypeBuilder { get; private set; }

      public ConstructorBuilder ConstructorBuilder { get; private set; }

      public bool IsGenerated { get; set; }

      public TypeBuildState(TypeBuilder typeBuilder, ConstructorBuilder constructorBuilder)
      {
        this.IsGenerated = false;
        this.TypeBuilder = typeBuilder;
        this.ConstructorBuilder = constructorBuilder;
      }
    }
  }
}
