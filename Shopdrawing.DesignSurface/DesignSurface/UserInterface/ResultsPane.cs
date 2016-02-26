// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResultsPane
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  internal sealed class ResultsPane : DockPanel, INotifyPropertyChanged
  {
    private DesignerContext designerContext;
    private PaletteRegistryEntry palette;
    private ResultsView view;
    private ResultsPane.ResultsErrorManager errorManager;
    private ResultsPane.ResultsMessageLogger messageLogger;

    public IProjectManager ProjectManager
    {
      get
      {
        return this.designerContext.ProjectManager;
      }
    }

    public IViewService ViewService
    {
      get
      {
        return this.designerContext.ViewService;
      }
    }

    public IErrorService ErrorManager
    {
      get
      {
        return (IErrorService) this.errorManager;
      }
    }

    public IMessageLoggingService MessageLoggingService
    {
      get
      {
        return (IMessageLoggingService) this.messageLogger;
      }
    }

    public ResultsView View
    {
      get
      {
        return this.view;
      }
      set
      {
        if (this.view == value)
          return;
        this.view = value;
        this.OnPropertyChanged("View");
      }
    }

    public PaletteRegistryEntry Palette
    {
      get
      {
        return this.palette;
      }
      set
      {
        this.palette = value;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ResultsPane(DesignerContext designerContext, IErrorTaskCollection errors)
    {
      this.designerContext = designerContext;
      this.view = ResultsView.Console;
      this.messageLogger = new ResultsPane.ResultsMessageLogger();
      this.errorManager = new ResultsPane.ResultsErrorManager(this, errors);
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      this.ApplyTemplate();
    }

    public new bool ApplyTemplate()
    {
      if (this.Children.Count == 0)
      {
        FrameworkElement element = Microsoft.Expression.DesignSurface.FileTable.GetElement("Resources\\ResultsPane.xaml");
        element.DataContext = (object) this;
        this.Children.Add((UIElement) element);
        this.messageLogger.SetImplementation((IMessageLoggingService) new ResultsPane.ConsoleMessageLogger(this, (TextBox) LogicalTreeHelper.FindLogicalNode((DependencyObject) element, "OutputTextBox")));
      }
      return base.ApplyTemplate();
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private sealed class ConsoleMessageLogger : IMessageLoggingService
    {
      private ResultsPane resultsPane;
      private TextBox textBox;
      private bool logged;

      public ConsoleMessageLogger(ResultsPane resultsPane, TextBox textBox)
      {
        this.resultsPane = resultsPane;
        this.textBox = textBox;
      }

      public void Clear()
      {
        this.textBox.Text = string.Empty;
        this.logged = false;
      }

      public void Write(string text)
      {
        bool flag = false;
        if (this.textBox.SelectionStart == this.textBox.Text.Length)
          flag = true;
        this.textBox.AppendText(text);
        if (flag)
        {
          this.textBox.Select(this.textBox.Text.Length, 0);
          this.textBox.ScrollToEnd();
        }
        if (this.logged)
          return;
        this.logged = true;
        this.resultsPane.View = ResultsView.Console;
      }

      public void WriteLine(string value)
      {
        this.Write(value + Environment.NewLine);
      }
    }

    private sealed class ResultsMessageLogger : IMessageLoggingService
    {
      private IMessageLoggingService logger;

      public void SetImplementation(IMessageLoggingService logger)
      {
        this.logger = logger;
      }

      public void Clear()
      {
        if (this.logger == null)
          return;
        this.logger.Clear();
      }

      public void Write(string text)
      {
        if (this.logger == null)
          return;
        this.logger.Write(text);
      }

      public void WriteLine(string text)
      {
        if (this.logger == null)
          return;
        this.logger.WriteLine(text);
      }
    }

    private sealed class ResultsErrorManager : IErrorService
    {
      private ResultsPane resultsPane;
      private IErrorTaskCollection errors;

      public IErrorTaskCollection Errors
      {
        get
        {
          return this.errors;
        }
      }

      public ResultsErrorManager(ResultsPane resultsPane, IErrorTaskCollection errors)
      {
        this.resultsPane = resultsPane;
        this.errors = errors;
      }

      void IErrorService.DisplayErrors()
      {
        if (!this.resultsPane.Palette.IsVisible)
          this.resultsPane.Palette.IsVisibleAndSelected = true;
        this.resultsPane.View = ResultsView.Errors;
      }
    }
  }
}
