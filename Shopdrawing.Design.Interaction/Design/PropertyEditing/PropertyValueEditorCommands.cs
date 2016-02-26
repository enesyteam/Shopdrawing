// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.PropertyValueEditorCommands
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System.Windows.Input;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public static class PropertyValueEditorCommands
  {
    private static RoutedCommand _showInlineEditor;
    private static RoutedCommand _showExtendedPopupEditor;
    private static RoutedCommand _showExtendedPinnedEditor;
    private static RoutedCommand _showDialogEditor;
    private static RoutedCommand _beginTransaction;
    private static RoutedCommand _commitTransaction;
    private static RoutedCommand _abortTransaction;
    private static RoutedCommand _finishEditing;
    private static RoutedCommand _showErrorMessage;
    private static RoutedCommand _showContextMenu;

    public static RoutedCommand ShowErrorMessage
    {
      get
      {
        if (PropertyValueEditorCommands._showErrorMessage == null)
          PropertyValueEditorCommands._showErrorMessage = new RoutedCommand("ShowErrorMessage", typeof (PropertyValueEditorCommands));
        return PropertyValueEditorCommands._showErrorMessage;
      }
    }

    public static RoutedCommand ShowInlineEditor
    {
      get
      {
        if (PropertyValueEditorCommands._showInlineEditor == null)
          PropertyValueEditorCommands._showInlineEditor = new RoutedCommand("ShowInlineEditor", typeof (PropertyValueEditorCommands));
        return PropertyValueEditorCommands._showInlineEditor;
      }
    }

    public static RoutedCommand ShowExtendedPopupEditor
    {
      get
      {
        if (PropertyValueEditorCommands._showExtendedPopupEditor == null)
          PropertyValueEditorCommands._showExtendedPopupEditor = new RoutedCommand("ShowExtendedPopupEditor", typeof (PropertyValueEditorCommands));
        return PropertyValueEditorCommands._showExtendedPopupEditor;
      }
    }

    public static RoutedCommand ShowExtendedPinnedEditor
    {
      get
      {
        if (PropertyValueEditorCommands._showExtendedPinnedEditor == null)
          PropertyValueEditorCommands._showExtendedPinnedEditor = new RoutedCommand("ShowExtendedPinnedEditor", typeof (PropertyValueEditorCommands));
        return PropertyValueEditorCommands._showExtendedPinnedEditor;
      }
    }

    public static RoutedCommand ShowDialogEditor
    {
      get
      {
        if (PropertyValueEditorCommands._showDialogEditor == null)
          PropertyValueEditorCommands._showDialogEditor = new RoutedCommand("ShowDialogEditor", typeof (PropertyValueEditorCommands));
        return PropertyValueEditorCommands._showDialogEditor;
      }
    }

    public static RoutedCommand BeginTransaction
    {
      get
      {
        if (PropertyValueEditorCommands._beginTransaction == null)
          PropertyValueEditorCommands._beginTransaction = new RoutedCommand("BeginTransaction", typeof (PropertyValueEditorCommands));
        return PropertyValueEditorCommands._beginTransaction;
      }
    }

    public static RoutedCommand CommitTransaction
    {
      get
      {
        if (PropertyValueEditorCommands._commitTransaction == null)
          PropertyValueEditorCommands._commitTransaction = new RoutedCommand("CommitTransaction", typeof (PropertyValueEditorCommands));
        return PropertyValueEditorCommands._commitTransaction;
      }
    }

    public static RoutedCommand AbortTransaction
    {
      get
      {
        if (PropertyValueEditorCommands._abortTransaction == null)
          PropertyValueEditorCommands._abortTransaction = new RoutedCommand("AbortTransaction", typeof (PropertyValueEditorCommands));
        return PropertyValueEditorCommands._abortTransaction;
      }
    }

    public static RoutedCommand FinishEditing
    {
      get
      {
        if (PropertyValueEditorCommands._finishEditing == null)
          PropertyValueEditorCommands._finishEditing = new RoutedCommand("FinishEditing", typeof (PropertyValueEditorCommands));
        return PropertyValueEditorCommands._finishEditing;
      }
    }

    public static RoutedCommand ShowContextMenu
    {
      get
      {
        if (PropertyValueEditorCommands._showContextMenu == null)
          PropertyValueEditorCommands._showContextMenu = new RoutedCommand("ShowContextMenu", typeof (PropertyValueEditorCommands));
        return PropertyValueEditorCommands._showContextMenu;
      }
    }
  }
}
