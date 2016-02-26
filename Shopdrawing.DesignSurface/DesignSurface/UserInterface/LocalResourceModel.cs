// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.LocalResourceModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Properties;
using System;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class LocalResourceModel : BaseResourceModel
  {
    private DocumentNode resourceKey;
    private DocumentNode resourceNode;

    public DocumentNode ResourceKey
    {
      get
      {
        return this.resourceKey;
      }
    }

    public DocumentNode ResourceNode
    {
      get
      {
        return this.resourceNode;
      }
    }

    public override string ResourceName
    {
      get
      {
        if (this.ResourceNode.IsInDocument && this.ResourceNode.DocumentRoot.DocumentContext != null)
          return XamlExpressionSerializer.GetStringFromExpression(this.ResourceKey, this.ResourceNode);
        return string.Empty;
      }
    }

    public LocalResourceModel(DocumentNode key, Type type, DocumentNode node, object value)
      : base(type, value)
    {
      this.resourceKey = key;
      this.resourceNode = node;
    }

    public override bool Equals(object obj)
    {
      LocalResourceModel localResourceModel = obj as LocalResourceModel;
      if (localResourceModel != null)
        return this.resourceNode == localResourceModel.resourceNode;
      return false;
    }

    public override int GetHashCode()
    {
      return this.resourceNode.GetHashCode();
    }

    public override string ToString()
    {
      return this.ResourceName;
    }
  }
}
