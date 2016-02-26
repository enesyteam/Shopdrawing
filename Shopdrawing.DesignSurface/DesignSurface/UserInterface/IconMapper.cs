// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.IconMapper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public static class IconMapper
  {
    private static Dictionary<string, DrawingBrush> IconCache = new Dictionary<string, DrawingBrush>();
    private static Dictionary<string, string> TypeMap = new Dictionary<string, string>();
    private static Dictionary<Type, List<IconMapper.CachedBrush>> ExtensibleIconCache = new Dictionary<Type, List<IconMapper.CachedBrush>>();

    static IconMapper()
    {
      IconMapper.TypeMap.Add(PlatformTypes.Object.FullName, string.Empty);
      IconMapper.TypeMap.Add(ProjectNeutralTypes.BehaviorTriggerAction.FullName, "action");
      IconMapper.TypeMap.Add(PlatformTypes.AmbientLight.FullName, "ambientLight");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.Behavior.FullName, "behavior");
      IconMapper.TypeMap.Add(PlatformTypes.Border.FullName, "border");
      IconMapper.TypeMap.Add(PlatformTypes.BulletDecorator.FullName, "bulletPanel");
      IconMapper.TypeMap.Add(PlatformTypes.Button.FullName, "button");
      IconMapper.TypeMap.Add(PlatformTypes.Canvas.FullName, "canvas");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.Calendar.FullName, "calendar");
      IconMapper.TypeMap.Add(PlatformTypes.CheckBox.FullName, "checkBox");
      IconMapper.TypeMap.Add(PlatformTypes.ComboBox.FullName, "comboBox");
      IconMapper.TypeMap.Add(PlatformTypes.ComboBoxItem.FullName, "comboBoxItem");
      IconMapper.TypeMap.Add(PlatformTypes.ContentPresenter.FullName, "contentPresenter");
      IconMapper.TypeMap.Add(PlatformTypes.ContextMenu.FullName, "contextMenu");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.DataGrid.FullName, "dataGrid");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.DataGridColumn.FullName, "dataGridColumn");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.DatePicker.FullName, "datePicker");
      IconMapper.TypeMap.Add(PlatformTypes.DependencyProperty.FullName, "property");
      IconMapper.TypeMap.Add(PlatformTypes.DirectionalLight.FullName, "directionLight");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.DockPanel.FullName, "dockPanel");
      IconMapper.TypeMap.Add(PlatformTypes.DocumentViewer.FullName, "documentViewer");
      IconMapper.TypeMap.Add(PlatformTypes.Effect.FullName, "effect");
      IconMapper.TypeMap.Add(PlatformTypes.FrameworkElement.FullName, "element");
      IconMapper.TypeMap.Add(PlatformTypes.Ellipse.FullName, "ellipse");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.Expander.FullName, "expander");
      IconMapper.TypeMap.Add(PlatformTypes.Grid.FullName, "grid");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.GridSplitter.FullName, "gridSplitter");
      IconMapper.TypeMap.Add(PlatformTypes.GroupBox.FullName, "groupBox");
      IconMapper.TypeMap.Add(PlatformTypes.HyperlinkButton.FullName, "hyperlinkButton");
      IconMapper.TypeMap.Add(PlatformTypes.Image.FullName, "image");
      IconMapper.TypeMap.Add(PlatformTypes.InkCanvas.FullName, "inkCanvas");
      IconMapper.TypeMap.Add(PlatformTypes.InkPresenter.FullName, "inkPresenter");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.Label.FullName, "label");
      IconMapper.TypeMap.Add(PlatformTypes.ListBox.FullName, "listBox");
      IconMapper.TypeMap.Add(PlatformTypes.ListView.FullName, "listView");
      IconMapper.TypeMap.Add(PlatformTypes.MediaElement.FullName, "mediaElement");
      IconMapper.TypeMap.Add(PlatformTypes.Menu.FullName, "menu");
      IconMapper.TypeMap.Add(PlatformTypes.MenuItem.FullName, "menuItem");
      IconMapper.TypeMap.Add(PlatformTypes.Model3D.FullName, "model3D");
      IconMapper.TypeMap.Add(PlatformTypes.Visual3D.FullName, "model3D");
      IconMapper.TypeMap.Add(PlatformTypes.MultiScaleImage.FullName, "multiScaleImage");
      IconMapper.TypeMap.Add(PlatformTypes.OrthographicCamera.FullName, "orthographicCam");
      IconMapper.TypeMap.Add(PlatformTypes.Panel.FullName, "panel");
      IconMapper.TypeMap.Add(PlatformTypes.PasswordBox.FullName, "passwordBox");
      IconMapper.TypeMap.Add(PlatformTypes.Path.FullName, "path");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.PathListBox.FullName, "pathListBox");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.PathListBoxItem.FullName, "pathListBoxItem");
      IconMapper.TypeMap.Add(PlatformTypes.PerspectiveCamera.FullName, "perspectiveCam");
      IconMapper.TypeMap.Add(PlatformTypes.PointLight.FullName, "pointLight");
      IconMapper.TypeMap.Add(PlatformTypes.Popup.FullName, "popup");
      IconMapper.TypeMap.Add(PlatformTypes.ProgressBar.FullName, "progressBar");
      IconMapper.TypeMap.Add(PlatformTypes.RadioButton.FullName, "radioButton");
      IconMapper.TypeMap.Add(PlatformTypes.Rectangle.FullName, "rectangle");
      IconMapper.TypeMap.Add(PlatformTypes.ResizeGrip.FullName, "resizeGrip");
      IconMapper.TypeMap.Add(PlatformTypes.RichTextBox.FullName, "richtextBox");
      IconMapper.TypeMap.Add(PlatformTypes.ScrollBar.FullName, "scrollBar");
      IconMapper.TypeMap.Add(PlatformTypes.ScrollViewer.FullName, "scrollViewer");
      IconMapper.TypeMap.Add(PlatformTypes.Separator.FullName, "separator");
      IconMapper.TypeMap.Add(PlatformTypes.Slider.FullName, "slider");
      IconMapper.TypeMap.Add(PlatformTypes.SpotLight.FullName, "spotlight");
      IconMapper.TypeMap.Add(PlatformTypes.StackPanel.FullName, "stackPanel");
      IconMapper.TypeMap.Add(PlatformTypes.Storyboard.FullName, "storyboard");
      IconMapper.TypeMap.Add(PlatformTypes.Style.FullName, "style");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.TabControl.FullName, "tabControl");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.TabPanel.FullName, "tabControl");
      IconMapper.TypeMap.Add(PlatformTypes.TextBlock.FullName, "textBlock");
      IconMapper.TypeMap.Add(PlatformTypes.TextBox.FullName, "textBox");
      IconMapper.TypeMap.Add(PlatformTypes.TextBoxBase.FullName, "textBox");
      IconMapper.TypeMap.Add(PlatformTypes.Timeline.FullName, "timeline");
      IconMapper.TypeMap.Add(PlatformTypes.ToggleButton.FullName, "toggleButton");
      IconMapper.TypeMap.Add(PlatformTypes.ToolBar.FullName, "toolbar");
      IconMapper.TypeMap.Add(PlatformTypes.ToolBarTray.FullName, "toolbar");
      IconMapper.TypeMap.Add(PlatformTypes.ToolBarPanel.FullName, "toolbar");
      IconMapper.TypeMap.Add(PlatformTypes.ToolTip.FullName, "tooltip");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.TreeView.FullName, "treeView");
      IconMapper.TypeMap.Add(PlatformTypes.UniformGrid.FullName, "uniformGrid");
      IconMapper.TypeMap.Add(PlatformTypes.UserControl.FullName, "userControl");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.Viewbox.FullName, "viewbox");
      IconMapper.TypeMap.Add(PlatformTypes.WebBrowser.FullName, "webBrowser");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.WrapPanel.FullName, "wrapPanel");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.RegularPolygon.FullName, "polygon");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.Arc.FullName, "arc");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.BlockArrow.FullName, "blockArrow");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.LineArrow.FullName, "lineArrow");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.Callout.FullName, "callout");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.DataGridDragDropTarget.FullName, "viewbox");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.ListBoxDragDropTarget.FullName, "viewbox");
      IconMapper.TypeMap.Add(ProjectNeutralTypes.TreeViewDragDropTarget.FullName, "viewbox");
    }

    public static DrawingBrush GetDrawingBrushForType(ITypeId type, bool selected, int requestedWidth, int requestedHeight)
    {
      IType type1 = type as IType;
      if (type1 != null)
      {
        List<IconMapper.CachedBrush> list;
        if (!IconMapper.ExtensibleIconCache.TryGetValue(type1.RuntimeType, out list))
        {
          list = new List<IconMapper.CachedBrush>(1);
          IconMapper.ExtensibleIconCache.Add(type1.RuntimeType, list);
        }
        bool flag = false;
        foreach (IconMapper.CachedBrush cachedBrush in list)
        {
          if (cachedBrush.Width == requestedWidth && cachedBrush.Height == requestedHeight)
          {
            if (cachedBrush.Brush != null)
              return cachedBrush.Brush;
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          string resourceName;
          Stream drawingBrushStream = IconMapper.GetExtensibleDrawingBrushStream(type1.RuntimeType, requestedWidth, requestedHeight, out resourceName);
          if (drawingBrushStream != null)
          {
            DrawingBrush drawingBrushFromStream = IconMapper.CreateDrawingBrushFromStream(drawingBrushStream, resourceName);
            list.Add(new IconMapper.CachedBrush(requestedWidth, requestedHeight, drawingBrushFromStream));
            return drawingBrushFromStream;
          }
          list.Add(new IconMapper.CachedBrush(requestedWidth, requestedHeight, (DrawingBrush) null));
        }
      }
      string str1;
      if (!IconMapper.TypeMap.TryGetValue(type.FullName, out str1) && type1 != null)
      {
        Type type2 = type1.RuntimeType;
        while (type2 != (Type) null && !IconMapper.TypeMap.TryGetValue(type2.FullName, out str1))
        {
          type2 = type2.BaseType;
          if (type2 == (Type) null)
            type2 = type1.PlatformMetadata.ResolveType(PlatformTypes.Object).RuntimeType;
        }
      }
      if (string.IsNullOrEmpty(str1))
        str1 = "element";
      bool flag1 = requestedWidth == 12 && requestedHeight == 12;
      string str2 = flag1 ? "_12x12" : "_24x24";
      string str3 = selected ? "_on" : "_off";
      string str4 = "asset_" + str1 + str3;
      string str5 = str4 + str2;
      DrawingBrush drawingBrush = IconMapper.FindDrawingBrush(str5);
      if (drawingBrush == null && flag1)
      {
        drawingBrush = IconMapper.FindDrawingBrush(str4 + "_24x24");
        IconMapper.IconCache.Add(str5, drawingBrush);
      }
      return drawingBrush;
    }

    private static DrawingBrush FindDrawingBrush(string resourceName)
    {
      DrawingBrush drawingBrush;
      if (!IconMapper.IconCache.TryGetValue(resourceName, out drawingBrush))
      {
        drawingBrush = (DrawingBrush) null;
        try
        {
          if (Application.Current != null)
          {
            if (Application.Current.MainWindow != null)
            {
              drawingBrush = (DrawingBrush) Application.Current.MainWindow.TryFindResource((object) resourceName);
              if (drawingBrush != null)
                IconMapper.IconCache.Add(resourceName, drawingBrush);
            }
          }
        }
        catch
        {
        }
      }
      return drawingBrush;
    }

    public static DrawingBrush CreateDrawingBrushFromStream(Stream stream, string resourceName)
    {
      try
      {
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = stream;
        bitmapImage.EndInit();
        DrawingBrush drawingBrush = new DrawingBrush((Drawing) new ImageDrawing((ImageSource) bitmapImage, new Rect(new Size(bitmapImage.Width, bitmapImage.Height))));
        AutomationElement.SetId((DependencyObject) drawingBrush, resourceName);
        drawingBrush.Freeze();
        return drawingBrush;
      }
      catch (Exception ex)
      {
      }
      return (DrawingBrush) null;
    }

    public static Stream GetExtensibleDrawingBrushStream(Type type, int requestedWidth, int requestedHeight, out string resourceName)
    {
      try
      {
        return new NewItemFactory().GetImageStream(type, new Size((double) requestedWidth, (double) requestedHeight), out resourceName);
      }
      catch (Exception ex)
      {
        resourceName = (string) null;
        return (Stream) null;
      }
    }

    private class CachedBrush
    {
      public int Width { get; set; }

      public int Height { get; set; }

      public DrawingBrush Brush { get; set; }

      public CachedBrush(int width, int height, DrawingBrush brush)
      {
        this.Width = width;
        this.Height = height;
        this.Brush = brush;
      }
    }
  }
}
