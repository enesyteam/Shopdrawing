// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.Workspace
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Workspaces.Extension;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.UserInterface
{
  public sealed class Workspace : IWorkspace
  {
    private string name;
    private FrameworkElement content;
    private WindowProfile windowProfile;

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public FrameworkElement Content
    {
      get
      {
        return this.content;
      }
    }

    public WindowProfile WindowProfile
    {
      get
      {
        return this.windowProfile;
      }
    }

    public Workspace(string name, FrameworkElement content)
    {
      this.name = name;
      this.content = content;
    }

    public View FindPalette(string name)
    {
      return (View) this.windowProfile.Find((Predicate<ViewElement>) (i =>
      {
        View view = i as View;
        if (view != null)
          return view.Name == name;
        return false;
      }));
    }

    public bool LoadConfiguration(Stream config)
    {
      try
      {
        using (ViewElementFactory.Current.AllowConstruction())
          this.windowProfile = (WindowProfile) XamlReader.Load(config);
        this.windowProfile.Name = this.name;
        if (!new ExpressionWindowProfileValidator().Validate(this.windowProfile))
          return false;
        if (this.content != null)
        {
          NakedView nakedView = (NakedView) this.windowProfile.Find((Predicate<ViewElement>) (v => v is NakedView));
          if (nakedView != null)
            nakedView.Content = this.content;
        }
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public bool SaveConfiguration(Stream config)
    {
      if (ViewManager.Instance.ActiveView != null)
      {
        if (ViewManager.Instance.ActiveView.Parent is AutoHideGroup)
          ViewManager.Instance.ActiveView = (View) null;
      }
      try
      {
        this.CreateWindowProfileSerializer().Serialize((object) this.windowProfile, config);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public void CopyConfiguration(IWorkspace sourceWorkspace)
    {
      if (sourceWorkspace == null)
        throw new ArgumentNullException("sourceWorkspace");
      if (sourceWorkspace == this)
        throw new ArgumentException("Cannot copy configuration from itself.", "sourceWorkspace");
      Workspace workspace = sourceWorkspace as Workspace;
      if (workspace == null)
        throw new ArgumentException("Source workspace must be of the type Workspace. Other types are not supported.", "sourceWorkspace");
      WindowProfileSerializer profileSerializer = this.CreateWindowProfileSerializer();
      this.windowProfile = workspace.WindowProfile.Copy(this.name, profileSerializer);
      if (this.content == null)
        return;
      NakedView nakedView = (NakedView) this.windowProfile.Find((Predicate<ViewElement>) (v => v is NakedView));
      if (nakedView == null)
        return;
      nakedView.Content = this.content;
    }

    private WindowProfileSerializer CreateWindowProfileSerializer()
    {
      WindowProfileSerializer profileSerializer = new WindowProfileSerializer();
      profileSerializer.MapNamespaceToAssembly(typeof (ExpressionDockGroup).Namespace, this.GetType().Assembly.GetName().Name, "ExpressionExtension");
      return profileSerializer;
    }
  }
}
