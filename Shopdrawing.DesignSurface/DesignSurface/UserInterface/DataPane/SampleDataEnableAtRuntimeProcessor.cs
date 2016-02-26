// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SampleDataEnableAtRuntimeProcessor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class SampleDataEnableAtRuntimeProcessor : AsyncDataXamlProcessor
  {
    private List<DocumentCompositeNode> documentNodesToModify = new List<DocumentCompositeNode>();

    public SampleDataSet SampleData { get; private set; }

    public SampleDataEnableAtRuntimeProcessor(IAsyncMechanism asyncMechanism, SampleDataSet dataSet, ChangeProcessingModes processingMode)
      : base(asyncMechanism, (IProjectContext) dataSet.ProjectContext, processingMode)
    {
      this.SampleData = dataSet;
    }

    protected override void Work()
    {
      if (this.IsKilled || !this.ShouldProcessCurrentDocument)
        return;
      SceneDocument sceneDocument = this.GetSceneDocument(this.CurrentDocument, true);
      if (sceneDocument == null)
        return;
      if (this.IsCollectingChanges)
        this.FindDataContextOwnersToModify(sceneDocument);
      if (!this.IsApplyingChanges)
        return;
      this.ApplyChanges(sceneDocument);
    }

    private void FindDataContextOwnersToModify(SceneDocument sceneDocument)
    {
      if (this.IsKilled)
        return;
      DocumentNode rootNode = sceneDocument.DocumentRoot.RootNode;
      bool enabledAtRuntime = this.SampleData.IsEnabledAtRuntime;
      this.ProcessPotentialDataContextOwner(rootNode, enabledAtRuntime);
      foreach (DocumentNode documentNode in rootNode.DescendantNodes)
        this.ProcessPotentialDataContextOwner(documentNode, enabledAtRuntime);
    }

    private void ProcessPotentialDataContextOwner(DocumentNode documentNode, bool needDesignTimeProperty)
    {
      IProperty dataContextProperty = DataContextHelper.GetDataContextProperty(documentNode.Type, needDesignTimeProperty);
      if (dataContextProperty == null)
        return;
      DocumentCompositeNode documentCompositeNode1 = documentNode as DocumentCompositeNode;
      if (documentCompositeNode1 == null)
        return;
      DocumentCompositeNode documentCompositeNode2 = documentCompositeNode1.Properties[(IPropertyId) dataContextProperty] as DocumentCompositeNode;
      if (documentCompositeNode2 == null)
        return;
      RawDataSourceInfoBase rawDataSourceInfo = DataContextHelper.GetRawDataSourceInfo((DocumentNode) documentCompositeNode2);
      if (!rawDataSourceInfo.HasSource || !rawDataSourceInfo.IsValidClr || !this.SampleData.IsTypeOwner(rawDataSourceInfo.SourceType))
        return;
      this.documentNodesToModify.Add(documentCompositeNode1);
    }

    private void ApplyChanges(SceneDocument sceneDocument)
    {
      List<DocumentCompositeNode> list = new List<DocumentCompositeNode>();
      for (int index = this.documentNodesToModify.Count - 1; index >= 0; --index)
      {
        DocumentCompositeNode documentCompositeNode = this.documentNodesToModify[index];
        if (documentCompositeNode.DocumentRoot == this.CurrentDocument.DocumentRoot)
        {
          list.Add(documentCompositeNode);
          this.documentNodesToModify.RemoveAt(index);
        }
      }
      if (this.IsKilled || list.Count == 0)
        return;
      SceneView sceneView = this.GetSceneView(sceneDocument);
      string description = string.Format((IFormatProvider) CultureInfo.CurrentCulture, this.SampleData.IsEnabledAtRuntime ? StringTable.SampleDataEnableTransaction : StringTable.SampleDataDisableTransaction, new object[1]
      {
        (object) this.SampleData.Name
      });
      using (SceneEditTransaction editTransaction = sceneDocument.CreateEditTransaction(description))
      {
        using (sceneView.ViewModel.AnimationEditor.DeferKeyFraming())
        {
          foreach (DocumentCompositeNode documentCompositeNode in list)
          {
            IProperty dataContextProperty = DataContextHelper.GetDataContextProperty(documentCompositeNode.Type);
            IProperty property = DesignTimeProperties.ResolveDesignTimePropertyKey(DesignTimeProperties.DesignDataContextProperty, documentCompositeNode.PlatformMetadata);
            DocumentNode documentNode1 = documentCompositeNode.Properties[(IPropertyId) dataContextProperty];
            DocumentNode documentNode2 = documentCompositeNode.Properties[(IPropertyId) property];
            SceneNode sceneNode = sceneView.ViewModel.GetSceneNode((DocumentNode) documentCompositeNode);
            if (documentNode2 == null)
              sceneNode.ClearValue((IPropertyId) dataContextProperty);
            else
              sceneNode.SetValue((IPropertyId) dataContextProperty, (object) documentNode2);
            if (documentNode1 == null)
              sceneNode.ClearValue((IPropertyId) property);
            else
              sceneNode.SetValue((IPropertyId) property, (object) documentNode1);
          }
          editTransaction.Commit();
        }
      }
    }
  }
}
