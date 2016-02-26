// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.CommandLineService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.Expression.Framework
{
  public sealed class CommandLineService : ICommandLineService
  {
    public string GetArgument(string name)
    {
      return this.GetArgument(name, (string[]) null);
    }

    public string[] GetArguments(string name)
    {
      return this.GetArguments(name, (string[]) null);
    }

    public string GetArgument(string name, string[] arguments)
    {
      StringCollection stringCollection = (StringCollection) this.Parse(arguments)[(object) name.ToUpperInvariant()];
      if (stringCollection == null || stringCollection.Count == 0)
        return (string) null;
      return stringCollection[stringCollection.Count - 1];
    }

    public string[] GetArguments(string name, string[] arguments)
    {
      StringCollection stringCollection = (StringCollection) this.Parse(arguments)[(object) name.ToUpperInvariant()];
      if (stringCollection == null)
        return (string[]) null;
      string[] array = new string[stringCollection.Count];
      stringCollection.CopyTo(array, 0);
      return array;
    }

    private IDictionary Parse(string[] arguments)
    {
      if (arguments == null)
        arguments = Environment.GetCommandLineArgs();
      IDictionary dictionary = (IDictionary) new Hashtable();
      for (int index = 1; index < arguments.Length; ++index)
      {
        string str1 = arguments[index];
        string str2 = string.Empty;
        string str3 = string.Empty;
        if (str1.Length > 0)
        {
          if ((int) str1[0] != 47 && (int) str1[0] != 45)
          {
            str3 = str1;
          }
          else
          {
            int num = str1.IndexOf(':');
            if (num == -1)
            {
              str2 = str1.Substring(1);
              if (str2 == "?")
                str2 = "help";
            }
            else
            {
              str2 = str1.Substring(1, num - 1);
              str3 = str1.Substring(num + 1);
            }
          }
        }
        string str4 = str2.ToUpperInvariant();
        StringCollection stringCollection = (StringCollection) dictionary[(object) str4];
        if (stringCollection == null)
        {
          stringCollection = new StringCollection();
          dictionary.Add((object) str4, (object) stringCollection);
        }
        stringCollection.Add(str3);
      }
      return dictionary;
    }
  }
}
