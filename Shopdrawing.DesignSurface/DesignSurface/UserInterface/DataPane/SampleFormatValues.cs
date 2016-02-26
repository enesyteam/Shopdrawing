// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SampleFormatValues
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class SampleFormatValues
  {
    private static readonly int loremIpsumMinWordLength = 3;
    private static readonly int loremIpsumMaxWordLength = 12;
    private Random random = new Random(DateTime.Now.Millisecond);
    private Dictionary<string, SampleFormatValues.StringCollection> formatDictionary;
    private ReadOnlyCollection<string> columns;
    private Dictionary<int, SampleFormatValues.StringCollection> latinWords;
    private static SampleFormatValues instance;
    private string csvFileContents;
    private int csvParseIndex;

    public static SampleFormatValues Instance
    {
      get
      {
        if (SampleFormatValues.instance == null)
          SampleFormatValues.instance = new SampleFormatValues();
        return SampleFormatValues.instance;
      }
    }

    public ReadOnlyCollection<string> Columns
    {
      get
      {
        return this.columns;
      }
    }

    private bool IsEndOfFile
    {
      get
      {
        return this.csvFileContents.Length <= this.csvParseIndex;
      }
    }

    private bool IsEndOfLine
    {
      get
      {
        if (!this.IsEndOfFile)
          return (int) this.csvFileContents[this.csvParseIndex] == 10;
        return true;
      }
    }

    private char CurrentChar
    {
      get
      {
        return this.csvFileContents[this.csvParseIndex];
      }
    }

    private SampleFormatValues()
    {
      this.formatDictionary = new Dictionary<string, SampleFormatValues.StringCollection>();
      this.ReadCsvFile(this.GetStringFileResource("SampleStrings.csv"));
      this.InitializeLatinWords();
    }

    internal string GetNextLoremIpsumWord(int maxWordLength)
    {
      maxWordLength = Math.Max(SampleFormatValues.loremIpsumMinWordLength, maxWordLength);
      maxWordLength = Math.Min(SampleFormatValues.loremIpsumMaxWordLength, maxWordLength);
      return this.latinWords[this.random.Next(SampleFormatValues.loremIpsumMinWordLength, maxWordLength + 1)].NextString;
    }

    public void ResetNextValues()
    {
      foreach (SampleFormatValues.StringCollection stringCollection in this.latinWords.Values)
        stringCollection.ResetIndex();
      foreach (SampleFormatValues.StringCollection stringCollection in this.formatDictionary.Values)
        stringCollection.ResetIndex();
    }

    public string GetNextStringGivenFormat(string formatName)
    {
      SampleFormatValues.StringCollection stringCollection;
      if (this.formatDictionary.TryGetValue(formatName, out stringCollection))
        return stringCollection.NextString;
      return string.Empty;
    }

    private List<string> ReadRow()
    {
      if (this.IsEndOfFile)
        return (List<string>) null;
      List<string> list = new List<string>();
      while (!this.IsEndOfLine)
        list.Add(this.ReadCell());
      ++this.csvParseIndex;
      return list;
    }

    private string ReadCell()
    {
      char[] chArray = new char[4]
      {
        ' ',
        '\r',
        '\t',
        '\n'
      };
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = false;
      string str = "";
      while (!this.IsEndOfLine)
      {
        char currentChar = this.CurrentChar;
        switch (currentChar)
        {
          case ',':
            ++this.csvParseIndex;
            goto label_6;
          case '"':
            flag = true;
            str = stringBuilder.ToString().TrimStart(chArray) + this.ReadQuotedString();
            stringBuilder = new StringBuilder();
            continue;
          default:
            stringBuilder.Append(currentChar);
            ++this.csvParseIndex;
            continue;
        }
      }
label_6:
      if (flag)
        return str + stringBuilder.ToString().TrimEnd(chArray);
      return stringBuilder.ToString().Trim();
    }

    private string ReadQuotedString()
    {
      ++this.csvParseIndex;
      StringBuilder stringBuilder = new StringBuilder();
      while (!this.IsEndOfFile)
      {
        char currentChar = this.CurrentChar;
        if ((int) currentChar == 34)
        {
          if (this.csvParseIndex < this.csvFileContents.Length - 1 && (int) this.csvFileContents[this.csvParseIndex + 1] == 34)
          {
            stringBuilder.Append('"');
            this.csvParseIndex += 2;
          }
          else
          {
            ++this.csvParseIndex;
            return stringBuilder.ToString();
          }
        }
        else
        {
          stringBuilder.Append(currentChar);
          ++this.csvParseIndex;
        }
      }
      return stringBuilder.ToString();
    }

    private void ReadCsvFile(string path)
    {
      if (!Microsoft.Expression.Framework.Documents.PathHelper.FileExists(path))
        return;
      List<string> list1 = new List<string>();
      try
      {
        this.csvFileContents = File.ReadAllText(path);
      }
      catch (IOException ex)
      {
        this.csvFileContents = string.Empty;
        this.columns = new ReadOnlyCollection<string>((IList<string>) list1);
        return;
      }
      List<List<string>> list2 = new List<List<string>>();
      for (List<string> list3 = this.ReadRow(); list3 != null; list3 = this.ReadRow())
        list2.Add(list3);
      List<string> list4 = new List<string>((IEnumerable<string>) list2[0]);
      list2.RemoveAt(0);
      foreach (string key in list4)
        this.formatDictionary.Add(key, new SampleFormatValues.StringCollection());
      foreach (List<string> list3 in list2)
      {
        for (int index = 0; index < list4.Count; ++index)
        {
          string word = list3[index];
          if (!string.IsNullOrEmpty(word))
            this.formatDictionary[list4[index]].AddString(word);
        }
      }
      list4.Sort((IComparer<string>) StringLogicalComparer.Instance);
      this.columns = new ReadOnlyCollection<string>((IList<string>) list4);
    }

    private string GetStringFileResource(string filename)
    {
      return Path.Combine(Path.Combine(TemplateManager.TranslatedFolder("SampleDataResources"), "Data"), filename);
    }

    private void InitializeLatinWords()
    {
      this.latinWords = new Dictionary<int, SampleFormatValues.StringCollection>();
      for (int key = SampleFormatValues.loremIpsumMinWordLength; key <= SampleFormatValues.loremIpsumMaxWordLength; ++key)
        this.latinWords.Add(key, new SampleFormatValues.StringCollection());
      string str = File.ReadAllText(this.GetStringFileResource("LoremIpsum.txt"));
      char[] chArray = new char[1]
      {
        ' '
      };
      foreach (string word in str.Split(chArray))
      {
        if (word.Length >= SampleFormatValues.loremIpsumMinWordLength && word.Length <= SampleFormatValues.loremIpsumMaxWordLength)
          this.latinWords[word.Length].AddString(word);
      }
    }

    private class StringCollection
    {
      private List<string> words = new List<string>();
      private int index;

      public string NextString
      {
        get
        {
          if (this.words.Count == 0)
            return string.Empty;
          if (this.index >= this.words.Count)
            this.index = 0;
          return this.words[this.index++];
        }
      }

      public void AddString(string word)
      {
        this.words.Add(word);
      }

      public void ResetIndex()
      {
        this.index = 0;
      }
    }
  }
}
