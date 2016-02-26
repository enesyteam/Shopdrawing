// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ReferencesFoundModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public class ReferencesFoundModel : NotifyingObject
  {
    private ObservableCollection<string> referenceNames = new ObservableCollection<string>();
    private List<string> scenesWithReferences = new List<string>();
    private ReferencesFoundModel.UpdateMethod updateMethod;
    private ReferencesFoundModel.UseScenario useScenario;
    private SceneNode resourceEntryNode;
    private object newKey;

    public bool IsDeleting
    {
      get
      {
        if (this.useScenario != ReferencesFoundModel.UseScenario.DeleteResource)
          return this.useScenario == ReferencesFoundModel.UseScenario.DeleteResourceDictionary;
        return true;
      }
    }

    public ReadOnlyCollection<string> ScenesWithReferences
    {
      get
      {
        return new ReadOnlyCollection<string>((IList<string>) this.scenesWithReferences);
      }
    }

    public ObservableCollection<string> ReferenceNames
    {
      get
      {
        return this.referenceNames;
      }
    }

    public DictionaryEntryNode ResourceEntryNode
    {
      get
      {
        return this.resourceEntryNode as DictionaryEntryNode;
      }
    }

    public object NewKey
    {
      get
      {
        return this.newKey;
      }
      set
      {
        this.newKey = value;
      }
    }

    public ReferencesFoundModel.UpdateMethod SelectedUpdateMethod
    {
      get
      {
        return this.updateMethod;
      }
      set
      {
        if (this.updateMethod == value)
          return;
        this.updateMethod = value;
        this.OnPropertyChanged("SelectedUpdateMethod");
      }
    }

    public ReferencesFoundModel(SceneNode resourceEntryNode, ICollection<SceneNode> references, ReferencesFoundModel.UseScenario useScenario)
    {
      this.resourceEntryNode = resourceEntryNode;
      foreach (SceneNode node in (IEnumerable<SceneNode>) references)
        this.referenceNames.Add(ReferencesFoundModel.SceneNodeElementName(node));
      if (references.Count > 0)
        this.AddReferencesFile(resourceEntryNode.ViewModel.Document);
      this.useScenario = useScenario;
      if (this.IsDeleting)
        this.updateMethod = ReferencesFoundModel.UpdateMethod.ConvertToLocal;
      else
        this.updateMethod = ReferencesFoundModel.UpdateMethod.UpdateReferences;
    }

    public static string SceneNodeElementName(SceneNode node)
    {
      string caption = node.ViewModel.Document.Caption;
      SceneElement sceneElement = (SceneElement) node.FindSceneNodeTypeAncestor(typeof (SceneElement));
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ReferencesFoundFormat, new object[2]
      {
        (object) (sceneElement != null ? sceneElement.DisplayName : node.ToString()),
        (object) caption
      });
    }

    public void AddReferencesFile(SceneDocument document)
    {
      if (this.scenesWithReferences.Contains(document.DocumentReference.Path))
        return;
      this.scenesWithReferences.Add(document.DocumentReference.Path);
    }

    public AsyncProcess FixReferencesAsync()
    {
      return (AsyncProcess) new ReferencesFoundModel.FixupResourceReferenceAsyncProcess(this, (IList<string>) this.scenesWithReferences);
    }

    public void FixupIndividualReference(DocumentCompositeNode resource, SceneNode reference)
    {
      switch (this.updateMethod)
      {
        case ReferencesFoundModel.UpdateMethod.UpdateReferences:
          DocumentNode node = reference.DocumentContext.CreateNode(this.newKey.GetType(), this.newKey);
          ResourceNodeHelper.SetResourceKey((DocumentCompositeNode) reference.DocumentNode, node);
          IPropertyId propertyId1 = (IPropertyId) reference.Parent.GetPropertyForChild(reference);
          reference.Parent.SetValue(new PropertyReference((ReferenceStep) propertyId1), (object) reference.DocumentNode.Clone(reference.DocumentContext));
          break;
        case ReferencesFoundModel.UpdateMethod.ConvertToLocal:
          DocumentNode documentNode = resource.Properties[DictionaryEntryNode.ValueProperty].Clone(reference.DocumentContext);
          IPropertyId propertyId2 = (IPropertyId) reference.Parent.GetPropertyForChild(reference);
          reference.Parent.SetValue(new PropertyReference((ReferenceStep) propertyId2), (object) documentNode);
          break;
        case ReferencesFoundModel.UpdateMethod.ResetToDefault:
          if (PlatformTypes.Setter.IsAssignableFrom((ITypeId) reference.Parent.Type))
          {
            reference.Parent.Remove();
            break;
          }
          IPropertyId propertyKey = (IPropertyId) reference.Parent.GetPropertyForChild(reference);
          reference.Parent.ClearLocalValue(propertyKey);
          break;
      }
    }

    public enum UseScenario
    {
      DeleteResource,
      DeleteResourceDictionary,
      RenameResource,
      MoveResource,
    }

    public enum UpdateMethod
    {
      UpdateReferences,
      ConvertToLocal,
      ResetToDefault,
      DontFix,
    }

    private class FixupResourceReferenceAsyncProcess : AsyncProcess
    {
      private IList<string> documentPaths;
      private int documentPathIndex;
      private ReferencesFoundModel model;

      public override int Count
      {
        get
        {
          return this.documentPaths.Count;
        }
      }

      public override int CompletedCount
      {
        get
        {
          return Math.Max(this.documentPathIndex, 0);
        }
      }

      public override string StatusText
      {
        get
        {
          if (this.documentPathIndex < 0 || this.documentPathIndex >= this.documentPaths.Count)
            return string.Empty;
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ReferencesFixupStatusText, new object[1]
          {
            (object) this.documentPaths[this.documentPathIndex]
          });
        }
      }

      public FixupResourceReferenceAsyncProcess(ReferencesFoundModel model, IList<string> documentPaths)
        : base((IAsyncMechanism) new CurrentDispatcherAsyncMechanism(DispatcherPriority.Background))
      {
        this.model = model;
        this.documentPaths = documentPaths;
        this.documentPathIndex = -1;
      }

      public override void Reset()
      {
        this.documentPathIndex = -1;
      }

      protected override void Work()
      {
        IProjectDocument projectDocument = this.model.ResourceEntryNode.ProjectContext.OpenDocument(this.documentPaths[this.documentPathIndex]);
        if (projectDocument == null)
          return;
        SceneDocument sceneDocument = projectDocument.Document as SceneDocument;
        if (sceneDocument == null)
          return;
        SceneViewModel sceneViewModel = (SceneViewModel) null;
        ISceneViewHost sceneViewHost = (ISceneViewHost) sceneDocument.ProjectContext.GetService(typeof (ISceneViewHost));
        if (sceneViewHost != null)
        {
          SceneView sceneView = sceneViewHost.OpenView(sceneDocument.DocumentRoot, false);
          sceneViewModel = sceneView != null ? sceneView.ViewModel : (SceneViewModel) null;
        }
        if (sceneViewModel == null)
          return;
        List<SceneNode> list = new List<SceneNode>();
        sceneViewModel.FindInternalResourceReferences((DocumentCompositeNode) this.model.ResourceEntryNode.DocumentNode, (ICollection<SceneNode>) list);
        if (list.Count <= 0)
          return;
        using (SceneEditTransaction editTransaction = sceneDocument.CreateEditTransaction(StringTable.ReferencesFixupEditTransaction))
        {
          foreach (SceneNode reference in list)
            this.model.FixupIndividualReference((DocumentCompositeNode) this.model.ResourceEntryNode.DocumentNode, reference);
          editTransaction.Commit();
        }
      }

      protected override bool MoveNext()
      {
        return ++this.documentPathIndex < this.documentPaths.Count;
      }
    }
  }
}
