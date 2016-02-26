// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.BaseResourceModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public abstract class BaseResourceModel
  {
    private Type resourceType;
    private object resourceValue;

    public virtual Type ResourceType
    {
      get
      {
        return this.resourceType;
      }
    }

    public virtual object ResourceValue
    {
      get
      {
        return this.resourceValue;
      }
      protected set
      {
        this.resourceValue = value;
      }
    }

    public virtual string ResourceName
    {
      get
      {
        if (this.resourceValue != null)
          return this.resourceValue.ToString();
        return string.Empty;
      }
    }

    protected BaseResourceModel()
    {
    }

    protected BaseResourceModel(Type resourceType, object resourceValue)
    {
      this.resourceType = resourceType;
      this.resourceValue = resourceValue;
    }

    public virtual int CompareTo(object obj)
    {
      BaseResourceModel baseResourceModel = obj as BaseResourceModel;
      if (baseResourceModel == null)
        throw new ArgumentException(ExceptionStringTable.CompareToMustBeCalledWithABaseResourceModel, "obj");
      return this.ResourceName.CompareTo(baseResourceModel.ResourceName);
    }
  }
}
