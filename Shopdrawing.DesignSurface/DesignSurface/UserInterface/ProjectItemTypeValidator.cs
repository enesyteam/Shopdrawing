// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ProjectItemTypeValidator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project.UserInterface;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public sealed class ProjectItemTypeValidator : ProjectValidatorBase, IMessageBubbleValidator
  {
    private ListBox listBox;
    private IMessageBubbleHelper helper;

    public void Initialize(UIElement targetElement, IMessageBubbleHelper helper)
    {
      this.listBox = (ListBox) targetElement;
      this.listBox.SelectionChanged += new SelectionChangedEventHandler(this.ProjectItemTypeValidator_SelectionChanged);
      this.helper = helper;
    }

    private void ProjectItemTypeValidator_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      MessageBubbleContent content = (MessageBubbleContent) null;
      if (this.listBox.SelectedItem == null)
        content = new MessageBubbleContent(StringTable.CreateProjectItemDialogMissingProjectType, MessageBubbleType.Error);
      this.helper.SetContent(content);
    }
  }
}
