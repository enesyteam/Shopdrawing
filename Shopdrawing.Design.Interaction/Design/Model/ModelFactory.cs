// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Model.ModelFactory
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Services;
using System;

namespace Microsoft.Windows.Design.Model
{
  public static class ModelFactory
  {
    public static ModelItem CreateItem(EditingContext context, Type itemType, params object[] arguments)
    {
      return ModelFactory.CreateItem(context, itemType, CreateOptions.None, arguments);
    }

    public static ModelItem CreateItem(EditingContext context, Type itemType, CreateOptions options, params object[] arguments)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (itemType == null)
        throw new ArgumentNullException("itemType");
      if (!EnumValidator.IsValid(options))
        throw new ArgumentOutOfRangeException("options");
      return context.Services.GetRequiredService<ModelService>().InvokeCreateItem(itemType, options, arguments);
    }

    public static ModelItem CreateItem(EditingContext context, object item)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (item == null)
        throw new ArgumentNullException("item");
      return context.Services.GetRequiredService<ModelService>().InvokeCreateItem(item);
    }

    public static ModelItem CreateStaticMemberItem(EditingContext context, Type type, string memberName)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (type == null)
        throw new ArgumentNullException("type");
      if (memberName == null)
        throw new ArgumentNullException("memberName");
      return context.Services.GetRequiredService<ModelService>().InvokeCreateStaticMemberItem(type, memberName);
    }

    public static ModelItem CreateItem(EditingContext context, TypeIdentifier typeIdentifier, params object[] arguments)
    {
      return ModelFactory.CreateItem(context, typeIdentifier, CreateOptions.None, arguments);
    }

    public static ModelItem CreateItem(EditingContext context, TypeIdentifier typeIdentifier, CreateOptions options, params object[] arguments)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      Type itemType = context.Services.GetRequiredService<ModelService>().InvokeResolveType(typeIdentifier);
      if (itemType != null)
        return context.Services.GetRequiredService<ModelService>().InvokeCreateItem(itemType, options, arguments);
      return (ModelItem) null;
    }

    public static ModelItem CreateStaticMemberItem(EditingContext context, TypeIdentifier typeIdentifier, string memberName)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      if (memberName == null)
        throw new ArgumentNullException("memberName");
      Type type = context.Services.GetRequiredService<ModelService>().InvokeResolveType(typeIdentifier);
      if (type != null)
        return context.Services.GetRequiredService<ModelService>().InvokeCreateStaticMemberItem(type, memberName);
      return (ModelItem) null;
    }

    public static Type ResolveType(EditingContext context, TypeIdentifier typeIdentifier)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      return context.Services.GetRequiredService<ModelService>().InvokeResolveType(typeIdentifier);
    }
  }
}
