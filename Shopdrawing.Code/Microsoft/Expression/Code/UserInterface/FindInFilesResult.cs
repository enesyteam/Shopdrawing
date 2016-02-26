// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.UserInterface.FindInFilesResult
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Framework.Data;
using System.Windows.Media;

namespace Microsoft.Expression.Code.UserInterface
{
  public class FindInFilesResult
  {
    public int Line { get; set; }

    public int Column { get; set; }

    public string FileName { get; set; }

    public string LineContent { get; set; }

    public string Project { get; set; }

    public DelegateCommand InvokeCommand { get; set; }

    public ImageSource Image { get; set; }
  }
}
