// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleDataCollection
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SampleDataCollection
  {
    private List<SampleDataSet> dataSets = new List<SampleDataSet>();
    private ProjectContext projectContext;
    private IMSBuildProject msBuildProject;
    private IExpressionInformationService expressionInformationService;
    private string sampleDataClrNamespacePrefix;
    private string dataStoreClrNamespacePrefix;

    public ReadOnlyCollection<SampleDataSet> SampleDataSetCollection
    {
      get
      {
        return new ReadOnlyCollection<SampleDataSet>((IList<SampleDataSet>) this.dataSets);
      }
    }

    public event EventHandler<SampleDataEventArgs> SampleDataAdded;

    public event EventHandler<SampleDataEventArgs> SampleDataRemoving;

    public SampleDataCollection(ProjectContext projectContext, IMSBuildProject msBuildProject, IExpressionInformationService expressionInformationService)
    {
      this.projectContext = projectContext;
      this.msBuildProject = msBuildProject;
      this.expressionInformationService = expressionInformationService;
      this.sampleDataClrNamespacePrefix = DataSetContext.GetClrNamespacePrefix(DataSetType.SampleDataSet, this.projectContext.RootNamespace);
      this.dataStoreClrNamespacePrefix = DataSetContext.GetClrNamespacePrefix(DataSetType.DataStoreSet, this.projectContext.RootNamespace);
    }

    public void LoadFromProject(Microsoft.Expression.Framework.IReadOnlyCollection<IProjectItem> projectItems)
    {
      string directoryName = Path.GetDirectoryName(this.projectContext.ProjectPath);
      string sampleDataFolder1 = Path.Combine(directoryName, DataSetContext.SampleData.DataRootFolder) + (object) Path.DirectorySeparatorChar;
      string sampleDataFolder2 = Path.Combine(directoryName, DataSetContext.DataStore.DataRootFolder) + (object) Path.DirectorySeparatorChar;
      List<SampleDataCollection.SampleDataSetProjectInfo> collection = new List<SampleDataCollection.SampleDataSetProjectInfo>();
      foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) projectItems)
      {
        SampleDataCollection.ProcessSampleDataFile(DataSetContext.SampleData, projectItem, sampleDataFolder1, collection);
        SampleDataCollection.ProcessSampleDataFile(DataSetContext.DataStore, projectItem, sampleDataFolder2, collection);
      }
      for (int index = 0; index < collection.Count; ++index)
        this.LoadSampleDataSetFromProjectInfo(collection[index]);
    }

    private static void ProcessSampleDataFile(DataSetContext dataSetContext, IProjectItem projectItem, string sampleDataFolder, List<SampleDataCollection.SampleDataSetProjectInfo> collection)
    {
      string path = projectItem.DocumentReference.Path;
      if (!path.StartsWith(sampleDataFolder, StringComparison.OrdinalIgnoreCase))
        return;
      string extension = Path.GetExtension(path);
      if (!extension.Equals(".xsd", StringComparison.OrdinalIgnoreCase) && !extension.Equals(".xaml", StringComparison.OrdinalIgnoreCase))
        return;
      string withoutExtension = Path.GetFileNameWithoutExtension(path);
      if (!string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{1}{3}", (object) sampleDataFolder, (object) withoutExtension, (object) Path.DirectorySeparatorChar, (object) extension).Equals(path, StringComparison.OrdinalIgnoreCase))
        return;
      SampleDataCollection.SampleDataSetProjectInfo sampleDataSetInfo = SampleDataCollection.GetOrCreateSampleDataSetInfo(dataSetContext, withoutExtension, collection);
      if (extension.Equals(".xsd", StringComparison.OrdinalIgnoreCase))
        sampleDataSetInfo.XsdItem = projectItem;
      else
        sampleDataSetInfo.XamlItem = projectItem;
    }

    private static SampleDataCollection.SampleDataSetProjectInfo GetOrCreateSampleDataSetInfo(DataSetContext dataSetContext, string name, List<SampleDataCollection.SampleDataSetProjectInfo> collection)
    {
      for (int index = 0; index < collection.Count; ++index)
      {
        SampleDataCollection.SampleDataSetProjectInfo dataSetProjectInfo = collection[index];
        if (dataSetProjectInfo.Name == name)
          return dataSetProjectInfo;
      }
      SampleDataCollection.SampleDataSetProjectInfo dataSetProjectInfo1 = new SampleDataCollection.SampleDataSetProjectInfo();
      dataSetProjectInfo1.DataSetContext = dataSetContext;
      dataSetProjectInfo1.Name = name;
      collection.Add(dataSetProjectInfo1);
      return dataSetProjectInfo1;
    }

    private bool LoadSampleDataSetFromProjectInfo(SampleDataCollection.SampleDataSetProjectInfo sampleDataInfo)
    {
      if (sampleDataInfo.XsdItem == null || sampleDataInfo.XamlItem == null || (!Microsoft.Expression.Framework.Documents.PathHelper.FileExists(sampleDataInfo.XsdItem.DocumentReference.Path) || !Microsoft.Expression.Framework.Documents.PathHelper.FileExists(sampleDataInfo.XamlItem.DocumentReference.Path)))
        return false;
      bool enableAtRuntime = !string.Equals(sampleDataInfo.XamlItem.Properties["BuildAction"], SampleDataSet.DesignTimeBuildType);
      SampleDataSet dataSet = this.InstantiateSampleDataSet(sampleDataInfo.DataSetContext, sampleDataInfo.Name, enableAtRuntime);
      dataSet.Load();
      if (!dataSet.IsValid)
      {
        dataSet.Close();
        return false;
      }
      this.dataSets.Add(dataSet);
      this.OnSampleDataAdded(dataSet);
      return true;
    }

    private void OnSampleDataAdded(SampleDataSet dataSet)
    {
      if (this.SampleDataAdded == null)
        return;
      this.SampleDataAdded((object) this, new SampleDataEventArgs(dataSet));
    }

    private void OnSampleDataRemoving(SampleDataSet dataSet)
    {
      if (this.SampleDataRemoving == null)
        return;
      this.SampleDataRemoving((object) this, new SampleDataEventArgs(dataSet));
    }

    public void Save()
    {
      foreach (SampleDataSet sampleDataSet in this.dataSets)
      {
        if (sampleDataSet.IsValid)
          sampleDataSet.Save();
      }
    }

    public void Close()
    {
      foreach (SampleDataSet sampleDataSet in this.dataSets)
        sampleDataSet.Close();
      this.dataSets.Clear();
    }

    public IType GetSampleType(IXmlNamespace xmlNamespace, string typeName)
    {
      if (this.dataSets.Count == 0)
        return (IType) null;
      string clrNamespace;
      string assemblyName;
      if (!XamlParser.TryParseClrNamespaceUri(xmlNamespace.Value, out clrNamespace, out assemblyName))
        return (IType) null;
      if (!clrNamespace.StartsWith(this.sampleDataClrNamespacePrefix, StringComparison.Ordinal) && !clrNamespace.StartsWith(this.dataStoreClrNamespacePrefix, StringComparison.Ordinal))
        return (IType) null;
      SampleDataSet sampleDataSet1 = this.GetSampleDataSet(clrNamespace.Substring(this.sampleDataClrNamespacePrefix.Length), true);
      if (sampleDataSet1 != null)
        return sampleDataSet1.GetSampleType(xmlNamespace, typeName);
      SampleDataSet sampleDataSet2 = this.GetSampleDataSet(clrNamespace.Substring(this.dataStoreClrNamespacePrefix.Length), true);
      if (sampleDataSet2 != null)
        return sampleDataSet2.GetSampleType(xmlNamespace, typeName);
      return (IType) null;
    }

    public IType GetSampleType(Type type)
    {
      if (this.dataSets.Count == 0)
        return (IType) null;
      SampleNonBasicType sampleType = SampleDataSet.SampleDataTypeFromType(type);
      if (sampleType == null)
        return (IType) null;
      if (this.dataSets.Find((Predicate<SampleDataSet>) (s => s == sampleType.DeclaringDataSet)) == null)
        return (IType) null;
      return (IType) sampleType;
    }

    public SampleDataSet GetSampleDataSet(string name, bool onlyIfDataSetValid)
    {
      SampleDataSet sampleDataSet = Enumerable.FirstOrDefault<SampleDataSet>((IEnumerable<SampleDataSet>) this.dataSets, (Func<SampleDataSet, bool>) (ds => ds.Name == name));
      if (sampleDataSet != null && onlyIfDataSetValid && !sampleDataSet.IsValid)
        sampleDataSet = (SampleDataSet) null;
      return sampleDataSet;
    }

    public string GetUniqueSampleDataSetName(string name)
    {
      IEnumerable<string> existingNames = Enumerable.Select<SampleDataSet, string>((IEnumerable<SampleDataSet>) this.dataSets, (Func<SampleDataSet, string>) (dataSet => dataSet.Name));
      return SampleDataNameHelper.GetUniqueName(name, this.msBuildProject, existingNames, (string) null, true);
    }

    public SampleDataSet CreateSampleDataSet(DataSetContext dataSetContext, string name, bool enableAtRuntime)
    {
      string sampleDataSetName = this.GetUniqueSampleDataSetName(name);
      if (string.IsNullOrEmpty(sampleDataSetName))
        return (SampleDataSet) null;
      SampleDataSet dataSet = this.InstantiateSampleDataSet(dataSetContext, sampleDataSetName, enableAtRuntime);
      this.dataSets.Add(dataSet);
      this.OnSampleDataAdded(dataSet);
      return dataSet;
    }

    public SampleDataSet ImportSampleDataFromXmlFile(string dataSourceName, bool enableAtRuntime, string xmlFileName)
    {
      SampleDataSet dataSet = (SampleDataSet) null;
      bool flag = false;
      try
      {
        dataSet = this.CreateSampleDataSet(DataSetContext.SampleData, dataSourceName, enableAtRuntime);
        if (dataSet != null)
          flag = dataSet.ImportFromXml(xmlFileName, false);
      }
      finally
      {
        if (!flag && dataSet != null)
        {
          this.SafelyRemoveSampleDataAndRootFolder(dataSet, true);
          dataSet = (SampleDataSet) null;
        }
      }
      return dataSet;
    }

    public SampleDataSet CreateDefaultNewSampleDataSource(DataSetContext dataSetContext, string dataSourceName, bool enableAtRuntime)
    {
      SampleDataSet dataSet = (SampleDataSet) null;
      bool flag = false;
      try
      {
        dataSet = this.CreateSampleDataSet(dataSetContext, dataSourceName, enableAtRuntime);
        if (dataSet != null)
        {
          using (dataSet.DisableChangeTracking())
          {
            if (dataSetContext.DataSetType == DataSetType.SampleDataSet)
            {
              string uniqueTypeName1 = dataSet.GetUniqueTypeName("Item");
              SampleCompositeType compositeType = dataSet.CreateCompositeType(uniqueTypeName1);
              string uniquePropertyName1 = compositeType.GetUniquePropertyName("Property1");
              compositeType.AddProperty(uniquePropertyName1, (SampleType) SampleBasicType.String);
              string uniquePropertyName2 = compositeType.GetUniquePropertyName("Property2");
              compositeType.AddProperty(uniquePropertyName2, (SampleType) SampleBasicType.Boolean);
              string uniqueTypeName2 = dataSet.GetUniqueTypeName("ItemCollection");
              SampleCollectionType collectionType = dataSet.CreateCollectionType(uniqueTypeName2, (SampleType) compositeType);
              string uniquePropertyName3 = dataSet.RootType.GetUniquePropertyName("Collection");
              dataSet.RootType.AddProperty(uniquePropertyName3, (SampleType) collectionType);
              dataSet.AutoGenerateValues();
            }
            else
            {
              string uniquePropertyName = dataSet.RootType.GetUniquePropertyName("Property1");
              SampleProperty sampleProperty = dataSet.RootType.AddProperty(uniquePropertyName, (SampleType) SampleBasicType.String);
              using (SampleDataValueBuilder valueBuilder = dataSet.CreateValueBuilder())
              {
                DocumentCompositeNode rootNode = valueBuilder.RootNode;
                valueBuilder.RootNode.Properties[(IPropertyId) sampleProperty] = valueBuilder.CreatePropertyValue(rootNode, "Property1", StringTable.DefaultValueDataStore);
              }
              dataSet.Save();
            }
          }
          flag = dataSet.IsSaved;
        }
      }
      finally
      {
        if (!flag && dataSet != null)
        {
          this.SafelyRemoveSampleDataAndRootFolder(dataSet, true);
          dataSet = (SampleDataSet) null;
        }
      }
      return dataSet;
    }

    public SampleDataSet ImportSampleDataFromXmlFile(string dataSourceName, string xmlFileName)
    {
      SampleDataSet dataSet = (SampleDataSet) null;
      bool flag = false;
      try
      {
        dataSet = this.CreateSampleDataSet(DataSetContext.SampleData, dataSourceName, false);
        if (dataSet != null)
          flag = dataSet.ImportFromXml(xmlFileName, false);
      }
      finally
      {
        if (!flag && dataSet != null)
        {
          this.SafelyRemoveSampleDataAndRootFolder(dataSet, true);
          dataSet = (SampleDataSet) null;
        }
      }
      return dataSet;
    }

    protected SampleDataSet InstantiateSampleDataSet(DataSetContext dataSetContext, string name, bool enableAtRuntime)
    {
      return new SampleDataSet(dataSetContext, name, enableAtRuntime, this.projectContext, this.msBuildProject, this.expressionInformationService);
    }

    public void DeleteSampleDataSet(SampleDataSet dataSet)
    {
      TypesChangedEventArgs e = new TypesChangedEventArgs((ICollection<TypeChangedInfo>) new List<TypeChangedInfo>()
      {
        new TypeChangedInfo(dataSet.RootType.RuntimeAssembly, ModificationType.Modified),
        new TypeChangedInfo(RuntimeGeneratedTypesHelper.BlendDefaultAssembly, ModificationType.Modified)
      });
      this.OnSampleDataRemoving(dataSet);
      this.dataSets.Remove(dataSet);
      dataSet.RemoveFromProjectAndClose();
      this.SafelyRemoveSampleDataAndRootFolder(dataSet, false);
      this.projectContext.OnTypesChanged(e);
    }

    private void SafelyRemoveSampleDataAndRootFolder(SampleDataSet dataSet, bool forceDeleteFiles)
    {
      dataSet.Close();
      if (forceDeleteFiles)
      {
        SampleDataCollection.DeleteFolder(dataSet.SampleDataFolder, false);
      }
      else
      {
        SampleDataCollection.DeleteFolder(dataSet.AssetFilesFolder, true);
        SampleDataCollection.DeleteFolder(dataSet.SampleDataFolder, true);
      }
      this.OnSampleDataRemoving(dataSet);
      this.dataSets.Remove(dataSet);
      if (Enumerable.FirstOrDefault<SampleDataSet>(Enumerable.Where<SampleDataSet>((IEnumerable<SampleDataSet>) this.dataSets, (Func<SampleDataSet, bool>) (item => item.Context == dataSet.Context))) != null)
        return;
      string path = Path.Combine(Path.GetDirectoryName(this.projectContext.ProjectPath), dataSet.Context.DataRootFolder);
      IProject project = (IProject) this.projectContext.GetService(typeof (IProject));
      IProjectItem projectItem = project.FindItem(DocumentReference.Create(path));
      if (projectItem == null || Enumerable.FirstOrDefault<IProjectItem>(projectItem.Children) != null)
        return;
      project.RemoveItems(0 != 0, projectItem);
      SampleDataCollection.DeleteFolder(dataSet.Context.DataRootFolder, true);
    }

    public static void DeleteFolder(string folder, bool onlyIfEmpty)
    {
      try
      {
        if (onlyIfEmpty)
          Directory.Delete(folder);
        else
          Directory.Delete(folder, true);
      }
      catch
      {
      }
    }

    private class SampleDataSetProjectInfo
    {
      public DataSetContext DataSetContext { get; set; }

      public string Name { get; set; }

      public IProjectItem XsdItem { get; set; }

      public IProjectItem XamlItem { get; set; }
    }
  }
}
