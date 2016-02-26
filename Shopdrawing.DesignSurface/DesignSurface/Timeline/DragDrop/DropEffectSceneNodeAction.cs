// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.DropEffectSceneNodeAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public class DropEffectSceneNodeAction : DropAction<SceneNode>
  {
    public DropEffectSceneNodeAction(SceneNode sceneNode, ISceneInsertionPoint insertionPoint)
      : base(sceneNode, insertionPoint)
    {
    }

    protected override bool OnQueryCanDrop(TimelineDragDescriptor descriptor)
    {
      descriptor.DisableInBetween();
      if (!this.InsertionPoint.CanInsert((ITypeId) this.SourceData.Type))
        return false;
      if (descriptor.AllowCopy)
        descriptor.SetCopyInto(this.InsertionPoint);
      else
        descriptor.SetMoveInto(this.InsertionPoint);
      DocumentNodePath valueAsDocumentNode = this.TargetNode.GetLocalValueAsDocumentNode(Base2DElement.EffectProperty);
      if (valueAsDocumentNode != null && valueAsDocumentNode.Node == this.SourceData.DocumentNode)
        return false;
      descriptor.TryReplace((object) this.SourceData, SmartInsertionPoint.From(this.InsertionPoint), this.DestinationCollection);
      return true;
    }

    protected override DragDropEffects OnHandleDrop(DragDropEffects dropEffects)
    {
      bool flag = (dropEffects & DragDropEffects.Copy) != DragDropEffects.None;
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(flag ? StringTable.UndoUnitCopy : StringTable.UndoUnitArrange))
      {
        SceneNode sceneNode = !flag ? this.Move(this.SourceData) : this.Copy(this.SourceData);
        if (sceneNode != null)
          this.ViewModel.SelectNodes((ICollection<SceneNode>) new SceneNode[1]
          {
            sceneNode
          });
        else
          dropEffects = DragDropEffects.None;
        editTransaction.Commit();
      }
      return dropEffects;
    }

    private SceneNode Move(SceneNode node)
    {
      if (node.IsAttached)
      {
        SceneElement sceneElement = (SceneElement) node.Parent;
        PropertyReference propertyReference = new PropertyReference((ReferenceStep) sceneElement.GetPropertyForChild(node));
        this.TargetNode.ViewModel.AnimationEditor.DeleteAllAnimations((SceneNode) sceneElement, propertyReference.ToString());
        this.TargetNode.ViewModel.RemoveElement(node);
      }
      return this.InsertNode(node);
    }

    private SceneNode Copy(SceneNode node)
    {
      DocumentNode node1 = node.DocumentNode.Clone(node.DocumentContext);
      if (node1 != null)
      {
        SceneNode sceneNode = this.ViewModel.GetSceneNode(node1);
        if (sceneNode != null)
          return this.InsertNode(sceneNode);
      }
      return (SceneNode) null;
    }

    private SceneNode InsertNode(SceneNode node)
    {
      SmartInsertionPoint smartInsertionPoint = SmartInsertionPoint.From(this.InsertionPoint);
      if (smartInsertionPoint == null)
        return (SceneNode) null;
      smartInsertionPoint.Insert(node);
      return node;
    }
  }
}
