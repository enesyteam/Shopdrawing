// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ElementBindingSourceNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ElementBindingSourceNode : ElementNode
  {
    private SchemaItem schemaItem;
    private ISchema schema;

    public SchemaItem SchemaItem
    {
      get
      {
        return this.schemaItem;
      }
    }

    public ISchema Schema
    {
      get
      {
        return this.schema;
      }
    }

    public string Path
    {
      get
      {
        return this.schemaItem.SelectionPath;
      }
    }

    public ElementBindingSourceNode(SceneElement element, SelectionContext<ElementNode> selectionContext)
      : base(element, selectionContext)
    {
      this.IsSelectable = true;
      this.schema = SchemaManager.GetSchemaForType(element.TrueTargetType, element.DocumentNode);
      this.schemaItem = new SchemaItem(this.schema);
      this.schemaItem.PropertyChanged += new PropertyChangedEventHandler(this.schemaItem_PropertyChanged);
      this.schemaItem.Root.IsExpanded = true;
    }

    private void schemaItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "SelectionPath"))
        return;
      this.OnPropertyChanged("Path");
    }
  }
}
