// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Extensibility.SceneNodeModelItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.ViewModel.Extensibility
{
  public class SceneNodeModelItem : ModelItem, ISceneNodeModelItem
  {
    private SceneNode sceneNode;

    public SceneNode SceneNode
    {
      get
      {
        return this.sceneNode;
      }
    }

    public override ModelItem Root
    {
      get
      {
        return (ModelItem) this.sceneNode.ViewModel.GetSceneNode(this.sceneNode.DocumentNode.DocumentRoot.RootNode).ModelItem;
      }
    }

    public override ViewItem View
    {
      get
      {
        if (this.sceneNode.ViewModel.IsEditable && this.sceneNode.ViewObject is IViewVisual)
          return (ViewItem) new SceneNodeViewItem(this.sceneNode);
        return (ViewItem) null;
      }
    }

    public override ModelProperty Content
    {
      get
      {
        if (this.sceneNode.DefaultContentProperty != null)
          return (ModelProperty) new SceneNodeModelProperty(this, this.sceneNode.DefaultContentProperty);
        return (ModelProperty) null;
      }
    }

    public override EditingContext Context
    {
      get
      {
        return this.sceneNode.ViewModel.ExtensibilityManager.EditingContext;
      }
    }

    public override ModelEventCollection Events
    {
      get
      {
        return (ModelEventCollection) null;
      }
    }

    public override Type ItemType
    {
      get
      {
        return this.sceneNode.TargetType;
      }
    }

    public override string Name
    {
      get
      {
        return this.sceneNode.Name;
      }
      set
      {
        using (SceneEditTransaction editTransaction = this.CreateEditTransaction("Name"))
        {
          this.sceneNode.Name = value;
          editTransaction.Commit();
        }
      }
    }

    public override ModelItem Parent
    {
      get
      {
        if (this.sceneNode.Parent == null)
          return (ModelItem) null;
        return (ModelItem) this.sceneNode.Parent.ModelItem;
      }
    }

    public override ModelPropertyCollection Properties
    {
      get
      {
        return (ModelPropertyCollection) new SceneNodeModelPropertyCollection(this.sceneNode);
      }
    }

    public override ModelProperty Source
    {
      get
      {
        DocumentNode documentNode = this.sceneNode.DocumentNode;
        if (documentNode.IsProperty)
          return (ModelProperty) new SceneNodeModelProperty(this.sceneNode.ViewModel.GetSceneNode((DocumentNode) documentNode.Parent).ModelItem, documentNode.SitePropertyKey);
        return (ModelProperty) null;
      }
    }

    public override event PropertyChangedEventHandler PropertyChanged;

    public SceneNodeModelItem(SceneNode sceneNode)
    {
      this.sceneNode = sceneNode;
    }

    public override IEnumerable<object> GetAttributes(Type attributeType)
    {
      foreach (Attribute attribute in TypeUtilities.GetAttributes(this.sceneNode.TargetType))
      {
        if (attributeType.IsAssignableFrom(attribute.GetType()))
          yield return (object) attribute;
      }
    }

    public override ModelEditingScope BeginEdit()
    {
      return this.BeginEdit(StringTable.ExtensibilityEditTransactionDescription);
    }

    public override ModelEditingScope BeginEdit(string description)
    {
      return this.sceneNode.ViewModel.ExtensibilityManager.CreateEditingScope(description);
    }

    public override object GetCurrentValue()
    {
      if (this.sceneNode.ViewObject == null)
        return (object) null;
      return this.sceneNode.ViewObject.PlatformSpecificObject;
    }

    public void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private SceneEditTransaction CreateEditTransaction(string propertyName)
    {
      return this.sceneNode.ViewModel.CreateEditTransaction(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PropertyChangeUndoDescription, new object[1]
      {
        (object) propertyName
      }));
    }
  }
}
