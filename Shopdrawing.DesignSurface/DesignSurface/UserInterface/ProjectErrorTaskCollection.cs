// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ProjectErrorTaskCollection
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project.Build;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  internal sealed class ProjectErrorTaskCollection : IErrorTaskCollection, ICollection<IErrorTask>, IEnumerable<IErrorTask>, IEnumerable, INotifyCollectionChanged
  {
    private DesignerContext context;
    private IErrorTaskCollection errors;
    private IErrorTaskCollection projectErrors;
    private IErrorTaskCollection documentErrors;

    public DateTime Timestamp
    {
      get
      {
        if (this.errors == null)
          return DateTime.Now;
        return this.errors.Timestamp;
      }
    }

    public int Count
    {
      get
      {
        if (this.errors == null)
          return 0;
        return this.errors.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return true;
      }
    }

    public event EventHandler TimestampChanged;

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public ProjectErrorTaskCollection(DesignerContext context)
    {
      this.context = context;
      this.context.ProjectManager.ActiveBuildTargetChanged += new EventHandler(this.ProjectManager_ActiveBuiltTargetChanged);
      this.context.ViewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.SetProjectErrors(this.GetProjectErrorsFromContext());
      this.SetDocumentErrors(this.GetDocumentErrorsFromContext());
      this.SetCurrentErrors();
    }

    public void Add(IErrorTask item)
    {
      throw new NotSupportedException();
    }

    public void Clear()
    {
      throw new NotSupportedException();
    }

    public bool Contains(IErrorTask item)
    {
      if (this.errors == null)
        return false;
      return this.errors.Contains(item);
    }

    public void CopyTo(IErrorTask[] array, int arrayIndex)
    {
      if (this.errors == null)
        return;
      this.errors.CopyTo(array, arrayIndex);
    }

    public bool Remove(IErrorTask item)
    {
      throw new NotSupportedException();
    }

    public IEnumerator<IErrorTask> GetEnumerator()
    {
      if (this.errors != null)
        return this.errors.GetEnumerator();
      return this.CreateEmptyEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    private IErrorTaskCollection GetProjectErrorsFromContext()
    {
      IProjectBuildContext activeBuildTarget = this.context.ProjectManager.ActiveBuildTarget;
      if (activeBuildTarget == null)
        return (IErrorTaskCollection) null;
      return activeBuildTarget.BuildErrors;
    }

    private IErrorTaskCollection GetDocumentErrorsFromContext()
    {
      SceneView sceneView = this.context.ViewService.ActiveView as SceneView;
      if (sceneView == null)
        return (IErrorTaskCollection) null;
      return sceneView.Errors;
    }

    private void SetProjectErrors(IErrorTaskCollection errors)
    {
      if (this.projectErrors == errors)
        return;
      this.UnregisterTimestampChangedEventHandler(this.projectErrors);
      this.projectErrors = errors;
      this.RegisterTimestampChangedEventHandler(this.projectErrors);
    }

    private void SetDocumentErrors(IErrorTaskCollection errors)
    {
      if (this.documentErrors == errors)
        return;
      this.UnregisterTimestampChangedEventHandler(this.documentErrors);
      this.documentErrors = errors;
      this.RegisterTimestampChangedEventHandler(this.documentErrors);
    }

    private void SetCurrentErrors()
    {
      IErrorTaskCollection errorTaskCollection = this.projectErrors;
      if (this.documentErrors != null && this.documentErrors.Count > 0 && (this.projectErrors == null || this.documentErrors.Timestamp > this.projectErrors.Timestamp))
        errorTaskCollection = this.documentErrors;
      if (this.errors == errorTaskCollection)
        return;
      this.UnregisterCollectionChangedEventHandler(this.errors);
      this.errors = errorTaskCollection;
      this.RegisterCollectionChangedEventHandler(this.errors);
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    private void RegisterCollectionChangedEventHandler(IErrorTaskCollection errors)
    {
      INotifyCollectionChanged collectionChanged = errors as INotifyCollectionChanged;
      if (collectionChanged == null)
        return;
      collectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Errors_CollectionChanged);
    }

    private void UnregisterCollectionChangedEventHandler(IErrorTaskCollection errors)
    {
      INotifyCollectionChanged collectionChanged = errors as INotifyCollectionChanged;
      if (collectionChanged == null)
        return;
      collectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.Errors_CollectionChanged);
    }

    private void RegisterTimestampChangedEventHandler(IErrorTaskCollection errors)
    {
      if (errors == null)
        return;
      errors.TimestampChanged += new EventHandler(this.Errors_TimestampChanged);
    }

    private void UnregisterTimestampChangedEventHandler(IErrorTaskCollection errors)
    {
      if (errors == null)
        return;
      errors.TimestampChanged -= new EventHandler(this.Errors_TimestampChanged);
    }

    private IEnumerator<IErrorTask> CreateEmptyEnumerator()
    {
      yield break;
    }

    private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
      if (this.CollectionChanged == null)
        return;
      this.CollectionChanged((object) this, args);
    }

    private void OnTimestampChanged()
    {
      if (this.TimestampChanged == null)
        return;
      this.TimestampChanged((object) this, EventArgs.Empty);
    }

    private void Errors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.OnCollectionChanged(e);
    }

    private void Errors_TimestampChanged(object sender, EventArgs e)
    {
      this.SetCurrentErrors();
      this.OnTimestampChanged();
    }

    private void ProjectManager_ActiveBuiltTargetChanged(object sender, EventArgs e)
    {
      this.SetProjectErrors(this.GetProjectErrorsFromContext());
      this.SetCurrentErrors();
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs e)
    {
      this.SetDocumentErrors(this.GetDocumentErrorsFromContext());
      this.SetCurrentErrors();
    }
  }
}
