// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.PotentialTransitionModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  public class PotentialTransitionModel : ModelBase
  {
    private StateModel fromModel;
    private StateModel toModel;
    private ICommand addCommand;

    public string FromStateName
    {
      get
      {
        if (this.fromModel == null)
          return (string) null;
        return this.fromModel.Name;
      }
    }

    public string ToStateName
    {
      get
      {
        if (this.toModel == null)
          return (string) null;
        return this.toModel.Name;
      }
    }

    public bool BalanceTransitionColumns
    {
      get
      {
        return true;
      }
    }

    public ICommand AddCommand
    {
      get
      {
        if (this.addCommand == null)
          this.addCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.Add));
        return this.addCommand;
      }
    }

    public PotentialTransitionModel(StateModel fromModel, StateModel toModel)
    {
      this.fromModel = fromModel;
      this.toModel = toModel;
    }

    public void Add()
    {
      VisualStateGroupSceneNode stateGroupSceneNode = this.toModel != null ? this.toModel.GroupNode : this.fromModel.GroupNode;
      using (SceneEditTransaction editTransaction = stateGroupSceneNode.ViewModel.CreateEditTransaction(StringTable.AddNewTransitionUndoUnit))
      {
        VisualStateSceneNode fromNode = this.fromModel != null ? this.fromModel.SceneNode : (VisualStateSceneNode) null;
        VisualStateSceneNode toNode = this.toModel != null ? this.toModel.SceneNode : (VisualStateSceneNode) null;
        stateGroupSceneNode.AddTransition(fromNode, toNode, stateGroupSceneNode.DefaultTransitionDuration);
        editTransaction.Commit();
      }
    }
  }
}
