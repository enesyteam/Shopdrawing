// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.TextLocation
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Text;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class TextLocation : ITextLocation
  {
    private int line;
    private int column;

    public int Line
    {
      get
      {
        return this.line;
      }
    }

    public int Column
    {
      get
      {
        return this.column;
      }
    }

    public TextLocation(int line, int column)
    {
      this.line = line;
      this.column = column;
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      ITextLocation textLocation = obj as ITextLocation;
      if (textLocation != null && this.line == textLocation.Line)
        return this.column == textLocation.Column;
      return false;
    }

    public override int GetHashCode()
    {
      return this.line.GetHashCode() ^ this.column.GetHashCode();
    }

    public override string ToString()
    {
      return "[" + (object) this.line + ", " + (string) (object) this.column + "]";
    }
  }
}
