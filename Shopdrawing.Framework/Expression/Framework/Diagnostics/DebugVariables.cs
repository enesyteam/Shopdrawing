// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.DebugVariables
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.ComponentModel;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public class DebugVariables : INotifyPropertyChanged
  {
    private static DebugVariables instance = new DebugVariables();
    private bool makeSelectedElementActive;
    private bool enableRealTimeFitting;
    private bool exceptionHandlerEnabled;
    private bool feedbackServiceDisplayMessageBoxesEnabled;

    public static DebugVariables Instance
    {
      get
      {
        if (DebugVariables.instance == null)
          DebugVariables.instance = new DebugVariables();
        return DebugVariables.instance;
      }
    }

    public bool MakeSelectedElementActive
    {
      get
      {
        return this.makeSelectedElementActive;
      }
      set
      {
        this.makeSelectedElementActive = value;
        this.OnPropertyChanged(new PropertyChangedEventArgs("MakeSelectedElementActive"));
      }
    }

    public bool EnableRealTimeFitting
    {
      get
      {
        return this.enableRealTimeFitting;
      }
      set
      {
        this.enableRealTimeFitting = value;
        this.OnPropertyChanged(new PropertyChangedEventArgs("EnableRealTimeFitting"));
      }
    }

    public bool ExceptionHandlerEnabled
    {
      get
      {
        return this.exceptionHandlerEnabled;
      }
      set
      {
        this.exceptionHandlerEnabled = value;
        this.OnPropertyChanged(new PropertyChangedEventArgs("ExceptionHandlerEnabled"));
      }
    }

    public bool FeedbackServiceDisplayMessageBoxesEnabled
    {
      get
      {
        return this.feedbackServiceDisplayMessageBoxesEnabled;
      }
      set
      {
        this.feedbackServiceDisplayMessageBoxesEnabled = value;
        this.OnPropertyChanged(new PropertyChangedEventArgs("FeedbackServiceDisplayMessageBoxesEnabled"));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    internal DebugVariables()
    {
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, e);
    }
  }
}
