// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationDialog.DataAccess.PhoneNumberInformation
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.Expression.Framework.ActivationDialog.DataAccess
{
  internal class PhoneNumberInformation
  {
    public string PhoneNumber { get; private set; }

    public string PhoneNumberType { get; private set; }

    public string OptionalInformation { get; private set; }

    public PhoneNumberInformation(string phoneNumber, string phoneNumberType, string optionalInformation)
    {
      this.PhoneNumber = phoneNumber;
      this.PhoneNumberType = phoneNumberType;
      this.OptionalInformation = optionalInformation;
    }
  }
}
