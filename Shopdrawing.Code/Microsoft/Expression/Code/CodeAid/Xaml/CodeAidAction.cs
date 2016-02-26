// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.CodeAid.Xaml.CodeAidAction
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

namespace Microsoft.Expression.Code.CodeAid.Xaml
{
  public struct CodeAidAction
  {
    private CodeAidActionType actionTypeFlags;

    public bool IsNone
    {
      get
      {
        return this.actionTypeFlags == CodeAidActionType.None;
      }
    }

    public CodeAidAction(CodeAidActionType actionType)
    {
      this = new CodeAidAction();
      this.actionTypeFlags = actionType;
    }

    public bool TestFlag(CodeAidActionType toTest)
    {
      return (this.actionTypeFlags & toTest) != CodeAidActionType.None;
    }
  }
}
