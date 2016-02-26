// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.TriggerActionModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System.ComponentModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  public class TriggerActionModel : INotifyPropertyChanged
  {
    private TriggerActionNode action;
    private DelegateCommand deleteCommand;

    public TriggerActionNode SceneNode
    {
      get
      {
        return this.action;
      }
      protected set
      {
        this.action = value;
      }
    }

    public string Description
    {
      get
      {
        return this.SceneNode.Type.Name;
      }
    }

    public ICommand DeleteCommand
    {
      get
      {
        return (ICommand) this.deleteCommand;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public TriggerActionModel(TriggerActionNode action)
    {
      this.action = action;
      this.deleteCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.Delete));
    }

    public virtual void Update()
    {
    }

    internal static TriggerActionModel ConstructModel(TriggerActionNode triggerActionNode)
    {
      TimelineActionNode action = triggerActionNode as TimelineActionNode;
      if (action != null)
        return (TriggerActionModel) new TimelineActionModel(action);
      if (triggerActionNode != null)
        return new TriggerActionModel(triggerActionNode);
      return (TriggerActionModel) null;
    }

    protected virtual void Delete()
    {
      using (SceneEditTransaction editTransaction = this.SceneNode.ViewModel.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
      {
        this.SceneNode.Remove();
        editTransaction.Commit();
      }
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
