// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.EventHandlerModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public sealed class EventHandlerModel : NotifyingObject
  {
    private IEventHandlerProvider eventHandlerProvider;
    private EventInformation eventInfo;
    private DocumentCompositeNode compositeNode;
    private CodeDomProvider codeDomProvider;
    private AttributeCollection eventAttributes;
    private string proxyValue;

    public IEventHandlerProvider Provider
    {
      get
      {
        return this.eventHandlerProvider;
      }
    }

    public string ProxyValue
    {
      get
      {
        return this.proxyValue;
      }
      set
      {
        if (!(this.proxyValue != value))
          return;
        this.proxyValue = value;
        this.OnPropertyChanged("ProxyValue");
      }
    }

    public ICommand GenerateAndCommitCommand
    {
      get
      {
        return (ICommand) EventHandlerModel.AsyncDelegateCommand(new DelegateCommand.SimpleEventHandler(this.GenerateAndCommit));
      }
    }

    public ICommand CommitIfChangedCommand
    {
      get
      {
        return (ICommand) EventHandlerModel.AsyncDelegateCommand(new DelegateCommand.SimpleEventHandler(this.CommitIfChanged));
      }
    }

    public ICommand CommitCommand
    {
      get
      {
        return (ICommand) EventHandlerModel.AsyncDelegateCommand(new DelegateCommand.SimpleEventHandler(this.Commit));
      }
    }

    public ICommand RevertCommand
    {
      get
      {
        return (ICommand) EventHandlerModel.AsyncDelegateCommand(new DelegateCommand.SimpleEventHandler(this.Revert));
      }
    }

    public IEvent EventKey
    {
      get
      {
        return this.eventInfo.EventKey;
      }
    }

    public EventInformation Event
    {
      get
      {
        return this.eventInfo;
      }
    }

    public DocumentCompositeNode Node
    {
      get
      {
        return this.compositeNode;
      }
    }

    public object ToolTip
    {
      get
      {
        DescriptionAttribute descriptionAttribute = (DescriptionAttribute) null;
        if (this.eventAttributes != null)
          descriptionAttribute = (DescriptionAttribute) this.eventAttributes[typeof (DescriptionAttribute)];
        IEvent eventKey = this.EventKey;
        string content = descriptionAttribute == null || string.IsNullOrEmpty(descriptionAttribute.Description) ? eventKey.Name : descriptionAttribute.Description;
        return (object) new DocumentationEntry(eventKey.DeclaringType.Name, eventKey.Name, PlatformTypeHelper.GetPropertyType((IProperty) eventKey), content);
      }
    }

    public string MethodName
    {
      get
      {
        DocumentPrimitiveNode documentPrimitiveNode = this.compositeNode.Properties[(IPropertyId) this.EventKey] as DocumentPrimitiveNode;
        if (documentPrimitiveNode != null)
        {
          DocumentNodeStringValue documentNodeStringValue = documentPrimitiveNode.Value as DocumentNodeStringValue;
          if (documentNodeStringValue != null)
          {
            string str = documentNodeStringValue.Value;
            if (str != null)
              return str;
          }
        }
        return string.Empty;
      }
    }

    public EventHandlerModel(EventInformation eventInfo, DocumentCompositeNode compositeNode, CodeDomProvider codeDomProvider, IEventHandlerProvider eventHandlerProvider, AttributeCollection eventAttributes)
    {
      this.eventInfo = eventInfo;
      this.compositeNode = compositeNode;
      this.eventHandlerProvider = eventHandlerProvider;
      this.codeDomProvider = codeDomProvider;
      this.proxyValue = this.MethodName;
      this.eventAttributes = eventAttributes;
    }

    public void GenerateAndCommit()
    {
      if (!this.ProcessAsyncCommand())
        return;
      string newName = EventHandlerModel.TrimName(this.proxyValue);
      if (string.IsNullOrEmpty(newName))
        newName = this.GenerateMethodName();
      this.ProxyValue = this.SetMethodName(newName);
    }

    public void CommitIfChanged()
    {
      if (!this.ProcessAsyncCommand() || !(this.proxyValue != this.MethodName))
        return;
      this.Commit();
    }

    public void Commit()
    {
      if (!this.ProcessAsyncCommand())
        return;
      this.ProxyValue = this.SetMethodName(EventHandlerModel.TrimName(this.proxyValue));
    }

    public void Revert()
    {
      if (!this.ProcessAsyncCommand())
        return;
      this.ProxyValue = this.MethodName;
    }

    public override string ToString()
    {
      string eventName = this.eventInfo.EventName;
      string methodName = this.MethodName;
      if (string.IsNullOrEmpty(methodName))
        return eventName;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: {1}", new object[2]
      {
        (object) eventName,
        (object) methodName
      });
    }

    public void Refresh()
    {
      this.OnPropertyChanged("MethodName");
      this.ProxyValue = this.MethodName;
    }

    private string GenerateMethodName()
    {
      string name = this.compositeNode.Name;
      if (string.IsNullOrEmpty(name))
        name = this.compositeNode.Type.Name;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", new object[2]
      {
        (object) name,
        (object) this.EventKey.Name
      });
    }

    private EventHandlerModel.NameType ValidateName(string methodName)
    {
      if (string.IsNullOrEmpty(methodName))
        return EventHandlerModel.NameType.Empty;
      return this.codeDomProvider.IsValidIdentifier(methodName) ? EventHandlerModel.NameType.Valid : EventHandlerModel.NameType.Invalid;
    }

    private string SetMethodName(string newName)
    {
      using (this.eventHandlerProvider.SetHandlerScope())
      {
        string methodName = this.MethodName;
        bool flag;
        switch (this.ValidateName(newName))
        {
          case EventHandlerModel.NameType.Valid:
            flag = this.eventHandlerProvider.AddEventHandler(this.eventInfo.ReturnType, newName, this.eventInfo.Parameters);
            if (!flag)
            {
              this.eventHandlerProvider.ReportEventHandlerNotAdded();
              break;
            }
            break;
          case EventHandlerModel.NameType.Empty:
            flag = true;
            break;
          default:
            this.eventHandlerProvider.ReportInvalidHandlerName(newName);
            flag = false;
            break;
        }
        if (!flag || !(newName != methodName))
          return methodName;
        this.eventHandlerProvider.SetHandler(this.compositeNode, this.EventKey, newName);
        return newName;
      }
    }

    private static string TrimName(string methodName)
    {
      if (methodName == null)
        return string.Empty;
      return methodName.Trim();
    }

    private bool ProcessAsyncCommand()
    {
      return this.Node != null && this.Node.DocumentRoot != null && this.Node.DocumentRoot.DocumentContext != null;
    }

    private static DelegateCommand AsyncDelegateCommand(DelegateCommand.SimpleEventHandler method)
    {
      return new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Send, (Delegate) (arg =>
      {
        method();
        return (object) null;
      }), (object) null)));
    }

    private enum NameType
    {
      Invalid,
      Valid,
      Empty,
    }
  }
}
