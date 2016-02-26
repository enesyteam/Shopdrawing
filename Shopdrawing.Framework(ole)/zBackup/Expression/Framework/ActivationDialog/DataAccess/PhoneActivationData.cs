// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationDialog.DataAccess.PhoneActivationData
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;

namespace Microsoft.Expression.Framework.ActivationDialog.DataAccess
{
  internal class PhoneActivationData : NotifyingObject
  {
    private string idNumber;

    public string GroupName { get; private set; }

    public string IdNumber
    {
      get
      {
        return this.idNumber;
      }
      set
      {
        if (this.idNumber == value)
          return;
        this.idNumber = value;
        this.OnPropertyChanged("IdNumber");
      }
    }

    public bool IsValid
    {
      get
      {
        if (this.IdNumber != null)
          return this.IdNumber.Length == 6;
        return false;
      }
    }

    public PhoneActivationData(string groupName)
      : this(groupName, string.Empty)
    {
    }

    public PhoneActivationData(string groupName, string idNumber)
    {
      this.GroupName = groupName;
      this.IdNumber = idNumber;
    }
  }
}
