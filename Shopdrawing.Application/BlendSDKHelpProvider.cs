// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.BlendSDKHelpProvider
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Project;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace Shopdrawing.App
{
  internal class BlendSDKHelpProvider : IHelpProvider
  {
    private static Dictionary<string, string> silverlightHelpTopicLookup = new Dictionary<string, string>()
    {
      {
        ProjectNeutralTypes.InvokeCommandAction.FullName,
        "d47180c6-2c4c-4f4f-8b42-0ebdd7ccfe1c"
      },
      {
        ProjectNeutralTypes.FluidMoveBehavior.FullName,
        "47182d64-77f6-4ac3-8bc0-150a02f99bc2"
      },
      {
        ProjectNeutralTypes.DataTrigger.FullName,
        "cbad7e34-89a9-4ca8-b016-1e868a8cba0b"
      },
      {
        ProjectNeutralTypes.NavigateBackAction.FullName,
        "1a27c1aa-4df9-4ce1-ad16-200815547db0"
      },
      {
        ProjectNeutralTypes.PlaySoundAction.FullName,
        "a38a71fd-189b-4f34-9a56-2478d6c97c56"
      },
      {
        ProjectNeutralTypes.NavigateForwardAction.FullName,
        "a8c5e733-070b-4d2b-818c-2f229b40c6ef"
      },
      {
        ProjectNeutralTypes.StoryboardCompletedTrigger.FullName,
        "08a176f9-a3e5-44bb-ba08-436cc30441f2"
      },
      {
        ProjectNeutralTypes.KeyTrigger.FullName,
        "b767b3a1-98c1-4ac1-91bd-4bcbcd3c5277"
      },
      {
        ProjectNeutralTypes.TimerTrigger.FullName,
        "8cb51b98-c349-4385-9d86-5181193cac09"
      },
      {
        ProjectNeutralTypes.SketchFlowAnimationTrigger.FullName,
        "bcdc85e9-89df-409a-9a37-5c863b3f46c5"
      },
      {
        ProjectNeutralTypes.ChangePropertyAction.FullName,
        "5ec8d7ba-4667-4650-8c99-60f9dab4e1a5"
      },
      {
        ProjectNeutralTypes.RemoveItemInListBoxAction.FullName,
        "fd5c80e6-9f19-4685-afe1-61167ac6e0d3"
      },
      {
        ProjectNeutralTypes.GoToStateAction.FullName,
        "637091ab-2316-4d45-92a1-65f7ee85087f"
      },
      {
        ProjectNeutralTypes.FluidMoveSetTagBehavior.FullName,
        "f30f50c1-b3e5-4ba3-83de-688bf7d266f8"
      },
      {
        ProjectNeutralTypes.RemoveElementAction.FullName,
        "38a0f5e3-7b9b-463c-970d-6e44ef67fd2c"
      },
      {
        ProjectNeutralTypes.HyperlinkAction.FullName,
        "111a6803-3989-43de-a94b-73b1926ea62a"
      },
      {
        ProjectNeutralTypes.CallMethodAction.FullName,
        "50081f20-9186-4bca-86b4-8769d52e0dd4"
      },
      {
        ProjectNeutralTypes.NavigateToScreenAction.FullName,
        "78085fc0-579b-4097-88ef-8b929415941c"
      },
      {
        ProjectNeutralTypes.ControlStoryboardAction.FullName,
        "b1a51861-d57f-4ed4-8c2b-b13e9e2352b9"
      },
      {
        ProjectNeutralTypes.PlaySketchFlowAnimationAction.FullName,
        "7cca093b-5ef1-4b7c-9ff6-b156767c34cc"
      },
      {
        ProjectNeutralTypes.TranslateZoomRotateBehavior.FullName,
        "9c54bafa-49e1-49de-805c-b1b9d07d3e45"
      },
      {
        ProjectNeutralTypes.BehaviorEventTrigger.FullName,
        "bddfd638-9511-49a7-811a-c7237a40d726"
      },
      {
        ProjectNeutralTypes.NavigationMenuAction.FullName,
        "7a3b0f9a-d5ac-48cc-80a8-c8a14741269f"
      },
      {
        ProjectNeutralTypes.DataStateBehavior.FullName,
        "6098e180-5592-4dd2-ba62-e661903c3ff3"
      },
      {
        ProjectNeutralTypes.DataStoreChangedTrigger.FullName,
        "7b04ba06-724f-42a0-a896-f65266f93d62"
      },
      {
        ProjectNeutralTypes.SetDataStoreValueAction.FullName,
        "b3bfb980-c23d-49cb-90c0-fa0cc1cda995"
      },
      {
        ProjectNeutralTypes.MouseDragElementBehavior.FullName,
        "13f7c56c-44a9-4475-9bc5-fd135266897e"
      },
      {
        ProjectNeutralTypes.ActivateStateAction.FullName,
        "2b2d0aff-9b27-40ea-b069-cffe3401cff4"
      },
      {
        ProjectNeutralTypes.PropertyChangedTrigger.FullName,
        "c00c9453-c62e-4cef-bb5f-a1e473dc408a"
      },
      {
        CategoryNames.CategoryConditions.ToString(),
        "8cec2cc5-a155-45d0-854d-cf41550e2512"
      }
    };
    private static Dictionary<string, string> wpfHelpTopicLookup = new Dictionary<string, string>()
    {
      {
        ProjectNeutralTypes.InvokeCommandAction.FullName,
        "d47180c6-2c4c-4f4f-8b42-0ebdd7ccfe1c"
      },
      {
        ProjectNeutralTypes.FluidMoveBehavior.FullName,
        "47182d64-77f6-4ac3-8bc0-150a02f99bc2"
      },
      {
        ProjectNeutralTypes.DataTrigger.FullName,
        "cbad7e34-89a9-4ca8-b016-1e868a8cba0b"
      },
      {
        ProjectNeutralTypes.NavigateBackAction.FullName,
        "1a27c1aa-4df9-4ce1-ad16-200815547db0"
      },
      {
        ProjectNeutralTypes.PlaySoundAction.FullName,
        "a38a71fd-189b-4f34-9a56-2478d6c97c56"
      },
      {
        ProjectNeutralTypes.NavigateForwardAction.FullName,
        "a8c5e733-070b-4d2b-818c-2f229b40c6ef"
      },
      {
        ProjectNeutralTypes.StoryboardCompletedTrigger.FullName,
        "08a176f9-a3e5-44bb-ba08-436cc30441f2"
      },
      {
        ProjectNeutralTypes.KeyTrigger.FullName,
        "b767b3a1-98c1-4ac1-91bd-4bcbcd3c5277"
      },
      {
        ProjectNeutralTypes.TimerTrigger.FullName,
        "8cb51b98-c349-4385-9d86-5181193cac09"
      },
      {
        ProjectNeutralTypes.SketchFlowAnimationTrigger.FullName,
        "bcdc85e9-89df-409a-9a37-5c863b3f46c5"
      },
      {
        ProjectNeutralTypes.ChangePropertyAction.FullName,
        "5ec8d7ba-4667-4650-8c99-60f9dab4e1a5"
      },
      {
        ProjectNeutralTypes.RemoveItemInListBoxAction.FullName,
        "fd5c80e6-9f19-4685-afe1-61167ac6e0d3"
      },
      {
        ProjectNeutralTypes.GoToStateAction.FullName,
        "637091ab-2316-4d45-92a1-65f7ee85087f"
      },
      {
        ProjectNeutralTypes.FluidMoveSetTagBehavior.FullName,
        "f30f50c1-b3e5-4ba3-83de-688bf7d266f8"
      },
      {
        ProjectNeutralTypes.RemoveElementAction.FullName,
        "38a0f5e3-7b9b-463c-970d-6e44ef67fd2c"
      },
      {
        ProjectNeutralTypes.CallMethodAction.FullName,
        "50081f20-9186-4bca-86b4-8769d52e0dd4"
      },
      {
        ProjectNeutralTypes.NavigateToScreenAction.FullName,
        "78085fc0-579b-4097-88ef-8b929415941c"
      },
      {
        ProjectNeutralTypes.ControlStoryboardAction.FullName,
        "b1a51861-d57f-4ed4-8c2b-b13e9e2352b9"
      },
      {
        ProjectNeutralTypes.PlaySketchFlowAnimationAction.FullName,
        "7cca093b-5ef1-4b7c-9ff6-b156767c34cc"
      },
      {
        ProjectNeutralTypes.TranslateZoomRotateBehavior.FullName,
        "9c54bafa-49e1-49de-805c-b1b9d07d3e45"
      },
      {
        ProjectNeutralTypes.BehaviorEventTrigger.FullName,
        "bddfd638-9511-49a7-811a-c7237a40d726"
      },
      {
        ProjectNeutralTypes.NavigationMenuAction.FullName,
        "7a3b0f9a-d5ac-48cc-80a8-c8a14741269f"
      },
      {
        ProjectNeutralTypes.DataStateBehavior.FullName,
        "6098e180-5592-4dd2-ba62-e661903c3ff3"
      },
      {
        ProjectNeutralTypes.LaunchUriOrFileAction.FullName,
        "80ad69a7-eaa1-411a-8841-f4f785f7c34e"
      },
      {
        ProjectNeutralTypes.DataStoreChangedTrigger.FullName,
        "7b04ba06-724f-42a0-a896-f65266f93d62"
      },
      {
        ProjectNeutralTypes.SetDataStoreValueAction.FullName,
        "b3bfb980-c23d-49cb-90c0-fa0cc1cda995"
      },
      {
        ProjectNeutralTypes.MouseDragElementBehavior.FullName,
        "13f7c56c-44a9-4475-9bc5-fd135266897e"
      },
      {
        ProjectNeutralTypes.ActivateStateAction.FullName,
        "2b2d0aff-9b27-40ea-b069-cffe3401cff4"
      },
      {
        ProjectNeutralTypes.PropertyChangedTrigger.FullName,
        "c00c9453-c62e-4cef-bb5f-a1e473dc408a"
      },
      {
        CategoryNames.CategoryConditions.ToString(),
        "8cec2cc5-a155-45d0-854d-cf41550e2512"
      }
    };

    public bool IsHelpAvailable(object context)
    {
      TopicHelpContext topicHelpContext = context as TopicHelpContext;
      if (topicHelpContext != null && !string.IsNullOrEmpty(this.DetermineHelpTopic(topicHelpContext.TopicIdentifier, topicHelpContext.Framework)))
        return this.GetBlendSDKHelp(topicHelpContext.Framework).Available;
      return false;
    }

    public void ProvideHelp(object context)
    {
      TopicHelpContext topicHelpContext = context as TopicHelpContext;
      if (topicHelpContext == null)
        return;
      string helpTopic = this.DetermineHelpTopic(topicHelpContext.TopicIdentifier, topicHelpContext.Framework);
      if (string.IsNullOrEmpty(helpTopic))
        return;
      this.ShowSDKHelpTopic(helpTopic, topicHelpContext.Framework);
    }

    private string DetermineHelpTopic(string topicIdentifier, FrameworkName framework)
    {
      if (string.IsNullOrEmpty(topicIdentifier))
        return string.Empty;
      Dictionary<string, string> dictionary = (Dictionary<string, string>) null;
      if (Microsoft.Expression.Project.FrameworkNameEqualityComparer.Instance.Equals(framework, BlendSdkHelper.CurrentSilverlightVersion, true, false, false))
        dictionary = BlendSDKHelpProvider.silverlightHelpTopicLookup;
      else if (Microsoft.Expression.Project.FrameworkNameEqualityComparer.Instance.Equals(framework, BlendSdkHelper.CurrentWpfVersion, true, false, false))
        dictionary = BlendSDKHelpProvider.wpfHelpTopicLookup;
      string str;
      if (dictionary != null && dictionary.TryGetValue(topicIdentifier, out str))
        return str;
      return string.Empty;
    }

    private void ShowSDKHelpTopic(string helpTopic, FrameworkName framework)
    {
      BlendSdkHelp blendSdkHelp = this.GetBlendSDKHelp(framework);
      if (!blendSdkHelp.Available)
        return;
      blendSdkHelp.ShowHelpTopic(this.BuildHelpTopicQuery(helpTopic));
    }

    private BlendSdkHelp GetBlendSDKHelp(FrameworkName framework)
    {
      return BlendSdkHelp.GetInstanceForFramework(this.GetForwardedFrameworkName(framework));
    }

    private FrameworkName GetForwardedFrameworkName(FrameworkName framework)
    {
      if (Microsoft.Expression.Project.FrameworkNameEqualityComparer.AreEquivalent(framework, BlendSdkHelper.Wpf35))
        return BlendSdkHelper.Wpf4;
      if (Microsoft.Expression.Project.FrameworkNameEqualityComparer.AreEquivalent(framework, BlendSdkHelper.Silverlight3))
        return BlendSdkHelper.Silverlight4;
      return framework;
    }

    private string BuildHelpTopicQuery(string helpTopic)
    {
      return "/html/" + helpTopic + ".htm";
    }
  }
}
