// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ITextFlowSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public interface ITextFlowSceneNode
  {
    IViewTextPointer ContentStart { get; }

    IViewTextPointer ContentEnd { get; }

    IPropertyId TextChildProperty { get; }

    ISceneNodeCollection<SceneNode> TextFlowCollectionForTextChildProperty { get; }

    ISceneNodeCollection<SceneNode> CollectionForTextChildProperty { get; }

    IViewTextPointer GetPositionFromPoint(Point point);

    void AddInlineTextChild(DocumentNode child);

    void InsertInlineTextChild(int index, DocumentNode child);

    SceneElement EnsureTextParent();
  }
}
