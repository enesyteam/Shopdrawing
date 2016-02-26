// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ProjectAttributeHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public static class ProjectAttributeHelper
  {
    public static ReadOnlyCollection<DefaultStateRecord> GetDefaultStateRecords(IType controlType, ITypeResolver typeResolver)
    {
      List<DefaultStateRecord> list = new List<DefaultStateRecord>();
      if (controlType == null || typeResolver == null || (typeResolver.PlatformMetadata.IsNullType((ITypeId) controlType) || typeResolver.PlatformMetadata.IsNullType((ITypeId) typeResolver.ResolveType(ProjectNeutralTypes.VisualStateManager))))
        return new ReadOnlyCollection<DefaultStateRecord>((IList<DefaultStateRecord>) list);
      foreach (Attribute attribute in TypeUtilities.GetAttributes(controlType.RuntimeType))
      {
        Type type1 = attribute.GetType();
        IType type2 = typeResolver.GetType(type1);
        if (!typeResolver.PlatformMetadata.IsNullType((ITypeId) type2) && ProjectNeutralTypes.TemplateVisualStateAttribute.IsAssignableFrom((ITypeId) type2))
        {
          DefaultStateRecord defaultStateRecord = new DefaultStateRecord();
          PropertyInfo property1 = type1.GetProperty("Name");
          defaultStateRecord.StateName = property1.GetValue((object) attribute, (object[]) null) as string;
          PropertyInfo property2 = type1.GetProperty("GroupName");
          defaultStateRecord.GroupName = property2.GetValue((object) attribute, (object[]) null) as string;
          if (!string.IsNullOrEmpty(defaultStateRecord.StateName) && !string.IsNullOrEmpty(defaultStateRecord.GroupName))
            list.Add(defaultStateRecord);
        }
      }
      return new ReadOnlyCollection<DefaultStateRecord>((IList<DefaultStateRecord>) list);
    }

    public static ReadOnlyCollection<TemplatePartAttributeRecord> GetTemplatePartAttributes(IType controlType, ITypeResolver typeResolver)
    {
      List<TemplatePartAttributeRecord> list = new List<TemplatePartAttributeRecord>();
      if (controlType == null || typeResolver == null || typeResolver.PlatformMetadata.IsNullType((ITypeId) controlType))
        return new ReadOnlyCollection<TemplatePartAttributeRecord>((IList<TemplatePartAttributeRecord>) list);
      foreach (Attribute attribute in TypeUtilities.GetAttributes(controlType.RuntimeType))
      {
        Type type1 = attribute.GetType();
        IType type2 = typeResolver.GetType(type1);
        if (PlatformTypes.TemplatePartAttribute.IsAssignableFrom((ITypeId) type2))
        {
          TemplatePartAttributeRecord partAttributeRecord = new TemplatePartAttributeRecord();
          PropertyInfo property1 = type1.GetProperty("Name");
          partAttributeRecord.NameAttribute = property1.GetValue((object) attribute, (object[]) null) as string;
          PropertyInfo property2 = type1.GetProperty("Type");
          partAttributeRecord.TypeAttribute = property2.GetValue((object) attribute, (object[]) null) as Type;
          list.Add(partAttributeRecord);
        }
      }
      return new ReadOnlyCollection<TemplatePartAttributeRecord>((IList<TemplatePartAttributeRecord>) list);
    }
  }
}
