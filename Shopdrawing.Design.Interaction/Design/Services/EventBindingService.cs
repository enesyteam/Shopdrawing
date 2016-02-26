// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Services.EventBindingService
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Model;
using System;
using System.CodeDom;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Services
{
  public abstract class EventBindingService
  {
    public event EventHandler<EventHandlerGeneratedEventArgs> EventHandlerGenerated;

    public abstract void SetClassName(string className);

    public abstract bool CreateMethod(ModelEvent modelEvent, string methodName);

    public abstract void AppendStatements(ModelEvent modelEvent, string methodName, CodeStatementCollection statements);

    public abstract string CreateUniqueMethodName(ModelEvent modelEvent);

    public abstract bool AllowClassNameForMethodName();

    public abstract IEnumerable<string> GetCompatibleMethods(ModelEvent modelEvent);

    public abstract bool IsExistingMethodName(ModelEvent modelEvent, string methodName);

    public abstract void ValidateMethodName(ModelEvent modelEvent, string methodName);

    public abstract bool RemoveMethod(ModelEvent modelEvent, string methodName);

    public abstract bool ShowMethod(ModelEvent modelEvent, string methodName);

    public abstract IEnumerable<string> GetMethodHandlers(ModelEvent modelEvent);

    public abstract bool AddEventHandler(ModelEvent modelEvent, string methodName);

    public abstract bool RemoveHandle(ModelEvent modelEvent, string methodName);

    public virtual bool RemoveHandlesForName(string elementName)
    {
      return false;
    }

    protected virtual void OnEventHandlerGenerated(ModelEvent modelEvent, string methodName)
    {
      if (this.EventHandlerGenerated == null)
        return;
      this.EventHandlerGenerated((object) this, new EventHandlerGeneratedEventArgs(modelEvent, methodName));
    }
  }
}
