// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.PlatformTypeHelper
// Assembly: Microsoft.Expression.DesignModel, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: CEEFEC81-4FB1-4567-B694-554E1BED5C03
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignModel.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Microsoft.Expression.DesignModel.Metadata
{
    public static class PlatformTypeHelper
    {
        public static readonly IConstructorArgumentProperties EmptyConstructorArgumentProperties = (IConstructorArgumentProperties)new PlatformTypeHelper.ConstructorArgumentProperties();
        private const BindingFlags InstanceBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        internal const BindingFlags StaticBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        private const BindingFlags InstanceOrStaticBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

        public static Type GetPropertyType(IProperty value)
        {
            IType propertyType = value.PropertyType;
            if (propertyType != null)
                return propertyType.RuntimeType;
            return (Type)null;
        }

        public static Type GetDeclaringType(IMember value)
        {
            IType declaringType = value.DeclaringType;
            if (declaringType != null)
                return declaringType.RuntimeType;
            return (Type)null;
        }

        public static IAssembly GetTargetAssembly(IType type)
        {
            IReferenceType referenceType = type as IReferenceType;
            if (referenceType != null)
            {
                IAssembly referenceAssembly = referenceType.ReferenceAssembly;
                if (referenceAssembly != null)
                    return referenceAssembly;
            }
            return type.RuntimeAssembly;
        }

        internal static Assembly GetRuntimeAssembly(IType type)
        {
            return AssemblyHelper.GetReflectionAssembly(type.RuntimeAssembly);
        }

        internal static Type GetType(Assembly assembly, string typeName)
        {
            try
            {
                return assembly.GetType(typeName, false);
            }
            catch (Exception ex)
            {
                return (Type)null;
            }
        }

        internal static Type GetType(IAssembly assembly, string typeName)
        {
            Assembly reflectionAssembly = AssemblyHelper.GetReflectionAssembly(assembly);
            if (reflectionAssembly != (Assembly)null)
                return PlatformTypeHelper.GetType(reflectionAssembly, typeName);
            return (Type)null;
        }

        internal static IType GetType(ITypeResolver typeResolver, IAssembly assembly, string typeName)
        {
            Type type = PlatformTypeHelper.GetType(assembly, typeName);
            if (type != (Type)null)
                return typeResolver.GetType(type);
            return (IType)null;
        }

        public static RoutedEvent GetRoutedEvent(IEvent value)
        {
            return ((Event)value).RoutedEvent as RoutedEvent;
        }

        public static IProperty GetProperty(ITypeResolver typeResolver, Type targetType, PropertyDescriptor propertyDescriptor)
        {
            if (typeResolver.IsCapabilitySet(PlatformCapability.UseDependencyPropertyDescriptor))
            {
                DependencyPropertyDescriptor propertyDescriptor1 = DependencyPropertyDescriptor.FromProperty(propertyDescriptor);
                if (propertyDescriptor1 != null)
                    return (IProperty)DependencyPropertyReferenceStep.GetReferenceStep(typeResolver, targetType, propertyDescriptor1.DependencyProperty);
            }
            IType type = typeResolver.GetType(targetType);
            if (typeResolver.PlatformMetadata.IsNullType((ITypeId)type))
                return (IProperty)null;
            MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(typeResolver, type);
            return type.GetMember(MemberType.Property, propertyDescriptor.Name, allowableMemberAccess) as IProperty;
        }

        public static IEvent GetEvent(ITypeResolver typeResolver, Type targetType, string memberName)
        {
            IType type = typeResolver.GetType(targetType);
            if (type == null)
                return (IEvent)null;
            MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(typeResolver, type);
            return (IEvent)type.GetMember(MemberType.Event, memberName, allowableMemberAccess);
        }

        public static IEvent GetEvent(ITypeResolver typeResolver, RoutedEvent routedEvent)
        {
            IType type = typeResolver.GetType(routedEvent.OwnerType);
            if (type != null)
            {
                MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(typeResolver, type);
                Event @event = (Event)type.GetMember(MemberType.Event, routedEvent.Name, allowableMemberAccess);
                if (@event != null && @event.RoutedEvent == routedEvent)
                    return (IEvent)@event;
            }
            return (IEvent)null;
        }

        internal static IMember GetMember(ITypeResolver typeResolver, Type type, MemberType memberTypes, string memberName)
        {
            if (TypeHelper.IsSet(memberTypes, MemberType.Property))
            {
                ITypeId typeId = (ITypeId)typeResolver.GetType(type);
                IProperty property = (IProperty)PlatformTypeHelper.GetProperty(typeResolver, typeId, memberTypes & MemberType.Property, memberName);
                if (property != null)
                    return (IMember)property;
            }
            if (TypeHelper.IsSet(memberTypes, MemberType.Event))
            {
                IEvent @event = PlatformTypeHelper.GetEvent(typeResolver, type, memberName);
                if (@event != null)
                    return (IMember)@event;
            }
            if (TypeHelper.IsSet(memberTypes, MemberType.Field))
            {
                FieldInfo fieldInfo = PlatformTypeHelper.GetFieldInfo(type, memberName);
                if (fieldInfo != (FieldInfo)null)
                    return (IMember)FieldReferenceStep.GetReferenceStep(typeResolver, fieldInfo);
            }
            if (TypeHelper.IsSet(memberTypes, MemberType.Method))
            {
                MethodInfo method = PlatformTypeHelper.GetMethod(type, memberName);
                if (method != (MethodInfo)null)
                {
                    IType type1 = typeResolver.GetType(method.DeclaringType);
                    if (type1 == null)
                        return (IMember)null;
                    return (IMember)Method.GetMethod(typeResolver, type1, method);
                }
            }
            return (IMember)null;
        }

        public static IMemberId GetRoutedCommandMember(ITypeResolver typeResolver, Type type, string commandName)
        {
            IType type1 = typeResolver.GetType(type);
            if (type1 == null)
                return (IMemberId)null;
            MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(typeResolver, type1);
            return type1.GetMember(MemberType.LocalProperty | MemberType.Field, commandName, allowableMemberAccess) ?? type1.GetMember(MemberType.LocalProperty | MemberType.Field, commandName + "Command", allowableMemberAccess);
        }

        internal static ConstructorInfo[] GetConstructors(Type type)
        {
            try
            {
                return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            }
            catch (Exception ex)
            {
            }
            return (ConstructorInfo[])null;
        }

        public static IList<IConstructor> GetConstructors(ITypeResolver typeResolver, IType typeId)
        {
            Type runtimeType = typeId.RuntimeType;
            List<IConstructor> list = (List<IConstructor>)null;
            if (runtimeType != (Type)null)
            {
                ConstructorInfo[] constructors = PlatformTypeHelper.GetConstructors(runtimeType);
                if (constructors != null && constructors.Length > 0)
                {
                    foreach (ConstructorInfo constructorInfo in constructors)
                    {
                        if (PlatformTypeHelper.IsAccessibleConstructor(constructorInfo))
                        {
                            if (list == null)
                                list = new List<IConstructor>(constructors.Length);
                            list.Add((IConstructor)new Constructor(typeResolver, typeId, constructorInfo, PlatformTypeHelper.GetNameWithParameters((MethodBase)constructorInfo)));
                        }
                    }
                }
            }
            if (list != null)
                return (IList<IConstructor>)new ReadOnlyCollection<IConstructor>((IList<IConstructor>)list);
            return (IList<IConstructor>)ReadOnlyCollections<IConstructor>.Empty;
        }

        internal static bool IsAccessibleConstructor(ConstructorInfo constructorInfo)
        {
            return !constructorInfo.IsPrivate;
        }

        internal static IConstructorArgumentProperties GetConstructorArgumentProperties(IType typeId)
        {
            if (!(typeId.RuntimeType != (Type)null) || !typeId.IsExpression)
                return PlatformTypeHelper.EmptyConstructorArgumentProperties;
            PlatformTypeHelper.ConstructorArgumentProperties argumentProperties = new PlatformTypeHelper.ConstructorArgumentProperties();
            foreach (IProperty property in typeId.Metadata.Properties)
            {
                ReferenceStep referenceStep = property as ReferenceStep;
                if (referenceStep != null)
                {
                    string constructorArgument = referenceStep.ConstructorArgument;
                    if (constructorArgument != null)
                        argumentProperties[constructorArgument] = property;
                }
            }
            return (IConstructorArgumentProperties)argumentProperties;
        }

        public static IList<IType> GetGenericTypeArguments(ITypeResolver typeResolver, Type type)
        {
            if (type != (Type)null && type.IsGenericType)
            {
                Type[] genericTypeArguments = PlatformTypeHelper.GetGenericTypeArguments(type);
                if (genericTypeArguments != null)
                {
                    List<IType> list = new List<IType>(genericTypeArguments.Length);
                    foreach (Type type1 in genericTypeArguments)
                    {
                        IType type2 = typeResolver.GetType(type1);
                        list.Add(type2);
                    }
                    return (IList<IType>)new ReadOnlyCollection<IType>((IList<IType>)list);
                }
            }
            return (IList<IType>)ReadOnlyCollections<IType>.Empty;
        }

        public static bool HasUnboundTypeArguments(Type type)
        {
            if (type.IsGenericType)
            {
                foreach (Type type1 in PlatformTypeHelper.GetGenericTypeArguments(type))
                {
                    if (type1.IsGenericParameter || PlatformTypeHelper.HasUnboundTypeArguments(type1))
                        return true;
                }
            }
            return false;
        }

        public static PropertyMetadata GetPropertyMetadata(DependencyProperty dependencyProperty, Type type)
        {
            if (typeof(DependencyObject).IsAssignableFrom(type))
            {
                PropertyMetadata metadata = dependencyProperty.GetMetadata(type);
                if (metadata != null)
                    return metadata;
            }
            return dependencyProperty.DefaultMetadata;
        }

        internal static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            return type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        }

        public static Type GetNullableType(Type type)
        {
            Type genericTypeDefinition = PlatformTypeHelper.GetGenericTypeDefinition(type);
            if (genericTypeDefinition != (Type)null && typeof(Nullable<>).IsAssignableFrom(genericTypeDefinition))
            {
                Type[] genericTypeArguments = PlatformTypeHelper.GetGenericTypeArguments(type);
                if (genericTypeArguments != null && genericTypeArguments.Length == 1)
                    return genericTypeArguments[0];
            }
            return (Type)null;
        }

        public static Type[] GetInterfaces(Type type)
        {
            try
            {
                return type.GetInterfaces();
            }
            catch (Exception ex)
            {
            }
            return (Type[])null;
        }

        public static Type GetGenericTypeDefinition(Type type)
        {
            try
            {
                if (type.IsGenericType)
                    return type.GetGenericTypeDefinition();
            }
            catch (Exception ex)
            {
            }
            return (Type)null;
        }

        public static Type[] GetGenericTypeArguments(Type type)
        {
            try
            {
                if (type.IsGenericType)
                    return type.GetGenericArguments();
            }
            catch (Exception ex)
            {
            }
            return (Type[])null;
        }

        internal static DesignerSerializationVisibility GetSerializationVisibility(PlatformTypes platformTypes, MemberInfo memberInfo)
        {
            DesignerSerializationVisibility result;
            if (PlatformNeutralAttributeHelper.TryGetAttributeValue<DesignerSerializationVisibility>((IEnumerable)platformTypes.GetCustomAttributes(memberInfo), PlatformTypes.DesignerSerializationVisibilityAttribute, "Visibility", out result))
                return result;
            return DesignerSerializationVisibility.Visible;
        }

        public static KeyValuePair<bool, object> GetDefaultValue(PlatformTypes platformTypes, MemberInfo memberInfo, Type valueType)
        {
            object result;
            if (PlatformNeutralAttributeHelper.TryGetAttributeValue<object>((IEnumerable)platformTypes.GetCustomAttributes(memberInfo), PlatformTypes.DefaultValueAttribute, "Value", out result) && (!valueType.IsValueType && (result == null || valueType.IsAssignableFrom(result.GetType())) || valueType.IsValueType && result != null && valueType.IsAssignableFrom(result.GetType())))
                return new KeyValuePair<bool, object>(true, result);
            return PlatformTypeHelper.GetDefaultValue(valueType);
        }

        public static KeyValuePair<bool, object> GetDefaultValue(Type valueType)
        {
            if (valueType.IsValueType)
                return new KeyValuePair<bool, object>(true, Activator.CreateInstance(valueType));
            return new KeyValuePair<bool, object>(false, (object)null);
        }

        public static Type GetReflectionType(ITypeId typeId)
        {
            IReflectionType reflectionType = typeId as IReflectionType;
            if (reflectionType != null)
                return reflectionType.ReflectionType;
            return (Type)null;
        }

        public static ReferenceStep GetProperty(ITypeResolver typeResolver, ITypeId typeId, MemberType memberTypes, string memberName)
        {
            IType typeId1 = typeResolver.ResolveType(typeId);
            if (typeResolver.PlatformMetadata.IsNullType((ITypeId)typeId1))
                return (ReferenceStep)null;
            MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(typeResolver, typeId1);
            return (ReferenceStep)typeId1.GetMember(memberTypes & MemberType.Property, memberName, allowableMemberAccess);
        }

        public static ConstructorInfo GetDefaultConstructor(Type type, bool supportInternal, out ConstructorAccessibility accessibility)
        {
            if (!type.IsNested ? type.IsPublic || type.IsNotPublic && supportInternal : type.IsNestedPublic || type.IsNestedAssembly && supportInternal)
            {
                if (type.IsInterface)
                    accessibility = ConstructorAccessibility.TypeIsInterface;
                else if (type.IsAbstract)
                    accessibility = ConstructorAccessibility.TypeIsAbstract;
                else if (type.ContainsGenericParameters)
                    accessibility = ConstructorAccessibility.TypeContainsGenericParameters;
                else if (type.IsValueType)
                {
                    accessibility = ConstructorAccessibility.TypeIsValueType;
                }
                else
                {
                    ConstructorInfo constructorInfo = (ConstructorInfo)null;
                    try
                    {
                        constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder)null, Type.EmptyTypes, (ParameterModifier[])null);
                    }
                    catch (Exception ex)
                    {
                    }
                    if (constructorInfo != (ConstructorInfo)null)
                    {
                        if (constructorInfo.IsPublic || constructorInfo.IsAssembly && supportInternal)
                        {
                            accessibility = ConstructorAccessibility.Accessible;
                            return constructorInfo;
                        }
                        accessibility = ConstructorAccessibility.ConstructorNotAccessible;
                    }
                    else
                        accessibility = ConstructorAccessibility.ConstructorMissing;
                }
            }
            else
                accessibility = ConstructorAccessibility.TypeNotAccessible;
            return (ConstructorInfo)null;
        }

        public static IType ConvertTypeId(ITypeId typeId, IPlatformMetadata targetPlatform)
        {
            IType type = typeId as IType;
            if (type != null && targetPlatform == type.PlatformMetadata)
                return type;
            return ((PlatformTypes)targetPlatform).GetPlatformType(typeId.FullName);
        }

        public static bool IsAccessibleType(ITypeResolver typeResolver, Type type)
        {
            IType type1 = typeResolver.GetType(type);
            if (type1 != null)
                return TypeHelper.IsAccessibleType(typeResolver, type1);
            return false;
        }

        public static MemberAccessType GetMemberAccess(Type type)
        {
            if (type.IsNested)
            {
                if (type.IsNestedPublic)
                    return MemberAccessType.Public;
                if (type.IsNestedAssembly)
                    return MemberAccessType.Internal;
            }
            else
            {
                if (type.IsPublic)
                    return MemberAccessType.Public;
                if (type.IsNotPublic)
                    return MemberAccessType.Internal;
            }
            return MemberAccessType.Private;
        }

        public static MemberAccessType GetMemberAccess(FieldInfo fieldInfo)
        {
            if (fieldInfo.IsPublic)
                return MemberAccessType.Public;
            if (fieldInfo.IsFamilyOrAssembly)
                return MemberAccessType.ProtectedOrInternal;
            if (fieldInfo.IsFamily)
                return MemberAccessType.Protected;
            return fieldInfo.IsAssembly ? MemberAccessType.Internal : MemberAccessType.Private;
        }

        public static MemberAccessType GetMemberAccess(MethodBase methodInfo)
        {
            if (methodInfo.IsPublic)
                return MemberAccessType.Public;
            if (methodInfo.IsFamilyOrAssembly)
                return MemberAccessType.ProtectedOrInternal;
            if (methodInfo.IsFamily)
                return MemberAccessType.Protected;
            return methodInfo.IsAssembly ? MemberAccessType.Internal : MemberAccessType.Private;
        }

        public static Type GetPropertyType(PropertyInfo propertyInfo)
        {
            try
            {
                return propertyInfo.PropertyType;
            }
            catch (Exception ex)
            {
                return (Type)null;
            }
        }

        public static IType GetPropertyTypeId(ITypeResolver typeResolver, PropertyInfo propertyInfo)
        {
            Type propertyType = PlatformTypeHelper.GetPropertyType(propertyInfo);
            if (propertyType != (Type)null)
                return typeResolver.GetType(propertyType);
            return (IType)null;
        }

        public static Type GetFieldType(FieldInfo fieldInfo)
        {
            try
            {
                return fieldInfo.FieldType;
            }
            catch (Exception ex)
            {
                return (Type)null;
            }
        }

        public static IType GetFieldTypeId(ITypeResolver typeResolver, FieldInfo fieldInfo)
        {
            Type fieldType = PlatformTypeHelper.GetFieldType(fieldInfo);
            if (fieldType != (Type)null)
                return typeResolver.GetType(fieldType);
            return (IType)null;
        }

        public static Type GetValueType(MethodInfo methodInfo)
        {
            try
            {
                return methodInfo.ReturnType;
            }
            catch (Exception ex)
            {
                return (Type)null;
            }
        }

        public static Type GetValueType(ParameterInfo parameterInfo)
        {
            try
            {
                return parameterInfo.ParameterType;
            }
            catch (Exception ex)
            {
                return (Type)null;
            }
        }

        public static string GetNameWithParameters(MethodBase methodInfo)
        {
            StringBuilder stringBuilder = new StringBuilder(methodInfo.Name);
            stringBuilder.Append("(");
            int num = 0;
            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
            {
                if (num > 0)
                    stringBuilder.Append(", ");
                stringBuilder.Append(parameterInfo.ParameterType.Name);
                ++num;
            }
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }

        public static MethodInfo GetMethod(Type type, string methodName)
        {
            return PlatformTypeHelper.GetMethod(type, methodName, -1, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        }

        private static MethodInfo GetMethod(Type type, string methodName, int numberOfArguments, BindingFlags bindingFlags)
        {
            MemberInfo[] member = type.GetMember(methodName, MemberTypes.Method, bindingFlags);
            MethodInfo methodInfo1 = (MethodInfo)null;
            foreach (MethodInfo methodInfo2 in member)
            {
                if (methodInfo2 != (MethodInfo)null)
                {
                    if (numberOfArguments >= 0)
                    {
                        ParameterInfo[] parameters = PlatformTypeHelper.GetParameters(methodInfo2);
                        if (parameters == null || parameters.Length != numberOfArguments)
                            continue;
                    }
                    if (methodInfo1 == (MethodInfo)null || methodInfo1.DeclaringType != methodInfo2.DeclaringType && methodInfo1.DeclaringType.IsAssignableFrom(methodInfo2.DeclaringType))
                        methodInfo1 = methodInfo2;
                }
            }
            return methodInfo1;
        }

        public static ParameterInfo[] GetParameters(MethodInfo methodInfo)
        {
            try
            {
                return methodInfo.GetParameters();
            }
            catch (Exception ex)
            {
                return (ParameterInfo[])null;
            }
        }

        public static ParameterInfo[] GetIndexParameters(PropertyInfo propertyInfo)
        {
            try
            {
                return propertyInfo.GetIndexParameters();
            }
            catch (Exception ex)
            {
                return (ParameterInfo[])null;
            }
        }

        [Conditional("DEBUG")]
        public static void ValidateMemberName(string memberName)
        {
        }

        private sealed class ConstructorArgumentProperties : IConstructorArgumentProperties, IEnumerable<string>, IEnumerable
        {
            private Dictionary<string, IProperty> dictionary;

            public int Count
            {
                get
                {
                    return this.dictionary.Count;
                }
            }

            public IProperty this[string argumentName]
            {
                get
                {
                    IProperty property;
                    this.dictionary.TryGetValue(argumentName, out property);
                    return property;
                }
                set
                {
                    this.dictionary[argumentName] = value;
                }
            }

            public ConstructorArgumentProperties()
            {
                this.dictionary = new Dictionary<string, IProperty>();
            }

            public IEnumerator<string> GetEnumerator()
            {
                return (IEnumerator<string>)this.dictionary.Keys.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)this.GetEnumerator();
            }
        }
    }
}
