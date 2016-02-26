// Decompiled with JetBrains decompiler
// Type: MS.Internal.ExtensibleIconLookup
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Media.Imaging;

namespace MS.Internal
{
  internal class ExtensibleIconLookup
  {
    private int priority = 50;
    private const string VisualStudio = "VisualStudio";
    private const string Expression = "Expression";
    private string fullName;
    private int requestedWidth;
    private int requestedHeight;
    private int? bestDistance;
    private string resourceName;
    private Stream stream;
    private static string applicationMoniker;

    public string ResourceName
    {
      get
      {
        return this.resourceName;
      }
    }

    public Stream Stream
    {
      get
      {
        return this.stream;
      }
    }

    public ExtensibleIconLookup(Type type, int requestedWidth, int requestedHeight)
    {
      this.fullName = type.FullName;
      this.requestedWidth = requestedWidth;
      this.requestedHeight = requestedHeight;
      this.ScanAssembly(type.Assembly);
      try
      {
        foreach (Assembly assembly in ExtensibleIconLookup.GetDesignAssemblies(type.Assembly))
          this.ScanAssembly(assembly);
      }
      catch (Exception ex)
      {
      }
    }

    private static IEnumerable<Assembly> GetDesignAssemblies(Assembly original)
    {
      string originalName = (string) null;
      try
      {
        originalName = Path.GetFileNameWithoutExtension(original.Location);
      }
      catch (NotSupportedException ex)
      {
      }
      if (!string.IsNullOrEmpty(originalName))
      {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
          if (!(assembly is AssemblyBuilder) && !assembly.FullName.StartsWith("Blend_RuntimeGeneratedTypesAssembly"))
          {
            string designName = (string) null;
            try
            {
              designName = Path.GetFileName(assembly.Location);
            }
            catch (NotSupportedException ex)
            {
            }
            if (!string.IsNullOrEmpty(designName) && (designName.StartsWith(originalName + ".design", StringComparison.OrdinalIgnoreCase) || designName.StartsWith(originalName + "." + ExtensibleIconLookup.GetApplicationMoniker() + ".design", StringComparison.OrdinalIgnoreCase)))
              yield return assembly;
          }
        }
      }
    }

    private static string GetApplicationMoniker()
    {
      if (string.IsNullOrEmpty(ExtensibleIconLookup.applicationMoniker))
      {
        bool flag = false;
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
          if (assembly.FullName.StartsWith("Microsoft.VisualStudio.Xaml", StringComparison.OrdinalIgnoreCase))
          {
            flag = true;
            break;
          }
        }
        ExtensibleIconLookup.applicationMoniker = flag ? "VisualStudio" : "Expression";
      }
      return ExtensibleIconLookup.applicationMoniker;
    }

    private void ScanAssembly(Assembly assembly)
    {
      string applicationMoniker = ExtensibleIconLookup.GetApplicationMoniker();
      string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".{0}.", new object[1]
      {
        (object) applicationMoniker
      });
      foreach (string name in assembly.GetManifestResourceNames())
      {
        if ((string.CompareOrdinal(applicationMoniker, "Expression") != 0 || name.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) && (string.CompareOrdinal(applicationMoniker, "VisualStudio") != 0 || name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || (name.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)) || (name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))))
        {
          int num1 = name.IndexOf(this.fullName + ".", StringComparison.Ordinal);
          if (num1 != -1)
          {
            string str2 = name.Substring(num1 + this.fullName.Length);
            bool flag = false;
            if (str2.StartsWith(str1, StringComparison.OrdinalIgnoreCase))
            {
              flag = true;
              if (this.priority > 0)
              {
                this.bestDistance = new int?();
                this.priority = 0;
              }
            }
            else if (this.priority > 0)
            {
              flag = true;
              this.priority = 1;
            }
            if (flag)
            {
              int pixelWidth;
              int pixelHeight;
              try
              {
                using (Stream manifestResourceStream = assembly.GetManifestResourceStream(name))
                {
                  BitmapImage bitmapImage = new BitmapImage();
                  bitmapImage.BeginInit();
                  bitmapImage.StreamSource = manifestResourceStream;
                  bitmapImage.EndInit();
                  pixelWidth = bitmapImage.PixelWidth;
                  pixelHeight = bitmapImage.PixelHeight;
                }
              }
              catch (Exception ex)
              {
                continue;
              }
              int num2 = Math.Max(Math.Abs(this.requestedWidth - pixelWidth), Math.Abs(this.requestedHeight - pixelHeight));
              if (!this.bestDistance.HasValue || num2 < this.bestDistance.Value)
              {
                this.resourceName = name;
                this.stream = assembly.GetManifestResourceStream(name);
                this.bestDistance = new int?(num2);
              }
            }
          }
        }
      }
    }
  }
}
