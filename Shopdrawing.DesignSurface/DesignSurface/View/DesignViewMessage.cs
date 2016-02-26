// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.DesignViewMessage
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.View
{
  public sealed class DesignViewMessage
  {
    private string header;
    private string details;

    public string Header
    {
      get
      {
        return this.header;
      }
    }

    public string Details
    {
      get
      {
        return this.details;
      }
    }

    public DesignViewMessage(string header, string details)
    {
      this.header = header;
      this.details = details;
    }

    public override bool Equals(object obj)
    {
      DesignViewMessage designViewMessage = obj as DesignViewMessage;
      if (designViewMessage != null && this.header == designViewMessage.header)
        return this.details == designViewMessage.details;
      return false;
    }

    public override int GetHashCode()
    {
      return this.header.GetHashCode() ^ this.details.GetHashCode();
    }
  }
}
