// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Documents.DocumentUtilities
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;

namespace Microsoft.Expression.Framework.Documents
{
  public static class DocumentUtilities
  {
    public static bool PromptUserAndSaveDocument(IDocument document, bool saveAsOnFailure, IMessageDisplayService messageManager)
    {
      bool flag = false;
      if (document.IsDirty)
      {
        IDictionary<MessageChoice, string> dictionary = (IDictionary<MessageChoice, string>) new Dictionary<MessageChoice, string>()
        {
          {
            MessageChoice.Yes,
            StringTable.SaveChangesYesResponse
          },
          {
            MessageChoice.No,
            StringTable.SaveChangesNoResponse
          }
        };
        MessageBoxArgs args = new MessageBoxArgs()
        {
          Message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.SaveChangesTo, new object[1]
          {
            (object) document.DocumentReference.DisplayName
          }),
          Button = MessageBoxButton.YesNoCancel,
          Image = MessageBoxImage.Exclamation,
          TextOverrides = dictionary
        };
        switch (messageManager.ShowMessage(args))
        {
          case MessageBoxResult.Yes:
            flag = DocumentUtilities.SaveDocument(document, saveAsOnFailure, messageManager);
            break;
          case MessageBoxResult.No:
            flag = true;
            break;
        }
      }
      else
        flag = true;
      return flag;
    }

    public static bool SaveDocument(IDocument document, bool saveAsOnFailure, bool forceSave, IMessageDisplayService messageManager)
    {
      bool flag = false;
      if (!forceSave)
      {
        if (!document.IsDirty)
        {
          flag = true;
          goto label_12;
        }
      }
      try
      {
        if (document.Container != null)
          document.Container.BeginCheckDocumentStatus(document);
        FileAttributes fileAttributes = !PathHelper.FileExists(document.DocumentReference.Path) ? FileAttributes.Normal : File.GetAttributes(document.DocumentReference.Path);
        if ((fileAttributes & FileAttributes.ReadOnly) != (FileAttributes) 0)
        {
          MessageBoxArgs args = new MessageBoxArgs()
          {
            Message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.OverwriteConfirmationMessage, new object[1]
            {
              (object) document.DocumentReference.DisplayName
            }),
            Button = MessageBoxButton.YesNoCancel,
            Image = MessageBoxImage.Exclamation
          };
          switch (messageManager.ShowMessage(args))
          {
            case MessageBoxResult.Yes:
              File.SetAttributes(document.DocumentReference.Path, fileAttributes & ~FileAttributes.ReadOnly);
              flag = document.Save();
              break;
            case MessageBoxResult.No:
              flag = true;
              break;
          }
        }
        else
          flag = document.Save();
      }
      catch (UnauthorizedAccessException ex)
      {
        flag = false;
        messageManager.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.SaveAccessError, new object[2]
        {
          (object) document.DocumentReference.DisplayName,
          (object) ex.Message
        }));
        int num = saveAsOnFailure ? 1 : 0;
      }
      catch (IOException ex)
      {
        flag = false;
        messageManager.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.SaveAccessError, new object[2]
        {
          (object) document.DocumentReference.DisplayName,
          (object) ex.Message
        }));
        int num = saveAsOnFailure ? 1 : 0;
      }
label_12:
      return flag;
    }

    public static bool SaveDocument(IDocument document, bool saveAsOnFailure, IMessageDisplayService messageManager)
    {
      return DocumentUtilities.SaveDocument(document, saveAsOnFailure, false, messageManager);
    }
  }
}
