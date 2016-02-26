// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.DocumentNodeNameScope
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public sealed class DocumentNodeNameScope
  {
    private Dictionary<string, DocumentNode> nameMap;

    public int Count
    {
      get
      {
        if (this.nameMap == null)
          return 0;
        return this.nameMap.Count;
      }
    }

    public void AddNode(string name, DocumentNode node)
    {
      if (this.nameMap == null)
        this.nameMap = new Dictionary<string, DocumentNode>((IEqualityComparer<string>) StringComparer.Ordinal);
      if (this.nameMap.ContainsKey(name))
        return;
      this.nameMap.Add(name, node);
    }

    public DocumentNode FindNode(string name)
    {
      DocumentNode documentNode = (DocumentNode) null;
      if (this.nameMap != null)
        this.nameMap.TryGetValue(name, out documentNode);
      return documentNode;
    }

    public void RemoveNode(string name)
    {
      if (this.nameMap == null || this.FindNode(name) == null)
        return;
      this.nameMap.Remove(name);
    }

    public IEnumerable<KeyValuePair<string, DocumentNode>> GetNames()
    {
      if (this.nameMap != null)
      {
        foreach (KeyValuePair<string, DocumentNode> keyValuePair in this.nameMap)
          yield return keyValuePair;
      }
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Count = {0}", new object[1]
      {
        (object) this.Count
      });
    }
  }
}
