// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.AdornerResources
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Win32;
using MS.Internal.Interaction;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Windows.Design.Interaction
{
  public static class AdornerResources
  {
    private static readonly object _syncLock = new object();
    private static int _loadIndex = -1;
    private static List<LoadResourcesCallback> _callbacks;
    private static ResourceDictionary _resources;
    private static bool _themeInUse;
    private static bool? _highContrast;
    [ThreadStatic]
    private static FrameworkElement _queryElement;

    internal static ResourceDictionary ThemeResources
    {
      get
      {
        AdornerResources.EnsureResources(false);
        AdornerResources._themeInUse = true;
        return AdornerResources._resources;
      }
    }

    static AdornerResources()
    {
      SystemEvents.UserPreferenceChanged += (UserPreferenceChangedEventHandler) ((sender, e) =>
      {
        if (!AdornerResources._themeInUse || !AdornerResources.IsResourceCategory(e.Category))
          return;
        AdornerResources.EnsureResources(true);
      });
    }

    private static void EnsureResources(bool forceUpdate)
    {
      bool flag = forceUpdate;
      lock (AdornerResources._syncLock)
      {
        if (AdornerResources._resources == null)
        {
          AdornerResources._resources = new ResourceDictionary();
          flag = true;
        }
        bool? local_3 = AdornerResources._highContrast;
        bool local_4 = SystemParameters.HighContrast;
        if ((local_3.GetValueOrDefault() != local_4 ? 1 : (!local_3.HasValue ? 1 : 0)) != 0)
        {
          AdornerResources._highContrast = new bool?(SystemParameters.HighContrast);
          flag = true;
        }
        if (!flag || AdornerResources._callbacks == null)
          return;
        AdornerResources._resources.BeginInit();
        try
        {
          AdornerResources._resources.MergedDictionaries.Clear();
          while (++AdornerResources._loadIndex < AdornerResources._callbacks.Count)
          {
            ResourceDictionary local_1 = AdornerResources._callbacks[AdornerResources._loadIndex]();
            if (local_1 != null)
              AdornerResources._resources.MergedDictionaries.Add(local_1);
          }
        }
        finally
        {
          AdornerResources._resources.EndInit();
          AdornerResources._loadIndex = -1;
        }
      }
    }

    public static object FindResource(ResourceKey key)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      object resource;
      if (AdornerResources.FindResourceHelper(key, out resource))
        return resource;
      if (AdornerResources._queryElement == null)
        AdornerResources._queryElement = new FrameworkElement();
      return AdornerResources._queryElement.FindResource((object) key);
    }

    public static object TryFindResource(ResourceKey key)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      object resource;
      if (AdornerResources.FindResourceHelper(key, out resource))
        return resource;
      if (AdornerResources._queryElement == null)
        AdornerResources._queryElement = new FrameworkElement();
      return AdornerResources._queryElement.TryFindResource((object) key);
    }

    private static bool FindResourceHelper(ResourceKey key, out object resource)
    {
      resource = (object) null;
      lock (AdornerResources._syncLock)
      {
        if (AdornerResources._loadIndex != -1)
        {
          resource = AdornerResources._resources[(object) key];
          while (resource == null && ++AdornerResources._loadIndex < AdornerResources._callbacks.Count)
          {
            ResourceDictionary local_0 = AdornerResources._callbacks[AdornerResources._loadIndex]();
            if (local_0 != null)
            {
              AdornerResources._resources.MergedDictionaries.Add(local_0);
              resource = local_0[(object) key];
            }
          }
          return true;
        }
      }
      return false;
    }

    private static bool IsResourceCategory(UserPreferenceCategory category)
    {
      if (category != UserPreferenceCategory.Accessibility && category != UserPreferenceCategory.Color)
        return category == UserPreferenceCategory.VisualStyle;
      return true;
    }

    public static ResourceKey CreateResourceKey(Type owningType, string publicMember)
    {
      if (owningType == null)
        throw new ArgumentNullException("owningType");
      if (publicMember == null)
        throw new ArgumentNullException("publicMember");
      return (ResourceKey) new AdornerResourceKey(owningType, publicMember);
    }

    public static void RegisterResources(LoadResourcesCallback callback)
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      lock (AdornerResources._syncLock)
      {
        if (AdornerResources._callbacks == null)
          AdornerResources._callbacks = new List<LoadResourcesCallback>();
        AdornerResources._callbacks.Add(callback);
      }
      if (!AdornerResources._themeInUse)
        return;
      ResourceDictionary resourceDictionary = callback();
      if (resourceDictionary == null)
        return;
      AdornerResources._resources.BeginInit();
      try
      {
        AdornerResources._resources.MergedDictionaries.Add(resourceDictionary);
      }
      finally
      {
        AdornerResources._resources.EndInit();
      }
    }

    public static void Refresh()
    {
      AdornerResources.EnsureResources(true);
    }
  }
}
