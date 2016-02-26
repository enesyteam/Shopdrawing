// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.ParentAdapter
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Model;
using MS.Internal.Features;
using System;

namespace Microsoft.Windows.Design.Interaction
{
  [FeatureConnector(typeof (ParentAdapterFeatureConnector))]
  public abstract class ParentAdapter : Adapter
  {
    public override Type AdapterType
    {
      get
      {
        return typeof (ParentAdapter);
      }
    }

    public virtual bool CanParent(ModelItem parent, Type childType)
    {
      if (parent == null)
        throw new ArgumentNullException("parent");
      if (childType == null)
        throw new ArgumentNullException("childType");
      return true;
    }

    public virtual bool IsParent(ModelItem parent, ModelItem child)
    {
      if (parent == null)
        throw new ArgumentNullException("parent");
      if (child == null)
        throw new ArgumentNullException("child");
      return child.Parent == parent;
    }

    public virtual ModelItem RedirectParent(ModelItem parent, Type childType)
    {
      if (parent == null)
        throw new ArgumentNullException("parent");
      if (childType == null)
        throw new ArgumentNullException("childType");
      return parent;
    }

    public virtual void Parent(ModelItem newParent, ModelItem child, int insertionIndex)
    {
      this.Parent(newParent, child);
    }

    public abstract void Parent(ModelItem newParent, ModelItem child);

    public abstract void RemoveParent(ModelItem currentParent, ModelItem newParent, ModelItem child);
  }
}
