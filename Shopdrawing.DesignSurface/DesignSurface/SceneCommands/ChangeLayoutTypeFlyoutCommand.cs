// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ChangeLayoutTypeFlyoutCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class ChangeLayoutTypeFlyoutCommand : SingleTargetDynamicMenuCommandBase
  {
    public static ITypeId[] LayoutTypes = new ITypeId[9]
    {
      PlatformTypes.Grid,
      PlatformTypes.StackPanel,
      ProjectNeutralTypes.DockPanel,
      PlatformTypes.Canvas,
      PlatformTypes.ScrollViewer,
      PlatformTypes.Border,
      ProjectNeutralTypes.WrapPanel,
      PlatformTypes.UniformGrid,
      ProjectNeutralTypes.Viewbox
    };
    private List<MenuItem> items;

    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled)
          return false;
        foreach (IPropertyId childProperty in this.TargetElement.ContentProperties)
        {
          ISceneNodeCollection<SceneNode> collectionForProperty = this.TargetElement.GetCollectionForProperty(childProperty);
          if (collectionForProperty != null)
          {
            foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) collectionForProperty)
            {
              if (!PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) sceneNode.Type))
                return false;
            }
          }
        }
        return true;
      }
    }

    public override IEnumerable Items
    {
      get
      {
        return (IEnumerable) this.ItemsList;
      }
    }

    private List<MenuItem> ItemsList
    {
      get
      {
        if (this.items == null)
        {
          this.items = new List<MenuItem>();
          foreach (ITypeId type in ChangeLayoutTypeFlyoutCommand.LayoutTypes)
          {
            if (((IPlatformTypes) this.ViewModel.ProjectContext.PlatformMetadata).IsLooselySupported((ITypeResolver) this.ViewModel.ProjectContext, type))
              this.items.Add(this.CreateMenuItem(type.Name, "Change_Layout_Type_" + type.Name, (ICommand) new ChangeLayoutTypeCommand(this.ViewModel, type)));
          }
        }
        return this.items;
      }
    }

    public ChangeLayoutTypeFlyoutCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public static bool ShouldShowChangeLayoutTypeMenu(ISelectionSet<SceneElement> selection)
    {
      bool flag = false;
      foreach (SceneElement sceneElement in selection.Selection)
      {
        foreach (ITypeId typeId in ChangeLayoutTypeFlyoutCommand.LayoutTypes)
        {
          if (typeId.IsAssignableFrom((ITypeId) sceneElement.Type))
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    public override object GetProperty(string propertyName)
    {
      if (propertyName == "IsVisible" && this.ItemsList.Count < 2)
        return (object) false;
      return base.GetProperty(propertyName);
    }
  }
}
