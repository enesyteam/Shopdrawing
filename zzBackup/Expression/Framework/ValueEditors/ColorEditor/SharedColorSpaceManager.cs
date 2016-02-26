// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ColorEditor.SharedColorSpaceManager
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Configuration;
using System;

namespace Microsoft.Expression.Framework.ValueEditors.ColorEditor
{
  public class SharedColorSpaceManager
  {
    private const string PropertyKey = "ColorEditorSpace";
    private IConfigurationObject configuration;

    public SharedColorSpaceManager(IConfigurationObject configuration)
    {
      this.configuration = configuration;
      object property = this.configuration.GetProperty("ColorEditorSpace");
      if (property != null && property is ColorSpace)
        ColorEditorModel.SharedColorSpace = (ColorSpace) property;
      ColorEditorModel.SharedColorSpaceChanged += new EventHandler(this.OnSharedColorSpaceChanged);
    }

    public void Unload()
    {
      ColorEditorModel.SharedColorSpaceChanged -= new EventHandler(this.OnSharedColorSpaceChanged);
    }

    private void OnSharedColorSpaceChanged(object sender, EventArgs e)
    {
      this.configuration.SetProperty("ColorEditorSpace", (object) ColorEditorModel.SharedColorSpace);
    }
  }
}
