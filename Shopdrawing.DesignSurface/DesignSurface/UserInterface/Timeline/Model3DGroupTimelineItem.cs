// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.Model3DGroupTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class Model3DGroupTimelineItem : Item3DTimelineItem
  {
    private DelegateCommand createLightTimelineCommand;

    public Model3DGroupTimelineItem(TimelineItemManager timelineItemManager, Model3DGroupElement element)
      : base(timelineItemManager, (Base3DElement) element)
    {
      this.createLightTimelineCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CreateLightTimelineCommandBinding_Execute));
    }

    protected override void AddContextMenuItems(ItemsControl contextMenu)
    {
      base.AddContextMenuItems(contextMenu);
      MenuItem menuItem = new MenuItem();
      menuItem.Command = (ICommand) this.createLightTimelineCommand;
      menuItem.Header = (object) StringTable.CreateLightTimelineCommandName;
      menuItem.IsEnabled = true;
      contextMenu.Items.Add((object) menuItem);
    }

    private void CreateLightTimelineCommandBinding_Execute()
    {
      using (SceneEditTransaction editTransaction = this.Element.DesignerContext.ActiveDocument.CreateEditTransaction(StringTable.CreateLight))
      {
        AmbientLightElement ambientLightElement = (AmbientLightElement) this.Element.ViewModel.CreateSceneNode((object) new AmbientLight(Color.FromRgb((byte) 127, (byte) 127, (byte) 127)));
        ambientLightElement.Name = "Light";
        ((Model3DGroupElement) this.Element).Children.Add((Model3DElement) ambientLightElement);
        editTransaction.Commit();
      }
    }
  }
}
