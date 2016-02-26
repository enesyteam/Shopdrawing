// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeCollectionProperty
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Windows.Design.PropertyEditing;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class SceneNodeCollectionProperty : SceneNodeProperty
  {
    private PropertyReference baseReference;

    public SceneNodeCollectionProperty(SceneNodeCollectionObjectSet objectSet, PropertyReference propertyReference, AttributeCollection attributes, PropertyValue parentValue)
      : base((SceneNodeObjectSet) objectSet, propertyReference, attributes, parentValue)
    {
      this.baseReference = objectSet.BaseReference;
    }

    protected override void OnPropertyReferenceChanged(PropertyReferenceChangedEventArgs e)
    {
      if (e.PropertyReference.Count > this.baseReference.Count && this.baseReference.IsPrefixOf(e.PropertyReference))
        e = new PropertyReferenceChangedEventArgs(e.DirtyViewState, e.PropertyReference.Subreference(this.baseReference.Count));
      base.OnPropertyReferenceChanged(e);
    }
  }
}
