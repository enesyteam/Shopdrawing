// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DesignObjectDataSourceDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Documents;
using System;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal sealed class DesignObjectDataSourceDialog : ObjectDataSourceDialog
  {
    public override string DataSourceNameSuffix
    {
      get
      {
        return "SampleData";
      }
    }

    public override string HeadingText
    {
      get
      {
        return StringTable.ClrObjectDialogDefaultHeading;
      }
    }

    public override string DataSourceNameError
    {
      get
      {
        string str = base.DataSourceNameError;
        if (string.IsNullOrEmpty(str) && PathHelper.IsDeviceName(this.DataSourceName))
          str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DesignDataSourceErrorDeviceName, new object[1]
          {
            (object) this.DataSourceName
          });
        return str;
      }
    }

    public DesignObjectDataSourceDialog(DataPanelModel model)
      : base(model, "SampleData")
    {
      this.Title = StringTable.DesignObjectDataSourceDialogTitle;
    }

    protected override AssemblyItem CreateAssemblyModel(Assembly runtimeAssembly, Assembly referenceAssembly)
    {
      return (AssemblyItem) new ClrAssemblyDataSourceModel(this.SelectionContext, this.Model, runtimeAssembly, referenceAssembly, true, false, true);
    }
  }
}
