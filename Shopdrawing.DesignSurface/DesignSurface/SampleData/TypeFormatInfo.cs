// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.TypeFormatInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class TypeFormatInfo
  {
    public SampleBasicType SampleType { get; private set; }

    public string Format { get; private set; }

    public string FormatParameters { get; private set; }

    public TypeFormatInfo(SampleBasicType sampleType, string format, string formatParameters)
    {
      this.SampleType = sampleType;
      this.Format = format;
      this.FormatParameters = formatParameters;
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) this, obj))
        return true;
      TypeFormatInfo typeFormatInfo = (TypeFormatInfo) obj;
      return this.SampleType == typeFormatInfo.SampleType && this.Format == typeFormatInfo.Format && this.FormatParameters == typeFormatInfo.FormatParameters;
    }

    public override int GetHashCode()
    {
      int hashCode = this.SampleType.GetHashCode();
      if (this.Format != null)
        hashCode ^= this.Format.GetHashCode();
      if (this.FormatParameters != null)
        hashCode ^= this.FormatParameters.GetHashCode();
      return hashCode;
    }
  }
}
