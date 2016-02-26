// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Services.ModelService
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using System;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Services
{
  public abstract class ModelService
  {
    public abstract ModelItem Root { get; }

    public abstract event EventHandler<ModelChangedEventArgs> ModelChanged;

    public abstract ModelItem ConvertItem(ModelItem item);

    protected abstract ModelItem CreateItem(Type itemType, CreateOptions options, params object[] arguments);

    protected abstract ModelItem CreateItem(object item);

    protected abstract ModelItem CreateStaticMemberItem(Type type, string memberName);

    public abstract IEnumerable<ModelItem> Find(ModelItem startingItem, Type type);

    public abstract IEnumerable<ModelItem> Find(ModelItem startingItem, TypeIdentifier typeIdentifier);

    public abstract IEnumerable<ModelItem> Find(ModelItem startingItem, Predicate<Type> match);

    public ModelItem FromName(ModelItem scope, string name)
    {
      return this.FromName(scope, name, StringComparison.Ordinal);
    }

    public abstract ModelItem FromName(ModelItem scope, string name, StringComparison comparison);

    internal ModelItem InvokeCreateItem(Type itemType, CreateOptions options, params object[] arguments)
    {
      return this.CreateItem(itemType, options, arguments);
    }

    internal ModelItem InvokeCreateStaticMemberItem(Type type, string memberName)
    {
      return this.CreateStaticMemberItem(type, memberName);
    }

    internal ModelItem InvokeCreateItem(object item)
    {
      return this.CreateItem(item);
    }

    internal Type InvokeResolveType(TypeIdentifier typeIdentifier)
    {
      return this.ResolveType(typeIdentifier);
    }

    protected abstract Type ResolveType(TypeIdentifier typeIdentifier);
  }
}
