// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.ReferenceRepairProcessor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal class ReferenceRepairProcessor : DocumentProcessor
  {
    private ReferenceChangeModel referenceChangeModel;

    public override string StatusText
    {
      get
      {
        return StringTable.ReferencesFixupGenericStatusText;
      }
    }

    protected override DocumentSearchScope SearchScope
    {
      get
      {
        return this.referenceChangeModel.ChangeScope;
      }
    }

    public ReferenceRepairProcessor(DesignerContext designerContext, ReferenceChangeModel referenceChangeModel)
      : this(designerContext, referenceChangeModel, (IAsyncMechanism) new CurrentDispatcherAsyncMechanism(DispatcherPriority.Background))
    {
    }

    public ReferenceRepairProcessor(DesignerContext designerContext, ReferenceChangeModel referenceChangeModel, IAsyncMechanism asyncMechanism)
      : base(designerContext, asyncMechanism)
    {
      this.referenceChangeModel = referenceChangeModel;
      this.Begun += new EventHandler(this.OnReferenceRepairProcessorBegun);
      this.Complete += new EventHandler(this.OnReferenceRepairProcessorComplete);
    }

    private void OnReferenceRepairProcessorBegun(object sender, EventArgs e)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.RepairNameReferences);
    }

    private void OnReferenceRepairProcessorComplete(object sender, EventArgs e)
    {
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.RepairNameReferences);
    }

    protected override void ProcessDocument(SceneDocument document)
    {
      DocumentNode rootNode = document.DocumentRoot.RootNode;
      foreach (ReferenceRepairer referenceRepairer in this.referenceChangeModel.ReferenceRepairers)
      {
        foreach (DocumentNode node in rootNode.SelectDescendantNodes(referenceRepairer.AppliesTo))
          referenceRepairer.Repair(node);
      }
    }
  }
}
