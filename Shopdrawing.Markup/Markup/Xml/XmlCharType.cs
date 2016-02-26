// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.XmlCharType
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal sealed class XmlCharType
  {
    private static byte[] s_CharProperties = new byte[65536];
    private const byte FWHITESPACE = (byte) 1;
    private const byte FSTARTNAME = (byte) 4;
    private const byte FNAME = (byte) 8;
    private const byte FCHARDATA = (byte) 16;
    public const char MAXWCHAR = '\xFFFF';
    public const int MAXCHARDATA = 1114111;

    static unsafe XmlCharType()
    {
      fixed (byte* properties = &XmlCharType.s_CharProperties[0])
      {
        XmlCharType.SetProperties(new char[8]
        {
          '\t',
          '\n',
          '\r',
          '\r',
          ' ',
          '\xD7FF',
          '\xE000',
          '�'
        }, (byte) 16, properties);
        XmlCharType.SetProperties(new char[30]
        {
          ':',
          ':',
          'A',
          'Z',
          '_',
          '_',
          'a',
          'z',
          'À',
          'Ö',
          'Ø',
          'ö',
          'ø',
          '˿',
          'Ͱ',
          'ͽ',
          '\x037F',
          '\x1FFF',
          '\x200C',
          '\x200D',
          '\x2070',
          '\x218F',
          'Ⰰ',
          '\x2FEF',
          '、',
          '\xD7FF',
          '豈',
          '\xFDCF',
          'ﷰ',
          '�'
        }, (byte) 28, properties);
        XmlCharType.SetProperties(new char[12]
        {
          '-',
          '-',
          '.',
          '.',
          '·',
          '·',
          '0',
          '9',
          '̀',
          'ͯ',
          '‿',
          '⁀'
        }, (byte) 24, properties);
        IntPtr num1 = (IntPtr) (properties + 9);
        int num2 = (int) (byte) ((uint) *(byte*) num1 | 1U);
        *(sbyte*) num1 = (sbyte) num2;
        IntPtr num3 = (IntPtr) (properties + 10);
        int num4 = (int) (byte) ((uint) *(byte*) num3 | 1U);
        *(sbyte*) num3 = (sbyte) num4;
        IntPtr num5 = (IntPtr) (properties + 13);
        int num6 = (int) (byte) ((uint) *(byte*) num5 | 1U);
        *(sbyte*) num5 = (sbyte) num6;
        IntPtr num7 = (IntPtr) (properties + 32);
        int num8 = (int) (byte) ((uint) *(byte*) num7 | 1U);
        *(sbyte*) num7 = (sbyte) num8;
      }
    }

    private XmlCharType()
    {
    }

    private static unsafe void SetProperties(char[] ranges, byte value, byte* properties)
    {
      int index = 0;
      while (index < ranges.Length)
      {
        byte* numPtr1 = properties + (int) ranges[index];
        for (byte* numPtr2 = properties + (int) ranges[index + 1]; numPtr1 <= numPtr2; ++numPtr1)
          *numPtr1 = value;
        index += 2;
      }
    }

    public static bool IsWhiteSpace(char ch)
    {
      return ((int) XmlCharType.s_CharProperties[(int) ch] & 1) != 0;
    }

    public static bool IsDigit(char ch)
    {
      if ((int) ch >= 48)
        return (int) ch <= 57;
      return false;
    }

    public static bool IsHexDigit(char ch)
    {
      if ((int) ch >= 48 && (int) ch <= 57 || (int) ch >= 97 && (int) ch <= 102)
        return true;
      if ((int) ch >= 65)
        return (int) ch <= 70;
      return false;
    }

    public static bool IsExtender(char ch)
    {
      return (int) ch == 183;
    }

    public static bool IsNameChar(char ch)
    {
      return ((int) XmlCharType.s_CharProperties[(int) ch] & 8) != 0;
    }

    public static bool IsStartNameChar(char ch)
    {
      return ((int) XmlCharType.s_CharProperties[(int) ch] & 4) != 0;
    }

    public static bool IsNCNameChar(char ch)
    {
      if (XmlCharType.IsNameChar(ch))
        return (int) ch != 58;
      return false;
    }

    public static bool IsStartNCNameChar(char ch)
    {
      if (XmlCharType.IsStartNameChar(ch))
        return (int) ch != 58;
      return false;
    }

    public static bool IsCharData(char ch)
    {
      return ((int) XmlCharType.s_CharProperties[(int) ch] & 16) != 0;
    }

    internal static bool IsOnlyWhitespace(string str)
    {
      if (str != null)
      {
        for (int index = 0; index < str.Length; ++index)
        {
          if (!XmlCharType.IsWhiteSpace(str[index]))
            return false;
        }
      }
      return true;
    }

    public static bool IsValidName(string name)
    {
      if (name == null || name.Length == 0 || !XmlCharType.IsStartNameChar(name[0]))
        return false;
      int index = 0;
      for (int length = name.Length; index < length; ++index)
      {
        if (!XmlCharType.IsNameChar(name[index]))
          return false;
      }
      return true;
    }
  }
}
