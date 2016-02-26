// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyMarkerCommands
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public sealed class PropertyMarkerCommands
  {
    private static readonly RoutedCommand clearValueCommand = new RoutedCommand("ClearValueCommand", typeof (PropertyMarkerCommands));
    private static readonly RoutedCommand localValueCommand = new RoutedCommand("LocalValueCommand", typeof (PropertyMarkerCommands));
    private static readonly RoutedCommand localResourceCommand = new RoutedCommand("LocalResourceCommand", typeof (PropertyMarkerCommands));
    private static readonly RoutedCommand systemResourceCommand = new RoutedCommand("SystemResourceCommand", typeof (PropertyMarkerCommands));
    private static readonly RoutedCommand dataBindingCommand = new RoutedCommand("DataBindingCommand", typeof (PropertyMarkerCommands));
    private static readonly RoutedCommand elementPropertyBindingCommand = new RoutedCommand("ElementPropertyBindingCommand", typeof (PropertyMarkerCommands));
    private static readonly RoutedCommand templateBindingCommand = new RoutedCommand("TemplateBindingCommand", typeof (PropertyMarkerCommands));
    private static readonly RoutedCommand customExpressionCommand = new RoutedCommand("CustomExpressionCommand", typeof (PropertyMarkerCommands));
    private static readonly RoutedCommand editResourceCommand = new RoutedCommand("EditResourceCommand", typeof (PropertyMarkerCommands));
    private static readonly RoutedCommand convertToResourceCommand = new RoutedCommand("ConvertToResourceCommand", typeof (PropertyMarkerCommands));
    private static readonly RoutedCommand recordCurrentValueCommand = new RoutedCommand("RecordCurrentValueCommand", typeof (PropertyMarkerCommands));

    public static RoutedCommand ClearValueCommand
    {
      get
      {
        return PropertyMarkerCommands.clearValueCommand;
      }
    }

    public static RoutedCommand LocalValueCommand
    {
      get
      {
        return PropertyMarkerCommands.localValueCommand;
      }
    }

    public static RoutedCommand LocalResourceCommand
    {
      get
      {
        return PropertyMarkerCommands.localResourceCommand;
      }
    }

    public static RoutedCommand SystemResourceCommand
    {
      get
      {
        return PropertyMarkerCommands.systemResourceCommand;
      }
    }

    public static RoutedCommand DataBindingCommand
    {
      get
      {
        return PropertyMarkerCommands.dataBindingCommand;
      }
    }

    public static RoutedCommand ElementPropertyBindingCommand
    {
      get
      {
        return PropertyMarkerCommands.elementPropertyBindingCommand;
      }
    }

    public static RoutedCommand TemplateBindingCommand
    {
      get
      {
        return PropertyMarkerCommands.templateBindingCommand;
      }
    }

    public static RoutedCommand CustomExpressionCommand
    {
      get
      {
        return PropertyMarkerCommands.customExpressionCommand;
      }
    }

    public static RoutedCommand EditResourceCommand
    {
      get
      {
        return PropertyMarkerCommands.editResourceCommand;
      }
    }

    public static RoutedCommand ConvertToResourceCommand
    {
      get
      {
        return PropertyMarkerCommands.convertToResourceCommand;
      }
    }

    public static RoutedCommand RecordCurrentValueCommand
    {
      get
      {
        return PropertyMarkerCommands.recordCurrentValueCommand;
      }
    }
  }
}
