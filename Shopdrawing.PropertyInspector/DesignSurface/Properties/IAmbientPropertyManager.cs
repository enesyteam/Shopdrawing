// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.IAmbientPropertyManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;

namespace Microsoft.Expression.DesignSurface.Properties
{
  public interface IAmbientPropertyManager : IDisposable
  {
    void ApplyAmbientProperties(SceneNode node);

    IDisposable SuppressApplyAmbientProperties();

    bool IsAmbientProperty(PropertyReference propertyReference);

    object GetAmbientValue(PropertyReference propertyReference);
  }
}
