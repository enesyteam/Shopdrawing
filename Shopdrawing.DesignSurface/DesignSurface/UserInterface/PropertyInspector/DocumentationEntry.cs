// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.DocumentationEntry
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Code;
using System;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class DocumentationEntry
  {
    private string typeName;
    private string propertyName;
    private string propertyTypeName;
    private string content;

    public string TypeName
    {
      get
      {
        return this.typeName;
      }
    }

    public string PropertyName
    {
      get
      {
        return this.propertyName;
      }
    }

    public string PropertyTypeName
    {
      get
      {
        return this.propertyTypeName;
      }
    }

    public string Content
    {
      get
      {
        return this.content;
      }
    }

    public DocumentationEntry(string typeName, string propertyName, Type propertyType, string content)
    {
      this.typeName = typeName;
      this.propertyName = propertyName;
      this.propertyTypeName = TypeNameFormatter.FormatTypeForDefaultLanguage(propertyType, false);
      this.content = content;
    }
  }
}
