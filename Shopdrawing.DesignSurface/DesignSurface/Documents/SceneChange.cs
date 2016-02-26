// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.SceneChange
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal class SceneChange
  {
    private SceneNode parent;

    public SceneNode Parent
    {
      get
      {
        return this.parent;
      }
    }

    public SceneChange(SceneNode parent)
    {
      this.parent = parent;
    }

    public static IEnumerable<SceneChange> Changes(DocumentNodeChangeList damage, SceneNode scope, Type sceneChangeType)
    {
      IEnumerable<DocumentNodeChange> values = scope != null ? damage.FindDescendantValues(scope.DocumentNode.Marker) : damage.Values;
      foreach (DocumentNodeChange documentNodeChange in values)
      {
        if (documentNodeChange.Annotation != null)
        {
          List<SceneChange> sceneChanges = (List<SceneChange>) documentNodeChange.Annotation;
          foreach (SceneChange sceneChange in sceneChanges)
          {
            if (sceneChangeType == (Type) null || sceneChangeType.IsAssignableFrom(sceneChange.GetType()))
              yield return sceneChange;
          }
        }
      }
    }

    public static IEnumerable<T> ChangesOfType<T>(DocumentNodeChangeList damage, SceneNode scope) where T : SceneChange
    {
      foreach (T obj in SceneChange.Changes(damage, scope, typeof (T)))
        yield return obj;
    }
  }
}
