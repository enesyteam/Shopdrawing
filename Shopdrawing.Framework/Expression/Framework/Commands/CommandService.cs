// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Commands.CommandService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Feedback;
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.Commands
{
  public class CommandService : ICommandService, ICommandTarget
  {
    private CommandService.CommandTargetTable targets = new CommandService.CommandTargetTable();
    private CommandService.CommandPropertyTable properties = new CommandService.CommandPropertyTable();
    private bool shortcutsEnabled = true;
    private IFeedbackService feedbackService;

    public ICommandCollection Commands
    {
      get
      {
        return this.targets.Commands;
      }
    }

    public bool ShortcutsEnabled
    {
      get
      {
        return this.shortcutsEnabled;
      }
      set
      {
        this.shortcutsEnabled = value;
      }
    }

    public event CommandChangedEventHandler CommandChanged;

    public event CommandExecutionEventHandler CommandExecuting;

    public event CommandExecutionEventHandler CommandExecuted;

    public CommandService()
    {
    }

    public CommandService(IServices services)
    {
      this.feedbackService = services.GetService<IFeedbackService>();
    }

    public CommandService(IFeedbackService feedbackService)
    {
      this.feedbackService = feedbackService;
    }

    public bool HandleShortcut(Key shortcutKey, ModifierKeys modifiers)
    {
      if (!this.ShortcutsEnabled)
        return false;
      foreach (string commandName in (IEnumerable) this.targets.Commands)
      {
        KeyBinding[] keyBindingArray = this.GetCommandProperty(commandName, "Shortcuts") as KeyBinding[];
        if (keyBindingArray != null)
        {
          foreach (KeyBinding keyBinding in keyBindingArray)
          {
            if (shortcutKey == keyBinding.Key && modifiers == keyBinding.Modifiers)
            {
              object commandProperty = this.GetCommandProperty(commandName, "IsEnabled");
              if (commandProperty != null && (bool) commandProperty)
              {
                this.Execute(commandName, CommandInvocationSource.AcceleratorKey);
                return true;
              }
            }
          }
        }
      }
      return false;
    }

    public void AddCommand(string commandName, ICommand command)
    {
      throw new NotSupportedException("Add commands only to command targets, not directly to the command service.");
    }

    public void RemoveCommand(string commandName)
    {
      throw new NotSupportedException("Remove commands only from command targets, not directly from the command service.");
    }

    public void Execute(string commandName, CommandInvocationSource invocationSource)
    {
      this.OnCommandExecuting(new CommandExecutionEventArgs(commandName));
      ICommandTarget target = this.targets.GetTarget(commandName);
      if (this.feedbackService != null)
        this.feedbackService.AddDataToStream2(1, this.feedbackService.GetFeedbackValue(commandName), (int) invocationSource);
      if (target == null)
        return;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.CommandExecute, commandName);
      target.Execute(commandName, invocationSource);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.CommandExecute, commandName);
      this.OnCommandExecuted(new CommandExecutionEventArgs(commandName));
    }

    public object GetCommandProperty(string commandName, string propertyName)
    {
      object obj = (object) null;
      ICommandTarget target = this.targets.GetTarget(commandName);
      if (target != null)
        obj = target.GetCommandProperty(commandName, propertyName);
      if (obj == null)
        obj = this.properties.GetProperty(commandName, propertyName);
      return obj;
    }

    public void SetCommandProperty(string commandName, string propertyName, object propertyValue)
    {
      ICommandTarget target = this.targets.GetTarget(commandName);
      if (target == null)
        return;
      target.SetCommandProperty(commandName, propertyName, propertyValue);
    }

    public void SetCommandPropertyDefault(string commandName, string propertyName, object propertyValue)
    {
      this.properties.SetProperty(commandName, propertyName, propertyValue);
    }

    public void AddTarget(ICommandTarget target)
    {
      this.RemoveTarget(target);
      target.CommandChanged += new CommandChangedEventHandler(this.Target_CommandChanged);
      CommandCollection commandCollection = new CommandCollection();
      foreach (string str in (IEnumerable) target.Commands)
      {
        this.targets.Add(str, target);
        commandCollection.Add(str);
      }
      this.OnCommandChanged(new CommandChangedEventArgs((ICommandCollection) commandCollection, (ICommandCollection) new CommandCollection()));
    }

    public void RemoveTarget(ICommandTarget target)
    {
      target.CommandChanged -= new CommandChangedEventHandler(this.Target_CommandChanged);
      CommandCollection commandCollection = new CommandCollection();
      foreach (string str in (IEnumerable) target.Commands)
      {
        this.targets.Remove(str, target);
        commandCollection.Add(str);
      }
      this.OnCommandChanged(new CommandChangedEventArgs((ICommandCollection) new CommandCollection(), (ICommandCollection) commandCollection));
    }

    protected virtual void OnCommandChanged(CommandChangedEventArgs e)
    {
      if (this.CommandChanged == null)
        return;
      CommandCollection commandCollection1 = new CommandCollection();
      CommandCollection commandCollection2 = new CommandCollection();
      foreach (string command in (IEnumerable) e.Added)
        commandCollection1.Add(command);
      foreach (string str in (IEnumerable) e.Removed)
      {
        commandCollection2.Add(str);
        if (this.targets.GetTarget(str) == null)
          commandCollection1.Add(str);
      }
      this.CommandChanged((object) this, new CommandChangedEventArgs((ICommandCollection) commandCollection1, (ICommandCollection) commandCollection2));
    }

    protected virtual void OnCommandExecuting(CommandExecutionEventArgs e)
    {
      if (this.CommandExecuting == null)
        return;
      this.CommandExecuting((object) this, e);
    }

    protected virtual void OnCommandExecuted(CommandExecutionEventArgs e)
    {
      if (this.CommandExecuted == null)
        return;
      this.CommandExecuted((object) this, e);
    }

    private void Target_CommandChanged(object sender, CommandChangedEventArgs e)
    {
      ICommandTarget target = (ICommandTarget) sender;
      foreach (string commandName in (IEnumerable) e.Added)
        this.targets.Add(commandName, target);
      foreach (string commandName in (IEnumerable) e.Removed)
        this.targets.Remove(commandName, target);
      this.OnCommandChanged(e);
    }

    public override string ToString()
    {
      using (StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        stringWriter.WriteLine("---- Active Command Target(s) --------");
        stringWriter.WriteLine(this.targets.ToString());
        stringWriter.WriteLine("---- Command Default Properties --------");
        stringWriter.WriteLine(this.properties.ToString());
        return stringWriter.ToString();
      }
    }

    private class CommandPropertyTable
    {
      private IDictionary table = (IDictionary) new Hashtable();

      public void SetProperty(string commandName, string propertyName, object propertyValue)
      {
        IDictionary dictionary = (IDictionary) this.table[(object) commandName];
        if (dictionary == null)
        {
          dictionary = (IDictionary) new Hashtable();
          this.table.Add((object) commandName, (object) dictionary);
        }
        if (propertyValue != null)
        {
          dictionary.Add((object) propertyName, propertyValue);
        }
        else
        {
          if (!dictionary.Contains((object) propertyName))
            return;
          dictionary.Remove((object) propertyName);
        }
      }

      public object GetProperty(string commandName, string propertyName)
      {
        IDictionary dictionary = (IDictionary) this.table[(object) commandName];
        if (dictionary != null)
          return dictionary[(object) propertyName];
        return (object) null;
      }

      public override string ToString()
      {
        using (StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
        {
          string[] array = new string[this.table.Keys.Count];
          this.table.Keys.CopyTo((Array) array, 0);
          Array.Sort<string>(array);
          foreach (string str1 in array)
          {
            stringWriter.Write("<" + str1 + " ");
            IDictionary dictionary = (IDictionary) this.table[(object) str1];
            foreach (string str2 in (IEnumerable) dictionary.Keys)
            {
              stringWriter.Write(str2 + "=\"");
              object obj = dictionary[(object) str2];
              stringWriter.Write(TypeDescriptor.GetConverter(obj.GetType()).ConvertToString((ITypeDescriptorContext) null, CultureInfo.InvariantCulture, obj));
              stringWriter.Write("\" ");
            }
            stringWriter.Write("/>");
            stringWriter.WriteLine();
          }
          return stringWriter.ToString();
        }
      }
    }

    private class CommandTargetTable
    {
      private CommandService.ListDictionary table = new CommandService.ListDictionary();

      public ICommandCollection Commands
      {
        get
        {
          CommandCollection commandCollection = new CommandCollection();
          foreach (string command in (IEnumerable) this.table.Keys)
            commandCollection.Add(command);
          return (ICommandCollection) commandCollection;
        }
      }

      public void Add(string commandName, ICommandTarget target)
      {
        this.table.Remove((object) commandName, (object) target);
        this.table.Add((object) commandName, (object) target);
      }

      public void Remove(string commandName, ICommandTarget target)
      {
        while (this.table.Contains((object) commandName, (object) target))
          this.table.Remove((object) commandName, (object) target);
      }

      public ICommandTarget GetTarget(string commandName)
      {
        ArrayList values = this.table.GetValues((object) commandName);
        if (values != null)
          return (ICommandTarget) values[values.Count - 1];
        return (ICommandTarget) null;
      }

      public override string ToString()
      {
        CommandService.ListDictionary listDictionary = new CommandService.ListDictionary();
        foreach (string str in (IEnumerable) this.Commands)
          listDictionary.Add((ICollection) this.table.GetValues((object) str), (object) str);
        using (StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
        {
          foreach (ICommandTarget commandTarget in (IEnumerable) listDictionary.Keys)
          {
            stringWriter.WriteLine("[" + commandTarget.GetType().FullName + "]");
            ArrayList values = listDictionary.GetValues((object) commandTarget);
            values.Sort();
            foreach (string str in values)
              stringWriter.WriteLine("\t" + str);
          }
          stringWriter.WriteLine();
          return stringWriter.ToString();
        }
      }
    }

    private class ListDictionary
    {
      private IDictionary table = (IDictionary) new Hashtable();

      public ICollection Keys
      {
        get
        {
          return this.table.Keys;
        }
      }

      public void Add(ICollection keys, object value)
      {
        foreach (object key in (IEnumerable) keys)
          this.Add(key, value);
      }

      public void Add(object key, object value)
      {
        ArrayList arrayList = (ArrayList) this.table[key];
        if (arrayList == null)
        {
          arrayList = new ArrayList();
          this.table.Add(key, (object) arrayList);
        }
        arrayList.Add(value);
      }

      public void Remove(object key, object value)
      {
        ArrayList arrayList = (ArrayList) this.table[key];
        if (arrayList == null)
          return;
        arrayList.Remove(value);
        if (arrayList.Count != 0)
          return;
        this.table.Remove(key);
      }

      public bool Contains(object key, object value)
      {
        ArrayList arrayList = (ArrayList) this.table[key];
        if (arrayList == null)
          return false;
        return arrayList.Contains(value);
      }

      public ArrayList GetValues(object key)
      {
        return (ArrayList) this.table[key];
      }
    }
  }
}
