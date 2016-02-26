// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.PropertyReference
// Assembly: Microsoft.Expression.DesignModel, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: CEEFEC81-4FB1-4567-B694-554E1BED5C03
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignModel.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignModel.Metadata
{
    public class PropertyReference : IComparable
    {
        private static Dictionary<PropertyReference.CastPair, MethodInfo> castOperators = new Dictionary<PropertyReference.CastPair, MethodInfo>();
        private static Dictionary<Assembly, string[]> knownAssemblyNamespaces = new Dictionary<Assembly, string[]>();
        private static string[] knownPlatformNamespaces = new string[9]
    {
      "System.Windows",
      "System.Windows.Controls",
      "System.Windows.Controls.Primitives",
      "System.Windows.Documents",
      "System.Windows.Shapes",
      "System.Windows.Media",
      "System.Windows.Media.Media3D",
      "System.Windows.Media.Animation",
      "System.Windows.Media.Effects"
    };
        private const char pathSeparatorChar = '/';
        private const char typeSeparatorChar = '.';
        private const string blendTypePrefix = "Microsoft.Expression";
        private const string blendType = "Blend";
        private ReferenceStep[] referenceSteps;
        private string path;

        public string Path
        {
            get
            {
                if (this.path == null)
                    this.InitializePath();
                return this.path;
            }
        }

        public string ShortPath
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int index = 0; index < this.Count; ++index)
                {
                    string name = this[index].Name;
                    if (index > 0 && (int)name[0] != 91)
                        stringBuilder.Append('/');
                    stringBuilder.Append(name);
                }
                return stringBuilder.ToString();
            }
        }

        public int Count
        {
            get
            {
                return this.referenceSteps.Length;
            }
        }

        public ReferenceStep this[int index]
        {
            get
            {
                return this.referenceSteps[index];
            }
        }

        public ReadOnlyCollection<ReferenceStep> ReferenceSteps
        {
            get
            {
                return new ReadOnlyCollection<ReferenceStep>((IList<ReferenceStep>)this.referenceSteps);
            }
        }

        public Type TargetType
        {
            get
            {
                return this[0].TargetType;
            }
        }

        public Type ValueType
        {
            get
            {
                return PlatformTypeHelper.GetPropertyType((IProperty)this[this.Count - 1]);
            }
        }

        public IType ValueTypeId
        {
            get
            {
                return this[this.Count - 1].PropertyType;
            }
        }

        public ReferenceStep FirstStep
        {
            get
            {
                return this[0];
            }
        }

        public ReferenceStep LastStep
        {
            get
            {
                return this[this.Count - 1];
            }
        }

        public IPlatformMetadata PlatformMetadata
        {
            get
            {
                return this.referenceSteps[0].DeclaringType.PlatformMetadata;
            }
        }

        public PropertyReference(ITypeResolver typeResolver, string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            this.CompilePath(typeResolver, path);
        }

        public PropertyReference(ReferenceStep singleStep)
        {
            if (singleStep == null)
                throw new ArgumentNullException("singleStep");
            this.referenceSteps = new ReferenceStep[1];
            this.referenceSteps[0] = singleStep;
        }

        public PropertyReference(ReferenceStep firstStep, ReferenceStep secondStep)
        {
            if (firstStep == null)
                throw new ArgumentNullException("firstStep");
            if (secondStep == null)
                throw new ArgumentNullException("secondStep");
            this.referenceSteps = new ReferenceStep[2];
            this.referenceSteps[0] = firstStep;
            this.referenceSteps[1] = secondStep;
        }

        private PropertyReference(ReferenceStep[] steps)
        {
            if (steps == null)
                throw new ArgumentNullException("steps");
            if (steps.Length == 0)
                throw new ArgumentException(ExceptionStringTable.CannotConstructPropertyReferenceFromEmptyCollectionOfReferenceSteps, "referenceSteps");
            this.referenceSteps = steps;
        }

        public PropertyReference(List<ReferenceStep> steps)
        {
            if (steps == null)
                throw new ArgumentNullException("steps");
            if (steps.Count == 0)
                throw new ArgumentException(ExceptionStringTable.CannotConstructPropertyReferenceFromEmptyCollectionOfReferenceSteps, "referenceSteps");
            this.referenceSteps = new ReferenceStep[steps.Count];
            for (int index = 0; index < steps.Count; ++index)
            {
                ReferenceStep referenceStep = steps[index];
                if (referenceStep == null)
                    throw new ArgumentNullException("Step " + (object)index + " is null");
                this.referenceSteps[index] = referenceStep;
            }
        }

        public PropertyReference(Stack<ReferenceStep> input)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (input.Count == 0)
                throw new ArgumentException(ExceptionStringTable.CannotConstructPropertyReferenceFromEmptyCollectionOfReferenceSteps, "referenceSteps");
            this.referenceSteps = new ReferenceStep[input.Count];
            int index = 0;
            foreach (ReferenceStep referenceStep in input)
            {
                this.referenceSteps[index] = referenceStep;
                ++index;
            }
        }

        [Conditional("DEBUG")]
        private void Validate()
        {
            int index = 1;
            while (index < this.referenceSteps.Length && (this.referenceSteps[index] != null && this.referenceSteps[0].PropertyType.PlatformMetadata == this.referenceSteps[index].PropertyType.PlatformMetadata))
                ++index;
        }

        public static PropertyReference CreateNewPropertyReferenceFromStepsWithoutCopy(ReferenceStep[] steps)
        {
            return new PropertyReference(steps);
        }

        public IProperty[] GetPropertyKeys()
        {
            return (IProperty[])this.referenceSteps;
        }

        public PropertyReference Subreference(int startIndex)
        {
            return this.Subreference(startIndex, -1);
        }

        public PropertyReference Subreference(int startIndex, int endIndex)
        {
            if (endIndex == -1)
                endIndex = this.Count - 1;
            ReferenceStep[] steps = new ReferenceStep[endIndex - startIndex + 1];
            for (int index = startIndex; index <= endIndex; ++index)
                steps[index - startIndex] = this.referenceSteps[index];
            return new PropertyReference(steps);
        }

        public PropertyReference Append(IPropertyId propertyKey)
        {
            return this.Append(this.PlatformMetadata.ResolveProperty(propertyKey) as ReferenceStep);
        }

        public PropertyReference Append(ReferenceStep step)
        {
            if (step == null)
                return this;
            ReferenceStep[] steps = new ReferenceStep[this.Count + 1];
            for (int index = 0; index < this.Count; ++index)
                steps[index] = this.referenceSteps[index];
            steps[this.Count] = step;
            return new PropertyReference(steps);
        }

        public PropertyReference Append(PropertyReference propertyReference)
        {
            if (propertyReference == null)
                return this;
            int count = this.Count;
            ReferenceStep[] steps = new ReferenceStep[count + propertyReference.Count];
            for (int index = 0; index < this.Count; ++index)
                steps[index] = this.referenceSteps[index];
            for (int index = 0; index < propertyReference.Count; ++index)
                steps[index + count] = propertyReference.referenceSteps[index];
            return new PropertyReference(steps);
        }

        public string BuildPropertyPath(out object[] dependencyProperties, bool shouldGenerateProperties)
        {
            string stringPath = (string)null;
            List<object> list = this.InternalBuildPropertyPathParts(out stringPath, shouldGenerateProperties);
            dependencyProperties = list.ToArray();
            return stringPath;
        }

        private List<object> InternalBuildPropertyPathParts(out string stringPath, bool shouldGenerateProperties)
        {
            string[] array1 = new string[4]
      {
        "CanonicalTransform.ScaleTransform",
        "CanonicalTransform.SkewTransform",
        "CanonicalTransform.RotateTransform",
        "CanonicalTransform.TranslateTransform"
      };
            string[] array2 = new string[5]
      {
        "CanonicalTransform3D.CenterToOriginTransform",
        "CanonicalTransform3D.ScaleTransform",
        "CanonicalTransform3D.RotateTransform",
        "CanonicalTransform3D.OriginToCenterTransform",
        "CanonicalTransform3D.TranslateTransform"
      };
            StringBuilder stringBuilder = new StringBuilder();
            List<object> list = new List<object>();
            foreach (ReferenceStep referenceStep in this.ReferenceSteps)
            {
                DependencyPropertyReferenceStep propertyReferenceStep1;
                if ((propertyReferenceStep1 = referenceStep as DependencyPropertyReferenceStep) != null)
                {
                    if (stringBuilder.Length > 0)
                        stringBuilder.Append(".");
                    if (shouldGenerateProperties)
                    {
                        stringBuilder.Append("(" + (object)list.Count + ")");
                        list.Add(propertyReferenceStep1.DependencyProperty);
                    }
                    else
                    {
                        stringBuilder.Append("(");
                        stringBuilder.Append(propertyReferenceStep1.DeclaringTypeId.Name);
                        stringBuilder.Append(".");
                        stringBuilder.Append(propertyReferenceStep1.Name);
                        stringBuilder.Append(")");
                    }
                }
                else
                {
                    IndexedClrPropertyReferenceStep propertyReferenceStep2;
                    if ((propertyReferenceStep2 = referenceStep as IndexedClrPropertyReferenceStep) != null)
                    {
                        stringBuilder.Append("[" + (object)propertyReferenceStep2.Index + "]");
                    }
                    else
                    {
                        ClrPropertyReferenceStep propertyReferenceStep3;
                        if ((propertyReferenceStep3 = referenceStep as ClrPropertyReferenceStep) != null)
                        {
                            int num1;
                            if ((num1 = Array.IndexOf<string>(array1, propertyReferenceStep3.ToString())) >= 0)
                            {
                                if (stringBuilder.Length > 0)
                                    stringBuilder.Append(".");
                                stringBuilder.Append("(" + (object)list.Count + ")[" + (string)(object)num1 + "]");
                                list.Add((object)TransformGroup.ChildrenProperty);
                            }
                            else
                            {
                                int num2;
                                if ((num2 = Array.IndexOf<string>(array2, propertyReferenceStep3.ToString())) >= 0)
                                {
                                    if (stringBuilder.Length > 0)
                                        stringBuilder.Append(".");
                                    stringBuilder.Append("(" + (object)list.Count + ")[" + (string)(object)num2 + "]");
                                    list.Add((object)Transform3DGroup.ChildrenProperty);
                                }
                                else
                                {
                                    if (stringBuilder.Length > 0)
                                        stringBuilder.Append(".");
                                    stringBuilder.Append(propertyReferenceStep3.Name);
                                }
                            }
                        }
                    }
                }
            }
            stringPath = stringBuilder.ToString();
            return list;
        }

        public object GetValue(object target)
        {
            return this.PartialGetValue(target, 0, this.Count - 1);
        }

        public object PartialGetValue(object target, int initialStepIndex, int finalStepIndex)
        {
            if (initialStepIndex < 0)
                throw new ArgumentOutOfRangeException("initialStepIndex");
            if (finalStepIndex >= this.Count)
                throw new ArgumentOutOfRangeException("finalStepIndex");
            if (initialStepIndex > finalStepIndex)
                throw new ArgumentException(ExceptionStringTable.InitialStepIndexMustBeLessThanOrEqualToFinalStepIndex);
            Type fromType = target != null ? target.GetType() : typeof(object);
            for (int index = initialStepIndex; index <= finalStepIndex; ++index)
            {
                ReferenceStep referenceStep = this[index];
                Type fromReferenceStep = this.GetTargetTypeFromReferenceStep(referenceStep);
                target = PropertyReference.EnsureCorrectType(target, fromType, fromReferenceStep);
                if (target == null)
                    return (object)null;
                target = referenceStep.GetValue(target);
                fromType = PlatformTypeHelper.GetPropertyType((IProperty)referenceStep);
            }
            return target;
        }

        public object GetBaseValue(object target)
        {
            return this.PartialGetBaseValue(target, 0, this.Count - 1);
        }

        public object PartialGetBaseValue(object target, int initialStepIndex, int finalStepIndex)
        {
            if (initialStepIndex < 0)
                throw new ArgumentOutOfRangeException("initialStepIndex");
            if (finalStepIndex >= this.Count)
                throw new ArgumentOutOfRangeException("finalStepIndex");
            if (initialStepIndex > finalStepIndex)
                throw new ArgumentException(ExceptionStringTable.InitialStepIndexMustBeLessThanOrEqualToFinalStepIndex);
            Type fromType = target != null ? target.GetType() : typeof(object);
            for (int index = initialStepIndex; index <= finalStepIndex; ++index)
            {
                ReferenceStep referenceStep = this[index];
                Type fromReferenceStep = this.GetTargetTypeFromReferenceStep(referenceStep);
                target = PropertyReference.EnsureCorrectType(target, fromType, fromReferenceStep);
                if (target == null)
                    return (object)null;
                target = referenceStep.GetBaseValue(target);
                fromType = PlatformTypeHelper.GetPropertyType((IProperty)referenceStep);
            }
            return target;
        }

        public object GetCurrentValue(object target)
        {
            return this.PartialGetCurrentValue(target, 0, this.Count - 1);
        }

        public object PartialGetCurrentValue(object target, int initialStepIndex, int finalStepIndex)
        {
            return this.PartialGetValue(target, initialStepIndex, finalStepIndex);
        }

        public object SetValue(object target, object valueToSet)
        {
            return this.ModifyValue(target, PropertyReference.Modification.SetValue, valueToSet, 0, this.Count - 1, false, -1);
        }

        public object SetValue(object target, object valueToSet, bool useReference)
        {
            return this.ModifyValue(target, PropertyReference.Modification.SetValue, valueToSet, 0, this.Count - 1, useReference, -1);
        }

        public object PartialSetValue(object target, object valueToSet, int initialStepIndex, int finalStepIndex)
        {
            return this.PartialSetValue(target, valueToSet, initialStepIndex, finalStepIndex, false);
        }

        public object PartialSetValue(object target, object valueToSet, int initialStepIndex, int finalStepIndex, bool useReference)
        {
            if (initialStepIndex < 0)
                throw new ArgumentOutOfRangeException("initialStepIndex");
            if (finalStepIndex >= this.Count)
                throw new ArgumentOutOfRangeException("finalStepIndex");
            if (initialStepIndex > finalStepIndex)
                throw new ArgumentException(ExceptionStringTable.InitialStepIndexMustBeLessThanOrEqualToFinalStepIndex);
            return this.ModifyValue(target, PropertyReference.Modification.SetValue, valueToSet, initialStepIndex, finalStepIndex, useReference, -1);
        }

        public object PartialAdd(object target, int index, object valueToAdd, int initialStepIndex, int finalStepIndex)
        {
            if (initialStepIndex < 0)
                throw new ArgumentOutOfRangeException("initialStepIndex");
            if (finalStepIndex >= this.Count)
                throw new ArgumentOutOfRangeException("finalStepIndex");
            if (initialStepIndex > finalStepIndex)
                throw new ArgumentException(ExceptionStringTable.InitialStepIndexMustBeLessThanOrEqualToFinalStepIndex);
            return this.ModifyValue(target, PropertyReference.Modification.Insert, valueToAdd, initialStepIndex, finalStepIndex, false, index);
        }

        public object PartialRemoveAt(object target, int initialStepIndex, int finalStepIndex, int index)
        {
            if (initialStepIndex < 0)
                throw new ArgumentOutOfRangeException("initialStepIndex");
            if (finalStepIndex >= this.Count)
                throw new ArgumentOutOfRangeException("finalStepIndex");
            if (initialStepIndex > finalStepIndex)
                throw new ArgumentException(ExceptionStringTable.InitialStepIndexMustBeLessThanOrEqualToFinalStepIndex);
            return this.ModifyValue(target, PropertyReference.Modification.Remove, (object)null, initialStepIndex, finalStepIndex, false, index);
        }

        public bool IsSet(object target)
        {
            Type fromType = target != null ? target.GetType() : typeof(object);
            foreach (ReferenceStep referenceStep in this.referenceSteps)
            {
                if (!referenceStep.IsSet(target))
                    return false;
                Type fromReferenceStep = this.GetTargetTypeFromReferenceStep(referenceStep);
                target = PropertyReference.EnsureCorrectType(target, fromType, fromReferenceStep);
                target = referenceStep.GetValue(target);
                fromType = PlatformTypeHelper.GetPropertyType((IProperty)referenceStep);
            }
            return true;
        }

        public bool IsValidPath(IViewObject target, ITypeResolver typeResolver)
        {
            bool flag;
            if (target == null)
            {
                return false;
            }
            try
            {
                Type type = (target != null ? target.TargetType : typeof(object));
                IPlatformTypes platformMetadata = (IPlatformTypes)this.PlatformMetadata;
                IPlatformTypes platformType = (IPlatformTypes)typeResolver.PlatformMetadata;
                PropertyReference platformPropertyReference = this;
                if (platformType != platformMetadata)
                {
                    platformPropertyReference = PlatformTypes.ConvertToPlatformPropertyReference(this, platformType);
                }
                if (platformPropertyReference != null)
                {
                    object value = (target != null ? target.PlatformSpecificObject : null);
                    ReferenceStep[] referenceStepArray = platformPropertyReference.referenceSteps;
                    int num = 0;
                    while (num < (int)referenceStepArray.Length)
                    {
                        ReferenceStep referenceStep = referenceStepArray[num];
                        DependencyPropertyReferenceStep dependencyPropertyReferenceStep = referenceStep as DependencyPropertyReferenceStep;
                        if (dependencyPropertyReferenceStep != null && typeResolver != null)
                        {
                            referenceStep = DependencyPropertyReferenceStep.GetReferenceStep(typeResolver, type, dependencyPropertyReferenceStep, MemberType.Property);
                            if (referenceStep == null)
                            {
                                flag = false;
                                return flag;
                            }
                        }
                        if (referenceStep.TargetType.IsAssignableFrom(type))
                        {
                            value = PropertyReference.EnsureCorrectType(value, type, referenceStep.TargetType);
                            value = referenceStep.GetValue(value);
                            type = (value != null ? value.GetType() : typeof(object));
                            num++;
                        }
                        else
                        {
                            flag = false;
                            return flag;
                        }
                    }
                    return true;
                }
                else
                {
                    flag = false;
                }
            }
            catch (TargetInvocationException targetInvocationException)
            {
                flag = false;
            }
            return flag;
        }

        public bool Contains(IPropertyId property, ITypeResolver typeResolver)
        {
            ReferenceStep referenceStep = typeResolver.ResolveProperty(property) as ReferenceStep;
            if (referenceStep != null)
                return this.ReferenceSteps.Contains(referenceStep);
            return false;
        }

        public int IndexOf(IPropertyId property, ITypeResolver typeResolver)
        {
            ReferenceStep referenceStep = typeResolver.ResolveProperty(property) as ReferenceStep;
            if (referenceStep != null)
                return this.ReferenceSteps.IndexOf(referenceStep);
            return -1;
        }

        public void ClearValue(object target)
        {
            this.PartialClearValue(target, 0, this.Count - 1);
        }

        public void PartialClearValue(object target, int initialStepIndex, int finalStepIndex)
        {
            if (initialStepIndex < 0)
                throw new ArgumentOutOfRangeException("initialStepIndex");
            if (finalStepIndex >= this.Count)
                throw new ArgumentOutOfRangeException("finalStepIndex");
            if (initialStepIndex > finalStepIndex)
                throw new ArgumentException(ExceptionStringTable.InitialStepIndexMustBeLessThanOrEqualToFinalStepIndex);
            Type fromType = target != null ? target.GetType() : typeof(object);
            for (int index = initialStepIndex; index < finalStepIndex; ++index)
            {
                ReferenceStep referenceStep = this[index];
                target = PropertyReference.EnsureCorrectType(target, fromType, referenceStep.TargetType);
                target = referenceStep.GetValue(target);
                fromType = PlatformTypeHelper.GetPropertyType((IProperty)referenceStep);
            }
            this[finalStepIndex].ClearValue(target);
        }

        public void Clear(object target)
        {
            this.ModifyValue(target, PropertyReference.Modification.Clear, (object)null, false);
        }

        public void Clear(object target, bool useReference)
        {
            this.ModifyValue(target, PropertyReference.Modification.Clear, (object)null, useReference);
        }

        public void Insert(object target, int index, object valueToAdd)
        {
            this.ModifyValue(target, PropertyReference.Modification.Insert, valueToAdd, false, index);
        }

        public void Insert(object target, int index, object valueToAdd, bool useReference)
        {
            this.ModifyValue(target, PropertyReference.Modification.Insert, valueToAdd, useReference, index);
        }

        public void Remove(object target, object valueToRemove)
        {
            this.ModifyValue(target, PropertyReference.Modification.Remove, valueToRemove, false);
        }

        public void RemoveAt(object target, int index)
        {
            this.ModifyValue(target, PropertyReference.Modification.Remove, (object)null, false, index);
        }

        public static void RegisterAssemblyNamespace(Assembly containingAssembly, string[] namespaces)
        {
            PropertyReference.knownAssemblyNamespaces[containingAssembly] = namespaces;
        }

        public static void RegisterAssemblyNamespace(Assembly containingAssembly, string knownNamespace)
        {
            string[] strArray1 = (string[])null;
            if (!PropertyReference.knownAssemblyNamespaces.TryGetValue(containingAssembly, out strArray1))
                strArray1 = new string[0];
            if (Array.IndexOf<string>(strArray1, knownNamespace) == -1)
            {
                string[] strArray2 = new string[strArray1.Length + 1];
                Array.Copy((Array)strArray1, (Array)strArray2, strArray1.Length);
                strArray2[strArray2.Length - 1] = knownNamespace;
                strArray1 = strArray2;
            }
            PropertyReference.RegisterAssemblyNamespace(containingAssembly, strArray1);
        }

        public bool IsPrefixOf(PropertyReference other)
        {
            if (this.Count >= other.Count)
                return false;
            for (int index = 0; index < this.Count; ++index)
            {
                if (this[index].SortValue != other[index].SortValue || !this[index].GetType().Equals(other[index].GetType()))
                    return false;
            }
            return true;
        }

        public bool IsIdentical(PropertyReference other)
        {
            if (this.Count != other.Count)
                return false;
            for (int index = 0; index < this.Count; ++index)
            {
                if (this.referenceSteps[index] != other.referenceSteps[index])
                    return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            PropertyReference propertyReference = obj as PropertyReference;
            return propertyReference != null && this.PlatformMetadata == propertyReference.PlatformMetadata && !(this.Path != propertyReference.Path);
        }

        public override int GetHashCode()
        {
            return this.PlatformMetadata.GetHashCode() ^ this.Path.GetHashCode();
        }

        public override string ToString()
        {
            return this.Path;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            PropertyReference propertyReference = (PropertyReference)obj;
            for (int index = 0; index < this.Count; ++index)
            {
                if (index >= propertyReference.Count)
                    return 1;
                if (this[index].SortValue < propertyReference[index].SortValue)
                    return -1;
                if (this[index].SortValue > propertyReference[index].SortValue)
                    return 1;
            }
            return propertyReference.Count > this.Count ? -1 : 0;
        }

        private void InitializePath()
        {
            StringBuilder stringBuilder = new StringBuilder();
            Type type = (Type)null;
            for (int index = 0; index < this.referenceSteps.Length; ++index)
            {
                ReferenceStep referenceStep = this.referenceSteps[index];
                string name = referenceStep.Name;
                Type runtimeType = referenceStep.DeclaringType.RuntimeType;
                if (index > 0 && (int)name[0] != 91)
                    stringBuilder.Append('/');
                if (runtimeType != type)
                {
                    stringBuilder.Append(runtimeType.Name);
                    if ((int)name[0] != 91)
                        stringBuilder.Append('.');
                }
                stringBuilder.Append(name);
                type = referenceStep.PropertyType.RuntimeType;
            }
            this.path = stringBuilder.ToString();
        }

        private void CompilePath(ITypeResolver typeResolver, string path)
        {
            string[] strArray = path.Split('/');
            this.path = (string)null;
            ArrayList steps = new ArrayList(2 * strArray.Length);
            Type declaringType = (Type)null;
            for (int index = 0; index < strArray.Length; ++index)
                declaringType = PropertyReference.CompileStep(typeResolver, steps, declaringType, strArray[index]);
            this.referenceSteps = new ReferenceStep[steps.Count];
            steps.CopyTo((Array)this.referenceSteps);
        }

        private static Type CompileStep(ITypeResolver typeResolver, ArrayList steps, Type declaringType, string name)
        {
            int num = name.IndexOf('[');
            ReferenceStep referenceStep1;
            if (num >= 0)
            {
                string indexString = name.Substring(num);
                if (indexString.LastIndexOf(']') != indexString.Length - 1)
                    throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, ExceptionStringTable.UnmatchedBracketsInName, new object[1]
          {
            (object) name
          }), "name");
                string str = name.Substring(0, num);
                ReferenceStep referenceStep2 = PropertyReference.CompilePropertyStep(typeResolver, declaringType, str, false);
                if (referenceStep2 != null)
                {
                    steps.Add((object)referenceStep2);
                    declaringType = PlatformTypeHelper.GetPropertyType((IProperty)referenceStep2);
                    str = (string)null;
                }
                referenceStep1 = PropertyReference.CompileIndexerStep(typeResolver, declaringType, str, indexString);
            }
            else
                referenceStep1 = PropertyReference.CompilePropertyStep(typeResolver, declaringType, name, true);
            steps.Add((object)referenceStep1);
            return PlatformTypeHelper.GetPropertyType((IProperty)referenceStep1);
        }

        private static ReferenceStep CompileIndexerStep(ITypeResolver typeResolver, Type declaringType, string prefix, string indexString)
        {
            if (prefix != null)
                declaringType = PropertyReference.GetNewDeclaringType(typeResolver, declaringType, prefix, (string)null, true);
            int index = int.Parse(indexString.Substring(1, indexString.Length - 2), (IFormatProvider)CultureInfo.InvariantCulture);
            return (ReferenceStep)IndexedClrPropertyReferenceStep.GetReferenceStep(typeResolver, declaringType, index);
        }

        private static ReferenceStep CompilePropertyStep(ITypeResolver typeResolver, Type declaringType, string name, bool throwOnFailure)
        {
            Type declaringType1 = declaringType;
            int length = name.LastIndexOf('.');
            string str1 = name;
            if (length >= 0)
            {
                string str2 = name.Substring(0, length);
                str1 = name.Substring(length + 1);
                if (str1.Length == 0)
                {
                    if (throwOnFailure)
                        throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, ExceptionStringTable.NameMustEndWithAValidMemberName, new object[1]
            {
              (object) '.'
            }), "name");
                    return (ReferenceStep)null;
                }
                IType platformType = ((PlatformTypes)typeResolver.PlatformMetadata).GetPlatformType(str2);
                if (platformType != null)
                    declaringType1 = platformType.RuntimeType;
                declaringType = PropertyReference.GetNewDeclaringType(typeResolver, declaringType1, str2, str1, throwOnFailure);
                if (declaringType == (Type)null)
                    return (ReferenceStep)null;
            }
            if (declaringType == (Type)null)
            {
                if (throwOnFailure)
                    throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, ExceptionStringTable.NoDeclaringTypeWasPassedIn, new object[1]
          {
            (object) str1
          }), "name");
                return (ReferenceStep)null;
            }
            ReferenceStep referenceStep = PlatformTypeHelper.GetMember(typeResolver, declaringType, MemberType.Property | MemberType.Field, str1) as ReferenceStep;
            if (referenceStep != null)
                return referenceStep;
            if (throwOnFailure)
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, ExceptionStringTable.TypeDoesNotDeclareMember, new object[2]
        {
          (object) declaringType.Name,
          (object) str1
        }), "name");
            return (ReferenceStep)null;
        }

        private static Type GetNewDeclaringType(ITypeResolver typeResolver, Type declaringType, string newDeclaringTypeName, string propertyName, bool throwOnFailure)
        {
            if (newDeclaringTypeName.Length == 0)
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, ExceptionStringTable.NoDeclaringTypeWasSpecified, new object[1]
        {
          (object) '.'
        }), "newDeclaringTypeName");
            Type typeFromName = PropertyReference.GetTypeFromName(typeResolver, newDeclaringTypeName, throwOnFailure);
            if (typeFromName == (Type)null)
            {
                if (throwOnFailure)
                    throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, ExceptionStringTable.TypeIsNotAValidType, new object[1]
          {
            (object) newDeclaringTypeName
          }), "newDeclaringTypeName");
                return (Type)null;
            }
            if (declaringType != (Type)null && declaringType.IsSubclassOf(typeFromName))
                return declaringType;
            if (typeof(DependencyObject).IsAssignableFrom(declaringType) && propertyName != null && PlatformTypes.GetDependencyPropertyFromName(propertyName, typeFromName) != null || (!(declaringType != (Type)null) || !(typeFromName != declaringType) || (typeFromName.IsSubclassOf(declaringType) || PropertyReference.CanFindCast(declaringType, typeFromName))))
                return typeFromName;
            if (throwOnFailure)
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, ExceptionStringTable.TypeIsNotASubclassOfDeclaringType, new object[2]
        {
          (object) newDeclaringTypeName,
          (object) declaringType.Name
        }), "newDeclaringTypeName");
            return (Type)null;
        }

        private static bool CanFindCast(Type firstType, Type secondType)
        {
            PropertyReference.CastPair castPair = new PropertyReference.CastPair(firstType, secondType);
            if (PropertyReference.castOperators.ContainsKey(castPair))
                return true;
            MethodInfo[] methods1 = firstType.GetMethods(BindingFlags.Static | BindingFlags.Public);
            MethodInfo[] methods2 = secondType.GetMethods(BindingFlags.Static | BindingFlags.Public);
            MethodInfo[] methodInfos = new MethodInfo[methods1.Length + methods2.Length];
            methods1.CopyTo((Array)methodInfos, 0);
            methods2.CopyTo((Array)methodInfos, methods1.Length);
            bool castMethod = PropertyReference.FindCastMethod(castPair, methodInfos);
            PropertyReference.FindCastMethod(new PropertyReference.CastPair(secondType, firstType), methodInfos);
            return castMethod;
        }

        private static bool FindCastMethod(PropertyReference.CastPair castPair, MethodInfo[] methodInfos)
        {
            foreach (MethodInfo methodInfo in methodInfos)
            {
                if ((methodInfo.Name == "op_Explicit" || methodInfo.Name == "op_Implicit") && (methodInfo.ReturnType == castPair.ToType && methodInfo.GetParameters()[0].ParameterType.IsAssignableFrom(castPair.FromType)))
                {
                    PropertyReference.castOperators[castPair] = methodInfo;
                    return true;
                }
            }
            return false;
        }

        private static Type GetTypeFromName(ITypeResolver typeResolver, string name, bool throwOnFailure)
        {
            if (name.LastIndexOf('.') > 0)
            {
                Type type = PropertyReference.LookupType(typeResolver, name);
                if (type != (Type)null)
                    return type;
            }
            else
            {
                foreach (object obj in PropertyReference.knownPlatformNamespaces)
                {
                    string fullName = (string)obj + (object)'.' + name;
                    Type type = PropertyReference.LookupType(typeResolver, fullName);
                    if (type != (Type)null)
                        return type;
                }
                foreach (KeyValuePair<Assembly, string[]> keyValuePair in PropertyReference.knownAssemblyNamespaces)
                {
                    Assembly key = keyValuePair.Key;
                    string[] strArray = keyValuePair.Value;
                    foreach (object obj in strArray)
                    {
                        string name1 = (string)obj + (object)'.' + name;
                        Type type = key.GetType(name1, false);
                        if (type != (Type)null)
                            return type;
                    }
                }
            }
            if (throwOnFailure)
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, ExceptionStringTable.CouldNotFindType, new object[1]
        {
          (object) name
        }), "name");
            return (Type)null;
        }

        private static Type LookupType(ITypeResolver typeResolver, string fullName)
        {
            IType platformType = ((PlatformTypes)typeResolver.PlatformMetadata).GetPlatformType(fullName);
            if (platformType != null && platformType.RuntimeType != (Type)null)
                return platformType.RuntimeType;
            foreach (IAssembly assembly in (IEnumerable<IAssembly>)typeResolver.AssemblyReferences)
            {
                IType type = typeResolver.GetType(assembly.Name, fullName);
                if (type != null && type.RuntimeType != (Type)null)
                    return type.RuntimeType;
            }
            return (Type)null;
        }

        [Conditional("DEBUG")]
        private static void ValidateType(object objToValidate, Type type)
        {
            if (objToValidate == null)
            {
                if (type.IsValueType && (!type.IsGenericType || !typeof(Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
                    throw new InvalidOperationException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, ExceptionStringTable.GotNullObjectWhenValueTypeWasExpected, new object[1]
          {
            (object) type.Name
          }));
            }
            else if (!type.IsInstanceOfType(objToValidate))
                throw new InvalidOperationException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, ExceptionStringTable.GotObjectOfIncorrectType, new object[2]
        {
          (object) objToValidate.GetType().Name,
          (object) type.Name
        }));
        }

        private object ModifyValue(object target, PropertyReference.Modification modification, object valueToUse, bool useReference)
        {
            return this.ModifyValue(target, modification, valueToUse, useReference, -1);
        }

        private object ModifyValue(object target, PropertyReference.Modification modification, object valueToUse, bool useReference, int index)
        {
            return this.ModifyValue(target, modification, valueToUse, 0, this.Count - 1, useReference, index);
        }

        private object ModifyValue(object target, PropertyReference.Modification modification, object valueToUse, int initialStepIndex, int finalStepIndex, bool useReference, int index)
        {
            object[] objArray1 = new object[finalStepIndex - initialStepIndex + 2];
            objArray1[0] = target;
            int num = finalStepIndex + 1;
            Type fromType1 = target != null ? target.GetType() : typeof(object);
            for (int index1 = initialStepIndex; index1 < num; ++index1)
            {
                ReferenceStep referenceStep = this[index1];
                int index2 = index1 - initialStepIndex;
                objArray1[index2] = PropertyReference.EnsureCorrectType(objArray1[index2], fromType1, referenceStep.TargetType);
                objArray1[index2 + 1] = referenceStep.GetValue(objArray1[index2]);
                objArray1[index2 + 1] = PropertyReference.EnsureCorrectType(objArray1[index2 + 1], typeof(object), PlatformTypeHelper.GetPropertyType((IProperty)referenceStep));
                fromType1 = PlatformTypeHelper.GetPropertyType((IProperty)referenceStep);
            }
            object[] objArray2 = new object[finalStepIndex - initialStepIndex + 2];
            for (int index1 = 0; index1 < objArray1.Length; ++index1)
                objArray2[index1] = PropertyReference.CopyObjectSafely(objArray1[index1]);
            if (modification == PropertyReference.Modification.SetValue)
            {
                objArray2[finalStepIndex - initialStepIndex + 1] = valueToUse;
            }
            else
            {
                IList list = objArray2[finalStepIndex - initialStepIndex + 1] as IList;
                if (list == null)
                    throw new InvalidOperationException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, ExceptionStringTable.MustBeAnIList, new object[2]
          {
            (object) modification,
            (object) objArray2[finalStepIndex - initialStepIndex + 1].GetType().Name
          }));
                switch (modification)
                {
                    case PropertyReference.Modification.Clear:
                        list.Clear();
                        break;
                    case PropertyReference.Modification.Insert:
                        if (index != -1)
                        {
                            list.Insert(index, valueToUse);
                            break;
                        }
                        list.Add(valueToUse);
                        break;
                    case PropertyReference.Modification.Remove:
                        if (index != -1)
                        {
                            list.RemoveAt(index);
                            break;
                        }
                        list.Remove(valueToUse);
                        break;
                    default:
                        throw new InvalidEnumArgumentException("modification", (int)modification, typeof(PropertyReference.Modification));
                }
            }
            for (int index1 = finalStepIndex; index1 >= initialStepIndex; --index1)
            {
                ReferenceStep referenceStep = this[index1];
                int index2 = index1 - initialStepIndex;
                Type fromType2 = index1 != initialStepIndex ? PlatformTypeHelper.GetPropertyType((IProperty)this[index1 - 1]) : (objArray2[0] != null ? objArray2[0].GetType() : typeof(object));
                objArray2[index2] = PropertyReference.EnsureCorrectType(objArray2[index2], fromType2, referenceStep.TargetType);
                Type fromType3 = index1 != finalStepIndex ? this[index1 + 1].TargetType : (objArray2[index2 + 1] != null ? objArray2[index2 + 1].GetType() : typeof(object));
                objArray2[index2 + 1] = PropertyReference.EnsureCorrectType(objArray2[index2 + 1], fromType3, PlatformTypeHelper.GetPropertyType((IProperty)referenceStep));
                if (!object.ReferenceEquals(objArray1[index2 + 1], objArray2[index2 + 1]))
                    objArray2[index2] = referenceStep.SetValue(objArray2[index2], objArray2[index2 + 1]);
            }
            if (initialStepIndex > 0)
            {
                Type propertyType = PlatformTypeHelper.GetPropertyType((IProperty)this[initialStepIndex - 1]);
                objArray2[0] = PropertyReference.EnsureCorrectType(objArray2[0], this[initialStepIndex].TargetType, propertyType);
            }
            return objArray2[0];
        }

        private Type GetTargetTypeFromReferenceStep(ReferenceStep referenceStep)
        {
            DependencyPropertyReferenceStep propertyReferenceStep = referenceStep as DependencyPropertyReferenceStep;
            if (propertyReferenceStep == null)
                return referenceStep.TargetType;
            Type runtimeType = propertyReferenceStep.PlatformTypes.ResolveType(PlatformTypes.DependencyObject).RuntimeType;
            if (referenceStep.TargetType.IsAssignableFrom(runtimeType))
                return referenceStep.TargetType;
            return runtimeType;
        }

        public static object EnsureCorrectType(object objToEnsure, Type fromType, Type toType)
        {
            if (toType.IsInstanceOfType(objToEnsure) || objToEnsure == null && !toType.IsValueType)
                return objToEnsure;
            MethodInfo methodInfo = (MethodInfo)null;
            if (PropertyReference.CanFindCast(fromType, toType) && PropertyReference.castOperators.TryGetValue(new PropertyReference.CastPair(fromType, toType), out methodInfo))
                return methodInfo.Invoke((object)null, new object[1]
        {
          objToEnsure
        });
            foreach (PropertyReference.CastPair index in PropertyReference.castOperators.Keys)
            {
                if (toType == index.ToType && index.FromType.IsAssignableFrom(fromType))
                    return PropertyReference.castOperators[index].Invoke((object)null, new object[1]
          {
            objToEnsure
          });
            }
            return (object)null;
        }

        private static object CopyObjectSafely(object sourceObject)
        {
            object obj = sourceObject;
            if (sourceObject != null)
            {
                Type type = sourceObject.GetType();
                if (type.IsValueType)
                {
                    obj = Activator.CreateInstance(type);
                    foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                        fieldInfo.SetValue(obj, fieldInfo.GetValue(sourceObject));
                }
                else
                {
                    Freezable freezable = sourceObject as Freezable;
                    if (freezable != null && freezable.IsFrozen)
                        obj = (object)freezable.Clone();
                }
            }
            return obj;
        }

        [Flags]
        public enum GetValueFlags
        {
            Local = 0,
            Computed = 1,
        }

        private struct CastPair
        {
            private Type from;
            private Type to;

            public Type FromType
            {
                get
                {
                    return this.from;
                }
            }

            public Type ToType
            {
                get
                {
                    return this.to;
                }
            }

            public CastPair(Type from, Type to)
            {
                this.from = from;
                this.to = to;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is PropertyReference.CastPair))
                    return false;
                PropertyReference.CastPair castPair = (PropertyReference.CastPair)obj;
                if (castPair.from == this.from)
                    return castPair.to == this.to;
                return false;
            }

            public override int GetHashCode()
            {
                return this.from.GetHashCode() ^ this.to.GetHashCode();
            }
        }

        public class Comparer : IComparer<PropertyReference>
        {
            public int Compare(PropertyReference x, PropertyReference y)
            {
                return x.CompareTo((object)y);
            }
        }

        private enum Modification
        {
            SetValue,
            Clear,
            Insert,
            Remove,
        }
    }
}
