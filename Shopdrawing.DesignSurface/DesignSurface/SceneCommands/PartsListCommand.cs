// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.PartsListCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface.SkinEditing;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class PartsListCommand : SingleTargetDynamicMenuCommandBase
  {
    public override bool IsAvailable
    {
      get
      {
        if (base.IsAvailable)
          return this.DesignerContext.ActiveSceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsTemplateParts);
        return false;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled && this.IsAvailable && (this.ViewModel.PartsModel.ShowPartsList && this.ViewModel.ElementSelectionSet.Count == 1))
        {
          switch (this.ViewModel.PartsModel.GetPartStatus((SceneNode) this.ViewModel.ElementSelectionSet.PrimarySelection))
          {
            case PartStatus.Assigned:
              return false;
            case PartStatus.WrongType:
              return true;
            case PartStatus.Unused:
              return true;
          }
        }
        return false;
      }
    }

    public override IEnumerable Items
    {
      get
      {
        List<MenuItem> list = new List<MenuItem>();
        foreach (PartInPartsExplorer part in (Collection<PartInPartsExplorer>) this.ViewModel.PartsModel.PartsList)
        {
          string str = part.Name.Replace("_", "__");
          MenuItem menuItem;
          if (part.TargetType.IsAssignableFrom((ITypeId) this.TargetElement.Type))
            menuItem = this.CreateMenuItem(str, str, (ICommand) new PartsListCommand.AssignToPartCommand(this, part));
          else if (MakePartCommand.GetControlTypeToInstantiate((ITypeResolver) this.ViewModel.ProjectContext, part.TargetType) != null)
          {
            menuItem = this.CreateMenuItem(str + "...", str, (ICommand) new MakePartCommand(this.ViewModel, part));
          }
          else
          {
            menuItem = this.CreateMenuItem(str, str, (ICommand) null);
            menuItem.IsEnabled = false;
          }
          menuItem.SetResourceReference(FrameworkElement.StyleProperty, (object) "MenuItemWithIcon");
          Image image = new Image();
          BitmapImage bitmapImage = new BitmapImage();
          bitmapImage.BeginInit();
          switch (part.Status)
          {
            case PartStatus.Assigned:
              bitmapImage.UriSource = new Uri("pack://application:,,,/Microsoft.Expression.DesignSurface;Component/UserInterface/SkinEditing/parts_assigned-part_on_16X16.png", UriKind.RelativeOrAbsolute);
              break;
            case PartStatus.WrongType:
              bitmapImage.UriSource = new Uri("pack://application:,,,/Microsoft.Expression.DesignSurface;Component/UserInterface/SkinEditing/parts_misassigned-part_on_16X16.png", UriKind.RelativeOrAbsolute);
              break;
            case PartStatus.Unused:
              bitmapImage.UriSource = new Uri("pack://application:,,,/Microsoft.Expression.DesignSurface;Component/UserInterface/SkinEditing/parts_part_on_16X16.png", UriKind.RelativeOrAbsolute);
              break;
          }
          bitmapImage.EndInit();
          image.Source = (ImageSource) bitmapImage;
          menuItem.Icon = (object) image;
          list.Add(menuItem);
        }
        return (IEnumerable) list;
      }
    }

    public PartsListCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override object GetProperty(string propertyName)
    {
      if (!(propertyName == "Text"))
        return base.GetProperty(propertyName);
      if (!this.IsEnabled)
        return (object) StringTable.MakeIntoPartMenuDisabled;
      return (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.MakeIntoPartMenuEnabled, new object[1]
      {
        (object) ((FrameworkTemplateElement) this.ViewModel.ActiveEditingContainer).TargetElementType.Name
      });
    }

    public MenuItem CreateSingleInstance()
    {
      string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.MakeIntoPartMenuEnabled, new object[1]
      {
        (object) (this.ViewModel.PartsModel.EditingContainer as ControlTemplateElement).TargetType.Name
      });
      MenuItem menuItem1 = this.CreateMenuItem(str, str, (ICommand) null);
      foreach (MenuItem menuItem2 in this.Items)
        menuItem1.Items.Add((object) menuItem2);
      return menuItem1;
    }

    private void AssignToPart(PartInPartsExplorer part)
    {
      DocumentNode first = this.ViewModel.ActiveEditingContainer.DocumentNode.FindFirst((Predicate<DocumentNode>) (node =>
      {
        if (node.Name != null)
          return node.Name.Equals(part.Name);
        return false;
      }));
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(StringTable.UndoMakeIntoPart))
      {
        if (first != null)
        {
          ((SceneNode) first.SceneNode).Name = (string) null;
          this.TargetElement.Name = part.Name;
          ((SceneNode) first.SceneNode).Name = part.Name;
        }
        else
          this.TargetElement.Name = part.Name;
        editTransaction.Commit();
      }
    }

    private class AssignToPartCommand : ICommand
    {
      private PartsListCommand parentCommand;
      private PartInPartsExplorer part;

      public event EventHandler CanExecuteChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public AssignToPartCommand(PartsListCommand parentCommand, PartInPartsExplorer part)
      {
        this.parentCommand = parentCommand;
        this.part = part;
      }

      public void Execute(object arg)
      {
        this.parentCommand.AssignToPart(this.part);
      }

      public bool CanExecute(object arg)
      {
        return true;
      }
    }
  }
}
