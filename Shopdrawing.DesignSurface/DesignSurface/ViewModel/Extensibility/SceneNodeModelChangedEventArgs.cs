// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Extensibility.SceneNodeModelChangedEventArgs
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel.Extensibility
{
  public class SceneNodeModelChangedEventArgs : ModelChangedEventArgs
  {
    private SceneViewModel model;
    private DocumentNodeChange change;

    public override IEnumerable<ModelItem> ItemsAdded
    {
      get
      {
        if (this.change.IsChildChange && this.change.NewChildNode != null)
          yield return (ModelItem) this.model.GetSceneNode(this.change.NewChildNode).ModelItem;
      }
    }

    public override IEnumerable<ModelItem> ItemsRemoved
    {
      get
      {
        if (this.change.IsChildChange && this.change.OldChildNode != null)
          yield return (ModelItem) this.model.GetSceneNode(this.change.OldChildNode).ModelItem;
      }
    }

    public override IEnumerable<ModelProperty> PropertiesChanged
    {
      get
      {
        if (this.change.IsPropertyChange)
          yield return (ModelProperty) new SceneNodeModelProperty(this.model.GetSceneNode((DocumentNode) this.change.ParentNode).ModelItem, this.change.PropertyKey);
      }
    }

    public override IEnumerable<string> PropertyNamesChanged
    {
      get
      {
        if (this.change.IsPropertyChange)
          yield return this.change.PropertyKey.Name;
      }
    }

    public SceneNodeModelChangedEventArgs(SceneViewModel model, DocumentNodeChange change)
    {
      this.model = model;
      this.change = change;
    }
  }
}
