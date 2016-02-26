// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.MakePartCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.UserInterface.SkinEditing;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class MakePartCommand : MakeControlCommand, ICommand
  {
    private PartInPartsExplorer part;

    protected override IType DefaultContainerType
    {
      get
      {
        return this.DesignerContext.ActiveSceneViewModel.ProjectContext.ResolveType(MakePartCommand.GetControlTypeToInstantiate((ITypeResolver) this.DesignerContext.ActiveSceneViewModel.ProjectContext, this.part.TargetType));
      }
    }

    protected override string UndoString
    {
      get
      {
        return StringTable.UndoUnitMakePart;
      }
    }

    public event EventHandler CanExecuteChanged
    {
      add
      {
      }
      remove
      {
      }
    }

    public MakePartCommand(SceneViewModel viewModel, PartInPartsExplorer part)
      : base(viewModel)
    {
      this.part = part;
    }

    protected override CreateResourceDialog CreateDialog(CreateResourceModel createResourceModel)
    {
      CreateResourceDialog createResourceDialog = new CreateResourceDialog(this.DesignerContext, createResourceModel);
      createResourceDialog.Title = StringTable.MakePartDialogTitle;
      return createResourceDialog;
    }

    bool ICommand.CanExecute(object parameter)
    {
      return true;
    }

    public override void PostProcessing(SceneNode oldElement, SceneNode newElement, DocumentCompositeNode styleNode)
    {
      base.PostProcessing(oldElement, newElement, styleNode);
      DocumentNode node1 = this.SceneViewModel.ActiveEditingContainer.DocumentNode.NameScope.FindNode(this.part.Name);
      if (node1 != null)
      {
        SceneNode node2 = node1.SceneNode as SceneNode;
        if (node2 != null)
        {
          this.RemovePartStatus(node2);
          this.SceneViewModel.TimelineItemManager.UpdateItems();
        }
      }
      newElement.Name = this.part.Name;
    }

    void ICommand.Execute(object parameter)
    {
      this.Execute();
    }

    public static ITypeId GetControlTypeToInstantiate(ITypeResolver typeResolver, ITypeId desiredType)
    {
      if (typeResolver == null || desiredType == null || typeResolver.PlatformMetadata.IsNullType(desiredType))
        return (ITypeId) null;
      IType type = typeResolver.ResolveType(desiredType);
      KeyValuePair<ITypeId, ITypeId> keyValuePair = Enumerable.FirstOrDefault<KeyValuePair<ITypeId, ITypeId>>((IEnumerable<KeyValuePair<ITypeId, ITypeId>>) PartsModel.InstanceTypesForAbstractParts, (Func<KeyValuePair<ITypeId, ITypeId>, bool>) (pair => pair.Key.Equals((object) type)));
      if (keyValuePair.Key != null)
        type = typeResolver.ResolveType(keyValuePair.Value);
      if (PlatformTypes.Control.IsAssignableFrom((ITypeId) type) && !PlatformTypes.UserControl.IsAssignableFrom((ITypeId) type) && type.HasDefaultConstructor(false))
        return (ITypeId) type;
      return (ITypeId) null;
    }

    private void RemovePartStatus(SceneNode node)
    {
      if (node == null || node.ViewModel == null || (node.ViewModel.DesignerContext == null || node.ViewModel.TimelineItemManager == null))
        return;
      ElementTimelineItem elementTimelineItem = node.ViewModel.TimelineItemManager.FindTimelineItem(node) as ElementTimelineItem;
      if (elementTimelineItem == null)
        return;
      node.Name = (string) null;
      elementTimelineItem.PartStatus = PartStatus.Unused;
      elementTimelineItem.Invalidate();
    }
  }
}
