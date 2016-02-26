// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.AnnotationViewModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Annotations
{
  public class AnnotationViewModel : DependencyObject, INotifyPropertyChanged
  {
    private AnnotationSceneNode annotationNode;
    private ICommand deleteAnnotationCommand;
    private ICommand copyAnnotationTextCommand;
    private ICommand unlinkAnnotationCommand;
    private ViewMode viewMode;

    public AnnotationSceneNode Annotation
    {
      get
      {
        return this.annotationNode;
      }
    }

    public string Text
    {
      get
      {
        return this.annotationNode.Text;
      }
      set
      {
        this.Set(AnnotationSceneNode.TextProperty, (object) value);
      }
    }

    public DateTime TimestampLocal
    {
      get
      {
        return this.Timestamp.ToLocalTime();
      }
    }

    public DateTime Timestamp
    {
      get
      {
        return this.annotationNode.Timestamp;
      }
    }

    public double Top
    {
      get
      {
        return this.annotationNode.Top;
      }
      set
      {
        this.Set(AnnotationSceneNode.TopProperty, (object) value);
      }
    }

    public double Left
    {
      get
      {
        return this.annotationNode.Left;
      }
      set
      {
        this.Set(AnnotationSceneNode.LeftProperty, (object) value);
      }
    }

    public string Author
    {
      get
      {
        string str = this.annotationNode.Author;
        if (string.IsNullOrEmpty(str))
          str = StringTable.UnknownUserName;
        return str;
      }
    }

    public string AuthorInitials
    {
      get
      {
        string str = this.annotationNode.AuthorInitials;
        if (string.IsNullOrEmpty(str))
          str = this.GenerateInitials(this.annotationNode.Author);
        return str;
      }
    }

    public int SerialNumber
    {
      get
      {
        return this.annotationNode.SerialNumber;
      }
    }

    public bool VisibleAtRuntime
    {
      get
      {
        return this.annotationNode.VisibleAtRuntime;
      }
      set
      {
        this.Set(AnnotationSceneNode.VisibleAtRuntimeProperty, (object) (bool) (value ? true : false));
      }
    }

    public bool Selected
    {
      get
      {
        if (this.AnnotationService == null)
          return false;
        return this.AnnotationService.IsSelected(this.annotationNode);
      }
      set
      {
        bool flag = this.AnnotationService.IsSelected(this.annotationNode);
        if (flag == value)
          return;
        this.AnnotationService.SelectedAnnotation = flag ? (AnnotationSceneNode) null : this.annotationNode;
        this.OnPropertyChanged("Selected");
      }
    }

    public ViewMode ViewMode
    {
      get
      {
        return this.viewMode;
      }
      set
      {
        this.viewMode = value;
        this.OnPropertyChanged("ViewMode");
      }
    }

    public bool ShowVisibleAtRuntime { get; set; }

    public Cursor SelectCursor
    {
      get
      {
        switch (this.GetCursorSource())
        {
          case AnnotationViewModel.CursorSource.SelectTool:
            return ToolCursors.SelectElementCursor;
          case AnnotationViewModel.CursorSource.SubselectTool:
            return ToolCursors.SubselectElementCursor;
          default:
            return Cursors.None;
        }
      }
    }

    public Cursor MoveCursor
    {
      get
      {
        switch (this.GetCursorSource())
        {
          case AnnotationViewModel.CursorSource.SelectTool:
            return ToolCursors.RelocateCursor;
          case AnnotationViewModel.CursorSource.SubselectTool:
            return ToolCursors.SubselectMoveCursor;
          default:
            return Cursors.None;
        }
      }
    }

    public ICommand DeleteAnnotationCommand
    {
      get
      {
        return this.deleteAnnotationCommand ?? (this.deleteAnnotationCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.DeleteAnnotation)));
      }
    }

    public ICommand CopyAnnotationTextCommand
    {
      get
      {
        return this.copyAnnotationTextCommand ?? (this.copyAnnotationTextCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CopyAnnotationText)));
      }
    }

    public ICommand UnlinkAnnotationCommand
    {
      get
      {
        return this.unlinkAnnotationCommand ?? (this.unlinkAnnotationCommand = (ICommand) new Microsoft.Expression.DesignSurface.Annotations.UnlinkAnnotationCommand(this.annotationNode));
      }
    }

    private AnnotationService AnnotationService
    {
      get
      {
        if (this.annotationNode == null || this.annotationNode.DesignerContext == null)
          return (AnnotationService) null;
        return this.annotationNode.DesignerContext.AnnotationService;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public AnnotationViewModel(AnnotationSceneNode annotationNode)
    {
      this.annotationNode = annotationNode;
    }

    public void RefreshProperty(string propertyName)
    {
      this.OnPropertyChanged(propertyName);
    }

    public void RefreshToolRelatedProperties()
    {
      this.OnPropertyChanged("SelectCursor");
      this.OnPropertyChanged("MoveCursor");
    }

    private AnnotationViewModel.CursorSource GetCursorSource()
    {
      if (this.annotationNode != null && this.annotationNode.DesignerContext != null)
      {
        ToolManager toolManager = this.annotationNode.DesignerContext.ToolManager;
        if (toolManager != null && toolManager.ActiveTool != null)
        {
          string identifier = toolManager.ActiveTool.Identifier;
          if (identifier.Equals("Selection"))
            return AnnotationViewModel.CursorSource.SelectTool;
          return !identifier.Equals("Subselection") ? AnnotationViewModel.CursorSource.None : AnnotationViewModel.CursorSource.SubselectTool;
        }
      }
      return AnnotationViewModel.CursorSource.None;
    }

    private void DeleteAnnotation()
    {
      this.AnnotationService.Delete(this.annotationNode);
    }

    private void CopyAnnotationText()
    {
      this.AnnotationService.CopyToClipboardAsText(this.annotationNode);
    }

    private string GenerateInitials(string name)
    {
      if (string.IsNullOrEmpty(name))
        return string.Empty;
      return new string(Enumerable.ToArray<char>(Enumerable.Select<string, char>((IEnumerable<string>) Regex.Split(name, "\\s"), (Func<string, char>) (s => Enumerable.First<char>((IEnumerable<char>) s)))));
    }

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler changedEventHandler = this.PropertyChanged;
      if (changedEventHandler == null)
        return;
      changedEventHandler((object) this, new PropertyChangedEventArgs(propertyName));
    }

    internal void Set(IPropertyId property, object value)
    {
      if (object.Equals(this.annotationNode.GetLocalValue(property), value))
        return;
      using (SceneEditTransaction editTransaction = this.annotationNode.ViewModel.CreateEditTransaction(StringTable.EditAnnotationUndoUnit))
      {
        this.annotationNode.SetValue(property, value);
        this.annotationNode.SetValue(AnnotationSceneNode.TimestampProperty, (object) DateTime.UtcNow);
        editTransaction.Commit();
      }
      this.OnPropertyChanged(property.Name);
    }

    private enum CursorSource
    {
      None,
      SelectTool,
      SubselectTool,
    }
  }
}
