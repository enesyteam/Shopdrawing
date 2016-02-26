// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ColorEditor.ColorEditorModel
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.ValueEditors.ColorEditor
{
  internal class ColorEditorModel : INotifyPropertyChanged
  {
    private static ColorModel SharedLastColorModel = new ColorModel(Colors.Gray);
    private static ColorSpace sharedColorSpace = ColorSpace.Rgb;
    private static List<WeakReference> AllModels = new List<WeakReference>();
    private Color initialColor;
    private ColorModel colorModel;
    private ICommand beginEditCommand;
    private ICommand continueEditCommand;
    private ICommand endEditCommand;
    private ICommand cancelEditCommand;
    private ICommand makeWhiteCommand;
    private ICommand makeBlackCommand;
    private ICommand makeLastColorCommand;
    private ICommand makeInitialColorCommand;
    private ICommand useHlsColorSpaceCommand;
    private ICommand useHsbColorSpaceCommand;
    private ICommand useRgbColorSpaceCommand;
    private ICommand useCmykColorSpaceCommand;

    public static ColorSpace SharedColorSpace
    {
      get
      {
        return ColorEditorModel.sharedColorSpace;
      }
      set
      {
        if (ColorEditorModel.sharedColorSpace == value)
          return;
        ColorEditorModel.sharedColorSpace = value;
        ColorEditorModel.FireAllColorEditorModelChanged("SharedColorSpace");
        if (ColorEditorModel.SharedColorSpaceChanged == null)
          return;
        ColorEditorModel.SharedColorSpaceChanged((object) null, EventArgs.Empty);
      }
    }

    public ColorModel ColorModel
    {
      get
      {
        return this.colorModel;
      }
    }

    public ColorModel LastColorModel
    {
      get
      {
        return ColorEditorModel.SharedLastColorModel;
      }
    }

    public Color InitialColor
    {
      get
      {
        return this.initialColor;
      }
      set
      {
        this.initialColor = value;
        this.OnPropertyChanged("InitialColor");
      }
    }

    public ICommand BeginEditCommand
    {
      get
      {
        return this.beginEditCommand;
      }
      set
      {
        this.beginEditCommand = value;
      }
    }

    public ICommand ContinueEditCommand
    {
      get
      {
        return this.continueEditCommand;
      }
      set
      {
        this.continueEditCommand = value;
      }
    }

    public ICommand EndEditCommand
    {
      get
      {
        return this.endEditCommand;
      }
      set
      {
        this.endEditCommand = value;
      }
    }

    public ICommand CancelEditCommand
    {
      get
      {
        return this.cancelEditCommand;
      }
      set
      {
        this.cancelEditCommand = value;
      }
    }

    public ICommand MakeWhiteCommand
    {
      get
      {
        return this.makeWhiteCommand;
      }
    }

    public ICommand MakeBlackCommand
    {
      get
      {
        return this.makeBlackCommand;
      }
    }

    public ICommand MakeLastColorCommand
    {
      get
      {
        return this.makeLastColorCommand;
      }
    }

    public ICommand MakeInitialColorCommand
    {
      get
      {
        return this.makeInitialColorCommand;
      }
    }

    public ICommand UseHlsColorSpaceCommand
    {
      get
      {
        return this.useHlsColorSpaceCommand;
      }
    }

    public ICommand UseHsbColorSpaceCommand
    {
      get
      {
        return this.useHsbColorSpaceCommand;
      }
    }

    public ICommand UseRgbColorSpaceCommand
    {
      get
      {
        return this.useRgbColorSpaceCommand;
      }
    }

    public ICommand UseCmykColorSpaceCommand
    {
      get
      {
        return this.useCmykColorSpaceCommand;
      }
    }

    public static event EventHandler SharedColorSpaceChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    public ColorEditorModel(Color initialColor)
    {
      this.makeWhiteCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.MakeWhite));
      this.makeBlackCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.MakeBlack));
      this.makeLastColorCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.MakeLastColor));
      this.makeInitialColorCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.MakeInitialColor));
      this.useHlsColorSpaceCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.UseHlsColorSpace));
      this.useHsbColorSpaceCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.UseHsbColorSpace));
      this.useRgbColorSpaceCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.UseRgbColorSpace));
      this.useCmykColorSpaceCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.UseCmykColorSpace));
      this.colorModel = new ColorModel(initialColor);
      this.initialColor = initialColor;
      ColorEditorModel.AllModels.Add(new WeakReference((object) this));
    }

    internal void Associate()
    {
      this.colorModel.ColorChanged += new EventHandler(this.colorModel_ColorChanged);
      this.OnColorModelChanged();
    }

    internal void Disassociate()
    {
      this.colorModel.ColorChanged -= new EventHandler(this.colorModel_ColorChanged);
    }

    private void colorModel_ColorChanged(object sender, EventArgs e)
    {
      this.OnColorModelChanged();
    }

    protected virtual void OnColorModelChanged()
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ColorChanged);
      this.OnPropertyChanged("ColorModel");
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ColorChanged);
    }

    public void SetLastColorModel()
    {
      ColorEditorModel.SharedLastColorModel.CopyFrom(this.colorModel);
      ColorEditorModel.FireAllColorEditorModelChanged("LastColorModel");
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private void BeginUpdate()
    {
      if (this.BeginEditCommand == null)
        return;
      this.BeginEditCommand.Execute((object) null);
    }

    private void EndUpdate()
    {
      if (this.EndEditCommand == null)
        return;
      this.EndEditCommand.Execute((object) null);
    }

    private void MakeWhite()
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.EditColor);
      this.BeginUpdate();
      this.ColorModel.Color = Colors.White;
      this.EndUpdate();
    }

    private void MakeBlack()
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.EditColor);
      this.BeginUpdate();
      this.ColorModel.Color = Colors.Black;
      this.EndUpdate();
    }

    private void MakeLastColor()
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.EditColor);
      this.BeginUpdate();
      this.ColorModel.CopyFrom(this.LastColorModel);
      this.EndUpdate();
    }

    private void MakeInitialColor()
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.EditColor);
      this.BeginUpdate();
      this.ColorModel.Color = this.InitialColor;
      this.EndUpdate();
    }

    private void UseHlsColorSpace()
    {
      ColorEditorModel.SharedColorSpace = ColorSpace.Hls;
    }

    private void UseHsbColorSpace()
    {
      ColorEditorModel.SharedColorSpace = ColorSpace.Hsb;
    }

    private void UseRgbColorSpace()
    {
      ColorEditorModel.SharedColorSpace = ColorSpace.Rgb;
    }

    private void UseCmykColorSpace()
    {
      ColorEditorModel.SharedColorSpace = ColorSpace.Cmyk;
    }

    private static void FireAllColorEditorModelChanged(string propertyName)
    {
      for (int index = ColorEditorModel.AllModels.Count - 1; index >= 0; --index)
      {
        if (ColorEditorModel.AllModels[index].IsAlive)
          ((ColorEditorModel) ColorEditorModel.AllModels[index].Target).OnPropertyChanged(propertyName);
        else
          ColorEditorModel.AllModels.RemoveAt(index);
      }
    }
  }
}
