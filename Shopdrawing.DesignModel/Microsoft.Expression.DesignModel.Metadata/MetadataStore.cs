// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.MetadataStore
// Assembly: Microsoft.Expression.DesignModel, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: CEEFEC81-4FB1-4567-B694-554E1BED5C03
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignModel.dll

using Microsoft.Windows.Design.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
    public static class MetadataStore
    {
        private static List<AttributeTable> tables = new List<AttributeTable>();
        private static Dictionary<string, Type> typeNameCache = new Dictionary<string, Type>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, Assembly> assemblyFullNames = new Dictionary<string, Assembly>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);

        static MetadataStore()
        {
            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(MetadataStore.OnAssemblyLoad);
        }

        private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (args.LoadedAssembly.ReflectionOnly)
                return;
            if (MetadataStore.typeNameCache.Count != 0)
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, Type> keyValuePair in MetadataStore.typeNameCache)
                {
                    if (keyValuePair.Value.Assembly.FullName.Equals(args.LoadedAssembly.FullName, StringComparison.OrdinalIgnoreCase))
                        list.Add(keyValuePair.Key);
                }
                list.ForEach((Action<string>)(key => MetadataStore.typeNameCache.Remove(key)));
            }
            if (MetadataStore.assemblyFullNames.Count == 0)
                return;
            MetadataStore.assemblyFullNames[args.LoadedAssembly.FullName] = args.LoadedAssembly;
        }

        public static void AddAttributeTable(AttributeTable table)
        {
            MetadataStore.tables.Insert(0, table);
        }

        public static TypeConverter GetTypeConverter(object value)
        {
            return MetadataStore.GetTypeConverter(value.GetType());
        }

        public static TypeConverter GetTypeConverterFromAttributes(Assembly targetAssembly, AttributeCollection attributes)
        {
            if (attributes != null)
            {
                TypeConverterAttribute converterAttribute = (TypeConverterAttribute)attributes[typeof(TypeConverterAttribute)];
                if (converterAttribute != null && !string.IsNullOrEmpty(converterAttribute.ConverterTypeName))
                {
                    Type typeFromName = MetadataStore.GetTypeFromName(targetAssembly, converterAttribute.ConverterTypeName);
                    try
                    {
                        return (TypeConverter)Activator.CreateInstance(typeFromName);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return (TypeConverter)null;
        }

        public static TypeConverter GetTypeConverter(Type type)
        {
            TypeConverter converterFromAttributes = MetadataStore.GetTypeConverterFromAttributes(type.Assembly, TypeUtilities.GetAttributes(type));
            if (converterFromAttributes != null)
                return converterFromAttributes;
            if (Nullable.GetUnderlyingType(type) != (Type)null)
                return (TypeConverter)new MetadataStore.NullableConverter(type);
            return TypeDescriptor.GetConverter(type);
        }

        private static Type GetTypeFromName(Assembly targetAssembly, string typeName)
        {
            if (typeName == null || typeName.Length == 0)
                return (Type)null;
            Type type = (Type)null;
            if (!MetadataStore.typeNameCache.TryGetValue(typeName, out type))
            {
                int startIndex = typeName.LastIndexOf(']');
                int length = startIndex != -1 ? typeName.IndexOf(',', startIndex) : typeName.IndexOf(',');
                if (length == -1)
                    type = targetAssembly.GetType(typeName);
                if (type == (Type)null && length != -1)
                {
                    string name = typeName.Substring(0, length);
                    string fullName;
                    try
                    {
                        fullName = new AssemblyName(typeName.Substring(length + 1)).FullName;
                    }
                    catch (IOException ex)
                    {
                        return (Type)null;
                    }
                    if (MetadataStore.assemblyFullNames.Count == 0)
                    {
                        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                            MetadataStore.assemblyFullNames[assembly.FullName] = assembly;
                    }
                    Assembly assembly1;
                    if (MetadataStore.assemblyFullNames.TryGetValue(fullName, out assembly1))
                        type = assembly1.GetType(name);
                }
                if (type == (Type)null)
                    type = Type.GetType(typeName);
                if (type != (Type)null)
                    MetadataStore.typeNameCache[typeName] = type;
            }
            return type;
        }

        public static IEnumerable<Attribute> GetAttributes(Type type)
        {
            foreach (AttributeTable attributeTable in MetadataStore.tables)
            {
                if (attributeTable.ContainsAttributes(type))
                {
                    foreach (Attribute attribute in attributeTable.GetCustomAttributes(type))
                        yield return attribute;
                }
            }
        }

        public static IEnumerable<Attribute> GetAttributes(Type ownerType, string memberName)
        {
            if (!(ownerType == (Type)null))
            {
                foreach (AttributeTable attributeTable in MetadataStore.tables)
                {
                    if (attributeTable.ContainsAttributes(ownerType))
                    {
                        foreach (Attribute attribute in attributeTable.GetCustomAttributes(ownerType, memberName))
                            yield return attribute;
                    }
                }
            }
        }

        private class NullableConverter : TypeConverter
        {
            private Type nullableType;
            private Type simpleType;
            private TypeConverter simpleTypeConverter;

            public NullableConverter(Type type)
            {
                this.nullableType = type;
                this.simpleType = Nullable.GetUnderlyingType(type);
                this.simpleTypeConverter = MetadataStore.GetTypeConverter(this.simpleType);
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == this.simpleType)
                    return true;
                if (this.simpleTypeConverter != null)
                    return this.simpleTypeConverter.CanConvertFrom(context, sourceType);
                return base.CanConvertFrom(context, sourceType);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == this.simpleType)
                    return true;
                if (this.simpleTypeConverter != null)
                    return this.simpleTypeConverter.CanConvertTo(context, destinationType);
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value == null || value.GetType() == this.simpleType)
                    return value;
                string str = value as string;
                if (str != null && string.IsNullOrEmpty(str))
                    return (object)null;
                if (this.simpleTypeConverter != null)
                    return this.simpleTypeConverter.ConvertFrom(context, culture, value);
                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == (Type)null)
                    throw new ArgumentNullException("destinationType");
                if (destinationType == this.simpleType && this.nullableType.IsInstanceOfType(value))
                    return value;
                if (value == null)
                {
                    if (destinationType == typeof(string))
                        return (object)string.Empty;
                }
                else if (this.simpleTypeConverter != null)
                    return this.simpleTypeConverter.ConvertTo(context, culture, value, destinationType);
                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
            {
                if (this.simpleTypeConverter != null)
                    return this.simpleTypeConverter.CreateInstance(context, propertyValues);
                return base.CreateInstance(context, propertyValues);
            }

            public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
            {
                if (this.simpleTypeConverter != null)
                    return this.simpleTypeConverter.GetCreateInstanceSupported(context);
                return base.GetCreateInstanceSupported(context);
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                if (this.simpleTypeConverter == null)
                    return base.GetProperties(context, value, attributes);
                object obj = value;
                return this.simpleTypeConverter.GetProperties(context, obj, attributes);
            }

            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                if (this.simpleTypeConverter != null)
                    return this.simpleTypeConverter.GetPropertiesSupported(context);
                return base.GetPropertiesSupported(context);
            }

            public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                //if (this.simpleTypeConverter != null)
                //{
                //    TypeConverter.StandardValuesCollection standardValues = this.simpleTypeConverter.GetStandardValues(context);
                //    if (this.GetStandardValuesSupported(context) && standardValues != null)
                //    {
                //        object[] objArray1 = new object[standardValues.Count + 1];
                //        int num1 = 0;
                //        object[] objArray2 = objArray1;
                //        int index = num1;
                //        int num2 = 1;
                //        int num3 = index + num2;
                //        // ISSUE: variable of the null type
                //        __Null local = null;
                //        objArray2[index] = (object)local;
                //        foreach (object obj in standardValues)
                //            objArray1[num3++] = obj;
                //        return new TypeConverter.StandardValuesCollection((ICollection)objArray1);
                //    }
                //}
                //return base.GetStandardValues(context);

                if (this.simpleTypeConverter != null)
                {
                    TypeConverter.StandardValuesCollection standardValues = this.simpleTypeConverter.GetStandardValues(context);
                    if (this.GetStandardValuesSupported(context) && standardValues != null)
                    {
                        object[] objArray = new object[standardValues.Count + 1];
                        int num = 0;
                        int num1 = num;
                        num = num1 + 1;
                        objArray[num1] = null;
                        foreach (object standardValue in standardValues)
                        {
                            int num2 = num;
                            num = num2 + 1;
                            objArray[num2] = standardValue;
                        }
                        return new TypeConverter.StandardValuesCollection(objArray);
                    }
                }
                return base.GetStandardValues(context);
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                if (this.simpleTypeConverter != null)
                    return this.simpleTypeConverter.GetStandardValuesExclusive(context);
                return base.GetStandardValuesExclusive(context);
            }

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                if (this.simpleTypeConverter != null)
                    return this.simpleTypeConverter.GetStandardValuesSupported(context);
                return base.GetStandardValuesSupported(context);
            }

            public override bool IsValid(ITypeDescriptorContext context, object value)
            {
                if (this.simpleTypeConverter == null)
                    return base.IsValid(context, value);
                object obj = value;
                if (obj != null)
                    return this.simpleTypeConverter.IsValid(context, obj);
                return true;
            }
        }
    }
}
