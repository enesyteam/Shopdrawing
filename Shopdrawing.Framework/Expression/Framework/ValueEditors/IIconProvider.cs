// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.IIconProvider
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows.Media;

namespace Microsoft.Expression.Framework.ValueEditors
{
  public interface IIconProvider
  {
    ImageSource GetIconAsImageSource(object key, object parameter);
  }
}
