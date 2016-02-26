// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.TypeReferences
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Expression.DesignModel.Markup
{
  public sealed class TypeReferences
  {
    private Dictionary<IType, TypeReferences.Reference> typeReferences;
    private HashSet<IType> undoReferences;
    private List<IType> unresolvedTypes;

    public IEnumerable<KeyValuePair<IType, int>> References
    {
      get
      {
        foreach (KeyValuePair<IType, TypeReferences.Reference> keyValuePair in this.typeReferences)
          yield return new KeyValuePair<IType, int>(keyValuePair.Key, keyValuePair.Value.Count);
        foreach (IType key in this.undoReferences)
          yield return new KeyValuePair<IType, int>(key, 1);
      }
    }

    public ICollection<IType> UnresolvedTypes
    {
      get
      {
        return (ICollection<IType>) this.unresolvedTypes;
      }
    }

    public int Count
    {
      get
      {
        return this.typeReferences.Count;
      }
    }

    public bool IsEditable
    {
      get
      {
        return this.unresolvedTypes.Count == 0;
      }
    }

    public event EventHandler IsEditableChanged;

    public TypeReferences()
    {
      this.Clear();
    }

    public void Clear()
    {
      this.typeReferences = new Dictionary<IType, TypeReferences.Reference>();
      this.undoReferences = new HashSet<IType>();
      this.unresolvedTypes = new List<IType>();
    }

    public int GetReferenceCount(IType type)
    {
      TypeReferences.Reference reference;
      if (this.typeReferences.TryGetValue(type, out reference))
        return reference.Count;
      return 0;
    }

    public void AddReference(IType type)
    {
      bool flag = false;
      TypeReferences.Reference reference;
      if (!this.typeReferences.TryGetValue(type, out reference))
      {
        reference = new TypeReferences.Reference();
        this.typeReferences.Add(type, reference);
        flag = true;
      }
      ++reference.Count;
      if (!flag)
        return;
      this.ResolveAllTypes();
    }

    public void RemoveReference(IType type)
    {
      bool flag = false;
      TypeReferences.Reference reference;
      if (this.typeReferences.TryGetValue(type, out reference))
      {
        --reference.Count;
        if (reference.Count == 0)
        {
          this.undoReferences.Add(type);
          this.typeReferences.Remove(type);
          flag = true;
        }
      }
      if (!flag)
        return;
      this.ResolveAllTypes();
    }

    public void ResolveAllTypes()
    {
      bool isEditable = this.IsEditable;
      this.unresolvedTypes = new List<IType>();
      foreach (IType type in this.typeReferences.Keys)
      {
        if (!type.IsResolvable)
          this.unresolvedTypes.Add(type);
      }
      if (this.IsEditable == isEditable)
        return;
      this.OnIsEditableChanged();
    }

    private void OnIsEditableChanged()
    {
      if (this.IsEditableChanged == null)
        return;
      this.IsEditableChanged((object) this, EventArgs.Empty);
    }

    [Conditional("ReportMessages")]
    private void ReportMessage(string message)
    {
    }

    private sealed class Reference
    {
      private int count;

      public int Count
      {
        get
        {
          return this.count;
        }
        set
        {
          this.count = value;
        }
      }

      public override string ToString()
      {
        return this.Count.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
    }
  }
}
