// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Services
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Microsoft.Expression.Framework
{
  public sealed class Services : IServices, IServiceProvider
  {
    private IDictionary<Type, object> serviceTable = (IDictionary<Type, object>) new Dictionary<Type, object>();
    private List<IPackage> packages = new List<IPackage>();
    private List<IAddIn> addIns = new List<IAddIn>();
    private List<string> excludedAddIns = new List<string>();

    public IList<object> GetServices
    {
      get
      {
        IList<object> list = (IList<object>) new List<object>();
        foreach (KeyValuePair<Type, object> keyValuePair in (IEnumerable<KeyValuePair<Type, object>>) this.serviceTable)
          list.Add(keyValuePair.Value);
        return list;
      }
    }

    public int Version
    {
      get
      {
        return 1;
      }
    }

    public IEnumerable<IPackage> Packages
    {
      get
      {
        return (IEnumerable<IPackage>) this.packages;
      }
    }

    public IEnumerable<IAddIn> AddIns
    {
      get
      {
        return (IEnumerable<IAddIn>) this.addIns;
      }
    }

    public object GetService(Type serviceType)
    {
      if (serviceType == (Type) null)
        throw new ArgumentNullException("serviceType");
      object obj;
      if (!this.serviceTable.TryGetValue(serviceType, out obj))
        return (object) null;
      return obj;
    }

    public void AddService(Type serviceType, object serviceInstance)
    {
      if (serviceType == (Type) null)
        throw new ArgumentNullException("serviceType");
      if (serviceInstance == null)
        throw new ArgumentNullException("serviceInstance");
      if (!serviceType.IsInstanceOfType(serviceInstance))
        throw new ArgumentException(ExceptionStringTable.AddServiceMustBeCalledWithAnInstanceOfSpecifiedType, "serviceInstance");
      this.serviceTable.Add(serviceType, serviceInstance);
      if (!(serviceType == typeof (IWindowService)))
        return;
      ((IWindowService) serviceInstance).Closed += new EventHandler(this.WindowService_Closed);
    }

    public void RemoveService(Type serviceType)
    {
      if (serviceType == (Type) null)
        throw new ArgumentNullException("serviceType");
      if (serviceType == typeof (ILicenseService))
        throw new ArgumentException("Cannot remove this service", "serviceType");
      this.serviceTable.Remove(serviceType);
    }

    public T GetService<T>() where T : class
    {
      return this.GetService(typeof (T)) as T;
    }

    public void RegisterPackage(IPackage package)
    {
      if (package == null)
        throw new ArgumentNullException("package");
      if (this.packages.Contains(package))
        return;
      package.Load((IServices) this);
      this.packages.Add(package);
    }

    public void UnregisterPackage(IPackage package)
    {
      if (package == null)
        throw new ArgumentNullException("package");
      if (!this.packages.Contains(package))
        return;
      this.packages.Remove(package);
      package.Unload();
    }

    public IAddIn LoadAddIn(string fileName)
    {
      if (this.excludedAddIns.Contains(Path.GetFileName(fileName).ToUpperInvariant()))
        return (IAddIn) null;
      try
      {
        switch (Path.GetExtension(fileName).ToUpperInvariant())
        {
          case ".ADDIN":
            using (XmlReader xmlReader = XmlReader.Create(fileName))
            {
              if (xmlReader.IsStartElement("AddIn"))
              {
                if (xmlReader.MoveToAttribute("AssemblyFile"))
                  return this.LoadAddIn(Path.Combine(Path.GetDirectoryName(fileName), xmlReader.Value));
                break;
              }
              break;
            }
          case ".DLL":
            string str = Path.Combine(Path.GetDirectoryName(this.GetType().Module.FullyQualifiedName), Environment.ExpandEnvironmentVariables(fileName));
            if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(str))
            {
              foreach (IAddIn addIn in this.AddIns)
              {
                if (new Uri(str).Equals((object) new Uri(addIn.Location)))
                  return addIn;
              }
              Assembly assembly = Assembly.LoadFrom(str);
              if (assembly != (Assembly) null)
              {
                List<Type> list = new List<Type>();
                object[] customAttributes = assembly.GetCustomAttributes(typeof (PackageAttribute), false);
                if (customAttributes != null && customAttributes.Length != 0)
                {
                  foreach (PackageAttribute packageAttribute in customAttributes)
                    list.Add(packageAttribute.PackageType);
                }
                else
                  list.AddRange((IEnumerable<Type>) assembly.GetExportedTypes());
                Services.AddIn addIn = new Services.AddIn(this, str);
                foreach (Type type in list)
                {
                  if (typeof (IPackage).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                  {
                    IPackage package = Activator.CreateInstance(type) as IPackage;
                    if (package != null)
                      addIn.RegisterPackage(package);
                  }
                }
                if (!EnumerableExtensions.CountIsMoreThan<IPackage>(addIn.Packages, 0))
                  return (IAddIn) null;
                this.addIns.Add((IAddIn) addIn);
                return (IAddIn) addIn;
              }
              break;
            }
            break;
        }
        return (IAddIn) null;
      }
      catch (TypeLoadException ex1)
      {
        IMessageDisplayService service = this.GetService<IMessageDisplayService>();
        if (service != null)
        {
          try
          {
            service.ShowError(string.Concat(new object[4]
            {
              (object) "Addin '",
              (object) fileName,
              (object) "' has an issue with a type mismatch\r",
              (object) ex1
            }));
          }
          catch (Exception ex2)
          {
          }
        }
        return (IAddIn) null;
      }
    }

    public void UnloadAddIn(IAddIn addIn)
    {
      if (!this.addIns.Contains(addIn))
        return;
      Services.AddIn addIn1 = (Services.AddIn) addIn;
      List<IPackage> list = new List<IPackage>();
      list.AddRange(addIn1.Packages);
      for (int index = list.Count - 1; index >= 0; --index)
        addIn1.UnregisterPackage(list[index]);
      this.addIns.Remove(addIn);
    }

    public void ExcludeAddIn(string fileName)
    {
      if (this.excludedAddIns.Contains(fileName.ToUpperInvariant()))
        return;
      this.excludedAddIns.Add(fileName.ToUpperInvariant());
    }

    public IEnumerable<IAddIn> LoadAddIns(string fileSpec)
    {
      List<IAddIn> list = new List<IAddIn>();
      fileSpec = Environment.ExpandEnvironmentVariables(fileSpec);
      string path = Path.Combine(Path.GetDirectoryName(this.GetType().Module.FullyQualifiedName), Path.GetDirectoryName(fileSpec));
      if (Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(path))
      {
        string fileName1 = Path.GetFileName(fileSpec);
        foreach (string fileName2 in Directory.GetFiles(path, fileName1))
        {
          IAddIn addIn = this.LoadAddIn(fileName2);
          if (addIn != null)
            list.Add(addIn);
        }
      }
      return (IEnumerable<IAddIn>) list;
    }

    private void WindowService_Closed(object sender, EventArgs e)
    {
      this.OnShuttingDown();
    }

    public void OnShuttingDown()
    {
      IWindowService service = this.GetService<IWindowService>();
      if (service != null)
        service.Closed -= new EventHandler(this.WindowService_Closed);
      for (int index = this.addIns.Count - 1; index >= 0; --index)
      {
        IAddIn addIn = this.addIns[index];
        try
        {
          this.UnloadAddIn(addIn);
        }
        catch
        {
        }
      }
      for (int index = this.packages.Count - 1; index >= 0; --index)
      {
        IPackage package = this.packages[index];
        try
        {
          this.UnregisterPackage(package);
        }
        catch
        {
        }
      }
    }

    private sealed class AddIn : IAddIn
    {
      private IList<IPackage> packages = (IList<IPackage>) new List<IPackage>();
      private Services services;
      private string location;

      public IEnumerable<IPackage> Packages
      {
        get
        {
          return (IEnumerable<IPackage>) this.packages;
        }
      }

      public string Location
      {
        get
        {
          return this.location;
        }
      }

      public AddIn(Services services, string location)
      {
        this.services = services;
        this.location = location;
      }

      public void RegisterPackage(IPackage package)
      {
        try
        {
          this.services.RegisterPackage(package);
          this.packages.Add(package);
        }
        catch (Exception ex)
        {
        }
      }

      public void UnregisterPackage(IPackage package)
      {
        try
        {
          this.services.UnregisterPackage(package);
          this.packages.Remove(package);
        }
        catch (Exception ex)
        {
        }
      }
    }
  }
}
