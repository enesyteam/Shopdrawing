// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ShaderEffects.MonochromeEffect
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.DesignSurface.ShaderEffects
{
  public class MonochromeEffect : ShaderEffect
  {
    public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof (MonochromeEffect), 0, SamplingMode.NearestNeighbor);
    private static PixelShader pixelShader = new PixelShader()
    {
      UriSource = new Uri("pack://application:,,,/Microsoft.Expression.DesignSurface;Component/ShaderEffects/MonochromeEffect.ps")
    };

    public Brush Input
    {
      get
      {
        return (Brush) this.GetValue(MonochromeEffect.InputProperty);
      }
      set
      {
        this.SetValue(MonochromeEffect.InputProperty, (object) value);
      }
    }

    public MonochromeEffect()
    {
      this.PixelShader = MonochromeEffect.pixelShader;
      this.UpdateShaderValue(MonochromeEffect.InputProperty);
    }
  }
}
