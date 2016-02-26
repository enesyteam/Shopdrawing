// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.SldlHResult
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;

namespace Microsoft.Expression.Framework
{
  [CLSCompliant(false)]
  public enum SldlHResult : uint
  {
    None = 0U,
    SL_E_CHPA_MAXIMUM_UNLOCK_EXCEEDED = 3221536776U,
    SL_E_VALUE_NOT_FOUND = 3221549074U,
    SL_E_RIGHT_NOT_GRANTED = 3221549075U,
    SL_E_PRODUCT_SKU_NOT_INSTALLED = 3221549077U,
    SL_E_NOT_SUPPORTED = 3221549078U,
    SL_E_LUA_ACCESSDENIED = 3221549093U,
    SL_E_TAMPER_DETECTED = 3221549095U,
    SL_E_CIDIID_INVALID_CHECK_DIGITS = 3221549133U,
    SL_E_INVALID_PRODUCT_KEY = 3221549136U,
  }
}
