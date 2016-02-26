// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Services.ContextMenuService
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Interaction;
using MS.Internal.Features;
using System;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Services
{
  public class ContextMenuService
  {
    private ContextMenuFeatureConnector _contextMenuExtensionServer;

    internal ContextMenuService(ContextMenuFeatureConnector contextMenuExtensionServer)
    {
      if (contextMenuExtensionServer == null)
        throw new ArgumentNullException("contextMenuExtensionServer");
      this._contextMenuExtensionServer = contextMenuExtensionServer;
    }

    public IEnumerable<MenuBase> GetItems()
    {
      return this._contextMenuExtensionServer.GetItems();
    }
  }
}
