// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.ChangedStateNameNodeInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal sealed class ChangedStateNameNodeInfo : ChangedNodeInfo
  {
    public DocumentCompositeNode CompositeNode
    {
      get
      {
        return (DocumentCompositeNode) this.Node;
      }
    }

    public IPropertyId ChangedProperty { get; private set; }

    public string DisplayInformation { get; private set; }

    public ChangedStateNameNodeInfo(DocumentCompositeNode compositeNode, IPropertyId changedProperty)
      : base((DocumentNode) compositeNode)
    {
      string fileName = Path.GetFileName(this.FilePath);
      this.ChangedProperty = changedProperty;
      this.DisplayInformation = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.StateNameBrokenReferenceListEntryFormat, new object[2]
      {
        (object) compositeNode.Type.Name,
        (object) fileName
      });
    }
  }
}
