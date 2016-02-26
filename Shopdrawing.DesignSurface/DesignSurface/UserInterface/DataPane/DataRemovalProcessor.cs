// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataRemovalProcessor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal abstract class DataRemovalProcessor : AsyncDataXamlProcessor
  {
    private List<DataRemovalProcessor.DocumentNodeInfo> documentNodesRemove = new List<DataRemovalProcessor.DocumentNodeInfo>();
    private bool secondPass;

    internal abstract string TransactionDescription { get; }

    public override int CompletedCount
    {
      get
      {
        return base.CompletedCount * 2;
      }
    }

    public override int Count
    {
      get
      {
        return base.Count + (this.secondPass ? base.CompletedCount : 0);
      }
    }

    public DataRemovalProcessor(IAsyncMechanism asyncMechanism, IProjectContext projectContext, ChangeProcessingModes processingMode)
      : base(asyncMechanism, projectContext, processingMode)
    {
    }

    internal abstract bool ShouldRemoveNode(DocumentCompositeNode documentNode);

    internal virtual void PostApplyChanges(SceneDocument sceneDocument)
    {
    }

    protected override bool MoveNext()
    {
      if (this.IsKilled)
        return false;
      bool flag = base.MoveNext();
      if (!flag && !this.secondPass)
      {
        this.secondPass = true;
        this.Reset();
        flag = base.MoveNext();
      }
      return flag;
    }

    protected override void Work()
    {
      if (this.IsKilled || !this.ShouldProcessCurrentDocument)
        return;
      SceneDocument sceneDocument = this.GetSceneDocument(this.CurrentDocument, true);
      if (sceneDocument == null)
        return;
      if (this.IsCollectingChanges && !this.secondPass)
        this.FindNodesToDelete(new DataRemovalProcessor.ProcessingContext(sceneDocument));
      if (!this.IsApplyingChanges || !this.secondPass)
        return;
      this.ApplyChanges(sceneDocument);
    }

    private void FindNodesToDelete(DataRemovalProcessor.ProcessingContext context)
    {
      this.CheckForDeletion(context);
      if (context.IsDeleted)
        return;
      foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, DocumentNode>>) context.NodeInfo.Node.Properties)
      {
        DocumentCompositeNode node = keyValuePair.Value as DocumentCompositeNode;
        if (node != null)
        {
          this.FindNodesToDelete(new DataRemovalProcessor.ProcessingContext(context, node, keyValuePair.Key));
          if (context.IsDeleted)
            return;
        }
      }
      if (!context.NodeInfo.Node.SupportsChildren)
        return;
      for (int index = 0; index < context.NodeInfo.Node.Children.Count; ++index)
      {
        DocumentCompositeNode node = context.NodeInfo.Node.Children[index] as DocumentCompositeNode;
        if (node != null)
        {
          this.FindNodesToDelete(new DataRemovalProcessor.ProcessingContext(context, node, index));
          if (context.IsDeleted)
            break;
        }
      }
    }

    private static DataRemovalProcessor.ProcessingContext GetAncestorContext(DataRemovalProcessor.ProcessingContext context, int maxLevel, ITypeId ancestorType)
    {
      int num = maxLevel;
      for (DataRemovalProcessor.ProcessingContext parentContext = context.ParentContext; parentContext != null && num > 0; --num)
      {
        if (ancestorType.IsAssignableFrom((ITypeId) parentContext.NodeInfo.Node.Type))
          return parentContext;
        parentContext = parentContext.ParentContext;
      }
      return (DataRemovalProcessor.ProcessingContext) null;
    }

    private static DataRemovalProcessor.ProcessingContext GetAncestorContextToDelete(DataRemovalProcessor.ProcessingContext context)
    {
      return DataRemovalProcessor.GetAncestorContext(context, 1, PlatformTypes.DictionaryEntry) ?? DataRemovalProcessor.GetAncestorContext(context, 3, PlatformTypes.Binding) ?? (DataRemovalProcessor.ProcessingContext) null;
    }

    private void CheckForDeletion(DataRemovalProcessor.ProcessingContext context)
    {
      if (!this.ShouldRemoveNode(context.NodeInfo.Node))
        return;
      DataRemovalProcessor.ProcessingContext processingContext1 = DataRemovalProcessor.GetAncestorContextToDelete(context);
      if (processingContext1 == null)
      {
        processingContext1 = context;
      }
      else
      {
        for (DataRemovalProcessor.ProcessingContext processingContext2 = context; processingContext2 != processingContext1; processingContext2 = processingContext2.ParentContext)
          processingContext2.IsDeleted = true;
      }
      processingContext1.IsDeleted = true;
      this.documentNodesRemove.Add(processingContext1.NodeInfo);
    }

    private void ApplyChanges(SceneDocument sceneDocument)
    {
      List<DataRemovalProcessor.DocumentNodeInfo> list = new List<DataRemovalProcessor.DocumentNodeInfo>();
      for (int index = this.documentNodesRemove.Count - 1; index >= 0; --index)
      {
        DataRemovalProcessor.DocumentNodeInfo documentNodeInfo = this.documentNodesRemove[index];
        if (documentNodeInfo.Node.DocumentRoot == this.CurrentDocument.DocumentRoot)
        {
          list.Add(documentNodeInfo);
          this.documentNodesRemove.RemoveAt(index);
        }
      }
      if (this.IsKilled || list.Count == 0)
        return;
      list.Sort((Comparison<DataRemovalProcessor.DocumentNodeInfo>) ((left, right) =>
      {
        if (left.Index == right.Index)
          return 0;
        return left.Index >= right.Index ? -1 : 1;
      }));
      using (SceneEditTransaction editTransaction = sceneDocument.CreateEditTransaction(this.TransactionDescription))
      {
        using (this.GetSceneView(sceneDocument).ViewModel.AnimationEditor.DeferKeyFraming())
        {
          foreach (DataRemovalProcessor.DocumentNodeInfo documentNodeInfo in list)
          {
            DocumentCompositeNode parent = documentNodeInfo.Node.Parent;
            if (documentNodeInfo.Property != null)
              parent.Properties[(IPropertyId) documentNodeInfo.Property] = (DocumentNode) null;
            else if (documentNodeInfo.Index >= 0)
              parent.Children.RemoveAt(documentNodeInfo.Index);
          }
          this.PostApplyChanges(sceneDocument);
          editTransaction.Commit();
        }
      }
    }

    private class DocumentNodeInfo
    {
      public DocumentCompositeNode Node { get; private set; }

      public IProperty Property { get; private set; }

      public int Index { get; private set; }

      public DocumentNodeInfo(DocumentCompositeNode node, IProperty property)
      {
        this.Node = node;
        this.Property = property;
        this.Index = -1;
      }

      public DocumentNodeInfo(DocumentCompositeNode node, int index)
      {
        this.Node = node;
        this.Property = (IProperty) null;
        this.Index = index;
      }

      public override string ToString()
      {
        if (this.Property != null)
          return this.Node.ToString() + (object) " - " + (string) (object) this.Node.Parent + "[" + (string) (object) this.Property + "]";
        if (this.Index < 0)
          return this.Node.ToString();
        return this.Node.ToString() + (object) " - " + (string) (object) this.Node.Parent + "[" + (string) (object) this.Index + "]";
      }
    }

    private class ProcessingContext
    {
      public DataRemovalProcessor.ProcessingContext ParentContext { get; private set; }

      public DataRemovalProcessor.DocumentNodeInfo NodeInfo { get; private set; }

      public bool IsDeleted { get; set; }

      public ProcessingContext(SceneDocument document)
      {
        this.ParentContext = (DataRemovalProcessor.ProcessingContext) null;
        this.NodeInfo = new DataRemovalProcessor.DocumentNodeInfo(document.DocumentRoot.RootNode as DocumentCompositeNode, -1);
      }

      public ProcessingContext(DataRemovalProcessor.ProcessingContext parent, DocumentCompositeNode node, IProperty property)
      {
        this.ParentContext = parent;
        this.NodeInfo = new DataRemovalProcessor.DocumentNodeInfo(node, property);
      }

      public ProcessingContext(DataRemovalProcessor.ProcessingContext parent, DocumentCompositeNode node, int index)
      {
        this.ParentContext = parent;
        this.NodeInfo = new DataRemovalProcessor.DocumentNodeInfo(node, index);
      }

      public override string ToString()
      {
        if (this.IsDeleted)
          return this.NodeInfo.ToString() + " [deleted]";
        return this.NodeInfo.ToString();
      }
    }
  }
}
