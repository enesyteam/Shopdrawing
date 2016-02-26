// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.UserThemeAssetProvider
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class UserThemeAssetProvider : ResourceDictionaryAssetProvider
  {
    private DesignerContext designerContext;
    private DocumentReference documentReference;
    private ResourceDictionaryContentProvider originalDocumentProvider;
    private WeakReference knownOtherDocument;
    private uint knownOtherDocumentChangeStamp;
    private bool textChanged;
    private ThemeManager themeManager;

    private XamlDocument KnownOtherDocument
    {
      get
      {
        if (this.knownOtherDocument == null || !this.knownOtherDocument.IsAlive)
          return (XamlDocument) null;
        return this.knownOtherDocument.Target as XamlDocument;
      }
      set
      {
        this.knownOtherDocument = new WeakReference((object) value);
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    public override bool IsCustomized
    {
      get
      {
        if (!this.IsLocal)
          return false;
        if (!this.ContentProvider.ProjectItem.FileExists)
          return true;
        if (this.ContentProvider.Document != null && (this.ContentProvider.Document != this.KnownOtherDocument || (int) this.ContentProvider.Document.ChangeStamp != (int) this.knownOtherDocumentChangeStamp))
        {
          this.KnownOtherDocument = this.ContentProvider.Document;
          this.knownOtherDocumentChangeStamp = this.ContentProvider.Document.ChangeStamp;
          this.textChanged = !this.ContentProvider.Document.IsTextUpToDate || this.ContentProvider.Document.TextBuffer.Length != this.originalDocumentProvider.Document.TextBuffer.Length || this.ContentProvider.Document.Text != this.originalDocumentProvider.Document.Text;
        }
        return this.textChanged;
      }
    }

    public bool IsLocal
    {
      get
      {
        return this.ContentProvider != null;
      }
    }

    private bool CanCopyLocal
    {
      get
      {
        IProject activeProject = this.DesignerContext.ActiveProject;
        if (this.IsLocal)
          return false;
        bool canInsert;
        this.FindExistingItem(activeProject, out canInsert);
        return canInsert;
      }
    }

    public string DisplayName
    {
      get
      {
        return this.documentReference.DisplayNameShort;
      }
    }

    protected override IEnumerable<DocumentNode> Items
    {
      get
      {
        if (!this.IsLocal)
          return this.originalDocumentProvider.Items;
        return base.Items;
      }
    }

    public UserThemeAssetProvider(DesignerContext designerContext, ThemeManager themeManager, DocumentReference documentReference)
    {
      this.designerContext = designerContext;
      this.documentReference = documentReference;
      this.themeManager = themeManager;
    }

    public override bool DoesProjectItemMatch(IProjectItem projectItem)
    {
      if (this.ProjectItem == null)
      {
        bool canInsert;
        return this.FindExistingItem(projectItem.Project, out canInsert) == projectItem;
      }
      return base.DoesProjectItemMatch(projectItem);
    }

    protected override bool UpdateAssets()
    {
      if (this.originalDocumentProvider == null)
      {
        Encoding encoding;
        this.originalDocumentProvider = new ResourceDictionaryContentProvider(this.designerContext, this.themeManager.GetTheme(DocumentReferenceLocator.GetDocumentLocator(this.documentReference), false, (IDocumentContext) null, ThemeContentProvider.LoadReference(this.documentReference, this.designerContext.TextBufferService, out encoding), encoding));
        this.ContentProvider = this.GetProvider(this.designerContext.ActiveProject);
      }
      return base.UpdateAssets();
    }

    public override ResourceAsset CreateAsset(ResourceModel resourceModel)
    {
      return (ResourceAsset) new UserThemeAssetProvider.UserThemeStyleAsset(this, resourceModel);
    }

    public void OnProjectChanged(IProject newProject)
    {
      if (this.originalDocumentProvider == null)
        return;
      ResourceDictionaryContentProvider provider = this.GetProvider(newProject);
      if (this.ContentProvider == provider)
        return;
      this.ContentProvider = provider;
      if (this.Assets.Count <= 0)
        return;
      this.Assets.Clear();
      this.NeedsUpdate = true;
    }

    private ResourceDictionaryContentProvider GetProvider(IProject activeProject)
    {
      ResourceDictionaryContentProvider dictionaryContentProvider = (ResourceDictionaryContentProvider) null;
      bool canInsert;
      IProjectItem existingItem = this.FindExistingItem(activeProject, out canInsert);
      if (existingItem != null)
        dictionaryContentProvider = this.designerContext.ResourceManager.GetContentProviderForResourceDictionary(existingItem);
      return dictionaryContentProvider;
    }

    private IProjectItem FindExistingItem(IProject targetProject, out bool canInsert)
    {
      if (targetProject != null)
      {
        canInsert = true;
        string str = Path.Combine(targetProject.ProjectRoot.Path, Path.GetFileName(this.documentReference.Path));
        if (new FileInfo(str).Exists)
          canInsert = false;
        foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) targetProject.Items)
        {
          string documentReference = projectItem.ProjectRelativeDocumentReference;
          if (!string.IsNullOrEmpty(documentReference))
          {
            if (StringComparer.OrdinalIgnoreCase.Compare(Path.Combine(targetProject.ProjectRoot.Path, documentReference.Trim(Path.DirectorySeparatorChar)), str) == 0)
            {
              canInsert = false;
              return projectItem;
            }
          }
        }
      }
      else
        canInsert = false;
      return (IProjectItem) null;
    }

    public override void ResetToDefault()
    {
      if (!this.IsLocal)
        return;
      using (TemporaryCursor.SetWaitCursor())
      {
        SceneDocument sceneDocument = this.ProjectItem.Document as SceneDocument;
        if (sceneDocument != null && this.ProjectItem.FileExists)
        {
          using (SceneEditTransaction editTransaction = sceneDocument.CreateEditTransaction(StringTable.UndoUnitResetDocumentToDefault))
          {
            ((XamlDocument) sceneDocument.DocumentRoot).Text = this.originalDocumentProvider.Document.Text;
            editTransaction.Commit();
          }
        }
        else
        {
          if (this.ProjectItem.IsOpen)
            this.ProjectItem.CloseDocument();
          this.ProjectItem.CreateDocument(this.originalDocumentProvider.Document.Text);
        }
      }
    }

    private bool CopyLocal()
    {
      bool flag = false;
      IProject activeProject = this.DesignerContext.ActiveProject;
      if (this.CanCopyLocal)
      {
        DocumentReference projectRoot = activeProject.ProjectRoot;
        Path.Combine(projectRoot.Path, Path.GetFileName(this.documentReference.Path));
        try
        {
          IProjectItem projectItem = activeProject.AddItem(new DocumentCreationInfo()
          {
            TargetFolder = projectRoot.Path,
            SourcePath = this.documentReference.Path
          });
          if (projectItem != null)
          {
            this.ContentProvider = this.designerContext.ResourceManager.GetContentProviderForResourceDictionary(projectItem);
            flag = true;
          }
          else
            flag = false;
        }
        catch (Exception ex)
        {
          this.DesignerContext.MessageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.FileOpenFailedDialogMessage, new object[2]
          {
            (object) this.documentReference.Path,
            (object) ex.Message
          }));
        }
      }
      return flag;
    }

    protected override string GetThemeName()
    {
      return this.documentReference.DisplayNameShort;
    }

    private class UserThemeStyleAsset : NonLocalStyleAsset
    {
      private UserThemeAssetProvider provider;

      public UserThemeStyleAsset(UserThemeAssetProvider provider, ResourceModel resourceModel)
        : base((ResourceDictionaryAssetProvider) provider, resourceModel)
      {
        this.provider = provider;
      }

      protected override DefaultTypeInstantiator GetInstantiator(SceneView sceneView)
      {
        return (DefaultTypeInstantiator) new UserThemeAssetProvider.UserThemeStyleAssetInstantiator(sceneView, this.provider, this);
      }

      protected override bool InternalCanCreateInstance(ISceneInsertionPoint insertionPoint)
      {
        if (!this.provider.IsLocal && !this.provider.CanCopyLocal)
          return false;
        return base.InternalCanCreateInstance(insertionPoint);
      }
    }

    private class UserThemeStyleAssetInstantiator : StyleAssetInstantiator
    {
      private UserThemeAssetProvider provider;

      public UserThemeStyleAssetInstantiator(SceneView sceneView, UserThemeAssetProvider provider, UserThemeAssetProvider.UserThemeStyleAsset styleAsset)
        : base(sceneView, (StyleAssetProvider) provider, (StyleAsset) styleAsset)
      {
        this.provider = provider;
      }

      internal override void ApplyBeforeInsertionDefaultsToElements(IList<SceneNode> nodes, SceneNode rootNode, DefaultTypeInstantiator.SceneElementNamingCallback callback)
      {
        if (!this.provider.IsLocal)
          this.provider.CopyLocal();
        else
          this.provider.ContentProvider.EnsureLinked(this.ViewModel);
        base.ApplyBeforeInsertionDefaultsToElements(nodes, rootNode, callback);
      }

      public override SceneNode CreateInstance(ITypeId instanceType, ISceneInsertionPoint insertionPoint, Rect rect, OnCreateInstanceAction action)
      {
        using (!this.provider.IsLocal ? TemporaryCursor.SetWaitCursor() : (IDisposable) null)
          return base.CreateInstance(instanceType, insertionPoint, rect, action);
      }
    }
  }
}
