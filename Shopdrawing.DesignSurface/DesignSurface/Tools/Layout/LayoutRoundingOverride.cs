// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Layout.LayoutRoundingOverride
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.Tools.Layout
{
  internal class LayoutRoundingOverride
  {
    private Dictionary<SceneElement, bool?> storedValues;

    internal void SetValue(IEnumerable<SceneElement> elements, bool value)
    {
      if (elements == null)
        throw new ArgumentNullException("elements");
      this.Restore(false);
      IProperty propertyKey = LayoutRoundingHelper.ResolveUseLayoutRoundingProperty((SceneNode) Enumerable.FirstOrDefault<SceneElement>(elements));
      if (propertyKey == null)
        return;
      this.storedValues = new Dictionary<SceneElement, bool?>();
      foreach (SceneElement element in elements)
      {
        IViewObject viewObject = element.ViewObject;
        if (element.IsViewObjectValid && !value.Equals(viewObject.GetValue(propertyKey)) && LayoutRoundingHelper.GetLayoutRoundingStatus(element) == LayoutRoundingStatus.On)
        {
          this.storedValues[element] = !viewObject.IsSet(propertyKey) ? new bool?() : new bool?((bool) viewObject.GetValue(propertyKey));
          viewObject.SetValue((ITypeResolver) element.ProjectContext, propertyKey, (object) (bool) (value ? true : false));
          LayoutRoundingOverride.InvalidateMeasure(viewObject);
        }
      }
    }

    public void Restore(bool forceClearPropertyInspector)
    {
      if (this.storedValues == null)
        return;
      IProperty propertyKey = LayoutRoundingHelper.ResolveUseLayoutRoundingProperty((SceneNode) Enumerable.FirstOrDefault<SceneElement>((IEnumerable<SceneElement>) this.storedValues.Keys));
      if (propertyKey != null)
      {
        foreach (KeyValuePair<SceneElement, bool?> keyValuePair in this.storedValues)
        {
          IViewObject viewObject = keyValuePair.Key.ViewObject;
          if (keyValuePair.Key.IsViewObjectValid)
          {
            bool? nullable = keyValuePair.Value;
            if (nullable.HasValue)
            {
              viewObject.SetValue((ITypeResolver) keyValuePair.Key.ProjectContext, propertyKey, (object) (bool) (nullable.Value ? true : false));
            }
            else
            {
              viewObject.ClearValue(propertyKey);
              if (forceClearPropertyInspector)
                LayoutRoundingOverride.ForceClearPropertyInspector((IPropertyId) propertyKey, keyValuePair.Key);
            }
            LayoutRoundingOverride.InvalidateMeasure(viewObject);
          }
        }
      }
      this.storedValues = (Dictionary<SceneElement, bool?>) null;
    }

    private static void InvalidateMeasure(IViewObject viewObject)
    {
      IViewVisual viewVisual = viewObject as IViewVisual;
      if (viewVisual == null)
        return;
      viewVisual.InvalidateMeasure();
    }

    private static void ForceClearPropertyInspector(IPropertyId resolvedProperty, SceneElement element)
    {
      using (element.ViewModel.ForceBaseValue())
      {
        SceneEditTransaction editTransaction = element.DesignerContext.ActiveDocument.CreateEditTransaction("Hidden", true);
        using (editTransaction)
        {
          element.SetValue(resolvedProperty, (object) true);
          editTransaction.Cancel();
        }
      }
    }
  }
}
