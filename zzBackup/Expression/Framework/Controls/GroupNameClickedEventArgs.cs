// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.GroupNameClickedEventArgs
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;

namespace Microsoft.Expression.Framework.Controls
{
  public class GroupNameClickedEventArgs : RoutedEventArgs
  {
    private string groupName;

    public string GroupName
    {
      get
      {
        return this.groupName;
      }
      set
      {
        this.groupName = value;
      }
    }

    public GroupNameClickedEventArgs(string groupName, Gallery gallery)
      : base(Gallery.GroupNameClickedEvent, (object) gallery)
    {
      this.groupName = groupName;
    }
  }
}
