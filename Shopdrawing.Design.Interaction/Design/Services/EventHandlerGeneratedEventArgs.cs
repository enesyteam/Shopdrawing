// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Services.EventHandlerGeneratedEventArgs
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Model;
using System;

namespace Microsoft.Windows.Design.Services
{
  public class EventHandlerGeneratedEventArgs : EventArgs
  {
    private ModelEvent _modelEvent;
    private string _methodName;

    public ModelEvent ModelEvent
    {
      get
      {
        return this._modelEvent;
      }
    }

    public string MethodName
    {
      get
      {
        return this._methodName;
      }
    }

    public EventHandlerGeneratedEventArgs(ModelEvent modelEvent, string methodName)
    {
      if (modelEvent == null)
        throw new ArgumentNullException("modelEvent");
      if (methodName == null)
        throw new ArgumentNullException("methodName");
      this._modelEvent = modelEvent;
      this._methodName = methodName;
    }
  }
}
