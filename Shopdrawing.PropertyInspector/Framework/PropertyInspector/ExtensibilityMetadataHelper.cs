// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.PropertyInspector.ExtensibilityMetadataHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.Framework.PropertyInspector
{
  public static class ExtensibilityMetadataHelper
  {
    public static PropertyValueEditor GetValueEditor(IEnumerable attributes, IMessageLoggingService exceptionLogger)
    {
      PropertyValueEditor propertyValueEditor = (PropertyValueEditor) null;
      if (attributes != null)
      {
        foreach (Attribute attribute in attributes)
        {
          EditorAttribute editorAttribute = attribute as EditorAttribute;
          if (editorAttribute != null)
          {
            try
            {
              Type type = Type.GetType(editorAttribute.EditorTypeName);
              if (type != (Type) null)
              {
                if (typeof (PropertyValueEditor).IsAssignableFrom(type))
                {
                  propertyValueEditor = (PropertyValueEditor) Activator.CreateInstance(type);
                  break;
                }
              }
            }
            catch (Exception ex)
            {
              if (exceptionLogger != null)
                exceptionLogger.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.ValueEditorLoadFailed, new object[1]
                {
                  (object) ExtensibilityMetadataHelper.GetExceptionMessage(ex)
                }));
            }
          }
        }
      }
      return propertyValueEditor;
    }

    public static IDictionary<Type, IList<NewItemFactory>> GetNewItemFactoriesFromAttributes(IList<NewItemTypesAttribute> attributes, IMessageLoggingService exceptionLogger)
    {
      Dictionary<Type, IList<NewItemFactory>> dictionary = new Dictionary<Type, IList<NewItemFactory>>();
      using (IEnumerator<NewItemTypesAttribute> enumerator = ((IEnumerable<NewItemTypesAttribute>) attributes).GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          NewItemTypesAttribute current = enumerator.Current;
          try
          {
            NewItemFactory newItemFactory = (NewItemFactory) Activator.CreateInstance(current.get_FactoryType());
            if (newItemFactory != null)
            {
              foreach (Type key in current.get_Types())
              {
                IList<NewItemFactory> list;
                if (!dictionary.TryGetValue(key, out list))
                {
                  list = (IList<NewItemFactory>) new List<NewItemFactory>();
                  dictionary.Add(key, list);
                }
                ((ICollection<NewItemFactory>) list).Add(newItemFactory);
              }
            }
          }
          catch (Exception ex)
          {
            if (exceptionLogger != null)
              exceptionLogger.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.CollectionItemFactoryInstantiateFailed, new object[1]
              {
                (object) ExtensibilityMetadataHelper.GetExceptionMessage(ex)
              }));
          }
        }
      }
      return (IDictionary<Type, IList<NewItemFactory>>) dictionary;
    }

    public static Type GetCategoryEditorType(EditorAttribute attribute, IMessageLoggingService exceptionLogger)
    {
      try
      {
        Type type = Type.GetType(attribute.EditorTypeName);
        if (type != (Type) null)
        {
          if (typeof (CategoryEditor).IsAssignableFrom(type))
            return type;
        }
      }
      catch (Exception ex)
      {
        if (exceptionLogger != null)
          exceptionLogger.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.CategoryEditorTypeLoadFailed, new object[1]
          {
            (object) ExtensibilityMetadataHelper.GetExceptionMessage(ex)
          }));
      }
      return (Type) null;
    }

    public static string GetExceptionMessage(Exception e)
    {
      if (e.InnerException == null)
        return e.Message;
      return e.InnerException.ToString();
    }
  }
}
