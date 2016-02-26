// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.ConflictModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  public sealed class ConflictModel
  {
    public IList<StateGroupModel> Groups { get; private set; }

    public TimelineSceneNode.PropertyNodePair TargetElementAndProperty { get; private set; }

    public string Message
    {
      get
      {
        string str1 = string.Empty;
        string str2 = string.Empty;
        foreach (StateGroupModel stateGroupModel in (IEnumerable<StateGroupModel>) this.Groups)
        {
          str1 += string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}{1}", new object[2]
          {
            (object) str2,
            (object) stateGroupModel.Name
          });
          str2 = StringTable.VisualStateConflictGroupsSplitter;
        }
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.VisualStateConflictPropertyWarning, (object) this.TargetElementAndProperty.SceneNode.DisplayName, (object) this.TargetElementAndProperty.PropertyReference.ShortPath, (object) str1);
      }
    }

    public ConflictModel(TimelineSceneNode.PropertyNodePair propertyNode, IEnumerable<StateGroupModel> groups)
    {
      this.Groups = (IList<StateGroupModel>) new List<StateGroupModel>(groups);
      this.TargetElementAndProperty = propertyNode;
    }
  }
}
