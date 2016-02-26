// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ClrPropertyPathHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Collections.Generic;
using System.Text;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public static class ClrPropertyPathHelper
  {
    private static char[] pathPartSeparators = ".[/".ToCharArray();

    public static string CombinePaths(string inheritedPath, string localPath)
    {
      if (string.IsNullOrEmpty(inheritedPath))
        return localPath;
      if (string.IsNullOrEmpty(localPath))
        return inheritedPath;
      string str = inheritedPath;
      return (int) localPath[0] == 91 || (int) localPath[0] == 47 || (int) inheritedPath[inheritedPath.Length - 1] == 47 ? str + localPath : str + "." + localPath;
    }

    public static int GetPathPartCount(string path)
    {
      if (string.IsNullOrEmpty(path))
        return 0;
      if (path.Length == 1)
        return 1;
      int num = (int) path[0] == 47 ? 2 : 1;
      for (int index = path.IndexOfAny(ClrPropertyPathHelper.pathPartSeparators, 1); index > 0; index = path.IndexOfAny(ClrPropertyPathHelper.pathPartSeparators, index + 1))
      {
        ++num;
        if ((int) path[index] == 46 && index + 1 < path.Length && (int) path[index + 1] == 91)
          ++index;
        else if ((int) path[index] == 47)
          ++num;
      }
      return num;
    }

    public static string[] SplitAtFirstProperty(string path)
    {
      if (string.IsNullOrEmpty(path))
        return new string[2]
        {
          string.Empty,
          string.Empty
        };
      string pathPart1 = path;
      string str = string.Empty;
      int index = path.IndexOfAny(ClrPropertyPathHelper.pathPartSeparators, 1);
      if (index > 0)
      {
        pathPart1 = path.Substring(0, index);
        if ((int) path[index] == 46)
        {
          if (index == path.Length - 1)
            return (string[]) null;
          ++index;
        }
        str = path.Substring(index);
      }
      ClrPathPart pathPart2 = ClrPropertyPathHelper.GetPathPart(pathPart1);
      if (pathPart2 == null || pathPart2.Category != ClrPathPartCategory.PropertyName)
        return (string[]) null;
      return new string[2]
      {
        pathPart1,
        str
      };
    }

    public static IList<ClrPathPart> SplitPath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return (IList<ClrPathPart>) new List<ClrPathPart>();
      if ((int) path[0] == 46 || (int) path[path.Length - 1] == 46)
        return (IList<ClrPathPart>) null;
      List<ClrPathPart> list = new List<ClrPathPart>();
      int startIndex = 0;
      for (int index = path.IndexOfAny(ClrPropertyPathHelper.pathPartSeparators, 1); index > 0; index = path.IndexOfAny(ClrPropertyPathHelper.pathPartSeparators, index + 1))
      {
        ClrPathPart pathPart = ClrPropertyPathHelper.GetPathPart(path.Substring(startIndex, index - startIndex));
        if (pathPart == null)
          return (IList<ClrPathPart>) null;
        list.Add(pathPart);
        if ((int) path[index] == 47)
        {
          list.Add(ClrPathPart.CurrentItem);
          startIndex = index + 1;
        }
        else
          startIndex = (int) path[index] != 46 ? index : index + 1;
      }
      ClrPathPart pathPart1 = ClrPropertyPathHelper.GetPathPart(path.Substring(startIndex));
      if (pathPart1 == null)
        return (IList<ClrPathPart>) null;
      list.Add(pathPart1);
      return (IList<ClrPathPart>) list;
    }

    public static ClrPathPart GetPathPart(string pathPart)
    {
      if (string.IsNullOrEmpty(pathPart) || (int) pathPart[0] == 46)
        return (ClrPathPart) null;
      if (pathPart == ClrPathPart.CurrentItem.Path)
        return ClrPathPart.CurrentItem;
      if ((int) pathPart[0] != 91)
        return new ClrPathPart(pathPart, ClrPathPartCategory.PropertyName);
      if (pathPart.Length < 3 || (int) pathPart[pathPart.Length - 1] != 93)
        return (ClrPathPart) null;
      int result;
      if (!int.TryParse(pathPart.Substring(1, pathPart.Length - 2), out result))
        return (ClrPathPart) null;
      return new ClrPathPart(pathPart, ClrPathPartCategory.IndexStep);
    }

    public static string CombinePathParts(IList<ClrPathPart> parts)
    {
      return ClrPropertyPathHelper.CombinePathParts(parts, 0, parts.Count);
    }

    public static string CombinePathParts(IList<ClrPathPart> parts, int startIndex)
    {
      return ClrPropertyPathHelper.CombinePathParts(parts, startIndex, parts.Count - startIndex);
    }

    public static string CombinePathParts(IList<ClrPathPart> parts, int startIndex, int count)
    {
      StringBuilder stringBuilder = new StringBuilder();
      ClrPathPart clrPathPart1 = (ClrPathPart) null;
      for (int index = 0; index < count && index + startIndex < parts.Count; ++index)
      {
        ClrPathPart clrPathPart2 = parts[index + startIndex];
        if (clrPathPart1 == null)
          stringBuilder.Append(clrPathPart2.NewPath);
        else if (clrPathPart2.Category == ClrPathPartCategory.CurrentItem || clrPathPart1.Category == ClrPathPartCategory.CurrentItem)
          stringBuilder.Append(clrPathPart2.NewPath);
        else if (clrPathPart2.Category == ClrPathPartCategory.IndexStep)
        {
          stringBuilder.Append(clrPathPart2.NewPath);
        }
        else
        {
          stringBuilder.Append(".");
          stringBuilder.Append(clrPathPart2.NewPath);
        }
        clrPathPart1 = clrPathPart2;
      }
      return stringBuilder.ToString();
    }
  }
}
