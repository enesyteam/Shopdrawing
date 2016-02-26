// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.UserInterface.CodeOptionsControl
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.Code.UserInterface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class CodeOptionsControl : StackPanel, IComponentConnector
  {
    private static readonly SerialAsyncProcess BackgroundFontBuilder = new SerialAsyncProcess((IAsyncMechanism) new CurrentDispatcherAsyncMechanism(DispatcherPriority.ApplicationIdle));
    private static string defaultPreviewFontFamilyName;
    private DelegateCommand.SimpleEventHandler resetToDefault;
    private ICollectionView fontFamiliesView;
    private ObservableCollection<FontFamilyItem> fontFamilies;
    private CodeOptionsModel codeOptionsModel;
    private static IEnumerator<FontFamilyItem> fontFamilyEnumerator;
    internal CodeOptionsControl codeOptionsControl;
    internal ComboBox CurrentSettings;
    internal ChoiceEditor FontFamily;
    internal ChoiceEditor FontSize;
    internal NumberEditor TabSize;
    internal WorkaroundRadioButton InsertSpacesCheckBox;
    internal WorkaroundRadioButton KeepTabsCheckBox;
    private bool _contentLoaded;

    public IEnumerable FontSizes
    {
      get
      {
        for (int i = 6; i <= 24; ++i)
          yield return (object) (double) i;
      }
    }

    public ICommand ResetToDefaultCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(this.resetToDefault);
      }
    }

    public ICollectionView FontFamilies
    {
      get
      {
        if (this.fontFamiliesView == null)
        {
          this.fontFamilies = new ObservableCollection<FontFamilyItem>();
          this.fontFamiliesView = CollectionViewSource.GetDefaultView((object) this.fontFamilies);
          this.fontFamiliesView.SortDescriptions.Add(new SortDescription("FamilyName", ListSortDirection.Ascending));
          this.BeginBackgroundBuildFontList();
        }
        return this.fontFamiliesView;
      }
    }

    static CodeOptionsControl()
    {
      CodeOptionsControl.BackgroundFontBuilder.Complete += new EventHandler(CodeOptionsControl.BackgroundFontBuilder_AllDone);
      CodeOptionsControl.BackgroundFontBuilder.Killed += new EventHandler(CodeOptionsControl.BackgroundFontBuilder_AllDone);
    }

    public CodeOptionsControl(CodeOptionsModel codeOptionsModel, DelegateCommand.SimpleEventHandler resetToDefault)
    {
      CodeOptionsControl.defaultPreviewFontFamilyName = ((System.Windows.Media.FontFamily) this.FindResource((object) SystemFonts.MessageFontFamilyKey)).ToString();
      this.resetToDefault = resetToDefault;
      this.codeOptionsModel = codeOptionsModel;
      this.DataContext = (object) this.codeOptionsModel;
      this.InitializeComponent();
    }

    private static void BackgroundFontBuilder_AllDone(object sender, EventArgs e)
    {
      CodeOptionsControl.BackgroundFontBuilder.Clear();
    }

    private void BeginBackgroundBuildFontList()
    {
      CodeOptionsControl.BackgroundFontBuilder.Add((AsyncProcess) new DelegateAsyncProcess(new Action<object, DoWorkEventArgs>(this.DoBuildFontFamilies)));
      if (CodeOptionsControl.BackgroundFontBuilder.IsAlive)
        return;
      CodeOptionsControl.BackgroundFontBuilder.Begin();
    }

    private void DoBuildFontFamilies(object sender, DoWorkEventArgs e)
    {
      if (CodeOptionsControl.fontFamilyEnumerator == null)
        CodeOptionsControl.fontFamilyEnumerator = CodeOptionsControl.GetFontFamilies().GetEnumerator();
      if (CodeOptionsControl.fontFamilyEnumerator.MoveNext())
      {
        this.fontFamilies.Add(CodeOptionsControl.fontFamilyEnumerator.Current);
        e.Result = (object) AsyncProcessResult.StillGoing;
      }
      else
        CodeOptionsControl.fontFamilyEnumerator = (IEnumerator<FontFamilyItem>) null;
    }

    private static IEnumerable<FontFamilyItem> GetFontFamilies()
    {
      foreach (System.Windows.Media.FontFamily fontFamily in (IEnumerable<System.Windows.Media.FontFamily>) Fonts.SystemFontFamilies)
      {
        FontFamilyItem fontFamilyItem = CodeOptionsControl.CreateFontFamilyItem(fontFamily);
        if (fontFamilyItem.IsFontReadable)
          yield return fontFamilyItem;
      }
    }

    internal static FontFamilyItem CreateFontFamilyItem(System.Windows.Media.FontFamily fontFamily)
    {
      XmlLanguage language1 = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
      XmlLanguage language2 = XmlLanguage.GetLanguage("en-US");
      string source;
      try
      {
        if (!fontFamily.FamilyNames.TryGetValue(language1, out source))
        {
          if (!fontFamily.FamilyNames.TryGetValue(language2, out source))
            source = fontFamily.Source;
        }
      }
      catch (ArgumentException ex)
      {
        source = fontFamily.Source;
      }
      return new FontFamilyItem(FontFamilyHelper.EnsureFamilyName(source), string.Empty, CodeOptionsControl.defaultPreviewFontFamilyName, fontFamily);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Code;component/userinterface/codeoptionscontrol.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.codeOptionsControl = (CodeOptionsControl) target;
          break;
        case 2:
          this.CurrentSettings = (ComboBox) target;
          break;
        case 3:
          this.FontFamily = (ChoiceEditor) target;
          break;
        case 4:
          this.FontSize = (ChoiceEditor) target;
          break;
        case 5:
          this.TabSize = (NumberEditor) target;
          break;
        case 6:
          this.InsertSpacesCheckBox = (WorkaroundRadioButton) target;
          break;
        case 7:
          this.KeepTabsCheckBox = (WorkaroundRadioButton) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
