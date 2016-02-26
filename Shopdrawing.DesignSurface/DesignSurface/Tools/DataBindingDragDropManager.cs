// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DataBindingDragDropManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Timeline.DragDrop;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal static class DataBindingDragDropManager
  {
    private static DataBindingDragDropHandler[] handlers = new DataBindingDragDropHandler[2]
    {
      (DataBindingDragDropHandler) new DataBindingDragDropAddTriggerHandler(),
      (DataBindingDragDropHandler) new DataBindingDragDropDefaultHandler()
    };
    private static bool? isAltModifier;
    private static DataBindingDragDropModel effectiveDragModel;
    private static DataBindingDragDropHandler targetHandler;

    public static void Reset()
    {
      DataBindingDragDropManager.isAltModifier = new bool?();
      DataBindingDragDropManager.effectiveDragModel = (DataBindingDragDropModel) null;
      DataBindingDragDropManager.targetHandler = (DataBindingDragDropHandler) null;
    }

    public static IDisposable GetDragDropToken()
    {
      return (IDisposable) new DataBindingDragDropManager.DragDropToken();
    }

    public static DataBindingDragDropModel GetDragFeedback(DataSchemaNodePathCollection information, BindingSceneInsertionPoint insertionPoint, DataBindingDragDropFlags dragFlags, ModifierKeys modifiers)
    {
      bool flag = (modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
      if (!DataBindingDragDropManager.isAltModifier.HasValue)
      {
        if (flag)
          DataBindingModeModel.Instance.SetMode(DataBindingMode.Details, false);
      }
      else if (DataBindingDragDropManager.isAltModifier.Value != flag)
        DataBindingModeModel.Instance.SetMode(flag ? DataBindingMode.Details : DataBindingMode.Default, false);
      DataBindingDragDropManager.isAltModifier = new bool?(flag);
      if ((modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
      {
        dragFlags &= ~DataBindingDragDropFlags.AutoPickProperty;
        dragFlags &= ~DataBindingDragDropFlags.AllowRetargetElement;
      }
      if (insertionPoint.Property != null)
        dragFlags &= ~DataBindingDragDropFlags.CreateElement;
      if (!BindingPropertyHelper.HasBindableProperties(insertionPoint.SceneNode))
        dragFlags &= ~(DataBindingDragDropFlags.SetBinding | DataBindingDragDropFlags.AutoPickProperty);
      DataBindingDragDropModel bindingDragDropModel = new DataBindingDragDropModel(information, insertionPoint, dragFlags, modifiers);
      if (!DataBindingDragDropModel.Equals(bindingDragDropModel, DataBindingDragDropManager.effectiveDragModel))
      {
        PerformanceUtility.StartPerformanceSequence(PerformanceEvent.DataBindingDragModelUpdate);
        DataBindingDragDropManager.effectiveDragModel = DataBindingDragDropManager.UpdateDragModel(bindingDragDropModel);
        PerformanceUtility.EndPerformanceSequence(PerformanceEvent.DataBindingDragModelUpdate);
      }
      return DataBindingDragDropManager.effectiveDragModel;
    }

    public static bool Drop(DataBindingDragDropModel dragModel, Point artboardSnappedDropPoint)
    {
      if (DataBindingDragDropManager.targetHandler == null)
        return false;
      bool flag = false;
      try
      {
        using (TemporaryCursor.SetWaitCursor())
        {
          using (dragModel.ViewModel.AnimationEditor.DeferKeyFraming())
          {
            using (DataBindingDragDropManager.targetHandler.InitDragModel(dragModel))
              flag = DataBindingDragDropManager.targetHandler.Handle(artboardSnappedDropPoint);
          }
        }
      }
      catch (Exception ex)
      {
      }
      return flag;
    }

    private static DataBindingDragDropModel UpdateDragModel(DataBindingDragDropModel dragModel)
    {
      if (!JoltHelper.DatabindingSupported(dragModel.TargetNode.ProjectContext))
      {
        dragModel.DropFlags = DataBindingDragDropFlags.None;
        return dragModel;
      }
      DataBindingDragDropHandler bindingDragDropHandler = (DataBindingDragDropHandler) null;
      for (int index = 0; index < DataBindingDragDropManager.handlers.Length; ++index)
      {
        bindingDragDropHandler = DataBindingDragDropManager.handlers[index];
        using (bindingDragDropHandler.InitDragModel(dragModel))
        {
          if (bindingDragDropHandler.CanHandle())
          {
            dragModel = bindingDragDropHandler.DragModel;
            break;
          }
          bindingDragDropHandler = (DataBindingDragDropHandler) null;
        }
      }
      DataBindingDragDropManager.targetHandler = bindingDragDropHandler;
      if (bindingDragDropHandler == null)
        dragModel.DropFlags = DataBindingDragDropFlags.None;
      else
        dragModel.DropFlags &= dragModel.DragFlags;
      return dragModel;
    }

    private class DragDropToken : IDisposable
    {
      private DataBindingMode originalMode;

      public DragDropToken()
      {
        this.originalMode = DataBindingModeModel.Instance.Mode;
        DataBindingDragDropManager.Reset();
      }

      public void Dispose()
      {
        DataBindingDragDropManager.Reset();
        DataBindingModeModel.Instance.SetMode(this.originalMode, true);
        GC.SuppressFinalize((object) this);
      }
    }
  }
}
