// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.SourceContextContainer`1
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public sealed class SourceContextContainer<T> where T : class
  {
    private INodeSourceContext containerContext;
    private T content;

    public INodeSourceContext ContainerContext
    {
      get
      {
        return this.containerContext;
      }
      set
      {
        this.containerContext = value;
      }
    }

    public T Content
    {
      get
      {
        return this.content;
      }
    }

    internal SourceContextContainer(INodeSourceContext containerContext, T content)
    {
      if ((object) content == null)
        throw new ArgumentNullException("content");
      this.containerContext = containerContext;
      this.content = content;
    }

    public override string ToString()
    {
      return this.content.ToString();
    }

    public static T ToContent(SourceContextContainer<T> container)
    {
      if (container == null)
        return default (T);
      return container.Content;
    }

    public static bool ContentReferenceEquals(SourceContextContainer<T> a, SourceContextContainer<T> b)
    {
      return (object) SourceContextContainer<T>.ToContent(a) == (object) SourceContextContainer<T>.ToContent(b);
    }
  }
}
