// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.JoltHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface
{
    internal static class JoltHelper
    {
        public static bool DatabindingSupported(IProjectContext projectContext)
        {
            if (projectContext != null)
                return projectContext.IsCapabilitySet(PlatformCapability.SupportsDatabinding);
            return false;
        }

        public static bool TriggersSupported(IProjectContext projectContext)
        {
            if (projectContext != null)
                return projectContext.IsCapabilitySet(PlatformCapability.SupportsTriggers);
            return false;
        }

        public static bool TypeAnimationSupported(IProjectContext projectContext, Type type)
        {
            return projectContext != null;
        }

        public static bool TypeSupported(ITypeResolver typeResolver, ITypeId type)
        {
            if (typeResolver == null)
                return false;
            return typeResolver.PlatformMetadata.IsSupported(typeResolver, type);
        }

        public static bool TypeHasEnumValues(IType typeId)
        {
            Type type = typeId != null ? typeId.RuntimeType : (Type)null;
            if (type == (Type)null)
                return false;
            if (!type.IsEnum && !PlatformTypes.Cursor.Equals(typeId) && (!PlatformTypes.FontWeight.Equals(typeId) && !PlatformTypes.FontStretch.Equals((typeId)) && !PlatformTypes.FontStyle.Equals(typeId)))
                return type == typeof(TextDecoration);
            return true;
        }

        public static int CompareDoubles(IProjectContext projectContext, double d1, double d2)
        {
            if (projectContext.IsCapabilitySet(PlatformCapability.UsesFloatsInternally))
                return ((float)d1).CompareTo((float)d2);
            return d1.CompareTo(d2);
        }

        public static double RoundDouble(IProjectContext projectContext, double d)
        {
            if (projectContext.IsCapabilitySet(PlatformCapability.UsesFloatsInternally))
                return RoundingHelper.RoundToDoublePrecision(d, 6);
            return d;
        }

        public static GridLength RoundGridLength(IProjectContext projectContext, GridLength gridLength)
        {
            return new GridLength(JoltHelper.RoundDouble(projectContext, gridLength.Value), gridLength.GridUnitType);
        }

        internal static bool CanCreateTypeInXaml(IProjectContext projectContext, IType valueTypeId)
        {
            if (valueTypeId.PlatformMetadata == projectContext.PlatformMetadata && valueTypeId.RuntimeType != (Type)null)
                return TypeUtilities.CanCreateTypeInXaml((ITypeResolver)projectContext, valueTypeId.RuntimeType);
            return false;
        }

        public static object GetComputedValueFromLocalSceneNode(SceneNode node, PropertyReference propertyReference)
        {
            SceneNode valueAsSceneNode1 = node.GetLocalValueAsSceneNode(propertyReference);
            if (valueAsSceneNode1 != null)
                return valueAsSceneNode1.CreateInstance();
            if (propertyReference.Count > 1)
            {
                SceneNode valueAsSceneNode2 = node.GetLocalValueAsSceneNode((IPropertyId)propertyReference[0]);
                if (valueAsSceneNode2 != null)
                    return propertyReference.Subreference(1).GetValue(valueAsSceneNode2.CreateInstance());
            }
            return null;
        }
    }
}
