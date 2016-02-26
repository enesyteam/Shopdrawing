// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Services.ExternalMarkupService
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Model;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Windows.Design.Services
{
  public abstract class ExternalMarkupService
  {
    public abstract ModelItem Load(string markup, IEnumerable<AssemblyName> additionalReferences);

    public abstract string Save(ModelItem root, out IEnumerable<AssemblyName> requiredAssemblies);
  }
}
