// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.UriNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class UriNode : SceneNode
  {
    public static readonly UriNode.ConcreteUriNodeFactory Factory = new UriNode.ConcreteUriNodeFactory();

    public string OriginalString
    {
      get
      {
        DocumentNodeStringValue documentNodeStringValue = ((DocumentPrimitiveNode) this.DocumentNode).Value as DocumentNodeStringValue;
        if (documentNodeStringValue != null)
          return documentNodeStringValue.Value;
        return (string) null;
      }
    }

    public Uri DesignTimeUri
    {
      get
      {
        Uri result = (Uri) null;
        if (Uri.TryCreate(this.OriginalString, UriKind.RelativeOrAbsolute, out result))
          return this.DocumentContext.MakeDesignTimeUri(result);
        return (Uri) null;
      }
    }

    public class ConcreteUriNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new UriNode();
      }
    }
  }
}
