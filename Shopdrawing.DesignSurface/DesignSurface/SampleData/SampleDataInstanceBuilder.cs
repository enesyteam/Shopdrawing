// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleDataInstanceBuilder
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SampleDataInstanceBuilder : ClrObjectInstanceBuilder
  {
    public override Type BaseType
    {
      get
      {
        return typeof (DesignTimeSampleRootType);
      }
    }

    public static void EnsureRegistered(IPlatform platform)
    {
      if (!(platform.InstanceBuilderFactory.GetBuilder(typeof (DesignTimeSampleRootType)).GetType() != typeof (SampleDataInstanceBuilder)))
        return;
      platform.InstanceBuilderFactory.Register((IInstanceBuilder) new SampleDataInstanceBuilder());
      platform.InstanceBuilderFactory.GetBuilder(typeof (DesignTimeSampleRootType));
    }

    public override bool Instantiate(IInstanceBuilderContext context, ViewNode viewNode)
    {
      bool flag = viewNode.Instance != null;
      base.Instantiate(context, viewNode);
      if (flag)
        return false;
      SampleDataSet sampleDataSet = SampleDataSet.SampleDataSetFromType(viewNode.DocumentNode.Type.RuntimeType);
      if (sampleDataSet == null)
        return !flag;
      DocumentCompositeNode documentCompositeNode = sampleDataSet != null ? sampleDataSet.ValidRootNodeFromXamlDocument : (DocumentCompositeNode) null;
      if (documentCompositeNode == null || !PlatformTypes.PlatformsCompatible((IPlatformMetadata) context.Platform.Metadata, documentCompositeNode.PlatformMetadata) || (this.IsSampleDataXamlContext(context, documentCompositeNode.DocumentRoot) || DesignDataHelper.GetDesignDataFile(viewNode.DocumentNode) != null))
        return !flag;
      using (StandaloneInstanceBuilderContext instanceBuilderContext = new StandaloneInstanceBuilderContext(context.DocumentContext, context))
      {
        using (instanceBuilderContext.ChangeSerializationContext(context.SerializationContext))
        {
          ViewNode viewNode1 = this.GetViewNode((IInstanceBuilderContext) instanceBuilderContext, (DocumentNode) documentCompositeNode);
          viewNode1.Instance = viewNode.Instance;
          this.Initialize((IInstanceBuilderContext) instanceBuilderContext, viewNode1, true);
          viewNode1.Instance = (object) null;
          viewNode.InstanceState = InstanceState.Valid;
        }
      }
      context.DocumentRootResolver.GetDocumentRoot(documentCompositeNode.DocumentRoot.DocumentContext.DocumentUrl);
      IProperty index = context.DocumentContext.TypeResolver.ResolveProperty(DesignTimeProperties.SampleDataTagProperty);
      ViewNode referenceSource = new ViewNode(context.ViewNodeManager, (DocumentNode) documentCompositeNode);
      viewNode.Properties[index] = referenceSource;
      context.ViewNodeManager.AddRelatedDocumentRoot(referenceSource, documentCompositeNode.DocumentRoot);
      return !flag;
    }

    public override void OnViewNodeInvalidating(IInstanceBuilderContext context, ViewNode target, ViewNode child, ref bool doesInvalidRootsContainTarget, List<ViewNode> invalidRoots)
    {
      base.OnViewNodeInvalidating(context, target, child, ref doesInvalidRootsContainTarget, invalidRoots);
      DocumentCompositeNode fromXamlDocument = SampleDataSet.SampleDataSetFromType(target.DocumentNode.Type.RuntimeType).ValidRootNodeFromXamlDocument;
      if (fromXamlDocument == null || !this.IsSampleDataXamlContext(context, fromXamlDocument.DocumentRoot))
        return;
      InstanceBuilderOperations.SetInvalid(context, target, ref doesInvalidRootsContainTarget, invalidRoots);
    }

    private bool IsSampleDataXamlContext(IInstanceBuilderContext context, IDocumentRoot sampleDataXamlDocumentRoot)
    {
      return sampleDataXamlDocumentRoot != null && context.ContainerRoot != null && context.ContainerRoot.DocumentNode.DocumentRoot == sampleDataXamlDocumentRoot;
    }
  }
}
