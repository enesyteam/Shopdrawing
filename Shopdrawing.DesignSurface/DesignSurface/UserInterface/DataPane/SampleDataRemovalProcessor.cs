// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SampleDataRemovalProcessor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.Framework;
using System;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal class SampleDataRemovalProcessor : DataRemovalProcessor
  {
    public SampleDataSet SampleData { get; private set; }

    internal override string TransactionDescription
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, this.SampleData.IsEnabledAtRuntime ? StringTable.SampleDataEnableTransaction : StringTable.SampleDataDisableTransaction, new object[1]
        {
          (object) this.SampleData.Name
        });
      }
    }

    public SampleDataRemovalProcessor(IAsyncMechanism asyncMechanism, SampleDataSet dataSet, ChangeProcessingModes processingMode)
      : base(asyncMechanism, (IProjectContext) dataSet.ProjectContext, processingMode)
    {
      this.SampleData = dataSet;
    }

    internal override bool ShouldRemoveNode(DocumentCompositeNode documentNode)
    {
      if (this.IsSampleDataTypeNode((DocumentNode) documentNode))
        return true;
      DocumentCompositeNode documentCompositeNode = this.ResolveResourceReferenceIfNeeded(documentNode);
      return documentCompositeNode != null && documentCompositeNode != documentNode && this.IsSampleDataTypeNode((DocumentNode) documentCompositeNode);
    }

    internal override void PostApplyChanges(SceneDocument sceneDocument)
    {
      sceneDocument.XamlDocument.StripExtraNamespaces(this.SampleData.XamlNamespace.Value);
    }

    private bool IsSampleDataTypeNode(DocumentNode compositeNode)
    {
      return this.SampleData.IsTypeOwner(DataContextHelper.GetDataType(compositeNode));
    }
  }
}
