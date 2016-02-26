// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.CompoundKeyFrameItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class CompoundKeyFrameItem : KeyFrameItem
  {
    private List<KeyFrameItem> childKeyFrameItems;
    private bool changingChildren;

    public ReadOnlyCollection<KeyFrameItem> ChildKeyFrameItems
    {
      get
      {
        return new ReadOnlyCollection<KeyFrameItem>((IList<KeyFrameItem>) this.childKeyFrameItems);
      }
    }

    public override bool IsSelected
    {
      get
      {
        bool flag = true;
        foreach (KeyFrameItem keyFrameItem in this.childKeyFrameItems)
        {
          if (!keyFrameItem.IsSelected)
          {
            flag = false;
            break;
          }
        }
        return flag;
      }
      set
      {
        foreach (KeyFrameItem keyFrameItem in this.childKeyFrameItems)
          keyFrameItem.IsSelected = value;
        this.OnPropertyChanged("IsSelected");
      }
    }

    public override IList<KeyFrameSceneNode> KeyFrameSceneNodesToSelect
    {
      get
      {
        List<KeyFrameSceneNode> list = new List<KeyFrameSceneNode>();
        foreach (KeyFrameItem keyFrameItem in this.childKeyFrameItems)
        {
          CompoundKeyFrameItem compoundKeyFrameItem = keyFrameItem as CompoundKeyFrameItem;
          if ((KeyFrameItem) compoundKeyFrameItem != (KeyFrameItem) null)
            list.AddRange((IEnumerable<KeyFrameSceneNode>) compoundKeyFrameItem.KeyFrameSceneNodesToSelect);
          else
            list.Add(((SimpleKeyFrameItem) keyFrameItem).KeyFrameSceneNode);
        }
        return (IList<KeyFrameSceneNode>) list;
      }
    }

    protected override SimpleKeyFrameItem KeyFrameItemToEdit
    {
      get
      {
        CompoundKeyFrameItem compoundKeyFrameItem = this.childKeyFrameItems[0] as CompoundKeyFrameItem;
        if ((KeyFrameItem) compoundKeyFrameItem != (KeyFrameItem) null)
          return compoundKeyFrameItem.KeyFrameItemToEdit;
        return (SimpleKeyFrameItem) this.childKeyFrameItems[0];
      }
    }

    public event EventHandler ChildKeyFrameTimeChanged;

    public CompoundKeyFrameItem(double time, TimelineItem timelineItem)
      : base(time, timelineItem)
    {
      this.childKeyFrameItems = new List<KeyFrameItem>();
      this.PropertyChanged += new PropertyChangedEventHandler(this.CompoundKeyFrameItem_PropertyChanged);
    }

    public void AddKeyFrameItem(KeyFrameItem keyFrameItem)
    {
      keyFrameItem.PropertyChanged += new PropertyChangedEventHandler(this.ChildKeyFrameItem_PropertyChanged);
      this.childKeyFrameItems.Add(keyFrameItem);
    }

    public void UpdateTimeFromChildren()
    {
      double time = this.childKeyFrameItems[0].Time;
      foreach (KeyFrameItem keyFrameItem in this.childKeyFrameItems)
      {
        if (keyFrameItem.Time != time)
          return;
      }
      if (time == this.Time)
        return;
      this.Time = time;
    }

    private void CompoundKeyFrameItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Time"))
        return;
      this.changingChildren = true;
      foreach (KeyFrameItem keyFrameItem in this.childKeyFrameItems)
        keyFrameItem.Time = this.Time;
      this.changingChildren = false;
    }

    private void OnChildKeyFrameTimeChanged()
    {
      if (this.ChildKeyFrameTimeChanged == null)
        return;
      this.ChildKeyFrameTimeChanged((object) this, EventArgs.Empty);
    }

    private void ChildKeyFrameItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Time" && !this.changingChildren)
      {
        this.OnChildKeyFrameTimeChanged();
      }
      else
      {
        if (!(e.PropertyName == "IsSelected"))
          return;
        this.OnPropertyChanged("IsSelected");
      }
    }
  }
}
