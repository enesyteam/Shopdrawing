// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.TopicHelpContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Runtime.Versioning;

namespace Microsoft.Expression.DesignSurface
{
  public class TopicHelpContext
  {
    public string TopicIdentifier { get; private set; }

    public FrameworkName Framework { get; private set; }

    private TopicHelpContext(IType type, FrameworkName framework)
    {
      this.TopicIdentifier = TopicHelpContext.ExtractTypeName(type);
      this.Framework = framework;
    }

    private TopicHelpContext(string topicIdentifier, FrameworkName framework)
    {
      this.TopicIdentifier = topicIdentifier;
      this.Framework = framework;
    }

    internal static TopicHelpContext CreateContextFromParameter(object parameter, DesignerContext designerContext)
    {
      TopicHelpContext topicHelpContext = (TopicHelpContext) null;
      if (parameter == null)
      {
        SceneNode[] selectedNodes = designerContext.SelectionManager.SelectedNodes;
        if (selectedNodes != null && selectedNodes.Length > 0)
          topicHelpContext = TopicHelpContext.CreateContext((object) selectedNodes[0].Type, selectedNodes[0].Type.PlatformMetadata.TargetFramework);
      }
      else
        topicHelpContext = TopicHelpContext.CreateContext(parameter, designerContext.ActiveProjectContext.TargetFramework);
      return topicHelpContext;
    }

    private static TopicHelpContext CreateContext(object topicObject, FrameworkName framework)
    {
      IType type;
      if ((type = topicObject as IType) != null)
        return new TopicHelpContext(type, framework);
      string topicIdentifier;
      if ((topicIdentifier = topicObject as string) != null)
        return new TopicHelpContext(topicIdentifier, framework);
      return new TopicHelpContext(string.Empty, framework);
    }

    private static string ExtractTypeName(IType type)
    {
      if (type == null)
        return string.Empty;
      return type.FullName;
    }
  }
}
