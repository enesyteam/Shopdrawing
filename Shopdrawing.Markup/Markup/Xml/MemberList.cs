// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.MemberList
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal sealed class MemberList
  {
    private Member[] elements;
    private int length;

    public int Length
    {
      get
      {
        return this.length;
      }
    }

    public Member this[int index]
    {
      get
      {
        return this.elements[index];
      }
      set
      {
        this.elements[index] = value;
      }
    }

    public MemberList()
    {
      this.elements = new Member[16];
    }

    public MemberList(int capacity)
    {
      this.elements = new Member[capacity];
    }

    public MemberList(params Member[] elements)
    {
      if (elements == null)
        elements = new Member[0];
      this.elements = elements;
      this.length = elements.Length;
    }

    public void Clear()
    {
      this.length = 0;
    }

    public void Add(Member element)
    {
      int length1 = this.elements.Length;
      int index1 = this.length++;
      if (index1 == length1)
      {
        int length2 = length1 * 2;
        if (length2 < 16)
          length2 = 16;
        Member[] memberArray = new Member[length2];
        for (int index2 = 0; index2 < length1; ++index2)
          memberArray[index2] = this.elements[index2];
        this.elements = memberArray;
      }
      this.elements[index1] = element;
    }

    public MemberList Clone()
    {
      Member[] memberArray1 = this.elements;
      int capacity = this.length;
      MemberList memberList = new MemberList(capacity);
      memberList.length = capacity;
      Member[] memberArray2 = memberList.elements;
      for (int index = 0; index < capacity; ++index)
        memberArray2[index] = memberArray1[index];
      return memberList;
    }

    public Member[] ToArray()
    {
      Member[] memberArray = new Member[this.length];
      Array.Copy((Array) this.elements, (Array) memberArray, this.length);
      return memberArray;
    }
  }
}
