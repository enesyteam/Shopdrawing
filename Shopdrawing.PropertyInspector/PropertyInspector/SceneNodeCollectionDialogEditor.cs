// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeCollectionDialogEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.PropertyInspector;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public sealed class SceneNodeCollectionDialogEditor : Grid
  {
    private SceneNodeCollectionDialogEditorModel model;
    private CollectionDialogEditor collectionDialogEditor;

    public SceneNodeCollectionDialogEditor()
    {
      this.Resources.MergedDictionaries.Add(FileTable.GetResourceDictionary("Resources/PropertyInspector/CollectionEditorResources.xaml"));
      this.model = new SceneNodeCollectionDialogEditorModel();
      this.collectionDialogEditor = new CollectionDialogEditor((CollectionDialogEditorModel) this.model);
      this.collectionDialogEditor.Style = (Style) this.Resources[(object) "BlendCollectionEditorStyle"];
      this.collectionDialogEditor.HorizontalAlignment = HorizontalAlignment.Stretch;
      this.collectionDialogEditor.VerticalAlignment = VerticalAlignment.Stretch;
      this.collectionDialogEditor.SetValue(SceneNodePropertyInspectorPane.InPropertyInspectorProperty, (object) true);
      this.collectionDialogEditor.SetBinding(CollectionDialogEditor.PropertyValueProperty, (BindingBase) new Binding());
      this.Children.Add((UIElement) this.collectionDialogEditor);
      this.Unloaded += new RoutedEventHandler(this.OnUnloaded);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      this.model.Unhook();
      this.model = (SceneNodeCollectionDialogEditorModel) null;
    }
  }
}
