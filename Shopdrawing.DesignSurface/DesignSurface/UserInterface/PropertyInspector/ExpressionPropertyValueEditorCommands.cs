// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ExpressionPropertyValueEditorCommands
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.ValueEditors;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class ExpressionPropertyValueEditorCommands
  {
    public static readonly DependencyProperty ExpressionPropertyValueEditorCommandsProperty = DependencyProperty.RegisterAttached("ExpressionPropertyValueEditorCommands", typeof (ExpressionPropertyValueEditorCommands), typeof (ExpressionPropertyValueEditorCommands), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.Inherits));
    private ExpressionPropertyValueEditorCommand beginTransaction;
    private ExpressionPropertyValueEditorCommand commitTransaction;
    private ExpressionPropertyValueEditorCommand abortTransaction;

    public ExpressionPropertyValueEditorCommand BeginTransaction
    {
      get
      {
        return this.beginTransaction;
      }
      set
      {
        this.beginTransaction = value;
      }
    }

    public ExpressionPropertyValueEditorCommand CommitTransaction
    {
      get
      {
        return this.commitTransaction;
      }
      set
      {
        this.commitTransaction = value;
      }
    }

    public ExpressionPropertyValueEditorCommand AbortTransaction
    {
      get
      {
        return this.abortTransaction;
      }
      set
      {
        this.abortTransaction = value;
      }
    }

    public static ExpressionPropertyValueEditorCommands GetExpressionPropertyValueEditorCommands(DependencyObject source)
    {
      return (ExpressionPropertyValueEditorCommands) source.GetValue(ExpressionPropertyValueEditorCommands.ExpressionPropertyValueEditorCommandsProperty);
    }

    public static void SetExpressionPropertyValueEditorCommands(DependencyObject target, ExpressionPropertyValueEditorCommands value)
    {
      target.SetValue(ExpressionPropertyValueEditorCommands.ExpressionPropertyValueEditorCommandsProperty, (object) value);
    }
  }
}
