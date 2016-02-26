// Decompiled with JetBrains decompiler
// Type: MS.Internal.Interaction.ModelHitTestHelper
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Interaction;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MS.Internal.Interaction
{
  internal static class ModelHitTestHelper
  {
    [ThreadStatic]
    private static Dictionary<Type, HitTestProvider> _hitTestProviders;

    private static Dictionary<Type, HitTestProvider> HitTestProviders
    {
      get
      {
        if (ModelHitTestHelper._hitTestProviders == null)
        {
          ModelHitTestHelper._hitTestProviders = new Dictionary<Type, HitTestProvider>();
          ModelHitTestHelper._hitTestProviders[typeof (Panel)] = (HitTestProvider) new ContainerHitTestProvider();
          ModelHitTestHelper._hitTestProviders[typeof (Decorator)] = (HitTestProvider) new ContainerHitTestProvider();
          ModelHitTestHelper._hitTestProviders[typeof (ContentControl)] = (HitTestProvider) new ContainerHitTestProvider();
        }
        return ModelHitTestHelper._hitTestProviders;
      }
    }

    public static HitTestProvider GetSingletonProvider(DependencyObject d)
    {
      for (Type key = d.GetType(); key != typeof (object); key = key.BaseType)
      {
        if (ModelHitTestHelper.HitTestProviders.ContainsKey(key))
          return ModelHitTestHelper.HitTestProviders[key];
      }
      return (HitTestProvider) null;
    }

    public static HitTestResult HitTest(Visual reference, Point point, HitTestFilterCallback filterCallback)
    {
      return ModelHitTestHelper.HitTest(reference, filterCallback, (HitTestResultCallback) null, (HitTestParameters) new PointHitTestParameters(point), (HitTestFilterCallback) null);
    }

    public static ViewHitTestResult HitTest(ViewItem reference, Point point, ViewHitTestFilterCallback filterCallback, ViewHitTestFilterCallback modelCallback)
    {
      return reference.HitTest(filterCallback, (ViewHitTestResultCallback) null, (HitTestParameters) new PointHitTestParameters(point));
    }

    public static HitTestResult HitTest(Visual root, HitTestFilterCallback filterCallback, HitTestResultCallback resultCallback, HitTestParameters hitTestParameters, HitTestFilterCallback modelCallback)
    {
      ModelHitTestHelper.HitTestFilterCallbackWrapper filterCallbackWrapper = new ModelHitTestHelper.HitTestFilterCallbackWrapper(filterCallback);
      ModelHitTestHelper.HitTestResultCallbackWrapper resultCallbackWrapper = new ModelHitTestHelper.HitTestResultCallbackWrapper(resultCallback, HitTestResultBehavior.Continue);
      VisualTreeHelper.HitTest(root, filterCallbackWrapper.FilterCallback, resultCallbackWrapper.ResultCallback, hitTestParameters);
      HitTestResult topMostHit = resultCallbackWrapper.TopMostHit;
      bool flag = filterCallback == null && resultCallback == null;
      VisualHitTestArgs args = new VisualHitTestArgs(root, root, hitTestParameters);
      PointHitTestParameters hitTestParameters1 = hitTestParameters as PointHitTestParameters;
      GeometryHitTestParameters hitTestParameters2 = hitTestParameters as GeometryHitTestParameters;
      foreach (DependencyObject dependencyObject in ModelHitTestHelper.GetDescendantsInZOrder((DependencyObject) root))
      {
        if (dependencyObject != null)
        {
          if (filterCallback == null && topMostHit != null && dependencyObject == topMostHit.VisualHit)
          {
            resultCallbackWrapper.PlayResults();
            return resultCallbackWrapper.TopMostHit;
          }
          Visual visual = dependencyObject as Visual;
          HitTestProvider singletonProvider = ModelHitTestHelper.GetSingletonProvider(dependencyObject);
          if (singletonProvider != null && (modelCallback == null || modelCallback((DependencyObject) visual) != HitTestFilterBehavior.Continue))
          {
            args.UpdateChild(dependencyObject);
            HitTestResult result = (HitTestResult) null;
            if (hitTestParameters1 != null && visual != null)
            {
              PointHitTestResult pointHitTestResult = singletonProvider.HitTestPoint(args);
              if (pointHitTestResult != null && pointHitTestResult.VisualHit != null)
                result = (HitTestResult) pointHitTestResult;
            }
            else if (hitTestParameters2 != null && visual != null)
            {
              GeometryHitTestResult geometryHitTestResult = singletonProvider.HitTestGeometry(args);
              if (geometryHitTestResult != null && geometryHitTestResult.IntersectionDetail != IntersectionDetail.Empty && geometryHitTestResult.IntersectionDetail != IntersectionDetail.NotCalculated)
                result = (HitTestResult) geometryHitTestResult;
            }
            if (result != null)
            {
              HitTestFilterBehavior testFilterBehavior = filterCallbackWrapper.FilterCallback(dependencyObject);
              switch (testFilterBehavior)
              {
                case HitTestFilterBehavior.Continue:
                case HitTestFilterBehavior.Stop:
                  resultCallbackWrapper.InsertResult(result);
                  break;
              }
              if (flag && testFilterBehavior != HitTestFilterBehavior.ContinueSkipSelf || testFilterBehavior == HitTestFilterBehavior.Stop)
              {
                resultCallbackWrapper.PlayResults();
                return result;
              }
            }
          }
        }
      }
      resultCallbackWrapper.PlayResults();
      return resultCallbackWrapper.TopMostHit;
    }

    private static IEnumerable<DependencyObject> GetDescendantsInZOrder(DependencyObject root)
    {
      if (root != null)
      {
        int childCount = VisualTreeHelper.GetChildrenCount(root);
        if (childCount > 0)
        {
          for (int i = childCount - 1; i >= 0; --i)
          {
            DependencyObject child = VisualTreeHelper.GetChild(root, i);
            if (child != null)
            {
              foreach (DependencyObject dependencyObject in ModelHitTestHelper.GetDescendantsInZOrder(child))
              {
                if (dependencyObject != null)
                  yield return dependencyObject;
              }
              yield return child;
            }
          }
        }
      }
    }

    private class HitTestResultCallbackWrapper
    {
      private HitTestResultCallback _resultCallback;
      private HitTestResultBehavior _onHitFoundBehavior;
      private List<HitTestResult> _results;
      private HitTestResultCallback _wrappedResultCallback;
      private int _resultInsertionIndex;

      public HitTestResultCallback ResultCallback
      {
        get
        {
          return this._wrappedResultCallback;
        }
      }

      public HitTestResult TopMostHit
      {
        get
        {
          if (this._results != null && this._results.Count > 0)
            return this._results[0];
          return (HitTestResult) null;
        }
      }

      private List<HitTestResult> RawResults
      {
        get
        {
          if (this._results == null)
            this._results = new List<HitTestResult>(1);
          return this._results;
        }
      }

      public HitTestResultCallbackWrapper(HitTestResultCallback resultCallback, HitTestResultBehavior behaviorOnResult)
      {
        this._resultCallback = resultCallback;
        this._resultInsertionIndex = 0;
        this._results = (List<HitTestResult>) null;
        this._onHitFoundBehavior = behaviorOnResult;
        this._wrappedResultCallback = new HitTestResultCallback(this.OnResult);
      }

      internal void InsertResult(HitTestResult result)
      {
        if (this.RawResults.Count > 0 && this._resultInsertionIndex < this.RawResults.Count)
          this.RawResults.Insert(this._resultInsertionIndex, result);
        else
          this.RawResults.Add(result);
        ++this._resultInsertionIndex;
      }

      private HitTestResultBehavior OnResult(HitTestResult hitItemsResult)
      {
        this.RawResults.Add(hitItemsResult);
        return this._onHitFoundBehavior;
      }

      internal void PlayResults()
      {
        if (this._resultCallback == null)
          return;
        using (List<HitTestResult>.Enumerator enumerator = this.RawResults.GetEnumerator())
        {
          do
            ;
          while (enumerator.MoveNext() && this._resultCallback(enumerator.Current) != HitTestResultBehavior.Stop);
        }
      }
    }

    private class HitTestFilterCallbackWrapper
    {
      private HitTestFilterCallback _filterCallback;
      private HitTestFilterCallback _wrappedFilterCallback;

      public HitTestFilterCallback FilterCallback
      {
        get
        {
          return this._wrappedFilterCallback;
        }
      }

      public HitTestFilterCallbackWrapper(HitTestFilterCallback filterCallback)
      {
        this._filterCallback = filterCallback;
        this._wrappedFilterCallback = new HitTestFilterCallback(this.OnFilterHitResult);
      }

      private HitTestFilterBehavior OnFilterHitResult(DependencyObject hit)
      {
        UIElement uiElement = hit as UIElement;
        if (uiElement != null && !uiElement.IsVisible)
          return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
        HitTestFilterBehavior testFilterBehavior = HitTestFilterBehavior.Continue;
        if (this._filterCallback != null)
          testFilterBehavior = this._filterCallback(hit);
        return testFilterBehavior;
      }
    }
  }
}
