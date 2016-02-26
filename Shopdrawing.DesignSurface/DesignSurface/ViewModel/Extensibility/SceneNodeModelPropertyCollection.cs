// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Extensibility.SceneNodeModelPropertyCollection
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.ViewModel.Extensibility
{
  public class SceneNodeModelPropertyCollection : ModelPropertyCollection
  {
    private SceneNode sceneNode;
    private ReadOnlyCollection<IProperty> properties;

    public SceneNodeModelPropertyCollection(SceneNode sceneNode)
    {
      this.sceneNode = sceneNode;
      this.properties = this.sceneNode.GetProperties();
    }

    protected override ModelProperty Find(string name, bool throwOnError)
    {
      IEnumerator<ModelProperty> enumerator = this.GetEnumerator();
      ModelProperty modelProperty = (ModelProperty) null;
      while (enumerator.MoveNext())
      {
        if (enumerator.Current.Name == name)
        {
          if (!enumerator.Current.IsAttached)
            return enumerator.Current;
          modelProperty = enumerator.Current;
        }
      }
      if (modelProperty != (ModelProperty) null)
        return modelProperty;
      if (throwOnError)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ExtensibilityPropertyNotFoundException, new object[1]
        {
          (object) name
        }));
      return (ModelProperty) null;
    }

    protected override ModelProperty Find(PropertyIdentifier value, bool throwOnError)
    {
      ModelProperty modelProperty = this.Find(value.Name, false);
      if (modelProperty != (ModelProperty) null)
        return modelProperty;
      Type type1 = value.DeclaringType;
      if (type1 == (Type) null)
        type1 = this.sceneNode.ViewModel.ExtensibilityManager.ResolveType(value.DeclaringTypeIdentifier);
      if (!type1.IsAssignableFrom(this.sceneNode.TargetType))
      {
        IType type2 = this.sceneNode.ProjectContext.GetType(type1);
        if (type2 != null)
        {
          IProperty property = type2.GetMember(MemberType.AttachedProperty | MemberType.DependencyProperty, value.Name, MemberAccessTypes.All) as IProperty;
          if (property != null)
            return (ModelProperty) new SceneNodeModelProperty(this.sceneNode.ModelItem, property);
        }
      }
      if (throwOnError)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ExtensibilityPropertyNotFoundException, new object[1]
        {
          (object) value.Name
        }));
      return (ModelProperty) null;
    }

    public override IEnumerator<ModelProperty> GetEnumerator()
    {
      foreach (IProperty property in this.properties)
        yield return (ModelProperty) new SceneNodeModelProperty(this.sceneNode.ModelItem, property);
    }
  }
}
