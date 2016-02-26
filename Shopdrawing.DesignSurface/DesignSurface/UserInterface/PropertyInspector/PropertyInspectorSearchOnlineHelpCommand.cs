// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyInspectorSearchOnlineHelpCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal class PropertyInspectorSearchOnlineHelpCommand : PropertyInspectorHelpCommand
  {
    private const string SearchStringFormat = "http://www.bing.com/search?q=Expression+Blend+{0}+{1}";

    public PropertyInspectorSearchOnlineHelpCommand(DesignerContext designerContext)
      : base(designerContext)
    {
    }

    private string GetFriendlyNameForFrameworkIdentifier(string id)
    {
      if (id == ".NETFramework")
        return "WPF";
      return id;
    }

    private string GetTypeNameFromFullName(string fullName)
    {
      int startIndex = fullName.LastIndexOf('.') + 1;
      string str = fullName;
      if (startIndex >= 0)
        str = fullName.Substring(startIndex);
      return str;
    }

    public override void Execute(object parameter)
    {
      TopicHelpContext contextFromParameter = TopicHelpContext.CreateContextFromParameter(parameter, this.DesignerContext);
      if (contextFromParameter == null)
        return;
      Process.Start(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "http://www.bing.com/search?q=Expression+Blend+{0}+{1}", new object[2]
      {
        (object) this.GetFriendlyNameForFrameworkIdentifier(contextFromParameter.Framework.Identifier),
        (object) this.GetTypeNameFromFullName(contextFromParameter.TopicIdentifier)
      }));
    }
  }
}
