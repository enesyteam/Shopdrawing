// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor.BrushSubtypeEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor
{
  internal abstract class BrushSubtypeEditor : BrushPropertyEditor
  {
    public abstract BrushCategory Category { get; }

    public abstract DataTemplate EditorTemplate { get; }

    public ObservableCollection<PropertyEntry> AdvancedProperties { get; private set; }

    protected BrushSubtypeEditor(BrushEditor brushEditor, SceneNodeProperty basisProperty)
      : base(brushEditor, basisProperty)
    {
      this.AdvancedProperties = new ObservableCollection<PropertyEntry>();
    }

    protected SceneNodeProperty RegisterProperty(IPropertyId propertyId, PropertyChangedEventHandler handler)
    {
      return this.RequestUpdates(this.BasisProperty.Reference.Append((ReferenceStep) this.BasisProperty.Reference.PlatformMetadata.ResolveProperty(propertyId)), handler);
    }

    protected void UnregisterProperty(SceneNodeProperty property)
    {
      this.RemovePropertyChangedListener(property);
    }
  }
}
