// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.KeyFrameSelectionSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class KeyFrameSelectionSet : SelectionSet<KeyFrameSceneNode, MarkerBasedSceneNodeCollection<KeyFrameSceneNode>>
  {
    public int FilteredSelectionCount
    {
      get
      {
        int num = 0;
        ReadOnlyCollection<KeyFrameSceneNode> selection = this.Selection;
        for (int index = 0; index < selection.Count; ++index)
        {
          if (!this.IsHiddenKeyFrame(selection[index]))
            ++num;
        }
        return num;
      }
    }

    public ReadOnlyCollection<SceneElement> DerivedTargetElements
    {
      get
      {
        List<SceneElement> list = new List<SceneElement>();
        foreach (KeyFrameSceneNode keyFrameSceneNode in this.Selection)
        {
          SceneElement sceneElement = keyFrameSceneNode.TargetElement as SceneElement;
          if (sceneElement != null && !list.Contains(sceneElement))
            list.Add(sceneElement);
        }
        return new ReadOnlyCollection<SceneElement>((IList<SceneElement>) list);
      }
    }

    public KeyFrameSelectionSet(SceneViewModel viewModel)
      : base(viewModel, (ISelectionSetNamingHelper) new KeyFrameSelectionSet.KeyFrameNamingHelper(), (SelectionSet<KeyFrameSceneNode, MarkerBasedSceneNodeCollection<KeyFrameSceneNode>>.IStorageProvider) new SceneNodeSelectionSetStorageProvider<KeyFrameSceneNode>(viewModel))
    {
    }

    public bool IsHiddenKeyFrame(KeyFrameSceneNode node)
    {
      return node.KeyFrameAnimation != null && node.KeyFrameAnimation.TargetProperty != null && DesignTimeProperties.ExplicitAnimationProperty.Equals((object) node.KeyFrameAnimation.TargetProperty[0]);
    }

    private class KeyFrameNamingHelper : ISelectionSetNamingHelper
    {
      public string Name
      {
        get
        {
          return StringTable.UndoUnitKeyFrameName;
        }
      }

      public string GetUndoString(object obj)
      {
        KeyFrameSceneNode keyFrameSceneNode = obj as KeyFrameSceneNode;
        if (keyFrameSceneNode == null)
          return "";
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}", new object[1]
        {
          (object) keyFrameSceneNode.Time
        });
      }
    }
  }
}
