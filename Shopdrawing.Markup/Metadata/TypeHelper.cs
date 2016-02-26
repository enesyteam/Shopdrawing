// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.TypeHelper
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Expression.DesignModel.Metadata
{
  public static class TypeHelper
  {
    public static bool IsAccessibleType(ITypeResolver typeResolver, IType typeId)
    {
      return TypeHelper.IsSet(TypeHelper.GetAllowableMemberAccess(typeResolver, typeId), typeId.Access);
    }

    public static MemberAccessTypes GetAllowableMemberAccess(ITypeResolver typeResolver, IType typeId)
    {
      return typeResolver.InTargetAssembly(typeId) ? MemberAccessTypes.PublicOrInternal : MemberAccessTypes.Public;
    }

    public static bool IsSet(MemberAccessTypes accessTypes, MemberAccessType accessType)
    {
      return (accessTypes & (MemberAccessTypes) accessType) != MemberAccessTypes.None;
    }

    public static bool IsSet(MemberType memberTypes, MemberType memberType)
    {
      return (memberTypes & memberType) != MemberType.None;
    }

    public static bool IsPropertyWritable(ITypeResolver typeResolver, IProperty propertyKey, bool allowProtectedProperties)
    {
      MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(typeResolver, propertyKey.DeclaringType);
      if (allowProtectedProperties && !propertyKey.IsAttachable || typeResolver.IsCapabilitySet(PlatformCapability.WorkaroundSL12782) && typeResolver.PlatformMetadata.KnownTypes.UserControl.IsAssignableFrom(propertyKey.DeclaringTypeId) && propertyKey.Equals((object) propertyKey.DeclaringType.Metadata.DefaultContentProperty))
        allowableMemberAccess |= MemberAccessTypes.Protected;
      MemberAccessType writeAccess = propertyKey.WriteAccess;
      return TypeHelper.IsSet(allowableMemberAccess, writeAccess);
    }

    [Conditional("DEBUG")]
    public static void VerifyExactlyOne(MemberType memberTypes)
    {
      int num1 = 0;
      foreach (int num2 in new MemberTypeEnumerable(memberTypes))
        ++num1;
      if (num1 != 1)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.MemberTypeValueShouldContainExactlyOneType, new object[1]
        {
          (object) memberTypes
        }));
    }

    public static string CombineNamespaceAndTypeName(string clrNamespace, string typeName)
    {
      if (string.IsNullOrEmpty(clrNamespace))
        return typeName;
      return clrNamespace + "." + typeName;
    }

    public static string GetTypeNamePart(string typeName)
    {
      int num = typeName.LastIndexOf('.');
      if (num == 0 || num == typeName.Length - 1)
        return (string) null;
      if (num >= 0)
        return typeName.Substring(num + 1);
      return typeName;
    }

    public static string GetClrNamespacePart(string typeName)
    {
      int length = typeName.LastIndexOf('.');
      if (length >= 0)
        return typeName.Substring(0, length);
      return string.Empty;
    }
  }
}
