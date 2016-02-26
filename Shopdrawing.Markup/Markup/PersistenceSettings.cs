// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.PersistenceSettings
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Markup
{
  public sealed class PersistenceSettings
  {
    private static ElementPersistenceSettings DefaultElementSettings = new ElementPersistenceSettings(typeof (object), 1, 1);
    private static PersistenceSettings Defaults = new PersistenceSettings((PersistenceSettings) null);
    private char attributeQuoteCharacter;
    private char markupExtensionQuoteCharacter;
    private string indentString;
    private string attributeIndentString;
    private int spacesPerTabStop;
    private Dictionary<IXmlNamespace, string> defaultXmlnsPrefixes;
    private string customXmlnsPrefix;
    private string emptyXmlnsPrefixSubstitute;
    private int lineLength;

    public char AttributeQuoteCharacter
    {
      get
      {
        return this.attributeQuoteCharacter;
      }
      set
      {
        if ((int) value != 39 && (int) value != 34)
          throw new ArgumentException(ExceptionStringTable.InvalidXmlAttributeQuote);
        this.attributeQuoteCharacter = value;
      }
    }

    public char MarkupExtensionQuoteCharacter
    {
      get
      {
        return this.markupExtensionQuoteCharacter;
      }
      set
      {
        if ((int) value != 39 && (int) value != 34)
          throw new ArgumentException(ExceptionStringTable.InvalidMarkupExtensionAttributeQuote);
        this.markupExtensionQuoteCharacter = value;
      }
    }

    public string IndentString
    {
      get
      {
        return this.indentString;
      }
      set
      {
        if (PersistenceSettings.IndexOfAnyExcept(value, " \t") >= 0)
          throw new ArgumentException(ExceptionStringTable.InvalidIndentString);
        this.indentString = value;
      }
    }

    public string AttributeIndentString
    {
      get
      {
        return this.attributeIndentString;
      }
      set
      {
        if (string.IsNullOrEmpty(value) || PersistenceSettings.IndexOfAnyExcept(value, " \t") >= 0)
          throw new ArgumentException(ExceptionStringTable.InvalidIndentString);
        this.attributeIndentString = value;
      }
    }

    public int SpacesPerTabStop
    {
      get
      {
        return this.spacesPerTabStop;
      }
      set
      {
        if (value < 1)
          throw new ArgumentException(ExceptionStringTable.InvalidSpacesPerTabStop);
        this.spacesPerTabStop = value;
      }
    }

    public string CustomXmlnsPrefix
    {
      get
      {
        return this.customXmlnsPrefix;
      }
      set
      {
        this.customXmlnsPrefix = value;
      }
    }

    public string EmptyXmlnsPrefixSubstitute
    {
      get
      {
        return this.emptyXmlnsPrefixSubstitute;
      }
      set
      {
        if (string.IsNullOrEmpty(value))
          throw new ArgumentException(ExceptionStringTable.InvalidXmlnsPrefix);
        this.emptyXmlnsPrefixSubstitute = value;
      }
    }

    public int LineLength
    {
      get
      {
        return this.lineLength;
      }
      set
      {
        this.lineLength = value;
      }
    }

    static PersistenceSettings()
    {
      PersistenceSettings.Defaults.attributeQuoteCharacter = '"';
      PersistenceSettings.Defaults.markupExtensionQuoteCharacter = '\'';
      PersistenceSettings.Defaults.indentString = "\t";
      PersistenceSettings.Defaults.attributeIndentString = " ";
      PersistenceSettings.Defaults.spacesPerTabStop = 4;
      PersistenceSettings.Defaults.defaultXmlnsPrefixes.Add((IXmlNamespace) XmlNamespace.AvalonXmlNamespace, string.Empty);
      PersistenceSettings.Defaults.defaultXmlnsPrefixes.Add((IXmlNamespace) XmlNamespace.XamlXmlNamespace, "x");
      PersistenceSettings.Defaults.defaultXmlnsPrefixes.Add((IXmlNamespace) XmlNamespace.PresentationOptionsXmlNamespace, "Options");
      PersistenceSettings.Defaults.defaultXmlnsPrefixes.Add((IXmlNamespace) XmlNamespace.CompatibilityXmlNamespace, "mc");
      PersistenceSettings.Defaults.defaultXmlnsPrefixes.Add((IXmlNamespace) XmlNamespace.DesignTimeXmlNamespace, "d");
      PersistenceSettings.Defaults.defaultXmlnsPrefixes.Add((IXmlNamespace) XmlNamespace.AnnotationsXmlNamespace, "Anno");
      PersistenceSettings.Defaults.customXmlnsPrefix = "Custom";
      PersistenceSettings.Defaults.emptyXmlnsPrefixSubstitute = "Default";
      PersistenceSettings.Defaults.lineLength = int.MaxValue;
    }

    public PersistenceSettings()
      : this(PersistenceSettings.Defaults)
    {
    }

    private PersistenceSettings(PersistenceSettings settings)
    {
      if (settings != null)
        this.Copy(settings);
      else
        this.defaultXmlnsPrefixes = new Dictionary<IXmlNamespace, string>();
    }

    private void Copy(PersistenceSettings settings)
    {
      this.attributeQuoteCharacter = settings.attributeQuoteCharacter;
      this.markupExtensionQuoteCharacter = settings.markupExtensionQuoteCharacter;
      this.indentString = settings.indentString;
      this.attributeIndentString = settings.attributeIndentString;
      this.spacesPerTabStop = settings.spacesPerTabStop;
      this.customXmlnsPrefix = settings.customXmlnsPrefix;
      this.emptyXmlnsPrefixSubstitute = settings.emptyXmlnsPrefixSubstitute;
      this.lineLength = settings.lineLength;
      this.defaultXmlnsPrefixes = new Dictionary<IXmlNamespace, string>((IDictionary<IXmlNamespace, string>) settings.defaultXmlnsPrefixes);
    }

    public void Reset()
    {
      this.Copy(PersistenceSettings.Defaults);
    }

    public string GetDefaultXmlnsPrefix(IXmlNamespace xmlNamespace)
    {
      string str;
      if (this.defaultXmlnsPrefixes.TryGetValue(xmlNamespace, out str))
        return str;
      return this.customXmlnsPrefix;
    }

    internal ElementPersistenceSettings GetElementSettings(Type type)
    {
      return PersistenceSettings.DefaultElementSettings;
    }

    private static int IndexOfAnyExcept(string text, string exceptions)
    {
      for (int index = 0; index < text.Length; ++index)
      {
        if (exceptions.IndexOf(text[index]) < 0)
          return index;
      }
      return -1;
    }
  }
}
