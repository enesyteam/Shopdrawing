// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.ModelBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  public class ModelBase : INotifyPropertyChanged
  {
    internal static Dictionary<string, List<string>> stateAndStateGroupsPredefinedOrder = new Dictionary<string, List<string>>()
    {
      {
        "CommonStates",
        new List<string>()
        {
          "Normal",
          "MouseOver",
          "Pressed",
          "Disabled"
        }
      },
      {
        "CheckStates",
        new List<string>()
        {
          "Unchecked",
          "Checked"
        }
      },
      {
        "FocusStates",
        new List<string>()
        {
          "Unfocused",
          "Focused"
        }
      }
    };
    private ModelBase parentModel;

    public ModelBase ParentModel
    {
      get
      {
        return this.parentModel;
      }
      set
      {
        this.parentModel = value;
      }
    }

    public virtual string Name
    {
      get
      {
        return (string) null;
      }
      set
      {
      }
    }

    public bool IsSelectedWithin { get; private set; }

    protected virtual bool GetDirectChildrenOrSelfSelected
    {
      get
      {
        return false;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    internal static int GetInsertionIndex<T>(T item, Collection<T> list, List<string> predefinedNames, SceneNode container) where T : ModelBase
    {
      int index = 0;
      if (predefinedNames.Contains(item.Name))
      {
        foreach (string str in predefinedNames)
        {
          if (!(str == item.Name))
          {
            if (index < list.Count && str == list[index].Name)
              ++index;
          }
          else
            break;
        }
      }
      else if (ModelBase.GetSceneNode((ModelBase) item).ShouldSerialize)
      {
        List<string> list1 = new List<string>();
        foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) container.GetCollectionForChild(ModelBase.GetSceneNode((ModelBase) item)))
        {
          if (sceneNode == ModelBase.GetSceneNode((ModelBase) item))
          {
            foreach (T obj in list)
            {
              if (!predefinedNames.Contains(obj.Name) && ModelBase.GetSceneNode((ModelBase) obj).ShouldSerialize)
              {
                if (!list1.Contains(obj.Name))
                  break;
              }
              ++index;
            }
          }
          else
            list1.Add(sceneNode.Name);
        }
      }
      else
      {
        foreach (T obj in list)
        {
          if (!predefinedNames.Contains(obj.Name))
          {
            if (!ModelBase.GetSceneNode((ModelBase) obj).ShouldSerialize)
            {
              if (string.Compare(obj.Name, item.Name, StringComparison.Ordinal) >= 0)
                break;
            }
            else
              break;
          }
          ++index;
        }
      }
      return index;
    }

    private static SceneNode GetSceneNode(ModelBase item)
    {
      StateModel stateModel = item as StateModel;
      if (stateModel != null)
        return (SceneNode) stateModel.SceneNode;
      StateGroupModel stateGroupModel;
      if ((stateGroupModel = item as StateGroupModel) != null)
        return (SceneNode) stateGroupModel.SceneNode;
      return (SceneNode) null;
    }

    protected void NotifyPropertyChanged(string name)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(name));
    }

    protected void UpdateSelectedWithin()
    {
      bool childrenOrSelfSelected = this.GetDirectChildrenOrSelfSelected;
      if (this.IsSelectedWithin == childrenOrSelfSelected)
        return;
      this.IsSelectedWithin = childrenOrSelfSelected;
      this.NotifyPropertyChanged("IsSelectedWithin");
      if (this.ParentModel == null)
        return;
      this.ParentModel.UpdateSelectedWithin();
    }
  }
}
