// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.EventInformation
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class EventInformation : TriggerSourceInformation
  {
    private static readonly string[] commonEvents = new string[6]
    {
      "Loaded",
      "MouseLeftButtonDown",
      "MouseLeftButtonUp",
      "MouseEnter",
      "MouseLeave",
      "MouseMove"
    };
    private IEvent eventKey;
    private ICollection<IParameterDeclaration> parameters;
    private Type returnType;
    private string groupBy;
    private bool isTypeEvent;
    private bool isMostDerivedTypeEvent;

    public override string DisplayName
    {
      get
      {
        return this.eventKey.Name;
      }
    }

    public string EventName
    {
      get
      {
        return this.eventKey.Name;
      }
    }

    public IEvent EventKey
    {
      get
      {
        return this.eventKey;
      }
    }

    public override string GroupBy
    {
      get
      {
        return this.groupBy;
      }
    }

    public bool IsMostDerivedTypeEvent
    {
      get
      {
        if (this.isTypeEvent)
          return this.isMostDerivedTypeEvent;
        return false;
      }
    }

    public bool IsTypeEvent
    {
      get
      {
        return this.isTypeEvent;
      }
    }

    public IType EventHandlerType
    {
      get
      {
        return this.eventKey.EventHandlerType;
      }
    }

    public Type ReturnType
    {
      get
      {
        if (this.returnType == (Type) null)
          this.returnType = EventInformation.GetEventHandlerReturnType(this.EventHandlerType);
        return this.returnType;
      }
    }

    public ICollection<IParameterDeclaration> Parameters
    {
      get
      {
        if (this.parameters == null)
          this.parameters = EventInformation.GetEventHandlerParameters(this.EventHandlerType);
        return this.parameters;
      }
    }

    public override string DetailedDescription
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(this.EventKey.DeclaringType.Name);
        stringBuilder.Append(".");
        stringBuilder.Append(this.EventKey.Name);
        stringBuilder.Append("(");
        ICollection<IParameterDeclaration> parameters = this.Parameters;
        int num = 0;
        foreach (IParameterDeclaration parameterDeclaration in (IEnumerable<IParameterDeclaration>) parameters)
        {
          if (num > 0)
            stringBuilder.Append(", ");
          stringBuilder.Append(parameterDeclaration.ParameterType.Name);
          stringBuilder.Append(" ");
          stringBuilder.Append(parameterDeclaration.Name);
          ++num;
        }
        stringBuilder.Append(")");
        return stringBuilder.ToString();
      }
    }

    protected EventInformation(IEvent eventKey, IType mostDerivedType)
    {
      this.eventKey = eventKey;
      this.groupBy = this.GetEventGroup(mostDerivedType);
    }

    private string GetEventGroup(IType mostDerivedType)
    {
      string name = this.EventKey.Name;
      if (Array.Exists<string>(EventInformation.commonEvents, (Predicate<string>) (s => name.Equals(s, StringComparison.OrdinalIgnoreCase))))
        return StringTable.EventGroupCommon;
      this.isTypeEvent = true;
      if (mostDerivedType == null || !this.EventKey.DeclaringType.Equals((object) mostDerivedType))
        return StringTable.EventGroupOther;
      this.isMostDerivedTypeEvent = true;
      return this.EventKey.DeclaringType.Name;
    }

    public static int CompareNames(EventInformation a, EventInformation b)
    {
      return string.Compare(a.EventName, b.EventName, StringComparison.OrdinalIgnoreCase);
    }

    public override int CompareTo(object obj)
    {
      EventInformation eventInformation = (EventInformation) obj;
      if (this.IsTypeEvent != eventInformation.IsTypeEvent)
        return !this.IsTypeEvent ? -1 : 1;
      if (this.IsMostDerivedTypeEvent != eventInformation.IsMostDerivedTypeEvent)
        return !this.IsMostDerivedTypeEvent ? 1 : -1;
      int num = string.Compare(this.GroupBy, eventInformation.GroupBy, StringComparison.OrdinalIgnoreCase);
      if (num != 0)
        return num;
      return string.Compare(this.EventName, eventInformation.EventName, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj)
    {
      EventInformation eventInformation = obj as EventInformation;
      if ((TriggerSourceInformation) eventInformation != (TriggerSourceInformation) null)
        return this.eventKey == eventInformation.eventKey;
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return this.eventKey.GetHashCode();
    }

    public override string ToString()
    {
      return this.DisplayName;
    }

    public static IEnumerable<EventInformation> GetEventsForType(ITypeResolver typeResolver, IType typeId, MemberType memberTypes)
    {
      IType mostDerivedType = (IType) null;
      for (; typeId != null; typeId = typeId.BaseType)
      {
        MemberAccessTypes allowableAccess = TypeHelper.GetAllowableMemberAccess(typeResolver, typeId);
        foreach (IEvent eventKey in typeId.GetEvents(allowableAccess))
        {
          if (TypeHelper.IsSet(memberTypes, MemberType.RoutedEvent) && eventKey.IncludesRoutedEvent && eventKey.RoutedEvent is RoutedEvent)
          {
            if (mostDerivedType == null)
              mostDerivedType = typeId;
            yield return (EventInformation) new RoutedEventInformation(eventKey, mostDerivedType);
          }
          else if (TypeHelper.IsSet(memberTypes, MemberType.ClrEvent) && eventKey.IncludesClrEvent)
          {
            if (mostDerivedType == null)
              mostDerivedType = typeId;
            yield return new EventInformation(eventKey, mostDerivedType);
          }
        }
      }
    }

    private static MethodInfo GetInvokeMethod(IType eventHandlerType)
    {
      return eventHandlerType.RuntimeType.GetMethod("Invoke");
    }

    private static Type GetEventHandlerReturnType(IType eventHandlerType)
    {
      return EventInformation.GetInvokeMethod(eventHandlerType).ReturnType;
    }

    private static ICollection<IParameterDeclaration> GetEventHandlerParameters(IType eventHandlerType)
    {
      List<IParameterDeclaration> list = new List<IParameterDeclaration>();
      foreach (ParameterInfo parameterInfo in EventInformation.GetInvokeMethod(eventHandlerType).GetParameters())
        list.Add((IParameterDeclaration) new ParameterInformation(parameterInfo.ParameterType, parameterInfo.Name));
      return (ICollection<IParameterDeclaration>) new ReadOnlyCollection<IParameterDeclaration>((IList<IParameterDeclaration>) list);
    }
  }
}
