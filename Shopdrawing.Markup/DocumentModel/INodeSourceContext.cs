// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.INodeSourceContext
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Text;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public interface INodeSourceContext
  {
    bool IsCloned { get; }

    int Ordering { get; }

    ITextRange TextRange { get; }

    ITextLocation TextLocation { get; }

    IReadableSelectableTextBuffer TextBuffer { get; }

    INodeSourceContext Clone(bool keepOldRanges);

    INodeSourceContext FreezeText(bool isClone);
  }
}
