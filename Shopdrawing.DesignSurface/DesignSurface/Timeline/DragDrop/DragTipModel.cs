// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.DragTipModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public sealed class DragTipModel
  {
    public string SourceName { get; private set; }

    public string SourceType { get; private set; }

    public string TargetName { get; private set; }

    public DropEffectId DropEffectId { get; private set; }

    public static DragTipModel FromDescriptor(TimelineDragDescriptor descriptor)
    {
      DragTipModel dragTipModel = (DragTipModel) null;
      if (descriptor != null)
      {
        dragTipModel = new DragTipModel();
        dragTipModel.SetEffect(descriptor);
        dragTipModel.SetSource(descriptor);
      }
      return dragTipModel;
    }

    private void SetEffect(TimelineDragDescriptor descriptor)
    {
      this.DropEffectId = DropEffectId.None;
      this.TargetName = string.Empty;
      if (descriptor.IsNestingContents)
      {
        if (descriptor.IsCreating)
          this.DropEffectId = DropEffectId.CreateAsParent;
        else if (descriptor.AllowCopy)
          this.DropEffectId = DropEffectId.CopyAsParent;
        else if (descriptor.AllowMove)
          this.DropEffectId = DropEffectId.MoveAsParent;
        this.TargetName = this.GetDisplayName(descriptor.ReplacedChild);
      }
      else if (descriptor.IsReplacingChild)
      {
        this.DropEffectId = !descriptor.AllowCopy ? DropEffectId.Replace : DropEffectId.ReplaceAsCopy;
        this.TargetName = this.GetDisplayName(descriptor.ReplacedChild);
      }
      else if (descriptor.IsCreating)
      {
        this.DropEffectId = DropEffectId.Create;
        this.TargetName = this.GetDisplayName(descriptor.TargetParent);
      }
      else if (descriptor.AllowCopy)
      {
        this.DropEffectId = DropEffectId.Copy;
        this.TargetName = this.GetDisplayName(descriptor.TargetParent);
      }
      else
      {
        if (!descriptor.AllowMove)
          return;
        this.DropEffectId = DropEffectId.Move;
        this.TargetName = this.GetDisplayName(descriptor.TargetParent);
      }
    }

    private void SetSource(TimelineDragDescriptor descriptor)
    {
      DocumentNodeMarkerSortedList result1 = (DocumentNodeMarkerSortedList) null;
      if (DragSourceHelper.FirstDataOfType<DocumentNodeMarkerSortedList>(descriptor.SourceObject, ref result1))
      {
        if (result1.Count == 1 && descriptor.TargetItem != null)
        {
          if (!this.SetSource((object) descriptor.TargetItem.TimelineItemManager.ViewModel.GetSceneNode(result1.MarkerAt(0).Node)))
            ;
        }
        else
        {
          this.SourceName = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DragMultipleItemsFormat, new object[1]
          {
            (object) result1.Count
          });
          this.SourceType = string.Empty;
        }
      }
      else
      {
        Asset result2 = (Asset) null;
        if (DragSourceHelper.FirstDataOfType<Asset>(descriptor.SourceObject, ref result2) && this.SetSource((object) result2))
          return;
        DataSchemaNodePathCollection result3 = (DataSchemaNodePathCollection) null;
        if (DragSourceHelper.FirstDataOfType<DataSchemaNodePathCollection>(descriptor.SourceObject, ref result3))
          this.SetDataBinding(descriptor.UserData as DataBindingDragDropModel);
        else
          this.SourceName = this.SourceType = string.Empty;
      }
    }

    private string GetDisplayName(TimelineItem item)
    {
      if (item != null)
        return item.DisplayName;
      return string.Empty;
    }

    private string GetDisplayName(SceneNode node)
    {
      if (node != null)
        return node.DisplayName;
      return string.Empty;
    }

    private bool SetSource(object obj)
    {
      SceneNode sceneNode = obj as SceneNode;
      if (sceneNode != null)
      {
        this.SourceName = sceneNode.DisplayName;
        this.SourceType = sceneNode.TargetType.Name;
        return true;
      }
      Asset asset = obj as Asset;
      if (asset == null)
        return false;
      this.SourceName = asset.Name;
      this.SourceType = asset.TypeName;
      if (this.SourceName == this.SourceType)
        this.SourceName = string.Empty;
      return true;
    }

    private void SetDataBinding(DataBindingDragDropModel feedback)
    {
      if (feedback == null)
        return;
      this.SourceName = feedback.SourceName;
      this.TargetName = feedback.TargetNodeName;
      string targetPropertyName = feedback.TargetPropertyName;
      if (!string.IsNullOrEmpty(targetPropertyName))
      {
        DragTipModel dragTipModel = this;
        string str = dragTipModel.TargetName + "." + targetPropertyName;
        dragTipModel.TargetName = str;
      }
      if ((feedback.DropFlags & DataBindingDragDropFlags.CreateElement) == DataBindingDragDropFlags.CreateElement)
      {
        if (DataBindingModeModel.Instance.Mode == DataBindingMode.Details)
          this.DropEffectId = DropEffectId.CreateDetails;
        else
          this.DropEffectId = DropEffectId.CreateElementAndBinding;
      }
      else
      {
        if ((feedback.DropFlags & DataBindingDragDropFlags.SetBinding) != DataBindingDragDropFlags.SetBinding)
          return;
        this.DropEffectId = DropEffectId.SetBinding;
      }
    }
  }
}
