// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.AccessHelper
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using Microsoft.Expression.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace Microsoft.Expression.Utility.IO
{
  public static class AccessHelper
  {
    private static IAccessService accessService = (IAccessService) new Microsoft.Expression.Utility.IO.AccessService(OSHelper.IsWin8);
    public static readonly SecurityIdentifier AllApplicationPackagesSecurityIdentifier = new SecurityIdentifier("S-1-15-2-1");
    private static readonly string AclLogFileFormat = "expression{0}_Acl_Log.txt";
    private static readonly string AclLoggingEnvVar = "EXPRESSION_LOG_ACLING";

    public static IAccessService AccessService
    {
      get
      {
        return AccessHelper.accessService;
      }
      set
      {
        if (value is Microsoft.Expression.Utility.IO.AccessService)
        {
          AccessHelper.accessService = value;
        }
        else
        {
          if (!AccessHelper.IsAccessServiceLocal)
            throw new ArgumentException("Cannot set the AccessService more than once.");
          AccessHelper.accessService = value;
        }
      }
    }

    public static bool IsAccessServiceLocal
    {
      get
      {
        return AccessHelper.AccessService is Microsoft.Expression.Utility.IO.AccessService;
      }
    }

    public static void AclDirectoryForApplicationPackages(string path, FileSystemRights rights = FileSystemRights.ReadAndExecute)
    {
      if (!OSHelper.IsWin8 || string.IsNullOrEmpty(path) || !PathHelper.IsDirectory(path))
        return;
      DirectorySecurity accessControl = Directory.GetAccessControl(path);
      accessControl.AddAccessRule(new FileSystemAccessRule((IdentityReference) AccessHelper.AllApplicationPackagesSecurityIdentifier, rights, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow));
      accessControl.AddAccessRule(new FileSystemAccessRule((IdentityReference) AccessHelper.AllApplicationPackagesSecurityIdentifier, rights, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
      Directory.SetAccessControl(path, accessControl);
    }

    public static void AclFileForApplicationPackages(string path, FileSystemRights rights = FileSystemRights.Read)
    {
      if (!OSHelper.IsWin8 || string.IsNullOrEmpty(path) || !PathHelper.FileExists(path))
        return;
      FileSecurity accessControl = File.GetAccessControl(path);
      accessControl.AddAccessRule(new FileSystemAccessRule((IdentityReference) AccessHelper.AllApplicationPackagesSecurityIdentifier, rights, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow));
      File.SetAccessControl(path, accessControl);
    }

    public static void AclPathsForApplicationPackages(string[] paths)
    {
      if (!OSHelper.IsWin8 || paths == null || paths.Length == 0)
        return;
      string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
      string str1 = Path.Combine(folderPath, "cmd.exe");
      string str2 = Path.Combine(folderPath, "icacls.exe");
      string str3 = AccessHelper.AllApplicationPackagesSecurityIdentifier.Value;
      List<string> list = new List<string>();
      string aclingLogFileName = AccessHelper.GetAclingLogFileName();
      string str4;
      if (string.IsNullOrEmpty(aclingLogFileName))
        str4 = string.Empty;
      else
        str4 = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ">> {0} 2>&1", new object[1]
        {
          (object) aclingLogFileName
        });
      string str5 = str4;
      foreach (string path in paths)
      {
        string str6 = PathHelper.TrimTrailingDirectorySeparators(path);
        if (PathHelper.FileOrDirectoryExists(path))
          list.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} \"{1}\" /grant \"*{2}\":(OI)(CI)(IO)(GR,GE) /grant \"*{2}\":(RX) {3}", (object) str2, (object) str6, (object) str3, (object) str5));
      }
      StringBuilder stringBuilder = new StringBuilder(list[0]);
      for (int index = 1; index < list.Count; ++index)
      {
        stringBuilder.Append(" & ");
        stringBuilder.Append(list[index]);
      }
      Process.Start(new ProcessStartInfo()
      {
        FileName = str1,
        Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/q /c \"{0}\"", new object[1]
        {
          (object) stringBuilder.ToString()
        }),
        UseShellExecute = true,
        Verb = "runas"
      }).WaitForExit(300000);
    }

    public static bool IsAccessibleByAllApplicationPackages(string path)
    {
      if (string.IsNullOrWhiteSpace(path) || !PathHelper.FileOrDirectoryExists(path))
        return true;
      foreach (AuthorizationRule authorizationRule in (ReadOnlyCollectionBase) File.GetAccessControl(path).GetAccessRules(true, true, typeof (SecurityIdentifier)))
      {
        FileSystemAccessRule systemAccessRule = authorizationRule as FileSystemAccessRule;
        if (systemAccessRule != null && systemAccessRule.AccessControlType == AccessControlType.Allow && (systemAccessRule.IdentityReference.Equals((object) AccessHelper.AllApplicationPackagesSecurityIdentifier) && systemAccessRule.FileSystemRights.HasFlag((Enum) FileSystemRights.ReadAndExecute)))
          return true;
      }
      return false;
    }

    private static string GetAclingLogFileName()
    {
      string path = string.Empty;
      if (string.Equals(Environment.GetEnvironmentVariable(AccessHelper.AclLoggingEnvVar), "1", StringComparison.OrdinalIgnoreCase))
        path = Path.Combine(Path.GetTempPath(), string.Format((IFormatProvider) CultureInfo.CurrentUICulture, AccessHelper.AclLogFileFormat, new object[1]
        {
          (object) Process.GetCurrentProcess().Id
        }));
      if (!PathHelper.IsValidPath(path))
        return string.Empty;
      return path;
    }
  }
}
