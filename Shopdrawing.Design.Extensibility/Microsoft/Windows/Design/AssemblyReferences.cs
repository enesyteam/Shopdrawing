// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.AssemblyReferences
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using MS.Internal.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Windows.Design
{
  public class AssemblyReferences : ContextItem
  {
    private AssemblyName[] _referencedAssemblies;
    private AssemblyName _localAssemblyName;
    private static Dictionary<string, Assembly> _loadedAssemblyTable;

    public override sealed Type ItemType
    {
      get
      {
        return typeof (AssemblyReferences);
      }
    }

    private static IDictionary<string, Assembly> LoadedAssemblyTable
    {
      get
      {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        Dictionary<string, Assembly> dictionary = AssemblyReferences._loadedAssemblyTable;
        if (dictionary == null || dictionary.Count != assemblies.Length)
        {
          if (dictionary == null)
            dictionary = new Dictionary<string, Assembly>(assemblies.Length);
          lock (dictionary)
          {
            for (int local_2 = 0; local_2 < assemblies.Length; ++local_2)
            {
              Assembly local_3 = assemblies[local_2];
              dictionary[local_3.FullName] = local_3;
            }
            AssemblyReferences._loadedAssemblyTable = dictionary;
          }
        }
        return (IDictionary<string, Assembly>) dictionary;
      }
    }

    public IEnumerable<AssemblyName> ReferencedAssemblies
    {
      get
      {
        return (IEnumerable<AssemblyName>) this._referencedAssemblies;
      }
    }

    public AssemblyName LocalAssemblyName
    {
      get
      {
        return this._localAssemblyName;
      }
    }

    public AssemblyReferences()
    {
      this._referencedAssemblies = new AssemblyName[0];
    }

    public AssemblyReferences(IEnumerable<AssemblyName> newReferences)
    {
      this.BuildAssemblyReferences(newReferences);
    }

    public AssemblyReferences(AssemblyName localAssemblyName, IEnumerable<AssemblyName> newReferences)
    {
      this._localAssemblyName = localAssemblyName;
      this.BuildAssemblyReferences(newReferences);
    }

    public IEnumerable<Type> GetTypes(Type baseType)
    {
      AssemblyName[] names = new AssemblyName[this._referencedAssemblies.Length];
      this._referencedAssemblies.CopyTo((Array) names, 0);
      IDictionary<string, Assembly> assemblyTable = AssemblyReferences.LoadedAssemblyTable;
      for (int idx = 0; idx < names.Length; ++idx)
      {
        Assembly a;
        if (assemblyTable.TryGetValue(names[idx].FullName, out a))
        {
          names[idx] = (AssemblyName) null;
          foreach (Type c in a.GetTypes())
          {
            if (baseType.IsAssignableFrom(c))
              yield return c;
          }
        }
      }
      for (int idx = 0; idx < names.Length; ++idx)
      {
        AssemblyName name = names[idx];
        if (name != null)
        {
          Type[] types = (Type[]) null;
          try
          {
            Assembly assembly = Assembly.Load(name);
            if (assembly != null)
              types = assembly.GetTypes();
          }
          catch (FileNotFoundException ex)
          {
          }
          catch (BadImageFormatException ex)
          {
          }
          catch (FileLoadException ex)
          {
          }
          catch (ReflectionTypeLoadException ex)
          {
          }
          if (types == null)
            break;
          foreach (Type c in types)
          {
            if (baseType.IsAssignableFrom(c))
              yield return c;
          }
        }
      }
    }

    protected override void OnItemChanged(EditingContext context, ContextItem previousItem)
    {
      AssemblyReferences assemblyReferences = (AssemblyReferences) previousItem;
      if (assemblyReferences._referencedAssemblies.Length > 0)
      {
        if (this._localAssemblyName != null)
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_LocalAssemblyNameChanged, new object[0]));
        this._localAssemblyName = assemblyReferences._localAssemblyName;
        List<AssemblyName> list = new List<AssemblyName>(assemblyReferences._referencedAssemblies.Length + this._referencedAssemblies.Length);
        list.AddRange((IEnumerable<AssemblyName>) assemblyReferences._referencedAssemblies);
        foreach (AssemblyName assemblyName1 in this._referencedAssemblies)
        {
          bool flag = true;
          foreach (AssemblyName assemblyName2 in list)
          {
            if (assemblyName2.FullName.Equals(assemblyName1.FullName))
            {
              flag = false;
              break;
            }
          }
          if (flag)
            list.Add(assemblyName1);
        }
        this._referencedAssemblies = list.ToArray();
      }
      base.OnItemChanged(context, previousItem);
    }

    private void BuildAssemblyReferences(IEnumerable<AssemblyName> newReferences)
    {
      if (newReferences == null)
        throw new ArgumentNullException("newReferences");
      int length = Enumerable.Count<AssemblyName>(newReferences);
      if (this._localAssemblyName != null)
        ++length;
      this._referencedAssemblies = new AssemblyName[length];
      int index = 0;
      foreach (AssemblyName assemblyName in newReferences)
      {
        if (assemblyName == null)
          throw new ArgumentNullException("newReferences");
        this._referencedAssemblies[index++] = assemblyName;
      }
      if (this._localAssemblyName == null)
        return;
      this._referencedAssemblies[index] = this._localAssemblyName;
    }
  }
}
