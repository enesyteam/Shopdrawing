// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DataBindingDragDropModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Timeline.DragDrop;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class DataBindingDragDropModel
  {
    private IProperty targetProperty;
    private DataSchemaNodePath targetPropertySpecialDataContext;
    private ITypeId newElementType;
    private IPropertyId newElementProperty;
    private DataSchemaNodePath relativeDropSchemaPath;
    private DataBindingDragDropFlags dropFlags;
    private IList<DataSchemaNode> nodesToCreateElements;
    private string tooltip;

    public DataSchemaNodePathCollection DataSource { get; private set; }

    public BindingSceneInsertionPoint InsertionPoint { get; private set; }

    public DataBindingDragDropFlags DragFlags { get; private set; }

    private ModifierKeys Modifiers { get; set; }

    public IProperty TargetProperty
    {
      get
      {
        return this.targetProperty;
      }
      set
      {
        this.targetProperty = value;
      }
    }

    public DataSchemaNodePath TargetPropertySpecialDataContext
    {
      get
      {
        return this.targetPropertySpecialDataContext;
      }
      set
      {
        this.targetPropertySpecialDataContext = value;
      }
    }

    public ITypeId NewElementType
    {
      get
      {
        return this.newElementType;
      }
      set
      {
        this.newElementType = value;
      }
    }

    public IPropertyId NewElementProperty
    {
      get
      {
        return this.newElementProperty;
      }
      set
      {
        this.newElementProperty = value;
      }
    }

    public DataSchemaNodePath RelativeDropSchemaPath
    {
      get
      {
        return this.relativeDropSchemaPath;
      }
      set
      {
        this.relativeDropSchemaPath = value;
      }
    }

    public DataBindingDragDropFlags DropFlags
    {
      get
      {
        return this.dropFlags;
      }
      set
      {
        this.dropFlags = value;
      }
    }

    public IList<DataSchemaNode> NodesToCreateElements
    {
      get
      {
        return this.nodesToCreateElements;
      }
      set
      {
        this.nodesToCreateElements = value;
      }
    }

    public string Tooltip
    {
      get
      {
        return this.tooltip;
      }
      set
      {
        this.tooltip = value;
      }
    }

    public SceneNode TargetNode
    {
      get
      {
        return this.InsertionPoint.SceneNode;
      }
    }

    public SceneViewModel ViewModel
    {
      get
      {
        if (this.TargetNode != null)
          return this.TargetNode.ViewModel;
        return (SceneViewModel) null;
      }
    }

    public SceneDocument Document
    {
      get
      {
        if (this.TargetNode != null)
          return this.TargetNode.ViewModel.Document;
        return (SceneDocument) null;
      }
    }

    public string DataSourceName
    {
      get
      {
        if (this.DataSource != null)
          return this.DataSource.PrimaryDataSourceName;
        return (string) null;
      }
    }

    public IPlatform Platform
    {
      get
      {
        return this.TargetNode.Platform;
      }
    }

    public IDocumentContext DocumentContext
    {
      get
      {
        return this.TargetNode.DocumentContext;
      }
    }

    public DataSchemaNodePath DetailsContainerSchemaPath
    {
      get
      {
        DataSchemaNode collectionItemNode = this.DataSource.PrimarySchemaNodePath.EffectiveCollectionItemNode;
        if (collectionItemNode == null)
          return new DataSchemaNodePath(this.DataSource.PrimarySchema, this.DataSource.PrimarySchema.Root);
        return new DataSchemaNodePath(this.DataSource.PrimaryAbsoluteSchema, collectionItemNode);
      }
    }

    public DataBindingDragDropModel AncestorPanelModel
    {
      get
      {
        SceneNode editingContainer = this.TargetNode.ViewModel.ActiveEditingContainer;
        for (SceneNode sceneNode = this.TargetNode; sceneNode != editingContainer; sceneNode = sceneNode.Parent)
        {
          PanelElement panelElement = sceneNode as PanelElement;
          if (panelElement != null)
          {
            if (panelElement == this.TargetNode)
              return this;
            DataBindingDragDropModel bindingDragDropModel = new DataBindingDragDropModel(this.DataSource, new BindingSceneInsertionPoint((SceneNode) panelElement, (IProperty) null, -1), this.DragFlags, this.Modifiers);
            bindingDragDropModel.relativeDropSchemaPath = this.relativeDropSchemaPath;
            if (this.nodesToCreateElements != null)
              bindingDragDropModel.nodesToCreateElements = (IList<DataSchemaNode>) new List<DataSchemaNode>((IEnumerable<DataSchemaNode>) this.nodesToCreateElements);
            return bindingDragDropModel;
          }
        }
        return (DataBindingDragDropModel) null;
      }
    }

    public string SourceName
    {
      get
      {
        string str = this.RelativeDropSchemaPath.Path;
        if (string.IsNullOrEmpty(str))
          str = this.DataSourceName;
        return str;
      }
    }

    public string TargetNodeName
    {
      get
      {
        string str = string.Empty;
        if ((this.DropFlags & DataBindingDragDropFlags.CreateElement) == DataBindingDragDropFlags.CreateElement)
          str = "[" + this.NewElementType.Name + "]";
        else if ((this.DropFlags & DataBindingDragDropFlags.SetBinding) == DataBindingDragDropFlags.SetBinding)
        {
          if (this.TargetNode.IsNamed)
            str = this.TargetNode.Name;
          else
            str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.ElementTimelineItemBracketedName, new object[1]
            {
              (object) this.TargetNode.Type.Name
            });
        }
        return str;
      }
    }

    public string TargetPropertyName
    {
      get
      {
        string str = string.Empty;
        if ((this.DropFlags & DataBindingDragDropFlags.CreateElement) == DataBindingDragDropFlags.CreateElement)
          str = DataBindingDragDropModel.GetPropertyNameForTooltip(this.NewElementProperty);
        else if ((this.DropFlags & DataBindingDragDropFlags.SetBinding) == DataBindingDragDropFlags.SetBinding && this.TargetProperty != null)
          str += DataBindingDragDropModel.GetPropertyNameForTooltip((IPropertyId) this.TargetProperty);
        return str;
      }
    }

    public DataBindingDragDropModel(DataSchemaNodePathCollection dataSource, BindingSceneInsertionPoint insertionPoint, DataBindingDragDropFlags dragFlags, ModifierKeys modifiers)
    {
      this.DataSource = dataSource;
      this.InsertionPoint = insertionPoint;
      this.DragFlags = dragFlags;
      this.Modifiers = modifiers;
    }

    [Conditional("DEBUG")]
    public void Freeze()
    {
    }

    [Conditional("DEBUG")]
    public void VerifyNotFrozen()
    {
    }

    public void OnUserSelectedProperty(IProperty property)
    {
      this.targetProperty = property;
    }

    public bool CheckDropFlags(DataBindingDragDropFlags flags)
    {
      return this.CheckDropFlags(flags, true);
    }

    public bool CheckDropFlags(DataBindingDragDropFlags flags, bool exactMatch)
    {
      return DataBindingDragDropModel.CheckFlags(this.DropFlags, flags, exactMatch);
    }

    public bool CheckDragFlags(DataBindingDragDropFlags flags)
    {
      return this.CheckDragFlags(flags, true);
    }

    public bool CheckDragFlags(DataBindingDragDropFlags flags, bool exactMatch)
    {
      return DataBindingDragDropModel.CheckFlags(this.DragFlags, flags, exactMatch);
    }

    private static bool CheckFlags(DataBindingDragDropFlags actualFlags, DataBindingDragDropFlags testFlags, bool exactMatch)
    {
      DataBindingDragDropFlags bindingDragDropFlags = actualFlags & testFlags;
      if (bindingDragDropFlags == DataBindingDragDropFlags.None)
        return false;
      if (bindingDragDropFlags == testFlags)
        return true;
      return !exactMatch;
    }

    private static string GetPropertyNameForTooltip(IPropertyId property)
    {
      if (property.MemberType == MemberType.DesignTimeProperty)
        return "d:" + property.Name;
      return property.Name;
    }

    public static bool Equals(DataBindingDragDropModel left, DataBindingDragDropModel right)
    {
      return left == right || left != null && right != null && (left.DataSource == right.DataSource && left.DragFlags == right.DragFlags) && (left.InsertionPoint.SceneNode == right.InsertionPoint.SceneNode && left.InsertionPoint.Property == right.InsertionPoint.Property && (left.InsertionPoint.InsertIndex == right.InsertionPoint.InsertIndex && left.Modifiers == right.Modifiers));
    }
  }
}
