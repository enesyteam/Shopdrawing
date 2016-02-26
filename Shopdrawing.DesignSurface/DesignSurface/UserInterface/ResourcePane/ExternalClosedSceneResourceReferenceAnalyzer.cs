// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ExternalClosedSceneResourceReferenceAnalyzer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public class ExternalClosedSceneResourceReferenceAnalyzer : AsyncProcess
  {
    private int currentProjectDocument = -1;
    private ReferencesFoundModel model;
    private IList<IProjectDocument> projectDocuments;

    public override int Count
    {
      get
      {
        return this.projectDocuments.Count;
      }
    }

    public override int CompletedCount
    {
      get
      {
        return Math.Max(this.currentProjectDocument, 0);
      }
    }

    public override string StatusText
    {
      get
      {
        return StringTable.ReferencesFixupGenericStatusText;
      }
    }

    public ExternalClosedSceneResourceReferenceAnalyzer(ReferencesFoundModel model)
      : base((IAsyncMechanism) new CurrentDispatcherAsyncMechanism(DispatcherPriority.Background))
    {
      this.model = model;
      this.projectDocuments = (IList<IProjectDocument>) new List<IProjectDocument>((IEnumerable<IProjectDocument>) model.ResourceEntryNode.ProjectContext.Documents);
    }

    public override void Reset()
    {
      this.currentProjectDocument = -1;
    }

    protected override void Work()
    {
      IProjectDocument projectDocument1 = this.projectDocuments[this.currentProjectDocument];
      if (projectDocument1 == null || projectDocument1.Document != null)
        return;
      IProjectDocument projectDocument2 = this.model.ResourceEntryNode.ProjectContext.OpenDocument(projectDocument1.Path);
      if (projectDocument2 == null)
        return;
      SceneDocument document = projectDocument2.Document as SceneDocument;
      if (document == null || !document.IsEditable)
        return;
      IDocumentRoot documentRoot = document.DocumentRoot;
      DocumentNode searchKey = (DocumentNode) null;
      SceneNode keyNode = this.model.ResourceEntryNode.KeyNode;
      if (keyNode != null)
        searchKey = keyNode.DocumentNode;
      foreach (DocumentNode documentNode1 in documentRoot.RootNode.SelectDescendantNodes((Predicate<DocumentNode>) (node =>
      {
        DocumentCompositeNode node1 = node as DocumentCompositeNode;
        return node1 != null && node1.Type.IsResource && ResourceNodeHelper.GetResourceKey(node1).Equals(searchKey);
      })))
      {
        ITypeMetadataFactory metadataFactory = this.model.ResourceEntryNode.ProjectContext.MetadataFactory;
        IType type = documentNode1.TypeResolver.ResolveType(PlatformTypes.FrameworkElement);
        DocumentCompositeNode documentCompositeNode = documentNode1.SelectFirstAncestorNode(type.RuntimeType) as DocumentCompositeNode;
        string caption = document.Caption;
        string str;
        if (documentCompositeNode != null)
        {
          IPropertyId index = (IPropertyId) metadataFactory.GetMetadata(documentCompositeNode.TargetType).NameProperty;
          DocumentNode documentNode2 = documentCompositeNode.Properties[index];
          if (documentNode2 != null)
          {
            DocumentPrimitiveNode documentPrimitiveNode = documentNode2 as DocumentPrimitiveNode;
            str = documentPrimitiveNode == null ? documentNode2.ToString() : documentPrimitiveNode.Value.ToString();
          }
          else
            str = documentCompositeNode.ToString();
        }
        else
          str = documentNode1.ToString();
        this.model.ReferenceNames.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ReferencesFoundFormat, new object[2]
        {
          (object) str,
          (object) caption
        }));
        this.model.AddReferencesFile(document);
      }
      this.model.ResourceEntryNode.ViewModel.DesignerContext.DocumentService.CloseDocument((IDocument) document);
    }

    protected override bool MoveNext()
    {
      return ++this.currentProjectDocument < this.projectDocuments.Count;
    }
  }
}
