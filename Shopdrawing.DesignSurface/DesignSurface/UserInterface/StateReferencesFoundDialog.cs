// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.StateReferencesFoundDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Controls;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  internal sealed class StateReferencesFoundDialog : Dialog
  {
    public StateReferencesFoundDialog(StateReferencesFoundModel stateReferencesFoundModel)
    {
      FrameworkElement element = FileTable.GetElement("UserInterface\\DocumentProcessors\\StateReferencesFoundDialog.xaml");
      element.DataContext = (object) stateReferencesFoundModel;
      this.DialogContent = (UIElement) element;
      this.Title = StringTable.ReferencesFoundDialogTitle;
      this.SizeToContent = SizeToContent.WidthAndHeight;
    }
  }
}
