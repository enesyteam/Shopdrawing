// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ExpressionMefHostingService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;

namespace Microsoft.Expression.Framework
{
  public sealed class ExpressionMefHostingService : IExpressionMefHostingService
  {
    private static string ExtensionSearchPattern = "*.Extension.dll";
    private List<object> parts = new List<object>();
    private List<string> assemblies = new List<string>();
    private List<string> folders = new List<string>();
    private List<Exception> compositionExceptions = new List<Exception>();
    private IServices services;
    private bool containsNonInternalParts;

    [ImportMany]
    public IEnumerable<IPackage> Packages { get; set; }

    public IEnumerable<Exception> CompositionExceptions
    {
      get
      {
        return (IEnumerable<Exception>) this.compositionExceptions;
      }
    }

    public ExpressionMefHostingService(IServices services)
    {
      this.services = services;
    }

    public void AddInternalPart(object part)
    {
      if (part == null)
        throw new ArgumentNullException("part");
      if (this.parts.Contains(part))
        return;
      this.parts.Add(part);
    }

    public void AddPart(object part)
    {
      this.containsNonInternalParts = true;
      this.AddInternalPart(part);
    }

    public void AddAssembly(string assembly)
    {
      if (assembly == null)
        throw new ArgumentNullException("assembly");
      assembly = Environment.ExpandEnvironmentVariables(assembly);
      if (this.assemblies.Contains(assembly))
        return;
      this.assemblies.Add(assembly);
    }

    public void AddFolder(string folder)
    {
      if (folder == null)
        throw new ArgumentNullException("folder");
      if (this.folders.Contains(folder))
        return;
      this.folders.Add(folder);
    }

    public void Compose()
    {
      this.ClearCompositionExceptions();
      if (!this.containsNonInternalParts && this.assemblies.Count <= 0 && this.folders.Count <= 0)
        return;
      List<ComposablePartCatalog> list = new List<ComposablePartCatalog>();
      foreach (string codeBase in this.assemblies)
      {
        try
        {
          list.Add((ComposablePartCatalog) new AssemblyCatalog(codeBase));
        }
        catch (Exception ex)
        {
          throw;
        }
      }
      foreach (string path in this.folders)
      {
        try
        {
          if (Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(path))
          {
            if (Directory.GetFiles(path).Length > 0)
              list.Add((ComposablePartCatalog) new DirectoryCatalog(path, ExpressionMefHostingService.ExtensionSearchPattern));
          }
        }
        catch (Exception ex)
        {
          throw;
        }
      }
      if (list.Count <= 0 && !this.containsNonInternalParts)
        return;
      CompositionContainer container = new CompositionContainer((ComposablePartCatalog) new AggregateCatalog((IEnumerable<ComposablePartCatalog>) list), new ExportProvider[0]);
      try
      {
        AttributedModelServices.ComposeParts(container, this.parts.ToArray());
      }
      catch (Exception ex)
      {
        throw;
      }
      foreach (IPackage package in this.Packages)
      {
        try
        {
          package.Load(this.services);
        }
        catch (Exception ex)
        {
          this.AddCompositionException(ex);
        }
      }
    }

    public void AddCompositionException(Exception exception)
    {
      this.compositionExceptions.Add(exception);
    }

    public void ClearCompositionExceptions()
    {
      this.compositionExceptions.Clear();
    }
  }
}
