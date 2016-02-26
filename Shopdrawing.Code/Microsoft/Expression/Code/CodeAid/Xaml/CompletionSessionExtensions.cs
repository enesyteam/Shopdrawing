// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.CodeAid.Xaml.CompletionSessionExtensions
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.VisualStudio.Language.Intellisense;

namespace Microsoft.Expression.Code.CodeAid.Xaml
{
  public static class CompletionSessionExtensions
  {
    public static bool IsSelected(this CompletionSelectionStatus status)
    {
      return (status.SelectionOptions & CompletionSelectionOptions.Selected) == CompletionSelectionOptions.Selected;
    }

    public static bool IsUnique(this CompletionSelectionStatus status)
    {
      return (status.SelectionOptions & CompletionSelectionOptions.Unique) == CompletionSelectionOptions.Unique;
    }

    public static bool IsHollowSelected(this CompletionSelectionStatus status)
    {
      if (!CompletionSessionExtensions.IsSelected(status))
        return status.SelectedCompletion != null;
      return false;
    }
  }
}
