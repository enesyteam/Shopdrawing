// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.SetterModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  public class SetterModel : INotifyPropertyChanged
  {
    private SetterSceneNode setter;
    private DelegateCommand deleteCommand;
    private DelegateCommand selectTargetCommand;

    public string Property
    {
      get
      {
        return this.setter.Property.Name;
      }
    }

    public SetterSceneNode Setter
    {
      get
      {
        return this.setter;
      }
    }

    public ICommand DeleteCommand
    {
      get
      {
        return (ICommand) this.deleteCommand;
      }
    }

    public ICommand SelectTargetCommand
    {
      get
      {
        return (ICommand) this.selectTargetCommand;
      }
    }

    public object Value
    {
      get
      {
        return this.setter.Value;
      }
    }

    public object UserFriendlyValue
    {
      get
      {
        DocumentCompositeNode documentCompositeNode = this.setter.DocumentNode as DocumentCompositeNode;
        if (documentCompositeNode != null)
        {
          DocumentCompositeNode expressionNode = documentCompositeNode.Properties[SetterSceneNode.ValueProperty] as DocumentCompositeNode;
          if (expressionNode != null && DocumentNodeUtilities.IsMarkupExtension((DocumentNode) expressionNode))
            return (object) XamlExpressionSerializer.GetUserFriendlyDescription(expressionNode, (DocumentNode) documentCompositeNode);
        }
        return this.setter.Value;
      }
    }

    public object Tooltip
    {
      get
      {
        return (object) (string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PropertyTriggerSetterTooltip, new object[1]
        {
          (object) this.TargetDisplayName
        }) + Environment.NewLine + StringTable.PropertyTriggerSetterHint);
      }
    }

    public SceneNode Target
    {
      get
      {
        return this.setter.TargetNode;
      }
    }

    public string TargetDisplayName
    {
      get
      {
        return SceneNodeToStringConverter.ConvertToString(this.setter.TargetNode);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public SetterModel(SetterSceneNode setter)
    {
      this.setter = setter;
      this.deleteCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.Delete));
      this.selectTargetCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.SelectTarget));
    }

    public void Initialize()
    {
    }

    public void Delete()
    {
      if (!this.setter.IsAttached)
        return;
      using (SceneEditTransaction editTransaction = this.setter.ViewModel.CreateEditTransaction(StringTable.TriggerChangeUndoUnit))
      {
        this.setter.Remove();
        editTransaction.Commit();
      }
    }

    public void SelectTarget()
    {
      SceneElement selectionToSet = this.Target as SceneElement;
      if (selectionToSet == null || !selectionToSet.IsSelectable)
        return;
      this.setter.ViewModel.ElementSelectionSet.SetSelection(selectionToSet);
    }

    public void Update()
    {
      this.OnPropertyChanged("Property");
      this.OnPropertyChanged("Value");
      this.OnPropertyChanged("UserFriendlyValue");
      this.OnPropertyChanged("Target");
      this.OnPropertyChanged("TargetDisplayName");
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
