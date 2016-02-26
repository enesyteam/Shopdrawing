// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.PropertyCacheHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Project;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.View
{
  public static class PropertyCacheHelper
  {
    private static bool InitializedWpf;
    private static bool InitializedSl;

    public static bool Synchronous { get; set; }

    public static void PopulateStartupReflectionCache()
    {
      object[] objArray = new object[27]
      {
        (object) new Rectangle(),
        (object) new Ellipse(),
        (object) new Path(),
        (object) new Grid(),
        (object) new Canvas(),
        (object) new StackPanel(),
        (object) new WrapPanel(),
        (object) new DockPanel(),
        (object) new ScrollViewer(),
        (object) new Border(),
        (object) new Viewbox(),
        (object) new TextBox(),
        (object) new TextBlock(),
        (object) new RichTextBox(),
        (object) new PasswordBox(),
        (object) new Label(),
        (object) new FlowDocumentScrollViewer(),
        (object) new Button(),
        (object) new CheckBox(),
        (object) new ComboBox(),
        (object) new ListBox(),
        (object) new RadioButton(),
        (object) new Slider(),
        (object) new TabControl(),
        (object) new GridSplitter(),
        (object) new ScrollBar(),
        (object) new UniformGrid()
      };
      foreach (object obj in objArray)
      {
        object instance = obj;
        if (PropertyCacheHelper.Synchronous)
          TypeDescriptor.GetProperties(instance);
        else
          UIThreadDispatcher.Instance.BeginInvoke<PropertyDescriptorCollection>(DispatcherPriority.ApplicationIdle, (Func<PropertyDescriptorCollection>) (() => TypeDescriptor.GetProperties(instance)));
      }
    }

    public static void PopulateDocumentSpecificPropertyCache(SceneViewModel viewModel)
    {
      PropertyCacheHelper.DispatchArguments arguments = new PropertyCacheHelper.DispatchArguments();
      arguments.ViewModel = viewModel;
      if (viewModel.Document == null || DocumentContextHelper.GetDesignDataMode((IProject) viewModel.ProjectContext.GetService(typeof (IProject)), viewModel.Document.Path) != DesignDataMode.None)
        return;
      if (!viewModel.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
      {
        if (PropertyCacheHelper.InitializedSl)
          return;
        PropertyCacheHelper.InitializedSl = true;
        arguments.TypeList = new ITypeId[17]
        {
          PlatformTypes.Rectangle,
          PlatformTypes.Ellipse,
          PlatformTypes.Path,
          PlatformTypes.Grid,
          PlatformTypes.Canvas,
          PlatformTypes.StackPanel,
          PlatformTypes.ScrollViewer,
          PlatformTypes.Border,
          PlatformTypes.TextBox,
          PlatformTypes.TextBlock,
          PlatformTypes.PasswordBox,
          PlatformTypes.Button,
          PlatformTypes.CheckBox,
          PlatformTypes.ListBox,
          PlatformTypes.RadioButton,
          PlatformTypes.Slider,
          PlatformTypes.ScrollBar
        };
      }
      else
      {
        if (PropertyCacheHelper.InitializedWpf)
          return;
        PropertyCacheHelper.InitializedWpf = true;
        arguments.TypeList = new ITypeId[27]
        {
          PlatformTypes.Rectangle,
          PlatformTypes.Ellipse,
          PlatformTypes.Path,
          PlatformTypes.Grid,
          PlatformTypes.Canvas,
          PlatformTypes.StackPanel,
          ProjectNeutralTypes.WrapPanel,
          ProjectNeutralTypes.DockPanel,
          PlatformTypes.ScrollViewer,
          PlatformTypes.Border,
          ProjectNeutralTypes.Viewbox,
          PlatformTypes.TextBox,
          PlatformTypes.TextBlock,
          PlatformTypes.RichTextBox,
          PlatformTypes.PasswordBox,
          ProjectNeutralTypes.Label,
          PlatformTypes.FlowDocumentScrollViewer,
          PlatformTypes.Button,
          PlatformTypes.CheckBox,
          PlatformTypes.ComboBox,
          PlatformTypes.ListBox,
          PlatformTypes.RadioButton,
          PlatformTypes.Slider,
          ProjectNeutralTypes.TabControl,
          ProjectNeutralTypes.GridSplitter,
          PlatformTypes.ScrollBar,
          PlatformTypes.UniformGrid
        };
      }
      if (PropertyCacheHelper.Synchronous)
        PropertyCacheHelper.WarmUpPropertyCacheByITypeId((object) arguments);
      else
        UIThreadDispatcher.Instance.BeginInvoke<object>(DispatcherPriority.ApplicationIdle, (Func<object>) (() => PropertyCacheHelper.WarmUpPropertyCacheByITypeId((object) arguments)));
    }

    private static object WarmUpPropertyCacheByITypeId(object arg)
    {
      PropertyCacheHelper.DispatchArguments arguments = (PropertyCacheHelper.DispatchArguments) arg;
      if (arguments.ViewModel.Document != null)
      {
        ITypeResolver typeResolver = (ITypeResolver) arguments.ViewModel.ProjectContext;
        IType type = typeResolver.ResolveType(arguments.TypeList[arguments.Index]);
        if (!typeResolver.PlatformMetadata.IsNullType((ITypeId) type))
          arguments.ViewModel.CreateSceneNode((ITypeId) type);
        ++arguments.Index;
        if (arguments.Index < arguments.TypeList.Length)
        {
          if (PropertyCacheHelper.Synchronous)
            PropertyCacheHelper.WarmUpPropertyCacheByITypeId((object) arguments);
          else
            UIThreadDispatcher.Instance.BeginInvoke<object>(DispatcherPriority.ApplicationIdle, (Func<object>) (() => PropertyCacheHelper.WarmUpPropertyCacheByITypeId((object) arguments)));
        }
      }
      return (object) null;
    }

    private class DispatchArguments
    {
      public ITypeId[] TypeList;
      public SceneViewModel ViewModel;
      public int Index;
    }
  }
}
