// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.LicenseFileManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Expression.DesignSurface
{
  public class LicenseFileManager : ILicenseFileManager
  {
    private DesignerContext context;

    internal LicenseFileManager(DesignerContext context)
    {
      this.context = context;
    }

    public void AddLicensedItem(string projectPath, string typeName, string assemblyName)
    {
      IProject matchByUrl = DocumentItemExtensions.FindMatchByUrl<IProject>(this.context.ProjectManager.CurrentSolution.Projects, projectPath);
      List<string> list = new List<string>();
      string path = Path.Combine(matchByUrl.ProjectRoot.Path, "Properties\\Licenses.licx");
      if (matchByUrl.FindItem(DocumentReference.Create(path)) == null)
      {
        IDocumentType documentType = this.context.DocumentTypeManager.DocumentTypes[DocumentTypeNamesHelper.Licx];
        matchByUrl.AddItem(new DocumentCreationInfo()
        {
          DocumentType = documentType,
          TargetPath = path
        });
      }
      else if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(path))
      {
        using (StreamReader streamReader = File.OpenText(path))
        {
          while (!streamReader.EndOfStream)
            list.Add(streamReader.ReadLine());
        }
      }
      string str1 = typeName + ", " + assemblyName;
      if (list.Contains(str1))
        return;
      list.Add(str1);
      string directoryName = Path.GetDirectoryName(path);
      if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(directoryName))
        Directory.CreateDirectory(directoryName);
      using (StreamWriter text = File.CreateText(path))
      {
        foreach (string str2 in list)
          text.WriteLine(str2);
      }
    }
  }
}
