// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourceConflictResolutionDialog
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
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class ResourceConflictResolutionDialog : Dialog, INotifyPropertyChanged, IComponentConnector
  {
    private ResourceConflictResolution resolution = ResourceConflictResolution.RenameNew;
    internal Grid DocumentRoot;
    internal TextBlock tbExplanation;
    internal RadioButton radioAdd;
    internal TextBlock tbAdd;
    internal RadioButton radioOverwrite;
    internal TextBlock tbOverwrite;
    internal RadioButton radioDiscard;
    internal TextBlock tbDiscard;
    internal Button AcceptButton;
    internal Button CancelButton;
    private bool _contentLoaded;

    public ResourceConflictResolution Resolution
    {
      get
      {
        return this.resolution;
      }
      set
      {
        this.resolution = value;
        this.OnPropertyChanged("Resolution");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ResourceConflictResolutionDialog(ResourceConflictResolution itemsToDisplay)
    {
      this.InitializeComponent();
      this.tbDiscard.IsEnabled = (itemsToDisplay & ResourceConflictResolution.UseExisting) == ResourceConflictResolution.UseExisting;
      this.radioDiscard.IsEnabled = (itemsToDisplay & ResourceConflictResolution.UseExisting) == ResourceConflictResolution.UseExisting;
      this.tbOverwrite.IsEnabled = (itemsToDisplay & ResourceConflictResolution.OverwriteOld) == ResourceConflictResolution.OverwriteOld;
      this.radioOverwrite.IsEnabled = (itemsToDisplay & ResourceConflictResolution.OverwriteOld) == ResourceConflictResolution.OverwriteOld;
      this.tbAdd.IsEnabled = (itemsToDisplay & ResourceConflictResolution.RenameNew) == ResourceConflictResolution.RenameNew;
      this.radioAdd.IsEnabled = (itemsToDisplay & ResourceConflictResolution.RenameNew) == ResourceConflictResolution.RenameNew;
      this.Title = StringTable.ResourceConflictResolutionDialogTitle;
    }

    private void HandleUseExisting(object sender, EventArgs e)
    {
      this.Resolution = ResourceConflictResolution.UseExisting;
    }

    private void HandleRenameNew(object sender, EventArgs e)
    {
      this.Resolution = ResourceConflictResolution.RenameNew;
    }

    private void HandleOverwriteOld(object sender, EventArgs e)
    {
      this.Resolution = ResourceConflictResolution.OverwriteOld;
    }

    public void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/resources/resourceconflictresolutiondialog.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.DocumentRoot = (Grid) target;
          break;
        case 2:
          this.tbExplanation = (TextBlock) target;
          break;
        case 3:
          this.radioAdd = (RadioButton) target;
          this.radioAdd.Click += new RoutedEventHandler(this.HandleRenameNew);
          break;
        case 4:
          this.tbAdd = (TextBlock) target;
          break;
        case 5:
          this.radioOverwrite = (RadioButton) target;
          this.radioOverwrite.Click += new RoutedEventHandler(this.HandleOverwriteOld);
          break;
        case 6:
          this.tbOverwrite = (TextBlock) target;
          break;
        case 7:
          this.radioDiscard = (RadioButton) target;
          this.radioDiscard.Click += new RoutedEventHandler(this.HandleUseExisting);
          break;
        case 8:
          this.tbDiscard = (TextBlock) target;
          break;
        case 9:
          this.AcceptButton = (Button) target;
          break;
        case 10:
          this.CancelButton = (Button) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
