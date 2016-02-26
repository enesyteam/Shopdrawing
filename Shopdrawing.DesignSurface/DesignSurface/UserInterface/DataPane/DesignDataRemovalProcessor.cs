// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DesignDataRemovalProcessor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal class DesignDataRemovalProcessor : DataRemovalProcessor
  {
    private string designDataFile;

    internal override string TransactionDescription
    {
      get
      {
        return StringTable.UndoUnitRemoveDataSource;
      }
    }

    public DesignDataRemovalProcessor(IAsyncMechanism asyncMechanism, string designDataFile, IProjectContext projectContext, ChangeProcessingModes processingMode)
      : base(asyncMechanism, projectContext, processingMode)
    {
      this.designDataFile = designDataFile;
    }

    internal override bool ShouldRemoveNode(DocumentCompositeNode documentNode)
    {
      if (documentNode.Type.RuntimeType == typeof (DesignDataExtension))
        return PathHelper.ArePathsEquivalent(DesignDataInstanceBuilder.GetSourceFilePath(documentNode), this.designDataFile);
      return false;
    }
  }
}
