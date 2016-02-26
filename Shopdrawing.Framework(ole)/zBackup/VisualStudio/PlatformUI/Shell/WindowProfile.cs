// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.WindowProfile
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  [ContentProperty("Children")]
  [DefaultProperty("Children")]
  public class WindowProfile : DependencyObject, ICustomXmlSerializable, IDependencyObjectCustomSerializerAccess
  {
    public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof (string), typeof (WindowProfile));

    public IObservableCollection<ViewElement> Children { get; private set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Name
    {
      get
      {
        return (string) this.GetValue(WindowProfile.NameProperty);
      }
      set
      {
        this.SetValue(WindowProfile.NameProperty, (object) value);
      }
    }

    public WindowProfile()
    {
      this.Children = (IObservableCollection<ViewElement>) new WindowProfile.WindowProfileElementCollection(this);
    }

    public ICustomXmlSerializer CreateSerializer()
    {
      return (ICustomXmlSerializer) new WindowProfileCustomSerializer(this);
    }

    bool IDependencyObjectCustomSerializerAccess.ShouldSerializeProperty(DependencyProperty dp)
    {
      return this.ShouldSerializeProperty(dp);
    }

    object IDependencyObjectCustomSerializerAccess.GetValue(DependencyProperty dp)
    {
      return this.GetValue(dp);
    }

    public ViewElement Find(Predicate<ViewElement> predicate)
    {
      if (predicate == null)
        return (ViewElement) null;
      foreach (ViewElement viewElement1 in (IEnumerable<ViewElement>) this.Children)
      {
        ViewElement viewElement2 = viewElement1.Find(predicate);
        if (viewElement2 != null)
          return viewElement2;
      }
      return (ViewElement) null;
    }

    public IEnumerable<ViewElement> FindAll(Predicate<ViewElement> predicate)
    {
      if (predicate != null)
      {
        foreach (ViewElement viewElement1 in (IEnumerable<ViewElement>) this.Children)
        {
          foreach (ViewElement viewElement2 in viewElement1.FindAll(predicate))
            yield return viewElement2;
        }
      }
    }

    public static WindowProfile FindWindowProfile(ViewElement view)
    {
      return (WindowProfile) ViewElement.FindRootElement(view).GetValue(ViewElement.WindowProfileProperty);
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, Name = {1}, Children = {2}", (object) this.GetType().Name, (object) this.Name, (object) this.Children.Count);
    }

    public static WindowProfile Create(string profileName)
    {
      MainSite mainSite = MainSite.Create();
      mainSite.Child = WindowProfile.CreateDefaultMainSiteContent();
      WindowProfile windowProfile = new WindowProfile();
      windowProfile.Name = profileName;
      windowProfile.Children.Add((ViewElement) mainSite);
      return windowProfile;
    }

    internal static ViewElement CreateDefaultMainSiteContent()
    {
      AutoHideChannel autoHideChannel1 = AutoHideChannel.Create();
      autoHideChannel1.Dock = Dock.Left;
      autoHideChannel1.Orientation = Orientation.Vertical;
      AutoHideChannel autoHideChannel2 = AutoHideChannel.Create();
      autoHideChannel2.Dock = Dock.Right;
      autoHideChannel2.Orientation = Orientation.Vertical;
      AutoHideChannel autoHideChannel3 = AutoHideChannel.Create();
      autoHideChannel3.Dock = Dock.Top;
      autoHideChannel3.Orientation = Orientation.Horizontal;
      AutoHideChannel autoHideChannel4 = AutoHideChannel.Create();
      autoHideChannel4.Dock = Dock.Bottom;
      autoHideChannel4.Orientation = Orientation.Horizontal;
      DocumentGroupContainer documentGroupContainer = DocumentGroupContainer.Create();
      documentGroupContainer.DockedWidth = new SplitterLength(1.0, SplitterUnitType.Fill);
      documentGroupContainer.DockedHeight = new SplitterLength(1.0, SplitterUnitType.Fill);
      documentGroupContainer.Children.Add((ViewElement) DocumentGroup.Create());
      DockRoot dockRoot = DockRoot.Create();
      dockRoot.DockedWidth = new SplitterLength(1.0, SplitterUnitType.Fill);
      dockRoot.DockedHeight = new SplitterLength(1.0, SplitterUnitType.Fill);
      dockRoot.Children.Add((ViewElement) documentGroupContainer);
      AutoHideRoot autoHideRoot = AutoHideRoot.Create();
      autoHideRoot.DockedWidth = new SplitterLength(1.0, SplitterUnitType.Fill);
      autoHideRoot.DockedHeight = new SplitterLength(1.0, SplitterUnitType.Fill);
      autoHideRoot.Children.Add((ViewElement) autoHideChannel1);
      autoHideRoot.Children.Add((ViewElement) autoHideChannel2);
      autoHideRoot.Children.Add((ViewElement) autoHideChannel3);
      autoHideRoot.Children.Add((ViewElement) autoHideChannel4);
      autoHideRoot.Children.Add((ViewElement) dockRoot);
      return (ViewElement) autoHideRoot;
    }

    public static WindowProfile Load(string profileXml)
    {
      using (StringReader stringReader = new StringReader(profileXml))
      {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          CheckCharacters = false,
          CloseInput = false
        };
        using (XmlReader reader = XmlReader.Create((TextReader) stringReader, settings))
          return WindowProfile.LoadInternal(reader);
      }
    }

    public static WindowProfile Load(Stream stream)
    {
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        CheckCharacters = false,
        CloseInput = false
      };
      using (XmlReader reader = XmlReader.Create(stream, settings))
        return WindowProfile.LoadInternal(reader);
    }

    private static WindowProfile LoadInternal(XmlReader reader)
    {
      try
      {
        WindowProfile profile;
        using (ViewElementFactory.Current.AllowConstruction())
          profile = (WindowProfile) XamlReader.Load(reader);
        new WindowProfileValidator().Validate(profile);
        return profile;
      }
      catch (XmlException ex)
      {
        throw new FileFormatException("Window profile contains malformed XML.", (Exception) ex);
      }
      catch (XamlParseException ex)
      {
        throw new FileFormatException("Window profile contains malformed XAML.", (Exception) ex);
      }
    }

    public void Save(Stream stream)
    {
      this.Save(stream, new WindowProfileSerializer());
    }

    public void Save(Stream stream, WindowProfileSerializer serializer)
    {
      serializer.Serialize((object) this, stream);
    }

    public WindowProfile Copy(string newName)
    {
      return this.Copy(newName, new WindowProfileSerializer());
    }

    public WindowProfile Copy(string newName, WindowProfileSerializer serializer)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          CheckCharacters = false
        };
        this.Save((Stream) memoryStream, serializer);
        memoryStream.Seek(0L, SeekOrigin.Begin);
        using (XmlReader reader = XmlReader.Create((Stream) memoryStream, settings))
        {
          WindowProfile windowProfile;
          using (ViewElementFactory.Current.AllowConstruction())
            windowProfile = (WindowProfile) XamlReader.Load(reader);
          windowProfile.Name = newName;
          return windowProfile;
        }
      }
    }

    private class WindowProfileElementCollection : OwnershipCollection<ViewElement>
    {
      public WindowProfile WindowProfile { get; private set; }

      public WindowProfileElementCollection(WindowProfile windowProfile)
      {
        this.WindowProfile = windowProfile;
      }

      protected override void LoseOwnership(ViewElement element)
      {
        element.SetValue(ViewElement.WindowProfilePropertyKey, (object) null);
      }

      protected override void TakeOwnership(ViewElement element)
      {
        element.SetValue(ViewElement.WindowProfilePropertyKey, (object) this.WindowProfile);
      }
    }
  }
}
