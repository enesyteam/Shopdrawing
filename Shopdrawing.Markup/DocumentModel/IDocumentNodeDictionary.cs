// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.IDocumentNodeDictionary
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public interface IDocumentNodeDictionary : IEnumerable<KeyValuePair<IProperty, DocumentNode>>, IEnumerable
  {
    int Count { get; }

    DocumentNode this[IPropertyId propertyKey] { get; set; }

    DocumentNode this[int item] { get; }

    IList<IProperty> Keys { get; }

    bool Contains(IProperty propertyKey);

    int IndexOf(IProperty propertyKey);
  }
}
