// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.StoryboardOption
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  public class StoryboardOption : INotifyPropertyChanged
  {
    private StoryboardTimelineSceneNode storyboard;

    public StoryboardTimelineSceneNode Storyboard
    {
      get
      {
        return this.storyboard;
      }
    }

    public string Name
    {
      get
      {
        if (this.storyboard != null)
          return this.storyboard.Name;
        return StringTable.TriggerNewTimeline;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public StoryboardOption(StoryboardTimelineSceneNode storyboard)
    {
      this.storyboard = storyboard;
    }

    public override bool Equals(object obj)
    {
      StoryboardOption storyboardOption = obj as StoryboardOption;
      if (storyboardOption != null)
        return this.storyboard == storyboardOption.storyboard;
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      if (this.storyboard != null)
        return this.storyboard.GetHashCode();
      return base.GetHashCode();
    }

    public void Update()
    {
      this.OnPropertyChanged("Name");
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
