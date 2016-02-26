// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.EditDesignTimeResourcesDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Controls;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class EditDesignTimeResourcesDialog : Dialog, IComponentConnector
  {
    private EditDesignTimeResourceModel model;
    internal EditDesignTimeResourcesDialog UserControlSelf;
    internal Icon warningIcon;
    internal ComboBox DefineIn_WhichDocument;
    internal CheckBox doNotAskCheck;
    internal UniformGrid buttonGrid;
    internal Button AcceptButton;
    internal Button CancelButton;
    private bool _contentLoaded;

    public EditDesignTimeResourcesDialog()
    {
      this.InitializeComponent();
    }

    internal EditDesignTimeResourcesDialog(EditDesignTimeResourceModel model)
      : this()
    {
      if (model == null)
        throw new ArgumentNullException("model");
      this.model = model;
      this.DataContext = (object) model;
      this.Title = StringTable.DesignTimeResourcesAddDictionary;
      if (model.Mode != EditDesignTimeResourceModelMode.Manual)
        return;
      this.buttonGrid.HorizontalAlignment = HorizontalAlignment.Right;
      this.warningIcon.Visibility = Visibility.Collapsed;
      this.doNotAskCheck.Visibility = Visibility.Collapsed;
    }

    protected override void OnAcceptButtonExecute()
    {
      this.Close(new bool?(this.model.TryCreateDesignTimeResourceReference()));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/resources/resourcepane/editdesigntimeresourcesdialog.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.UserControlSelf = (EditDesignTimeResourcesDialog) target;
          break;
        case 2:
          this.warningIcon = (Icon) target;
          break;
        case 3:
          this.DefineIn_WhichDocument = (ComboBox) target;
          break;
        case 4:
          this.doNotAskCheck = (CheckBox) target;
          break;
        case 5:
          this.buttonGrid = (UniformGrid) target;
          break;
        case 6:
          this.AcceptButton = (Button) target;
          break;
        case 7:
          this.CancelButton = (Button) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
