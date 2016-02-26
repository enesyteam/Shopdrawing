// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.GroupIntoLayoutTypeFlyoutCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Globalization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class GroupIntoLayoutTypeFlyoutCommand : SingleTargetDynamicMenuCommandBase
  {
    public static ITypeId[] LayoutTypes = ChangeLayoutTypeFlyoutCommand.LayoutTypes;

    private ITypeId PreferredShortcutType
    {
      get
      {
        foreach (ITypeId type in GroupIntoLayoutTypeFlyoutCommand.LayoutTypes)
        {
          if (((IPlatformTypes) this.DesignerContext.ActiveSceneViewModel.ProjectContext.PlatformMetadata).IsLooselySupported((ITypeResolver) this.DesignerContext.ActiveSceneViewModel.ProjectContext, type))
            return type;
        }
        return (ITypeId) null;
      }
    }

    public override IEnumerable Items
    {
      get
      {
        List<MenuItem> list = new List<MenuItem>();
        IPlatformTypes platformTypes = (IPlatformTypes) this.DesignerContext.ActiveSceneViewModel.ProjectContext.PlatformMetadata;
        foreach (ITypeId type in GroupIntoLayoutTypeFlyoutCommand.LayoutTypes)
        {
          if (platformTypes.IsLooselySupported((ITypeResolver) this.DesignerContext.ActiveSceneViewModel.ProjectContext, type))
          {
            MenuItem menuItem = this.CreateMenuItem(type.Name, "Object_GroupInto_" + type.Name, (ICommand) new GroupIntoLayoutTypeCommand(this.ViewModel, type, false));
            if (type.Equals((object) this.PreferredShortcutType))
            {
              KeyBinding[] keyBindingArray = new KeyBinding[1]
              {
                new KeyBinding()
                {
                  Modifiers = ModifierKeys.Control,
                  Key = Key.G
                }
              };
              menuItem.InputGestureText = CultureManager.GetShortcutText(keyBindingArray);
            }
            list.Add(menuItem);
          }
        }
        if (this.ViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.DragDropInToolkit) && this.HasValidTarget)
        {
          GroupCommandHelpers.DragDropLayoutAssocation layoutAssocation = Enumerable.FirstOrDefault<GroupCommandHelpers.DragDropLayoutAssocation>((IEnumerable<GroupCommandHelpers.DragDropLayoutAssocation>) GroupCommandHelpers.DragDropLayoutTypes, (Func<GroupCommandHelpers.DragDropLayoutAssocation, bool>) (item => item.Child.IsAssignableFrom((ITypeId) this.TargetElement.Type)));
          if (layoutAssocation != null)
          {
            ITypeId container = layoutAssocation.Container;
            if (!platformTypes.IsSupported((ITypeResolver) this.DesignerContext.ActiveSceneViewModel.ProjectContext, container) || !container.IsAssignableFrom((ITypeId) this.TargetElement.Parent.Type))
              list.Add(this.CreateMenuItem(StringTable.DragDropGroupIntoCommandName, "Object_GroupInto_DragDrop", (ICommand) new GroupIntoLayoutTypeCommand(this.ViewModel, container, true)));
          }
        }
        return (IEnumerable) list;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled || !GroupCommandHelpers.IsGrouping2D(this.ViewModel))
          return false;
        ReadOnlyCollection<SceneElement> selection = this.DesignerContext.SelectionManager.ElementSelectionSet.Selection;
        bool flag = false;
        SceneNode sceneNode = (SceneNode) null;
        SceneElement sceneElement1 = this.ViewModel.RootNode as SceneElement;
        if (sceneElement1 != null && selection.Contains(sceneElement1) || selection.Count == 0)
          return false;
        foreach (SceneElement sceneElement2 in selection)
        {
          if (!(sceneElement2 is BaseFrameworkElement) || !sceneElement2.IsViewObjectValid)
            return false;
          if (!flag)
          {
            sceneNode = sceneElement2.Parent;
            flag = true;
          }
          else if (sceneElement2.Parent != sceneNode)
            return false;
        }
        return true;
      }
    }

    public GroupIntoLayoutTypeFlyoutCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      new GroupIntoLayoutTypeCommand(this.ViewModel, this.PreferredShortcutType, false).Execute((object) null);
    }

    public override object GetProperty(string propertyName)
    {
      if (propertyName == "IsVisible")
        return (object) (bool) (GroupCommandHelpers.IsGrouping2D(this.ViewModel) ? true : false);
      return base.GetProperty(propertyName);
    }
  }
}
