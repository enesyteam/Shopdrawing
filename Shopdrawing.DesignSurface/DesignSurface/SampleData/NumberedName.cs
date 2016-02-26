// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.NumberedName
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class NumberedName
  {
    public string BaseName { get; private set; }

    public int MinNumberLength { get; private set; }

    public int CurrentNumber { get; private set; }

    public string CurrentName { get; private set; }

    public NumberedName(string name)
    {
      this.CurrentName = name;
      this.BaseName = name;
      this.CurrentNumber = -1;
      this.Initialize(name);
    }

    private void Initialize(string name)
    {
      int num = -1;
      for (int index = name.Length - 1; index >= 0 && char.IsDigit(name, index); --index)
        num = index;
      if (num <= 0)
        return;
      string s = name.Substring(num);
      int result = 1;
      if (!int.TryParse(s, out result) || result >= 1073741823)
        return;
      this.CurrentNumber = result;
      this.MinNumberLength = s.Length;
      this.BaseName = name.Substring(0, num);
    }

    public bool Increment()
    {
      if (this.CurrentNumber < 0)
      {
        this.CurrentNumber = 1;
      }
      else
      {
        if (this.CurrentNumber >= int.MaxValue)
          return false;
        ++this.CurrentNumber;
      }
      string str = this.CurrentNumber.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      int count = this.MinNumberLength - str.Length;
      if (count > 0)
        str = new string('0', count) + str;
      this.CurrentName = this.BaseName + str;
      return true;
    }
  }
}
