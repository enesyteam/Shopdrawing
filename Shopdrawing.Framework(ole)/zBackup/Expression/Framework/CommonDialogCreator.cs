// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.CommonDialogCreator
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.Extensions.ServiceProvider;
using System;
using System.Windows;

namespace Microsoft.Expression.Framework
{
  [CLSCompliant(false)]
  public abstract class CommonDialogCreator
  {
    protected IServices Services { get; private set; }

    [CLSCompliant(false)]
    public abstract ILicensingDialogQuery GetInstance { get; }

    protected CommonDialogCreator(IServices services)
    {
      this.Services = services;
    }

    protected void MergeLicensingResources(LicensingDialogBase dialog)
    {
      ResourceDictionary resourceDictionary = FileTable.GetResourceDictionary("Licensing/LicensingDialogResources.xaml");
      dialog.Resources.MergedDictionaries.Add(resourceDictionary);
      ResourceDictionary licensingDialogResources = ServiceProviderExtensions.LicenseService((IServiceProvider) this.Services).LicensingDialogResources;
      if (licensingDialogResources == null)
        return;
      dialog.Resources.MergedDictionaries.Add(licensingDialogResources);
    }
  }
}
