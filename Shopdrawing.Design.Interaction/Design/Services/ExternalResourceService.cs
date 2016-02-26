// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Services.ExternalResourceService
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Services
{
  public abstract class ExternalResourceService
  {
    public abstract ModelResource ApplicationModel { get; }

    public abstract IEnumerable<Uri> ResourceUris { get; }

    public abstract ModelResource GetModelResource(Uri uri);

    public abstract BinaryResource GetBinaryResource(Uri uri);

    public abstract Uri TranslateStreamUri(Uri streamUri);
  }
}
