// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.SetCurrentSelectionCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class SetCurrentSelectionCommand : DynamicMenuCommand
  {
    private Point mousePoint = new Point();
    private SceneView view;

    public override bool IsEnabled
    {
      get
      {
        this.mousePoint = Mouse.GetPosition((IInputElement) this.view.Artboard.ContentArea);
        return true;
      }
    }

    public override IEnumerable Items
    {
      get
      {
        IList<SceneElement> elementsInRectangle = this.view.GetElementsInRectangle(new Rect(this.mousePoint.X - 0.5, this.mousePoint.Y - 0.5, 1.0, 1.0), new HitTestModifier(this.GetSelectableElement), (InvisibleObjectHitTestModifier) null, false);
        ArrayList arrayList = new ArrayList();
        foreach (SceneElement sceneElement in (IEnumerable<SceneElement>) elementsInRectangle)
        {
          MenuItem menuItem = new MenuItem();
          string displayName = sceneElement.DisplayName;
          menuItem.Header = (object) displayName;
          menuItem.SetValue(AutomationElement.IdProperty, (object) displayName);
          menuItem.IsChecked = this.view.ViewModel.ElementSelectionSet.Selection.Contains(sceneElement);
          menuItem.Command = (System.Windows.Input.ICommand) new SetCurrentSelectionCommand.SelectSpecificSceneElementCommand(this.view.ViewModel, sceneElement);
          arrayList.Add((object) menuItem);
        }
        return (IEnumerable) arrayList;
      }
    }

    public SetCurrentSelectionCommand(SceneView view)
    {
      this.view = view;
    }

    public override void Execute()
    {
      throw new InvalidOperationException();
    }

    private SceneElement GetSelectableElement(DocumentNodePath nodePath)
    {
      IStoryboardContainer storyboardContainer = this.view.ViewModel.ActiveStoryboardContainer;
      DocumentNodePath targetElementPath = storyboardContainer.TargetElement != null ? storyboardContainer.TargetElement.DocumentNodePath : ((SceneNode) storyboardContainer).DocumentNodePath;
      DocumentNodePath editingContainer = this.view.ViewModel.GetAncestorInEditingContainer(nodePath, this.view.ViewModel.ActiveEditingContainerPath, targetElementPath);
      SceneElement sceneElement = (SceneElement) null;
      if (editingContainer != null)
      {
        sceneElement = this.view.ViewModel.GetUnlockedAncestor(editingContainer);
        if (!sceneElement.IsSelectable)
          sceneElement = (SceneElement) null;
      }
      return sceneElement;
    }

    private class SelectSpecificSceneElementCommand : System.Windows.Input.ICommand
    {
      private SceneViewModel viewModel;
      private SceneElement sceneElement;

      public event EventHandler CanExecuteChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public SelectSpecificSceneElementCommand(SceneViewModel viewModel, SceneElement sceneElement)
      {
        this.viewModel = viewModel;
        this.sceneElement = sceneElement;
      }

      public bool CanExecute(object arg)
      {
        return true;
      }

      public void Execute(object arg)
      {
        this.viewModel.ElementSelectionSet.SetSelection(this.sceneElement);
      }
    }
  }
}
