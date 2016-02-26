// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.ApplicationCommandTarget
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.Code.Documents;
using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;
using System.Runtime.Versioning;

namespace Shopdrawing.App
{
  internal class ApplicationCommandTarget : CommandTarget
  {
    public ApplicationCommandTarget(IServices services)
    {
      this.AddCommand("Application_HelpTopics", (ICommand) new ApplicationCommandTarget.HelpTopicsCommand());
      this.AddCommand("Application_Website", (ICommand) new ApplicationCommandTarget.WebsiteCommand(services, "http://go.microsoft.com/fwlink/?LinkId=147811"));
      this.AddCommand("Application_OnlineForums", (ICommand) new ApplicationCommandTarget.WebsiteCommand(services, "http://go.microsoft.com/fwlink/?LinkId=147812"));
      this.AddCommand("Application_Learn", (ICommand) new ApplicationCommandTarget.WebsiteCommand(services, "http://go.microsoft.com/fwlink/?LinkId=147813"));
      this.AddCommand("Application_Downloads", (ICommand) new ApplicationCommandTarget.WebsiteCommand(services, "http://go.microsoft.com/fwlink/?LinkId=147814"));
      this.AddCommand("Application_CommunityNews", (ICommand) new ApplicationCommandTarget.WebsiteCommand(services, "http://go.microsoft.com/fwlink/?LinkId=147816"));
      this.AddCommand("Application_Gallery", (ICommand) new ApplicationCommandTarget.WebsiteCommand(services, "http://go.microsoft.com/fwlink/?LinkId=147817"));
      this.AddCommand("Application_InsideExpression", (ICommand) new ApplicationCommandTarget.WebsiteCommand(services, "http://go.microsoft.com/fwlink/?LinkId=147818"));
      this.AddCommand("Application_TeamBlog", (ICommand) new ApplicationCommandTarget.WebsiteCommand(services, "http://go.microsoft.com/fwlink/?LinkId=147819"));
      this.AddCommand("Application_RegisterProduct", (ICommand) new ApplicationCommandTarget.WebsiteCommand(services, "http://go.microsoft.com/fwlink/?LinkId=149948"));
      this.AddCommand("Application_WelcomeScreen", (ICommand) new ApplicationCommandTarget.WelcomeScreenCommand(services));
      this.AddCommand("Application_KeyboardShortcuts", (ICommand) new ApplicationCommandTarget.KeyboardShortcutsCommand());
      this.AddCommand("Application_PrivacyStatement", (ICommand) new ApplicationCommandTarget.WebsiteCommand(services, "http://go.microsoft.com/fwlink/?LinkId=185090"));
      this.AddCommand("Application_Tutorials", (ICommand) new ApplicationCommandTarget.TutorialsCommand(services));
      this.AddCommand("Application_FeedbackOptions", (ICommand) new FeedbackOptionsCommand(services));
      this.AddCommand("Application_About", (ICommand) new AboutCommand(services));
      this.AddCommand("Edit_FindInFiles", (ICommand) new FindInFilesCommand(services));
      this.AddCommand("Application_ShowControlStylingTips", (ICommand) new ShowControlStylingTipsCommand(services));
      this.AddCommand("Application_SdkHelpTopics", (ICommand) new ApplicationCommandTarget.SdkHelpTopicsCommand(services));
      this.AddCommand("Application_EnterProductKey", (ICommand) new BlendEnterProductKeyCommand(services));
      this.AddCommand("Application_Activate", (ICommand) new ActivateCommand(services));
    }

    private class WelcomeScreenCommand : Command
    {
      private IServices services;

      public WelcomeScreenCommand(IServices services)
      {
        this.services = services;
      }

      public override void Execute()
      {
        WelcomeScreen.Show(this.services);
      }
    }

    private class WebsiteCommand : Command
    {
      private IServices services;
      private Uri url;

      public WebsiteCommand(IServices services, string url)
      {
        this.services = services;
        this.url = new Uri(url, UriKind.Absolute);
      }

      public override void Execute()
      {
        WebPageLauncher.Navigate(this.url, this.services.GetService<IMessageDisplayService>());
      }
    }

    private class HelpTopicsCommand : Command
    {
      public override void Execute()
      {
        BlendHelp.Instance.ShowHelpTableOfContents();
      }
    }

    private class SdkHelpTopicsCommand : Command
    {
      private IServices services;

      protected FrameworkName Framework
      {
        get
        {
          IProjectManager service = this.services.GetService<IProjectManager>();
          DocumentView documentView = this.services.GetService<IViewService>().ActiveView as DocumentView;
          if (documentView != null)
          {
            foreach (IProject project in service.CurrentSolution.Projects)
            {
              if (project.FindItem(documentView.Document.DocumentReference) != null)
                return project.TargetFramework;
            }
          }
          return (FrameworkName) null;
        }
      }

      public override bool IsEnabled
      {
        get
        {
          if (this.Framework != (FrameworkName) null && BlendSdkHelper.IsSdkInstalled(this.Framework))
            return base.IsEnabled;
          return false;
        }
      }

      public override bool IsAvailable
      {
        get
        {
          return BlendSdkHelper.IsAnySdkInstalled;
        }
      }

      public SdkHelpTopicsCommand(IServices services)
      {
        this.services = services;
      }

      public override void Execute()
      {
        BlendSdkHelp.GetInstanceForFramework(this.Framework).ShowHelpTableOfContents();
      }
    }

    private class KeyboardShortcutsCommand : Command
    {
      public override void Execute()
      {
        BlendHelp.Instance.ShowHelpTopic("/html/cd6ec3da-2216-4975-ae7e-184a3e09ae4a.htm");
      }
    }

    private class TutorialsCommand : Command
    {
      private IServices services;

      public TutorialsCommand(IServices services)
      {
        this.services = services;
      }

      public override void Execute()
      {
        WebPageLauncher.Navigate(new Uri("http://go.microsoft.com/fwlink/?LinkId=82128", UriKind.Absolute), this.services.GetService<IMessageDisplayService>());
      }
    }
  }
}
