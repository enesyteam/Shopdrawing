// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SystemFontFamilyItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System.ComponentModel;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class SystemFontFamilyItem : SourcedFontFamilyItem
  {
    public const string SystemFontCategory = "System Font";
    private SystemFontFamily systemFontFamily;

    public SystemFontFamily SystemFontFamily
    {
      get
      {
        return this.systemFontFamily;
      }
    }

    public override FontFamily DisplayFontFamily
    {
      get
      {
        if (this.PreviewFamilyName == this.FamilyName)
          return this.FontFamily;
        return new FontFamily(this.PreviewFamilyName);
      }
    }

    protected override FontFamily FontFamilySource
    {
      get
      {
        return this.systemFontFamily.FontFamily;
      }
    }

    public override FontFamily SerializeFontFamily
    {
      get
      {
        return this.systemFontFamily.FontFamily;
      }
    }

    public bool DisplayAsNativeSilverlightFont
    {
      get
      {
        if (this.systemFontFamily.IsNativeSilverlightFont)
          return this.DocumentContext.TypeResolver.IsCapabilitySet(PlatformCapability.ShowSilverlightNativeFonts);
        return false;
      }
    }

    public SystemFontFamilyItem(SystemFontFamily systemFontFamily, IDocumentContext documentContext)
      : base(systemFontFamily.FontFamily, "System Font", documentContext)
    {
      this.systemFontFamily = systemFontFamily;
      this.PropertyChanged += new PropertyChangedEventHandler(this.SystemFontFamilyItem_PropertyChanged);
    }

    public SystemFontFamilyItem(SystemFontFamily systemFontFamily, SceneNodeObjectSet sceneNodeObjectSet)
      : base(systemFontFamily.FontFamily, "System Font", sceneNodeObjectSet)
    {
      this.systemFontFamily = systemFontFamily;
      this.PropertyChanged += new PropertyChangedEventHandler(this.SystemFontFamilyItem_PropertyChanged);
    }

    private void SystemFontFamilyItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "PreviewFamilyName"))
        return;
      this.OnPropertyChanged("DisplayFontFamily");
    }
  }
}
