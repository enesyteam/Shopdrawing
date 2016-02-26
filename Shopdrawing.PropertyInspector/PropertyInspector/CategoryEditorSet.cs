// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.CategoryEditorSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public sealed class CategoryEditorSet : IEnumerable<CategoryEditor>, IEnumerable
  {
    private Dictionary<CategoryEditorSet.UniqueTypeToken, CategoryEditor> editors;

    public CategoryEditorSet()
    {
      this.editors = new Dictionary<CategoryEditorSet.UniqueTypeToken, CategoryEditor>();
    }

    private CategoryEditorSet(Dictionary<CategoryEditorSet.UniqueTypeToken, CategoryEditor> editors)
    {
      this.editors = editors;
    }

    public bool TryAddCategoryEditorType(Type categoryEditorType)
    {
      if (!typeof (CategoryEditor).IsAssignableFrom(categoryEditorType))
        throw new InvalidOperationException();
      try
      {
        this.AddCategoryEditor((CategoryEditor) Activator.CreateInstance(categoryEditorType), null);
        return true;
      }
      catch
      {
      }
      return false;
    }

    public void AddCategoryEditor(CategoryEditor editor)
    {
      this.AddCategoryEditor(editor, null);
    }

    public void AddCategoryEditor(CategoryEditor editor, object uniqueToken)
    {
      if (editor == null)
        throw new ArgumentNullException("editor");
      CategoryEditorSet.UniqueTypeToken key = new CategoryEditorSet.UniqueTypeToken((editor).GetType(), uniqueToken);
      if (this.editors.ContainsKey(key))
        return;
      this.editors[key] = editor;
    }

    public CategoryEditorSet Complement(CategoryEditorSet otherSet)
    {
      CategoryEditorSet categoryEditorSet = new CategoryEditorSet();
      using (Dictionary<CategoryEditorSet.UniqueTypeToken, CategoryEditor>.Enumerator enumerator = this.editors.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          KeyValuePair<CategoryEditorSet.UniqueTypeToken, CategoryEditor> current = enumerator.Current;
          if (!otherSet.editors.ContainsKey(current.Key))
            categoryEditorSet.editors.Add(current.Key, current.Value);
        }
      }
      return categoryEditorSet;
    }

    public CategoryEditorSet Union(CategoryEditorSet categoryEditorSet)
    {
      CategoryEditorSet categoryEditorSet1 = new CategoryEditorSet(new Dictionary<CategoryEditorSet.UniqueTypeToken, CategoryEditor>((IDictionary<CategoryEditorSet.UniqueTypeToken, CategoryEditor>) this.editors));
      using (Dictionary<CategoryEditorSet.UniqueTypeToken, CategoryEditor>.Enumerator enumerator = categoryEditorSet.editors.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          KeyValuePair<CategoryEditorSet.UniqueTypeToken, CategoryEditor> current = enumerator.Current;
          if (!categoryEditorSet1.editors.ContainsKey(current.Key))
            categoryEditorSet1.editors.Add(current.Key, current.Value);
        }
      }
      return categoryEditorSet1;
    }

    public IEnumerator<CategoryEditor> GetEnumerator()
    {
      return (IEnumerator<CategoryEditor>) this.editors.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    private sealed class UniqueTypeToken
    {
      private Type type;
      private object token;

      public UniqueTypeToken(Type type, object token)
      {
        this.type = type;
        this.token = token;
      }

      public override bool Equals(object obj)
      {
        CategoryEditorSet.UniqueTypeToken uniqueTypeToken = obj as CategoryEditorSet.UniqueTypeToken;
        if (uniqueTypeToken != null && uniqueTypeToken.type.Equals(this.type))
          return object.Equals(uniqueTypeToken.token, this.token);
        return false;
      }

      public override int GetHashCode()
      {
        if (this.token != null)
          return this.type.GetHashCode() ^ this.token.GetHashCode();
        return this.type.GetHashCode();
      }
    }
  }
}
