// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.UserInterface.FindInFilesDialog
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.Documents;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Documents;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Microsoft.Expression.Code.UserInterface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class FindInFilesDialog : Dialog, IComponentConnector
  {
    private static readonly string heightConfigPropertyName = "Height";
    private static readonly string widthConfigPropertyName = "Width";
    private static readonly string lookInConfigPropertyName = "LookIn";
    private static readonly string fileTypesConfigPropertyName = "FileTypes";
    private static readonly string matchCaseConfigPropertyName = "MatchCase";
    private static readonly string[] allExtensions = new string[6]
    {
      ".CS",
      ".VB",
      ".XAML",
      ".HTML",
      ".JS",
      ".XML"
    };
    private static readonly string[] xamlExtensions = new string[1]
    {
      ".XAML"
    };
    private static readonly string[] codeExtensions = new string[2]
    {
      ".CS",
      ".VB"
    };
    private ObservableCollection<string> lookInStrings = new ObservableCollection<string>();
    private ObservableCollection<string> fileTypes = new ObservableCollection<string>();
    private ObservableCollection<FindInFilesResult> searchResults = new ObservableCollection<FindInFilesResult>();
    private static FindInFilesDialog findInFilesDialog;
    private IServices services;
    private IProjectManager projectManager;
    private IViewService viewService;
    private IConfigurationObject configuration;
    private StringComparison comparison;
    private string findString;
    private bool matchCase;
    private int lookInSelectionIndex;
    private int fileTypesSelectionIndex;
    internal Grid LayoutRoot;
    internal TextBox FindText;
    internal Button CancelButton;
    private bool _contentLoaded;

    public string FindString
    {
      get
      {
        return this.findString;
      }
      set
      {
        this.findString = value;
      }
    }

    public bool MatchCase
    {
      get
      {
        return this.matchCase;
      }
      set
      {
        this.matchCase = value;
        this.comparison = this.MatchCase ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
      }
    }

    public ObservableCollection<string> LookInStrings
    {
      get
      {
        return this.lookInStrings;
      }
    }

    public int LookInSelectionIndex
    {
      get
      {
        return this.lookInSelectionIndex;
      }
      set
      {
        this.lookInSelectionIndex = value;
      }
    }

    public ObservableCollection<string> FileTypes
    {
      get
      {
        return this.fileTypes;
      }
    }

    public ObservableCollection<FindInFilesResult> SearchResults
    {
      get
      {
        return this.searchResults;
      }
    }

    public int FileTypesSelectionIndex
    {
      get
      {
        return this.fileTypesSelectionIndex;
      }
      set
      {
        this.fileTypesSelectionIndex = value;
      }
    }

    public FindInFilesDialog(IServices services)
    {
      this.services = services;
      this.projectManager = this.services.GetService<IProjectManager>();
      this.configuration = this.services.GetService<IConfigurationService>()["FindInFiles"];
      this.viewService = this.services.GetService<IViewService>();
      this.InitializeDialog();
      this.projectManager.SolutionClosed += new EventHandler<SolutionEventArgs>(this.projectManager_SolutionClosed);
    }

    public static FindInFilesDialog GetFindInFilesDialog(IServices services)
    {
      if (FindInFilesDialog.findInFilesDialog == null)
        FindInFilesDialog.findInFilesDialog = new FindInFilesDialog(services);
      return FindInFilesDialog.findInFilesDialog;
    }

    public static void CloseIfOpen()
    {
      if (FindInFilesDialog.findInFilesDialog == null)
        return;
      FindInFilesDialog.findInFilesDialog.Close();
    }

    public new void Show()
    {
      base.Show();
      this.Activate();
      UIThreadDispatcher.Instance.Invoke(DispatcherPriority.Background, (Action) (() =>
      {
        TextBox textBox = (TextBox) LogicalTreeHelper.FindLogicalNode((DependencyObject) this, "FindText");
        textBox.Focus();
        textBox.SelectAll();
      }));
    }

    private void InitializeDialog()
    {
      this.InitializeComponent();
      this.Title = StringTable.FindInFilesCommandName;
      this.ResizeMode = ResizeMode.CanResizeWithGrip;
      this.MinHeight = 400.0;
      this.MinWidth = 610.0;
      this.Height = (double) this.configuration.GetProperty(FindInFilesDialog.heightConfigPropertyName, (object) this.MinHeight);
      this.Width = (double) this.configuration.GetProperty(FindInFilesDialog.widthConfigPropertyName, (object) this.MinWidth);
      this.LookInSelectionIndex = (int) this.configuration.GetProperty(FindInFilesDialog.lookInConfigPropertyName, (object) 0);
      this.FileTypes.Add(StringTable.FindInFilesSearchAll);
      this.FileTypes.Add(StringTable.FindInFilesSearchXAML);
      this.FileTypes.Add(StringTable.FindInFilesSearchCode);
      this.LookInStrings.Add(StringTable.FindInFilesLookInSolution);
      this.LookInStrings.Add(StringTable.FindInFilesLookInProject);
      this.LookInStrings.Add(StringTable.FindInFilesLookInOpenDocuments);
      this.LookInStrings.Add(StringTable.FindInFilesLookInCurrentDocument);
      this.FileTypesSelectionIndex = (int) this.configuration.GetProperty(FindInFilesDialog.fileTypesConfigPropertyName, (object) 0);
      this.MatchCase = (bool) this.configuration.GetProperty(FindInFilesDialog.matchCaseConfigPropertyName, (object) false);
      this.DataContext = (object) this;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        FindInFilesDialog.findInFilesDialog = null;
        base.OnClosing(e);
        base.Owner.Activate();
        this.configuration.SetProperty(FindInFilesDialog.heightConfigPropertyName, base.Height);
        this.configuration.SetProperty(FindInFilesDialog.widthConfigPropertyName, base.Width);
        this.configuration.SetProperty(FindInFilesDialog.lookInConfigPropertyName, this.LookInSelectionIndex);
        this.configuration.SetProperty(FindInFilesDialog.fileTypesConfigPropertyName, this.FileTypesSelectionIndex);
        this.configuration.SetProperty(FindInFilesDialog.matchCaseConfigPropertyName, this.MatchCase);
    }

    protected override void OnCancelButtonExecute()
    {
      this.Close();
    }

    private void projectManager_SolutionClosed(object sender, SolutionEventArgs e)
    {
      this.SearchResults.Clear();
    }

    private void Find(object sender, RoutedEventArgs e)
    {
      using (TemporaryCursor.SetWaitCursor())
      {
        this.searchResults.Clear();
        FindInFilesDialog.LookIn lookInValue = this.GetLookInValue();
        if (this.FindString == null || this.FindString.Trim().Length <= 0)
          return;
        ISolution currentSolution = this.projectManager.CurrentSolution;
        if (currentSolution == null)
          return;
        switch (lookInValue)
        {
          case FindInFilesDialog.LookIn.Solution:
            using (IEnumerator<IProject> enumerator = currentSolution.Projects.GetEnumerator())
            {
              while (enumerator.MoveNext())
                this.FindInProject(enumerator.Current);
              break;
            }
          case FindInFilesDialog.LookIn.Project:
            DocumentView documentView1 = this.viewService.ActiveView as DocumentView;
            IProject project1 = documentView1 == null ? EnumerableExtensions.SingleOrNull<IProject>(this.projectManager.ItemSelectionSet.SelectedProjects) : currentSolution.FindProjectContainingOpenItem(documentView1.Document.DocumentReference);
            if (project1 == null)
              break;
            this.FindInProject(project1);
            break;
          case FindInFilesDialog.LookIn.OpenDocuments:
            using (IEnumerator<IView> enumerator = this.viewService.Views.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                DocumentView documentView2 = enumerator.Current as DocumentView;
                if (documentView2 != null)
                  this.FindInFile(currentSolution.FindProjectContainingOpenItem(documentView2.Document.DocumentReference).FindItem(documentView2.Document.DocumentReference), new FindInFilesDialog.ProcessResult(this.SaveResult), (FindInFilesResult) null);
              }
              break;
            }
          case FindInFilesDialog.LookIn.CurrentDocument:
            DocumentView documentView3 = this.viewService.ActiveView as DocumentView;
            if (documentView3 != null)
            {
              this.FindInFile(currentSolution.FindProjectContainingOpenItem(documentView3.Document.DocumentReference).FindItem(documentView3.Document.DocumentReference), new FindInFilesDialog.ProcessResult(this.SaveResult), (FindInFilesResult) null);
              break;
            }
            IProject project2 = EnumerableExtensions.SingleOrNull<IProject>(this.projectManager.ItemSelectionSet.SelectedProjects);
            IDocumentItem documentItem = EnumerableExtensions.SingleOrNull<IDocumentItem>(this.projectManager.ItemSelectionSet.Selection);
            if (project2 == null || documentItem == null)
              break;
            this.FindInFile(project2.FindItem(documentItem.DocumentReference), new FindInFilesDialog.ProcessResult(this.SaveResult), (FindInFilesResult) null);
            break;
        }
      }
    }

    private void FindInProject(IProject project)
    {
      foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) project.Items)
        this.FindInFile(projectItem, new FindInFilesDialog.ProcessResult(this.SaveResult), (FindInFilesResult) null);
    }

    private bool FindInFile(IProjectItem projectItem, FindInFilesDialog.ProcessResult callback, FindInFilesResult previousResult)
    {
      if (!this.IsTargetExtension(projectItem.DocumentType.DefaultFileExtension))
        return false;
      try
      {
        if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(projectItem.DocumentReference.Path))
        {
          if (!projectItem.IsDirty)
          {
            using (TextReader textReader = (TextReader) new StreamReader((Stream) new FileStream(projectItem.DocumentReference.Path, FileMode.Open, FileAccess.Read)))
            {
              if (!this.SearchText(projectItem, textReader, callback, previousResult))
                return false;
            }
          }
          else
          {
            string s = (string) null;
            IReadableTextBuffer readableTextBuffer = projectItem.Document as IReadableTextBuffer;
            if (readableTextBuffer != null)
            {
              s = readableTextBuffer.GetText(0, readableTextBuffer.Length);
            }
            else
            {
              CodeDocument codeDocument = projectItem.Document as CodeDocument;
              if (codeDocument != null)
                s = codeDocument.Document != null ? codeDocument.Document.Text : string.Empty;
            }
            if (s != null)
            {
              using (TextReader textReader = (TextReader) new StringReader(s))
              {
                if (!this.SearchText(projectItem, textReader, callback, previousResult))
                  return false;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return true;
    }

    private bool SearchText(IProjectItem projectItem, TextReader textReader, FindInFilesDialog.ProcessResult callback, FindInFilesResult previousResult)
    {
      string str1 = string.Empty;
      int num1 = -1;
      string str2;
      int num2;
      do
      {
        ++num1;
        str2 = textReader.ReadLine();
        if (str2 != null)
          num2 = str2.IndexOf(this.FindString, this.comparison);
        else
          goto label_4;
      }
      while (num2 < 0 || callback(projectItem, num1 + 1, num2 + 1, str2.Trim(), previousResult));
      return false;
label_4:
      return true;
    }

    private bool SaveResult(IProjectItem projectItem, int line, int column, string lineContent, FindInFilesResult previousResult)
    {
      FindInFilesResult result = new FindInFilesResult();
      result.Column = column;
      result.Line = line;
      result.LineContent = lineContent;
      result.Project = projectItem.Project.Name;
      result.Image = projectItem.DocumentType.Image;
      result.FileName = projectItem.DocumentReference.DisplayName;
      result.InvokeCommand = new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => this.ActivateResult(projectItem, result)));
      this.searchResults.Add(result);
      return true;
    }

    private bool UpdateResult(IProjectItem projectItem, int line, int column, string lineContent, FindInFilesResult previousResult)
    {
      if (line < previousResult.Line || column < previousResult.Column)
        return true;
      if (line > previousResult.Line)
      {
        previousResult.Line = line;
        previousResult.Column = column;
      }
      else if (column > previousResult.Column)
        previousResult.Column = column;
      return false;
    }

    private bool UpdateToLastResult(IProjectItem projectItem, int line, int column, string lineContent, FindInFilesResult previousResult)
    {
      previousResult.Line = line;
      previousResult.Column = column;
      return true;
    }

    private void ActivateResult(IProjectItem projectItem, FindInFilesResult result)
    {
      if (projectItem == null)
        return;
      if (this.FindInFile(projectItem, new FindInFilesDialog.ProcessResult(this.UpdateResult), result))
      {
        result.Line = -1;
        result.Column = -1;
        this.FindInFile(projectItem, new FindInFilesDialog.ProcessResult(this.UpdateToLastResult), result);
      }
      if (result.Line == -1 || result.Column == -1)
        return;
      IDocumentView documentView = projectItem.OpenView(true);
      if (documentView == null)
        return;
      ISetCaretPosition setCaretPosition = documentView as ISetCaretPosition;
      if (setCaretPosition == null)
        return;
      setCaretPosition.SetCaretPosition(result.Line, result.Column);
    }

    private bool IsTargetExtension(string extension)
    {
      if (string.IsNullOrEmpty(extension))
        return false;
      switch (this.GetSearchFileTypesValue())
      {
        case FindInFilesDialog.SearchFileTypes.XAML:
          return Enumerable.Contains<string>((IEnumerable<string>) FindInFilesDialog.xamlExtensions, extension.ToUpperInvariant());
        case FindInFilesDialog.SearchFileTypes.Code:
          return Enumerable.Contains<string>((IEnumerable<string>) FindInFilesDialog.codeExtensions, extension.ToUpperInvariant());
        default:
          return Enumerable.Contains<string>((IEnumerable<string>) FindInFilesDialog.allExtensions, extension.ToUpperInvariant());
      }
    }

    private FindInFilesDialog.LookIn GetLookInValue()
    {
      switch (this.LookInSelectionIndex)
      {
        case 1:
          return FindInFilesDialog.LookIn.Project;
        case 2:
          return FindInFilesDialog.LookIn.OpenDocuments;
        case 3:
          return FindInFilesDialog.LookIn.CurrentDocument;
        default:
          return FindInFilesDialog.LookIn.Solution;
      }
    }

    private FindInFilesDialog.SearchFileTypes GetSearchFileTypesValue()
    {
      switch (this.FileTypesSelectionIndex)
      {
        case 1:
          return FindInFilesDialog.SearchFileTypes.XAML;
        case 2:
          return FindInFilesDialog.SearchFileTypes.Code;
        default:
          return FindInFilesDialog.SearchFileTypes.All;
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Code;component/userinterface/findinfilesdialog.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.LayoutRoot = (Grid) target;
          break;
        case 2:
          this.FindText = (TextBox) target;
          break;
        case 3:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.Find);
          break;
        case 4:
          this.CancelButton = (Button) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    private delegate bool ProcessResult(IProjectItem projectItem, int line, int column, string lineContent, FindInFilesResult previousResult);

    private enum LookIn
    {
      Solution,
      Project,
      OpenDocuments,
      CurrentDocument,
    }

    private enum SearchFileTypes
    {
      All,
      XAML,
      Code,
    }
  }
}
