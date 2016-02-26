// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleCodeGenerator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SampleCodeGenerator
  {
    internal static readonly string EOL = "\r\n";
    private static readonly string ClrNamespaceTemplate = "CLR_NAMESPACE";
    private static readonly string CompositeTypeTemplate = "COMPOSITE_TYPE";
    private static readonly string PropertyNameTemplate = "PROPERTY_NAME";
    private static readonly string PropertyTypeTemplate = "PROPERTY_TYPE";
    private static readonly string PropertyValueTemplate = "PROPERTY_VALUE";
    private static readonly string CollectionTypeTemplate = "COLLECTION_TYPE";
    private static readonly string ItemTypeTemplate = "ITEM_TYPE";
    private static readonly string AssemblyTemplate = "PROJECT_ASSEMBLY_NAME";
    private static readonly string RootFolderTemplate = "SAMPLE_DATA_ROOT_FOLDER";
    private static readonly string SampleDataNameTemplate = "SAMPLE_DATA_NAME";
    private static readonly string GlobalStorageTypeTemplate = "GLOBALSTORAGE_TYPE";
    private static readonly string DataStoreTypeTemplate = "DATASTORE_TYPE";
    private static readonly string DataStoreRootFolderTemplate = "DATA_STORE_ROOT_FOLDER";
    private static readonly string DataStoreNameTemplate = "DATA_STORE_NAME";
    private SampleCodeGenerator.SampleCodeSnippets codeSnippets;
    private SampleDataSet dataSet;
    private string sourceCodeFile;
    private TextWriter textWriter;

    private SampleCodeGenerator(SampleDataSet dataSet, string sourceCodeFile, string language)
    {
      if (dataSet.Context == DataSetContext.SampleData)
        this.codeSnippets = language == "VB" ? SampleCodeGenerator.SampleCodeSnippetFactory.VB : SampleCodeGenerator.SampleCodeSnippetFactory.CSharp;
      else if (dataSet.Context == DataSetContext.DataStore)
        this.codeSnippets = language == "VB" ? SampleCodeGenerator.SampleCodeSnippetFactory.DataStoreVB : SampleCodeGenerator.SampleCodeSnippetFactory.DataStoreCSharp;
      this.dataSet = dataSet;
      this.sourceCodeFile = sourceCodeFile;
    }

    public static void GenerateSourceCodeFile(SampleDataSet dataSet, string sourceCodeFile, string language)
    {
      new SampleCodeGenerator(dataSet, sourceCodeFile, language).GenerateCode();
    }

    public void GenerateCode()
    {
      if (this.dataSet.Context == DataSetContext.SampleData)
      {
        this.GenerateCodeForSampleData();
      }
      else
      {
        if (this.dataSet.Context != DataSetContext.DataStore)
          return;
        this.GenerateCodeForDataStore();
      }
    }

    public void GenerateCodeForDataStore()
    {
      using (this.textWriter = (TextWriter) new StreamWriter(this.sourceCodeFile, false, Encoding.UTF8))
      {
        this.textWriter.WriteLine((this.codeSnippets.Comment + StringTable.SampleDataDoNotEditComment.TrimEnd().TrimStart(SampleCodeGenerator.EOL.ToCharArray())).Replace(SampleCodeGenerator.EOL, SampleCodeGenerator.EOL + this.codeSnippets.Comment));
        string str1 = this.dataSet.ClrNamespace;
        string rootNamespace = this.dataSet.ProjectContext.RootNamespace;
        if (!string.IsNullOrEmpty(rootNamespace))
        {
          string str2 = rootNamespace + ".";
          str1 = str1.Substring(str2.Length);
        }
        this.WriteSnippet(this.codeSnippets.NamespaceHeader, SampleCodeGenerator.ClrNamespaceTemplate, str1);
        this.textWriter.WriteLine();
        this.WriteSnippet(this.codeSnippets.GlobalStorageTypeHeader, SampleCodeGenerator.GlobalStorageTypeTemplate, this.dataSet.RootType.Name + "GlobalStorage", SampleCodeGenerator.DataStoreTypeTemplate, this.dataSet.RootType.Name);
        foreach (SampleProperty sampleProperty in (IEnumerable<SampleProperty>) this.dataSet.RootType.SampleProperties)
        {
          this.textWriter.WriteLine();
          string typeName = this.codeSnippets.GetTypeName(sampleProperty.PropertySampleType);
          string propertyDefaultValue = this.codeSnippets.GetPropertyDefaultValue(sampleProperty);
          this.WriteSnippet(this.codeSnippets.GlobalStorageGetSetProperty, SampleCodeGenerator.PropertyNameTemplate, sampleProperty.Name, SampleCodeGenerator.PropertyTypeTemplate, typeName, SampleCodeGenerator.PropertyValueTemplate, propertyDefaultValue);
        }
        this.WriteSnippet(this.codeSnippets.GlobalStorageTypeFooter);
        this.textWriter.WriteLine();
        this.WriteSnippet(this.codeSnippets.DataStoreTypeHeader, SampleCodeGenerator.DataStoreTypeTemplate, this.dataSet.Name);
        this.textWriter.WriteLine();
        this.WriteSnippet(this.codeSnippets.DataStoreTypeConstructor, SampleCodeGenerator.DataStoreTypeTemplate, this.dataSet.Name, SampleCodeGenerator.AssemblyTemplate, this.dataSet.ProjectContext.ProjectAssembly.Name, SampleCodeGenerator.DataStoreRootFolderTemplate, this.dataSet.Context.DataRootFolder, SampleCodeGenerator.DataStoreNameTemplate, this.dataSet.Name, SampleCodeGenerator.GlobalStorageTypeTemplate, this.dataSet.RootType.Name + "GlobalStorage");
        foreach (SampleProperty sampleProperty in (IEnumerable<SampleProperty>) this.dataSet.RootType.SampleProperties)
        {
          this.textWriter.WriteLine();
          string typeName = this.codeSnippets.GetTypeName(sampleProperty.PropertySampleType);
          string propertyDefaultValue = this.codeSnippets.GetPropertyDefaultValue(sampleProperty);
          this.WriteSnippet(this.codeSnippets.DataStoreGetSetProperty, SampleCodeGenerator.PropertyNameTemplate, sampleProperty.Name, SampleCodeGenerator.PropertyTypeTemplate, typeName, SampleCodeGenerator.PropertyValueTemplate, propertyDefaultValue, SampleCodeGenerator.GlobalStorageTypeTemplate, this.dataSet.RootType.Name + "GlobalStorage");
        }
        this.WriteSnippet(this.codeSnippets.DataStoreTypeFooter);
        this.WriteSnippet(this.codeSnippets.NamespaceFooter);
      }
    }

    public void GenerateCodeForSampleData()
    {
      using (this.textWriter = (TextWriter) new StreamWriter(this.sourceCodeFile, false, Encoding.UTF8))
      {
        this.textWriter.WriteLine((this.codeSnippets.Comment + StringTable.SampleDataDoNotEditComment.TrimEnd().TrimStart(SampleCodeGenerator.EOL.ToCharArray())).Replace(SampleCodeGenerator.EOL, SampleCodeGenerator.EOL + this.codeSnippets.Comment));
        string str1 = this.dataSet.ClrNamespace;
        string rootNamespace = this.dataSet.ProjectContext.RootNamespace;
        if (!string.IsNullOrEmpty(rootNamespace))
        {
          string str2 = rootNamespace + ".";
          str1 = str1.Substring(str2.Length);
        }
        this.WriteSnippet(this.codeSnippets.NamespaceHeader, SampleCodeGenerator.ClrNamespaceTemplate, str1, SampleCodeGenerator.CompositeTypeTemplate, this.dataSet.RootType.Name);
        this.textWriter.WriteLine();
        this.WriteCompositeType(this.dataSet.RootType, true);
        foreach (SampleNonBasicType sampleNonBasicType in this.dataSet.Types)
        {
          if (sampleNonBasicType != this.dataSet.RootType)
          {
            this.textWriter.WriteLine();
            if (sampleNonBasicType.IsCollection)
              this.WriteCollectionType((SampleCollectionType) sampleNonBasicType);
            else
              this.WriteCompositeType((SampleCompositeType) sampleNonBasicType, false);
          }
        }
        this.WriteSnippet(this.codeSnippets.NamespaceFooter);
      }
      this.textWriter = (TextWriter) null;
    }

    private void WriteCompositeType(SampleCompositeType compositeType, bool root)
    {
      string typeName1 = this.codeSnippets.GetTypeName((SampleType) compositeType);
      this.WriteSnippet(this.codeSnippets.CompositeTypeHeader, SampleCodeGenerator.CompositeTypeTemplate, typeName1);
      if (root)
      {
        this.textWriter.WriteLine();
        this.WriteSnippet(this.codeSnippets.RootTypeConstructor, SampleCodeGenerator.CompositeTypeTemplate, typeName1, SampleCodeGenerator.AssemblyTemplate, this.dataSet.ProjectContext.ProjectAssembly.Name, SampleCodeGenerator.RootFolderTemplate, this.dataSet.Context.DataRootFolder, SampleCodeGenerator.SampleDataNameTemplate, this.dataSet.Name);
      }
      foreach (SampleProperty sampleProperty in (IEnumerable<SampleProperty>) compositeType.SampleProperties)
      {
        this.textWriter.WriteLine();
        string typeName2 = this.codeSnippets.GetTypeName(sampleProperty.PropertySampleType);
        string propertyDefaultValue = this.codeSnippets.GetPropertyDefaultValue(sampleProperty);
        if (sampleProperty.IsCollection)
          this.WriteSnippet(this.codeSnippets.GetProperty, SampleCodeGenerator.PropertyNameTemplate, sampleProperty.Name, SampleCodeGenerator.PropertyTypeTemplate, typeName2, SampleCodeGenerator.PropertyValueTemplate, propertyDefaultValue);
        else
          this.WriteSnippet(this.codeSnippets.GetSetProperty, SampleCodeGenerator.PropertyNameTemplate, sampleProperty.Name, SampleCodeGenerator.PropertyTypeTemplate, typeName2, SampleCodeGenerator.PropertyValueTemplate, propertyDefaultValue);
      }
      this.WriteSnippet(this.codeSnippets.CompositeTypeFooter);
    }

    private void WriteCollectionType(SampleCollectionType collectionType)
    {
      string typeName1 = this.codeSnippets.GetTypeName((SampleType) collectionType);
      string typeName2 = this.codeSnippets.GetTypeName(collectionType.ItemSampleType);
      this.WriteSnippet(this.codeSnippets.CollectionType, SampleCodeGenerator.CollectionTypeTemplate, typeName1, SampleCodeGenerator.ItemTypeTemplate, typeName2);
    }

    private void WriteSnippet(string snippet, params string[] templateValuePairs)
    {
      string str = snippet;
      int index = 0;
      while (index < templateValuePairs.Length)
      {
        string oldValue = templateValuePairs[index];
        string newValue = templateValuePairs[index + 1];
        str = str.Replace(oldValue, newValue);
        index += 2;
      }
      this.textWriter.WriteLine(str);
    }

    internal class SampleCodeSnippets
    {
      public DataSetContext Context { get; internal set; }

      public string NamespaceHeader { get; internal set; }

      public string NamespaceFooter { get; internal set; }

      public string CompositeTypeHeader { get; internal set; }

      public string CompositeTypeFooter { get; internal set; }

      public string GetSetProperty { get; internal set; }

      public string GetProperty { get; internal set; }

      public string CollectionType { get; internal set; }

      public string RootTypeConstructor { get; internal set; }

      public string NewInstanceFieldValue { get; internal set; }

      public string Comment { get; internal set; }

      public string GlobalStorageTypeHeader { get; internal set; }

      public string GlobalStorageTypeFooter { get; internal set; }

      public string GlobalStorageGetSetProperty { get; internal set; }

      public string DataStoreTypeHeader { get; internal set; }

      public string DataStoreTypeFooter { get; internal set; }

      public string DataStoreTypeConstructor { get; internal set; }

      public string DataStoreGetSetProperty { get; internal set; }

      public Dictionary<SampleBasicType, string> BasicTypeNames { get; internal set; }

      public Dictionary<SampleBasicType, string> BasicTypeValues { get; internal set; }

      public string GetTypeName(SampleType sampleType)
      {
        if (!sampleType.IsBasicType)
          return sampleType.Name;
        return this.BasicTypeNames[(SampleBasicType) sampleType];
      }

      public string GetPropertyDefaultValue(SampleProperty sampleProperty)
      {
        SampleType propertySampleType = sampleProperty.PropertySampleType;
        return !propertySampleType.IsBasicType ? this.NewInstanceFieldValue.Replace(SampleCodeGenerator.PropertyTypeTemplate, propertySampleType.Name) : this.BasicTypeValues[(SampleBasicType) propertySampleType];
      }
    }

    private static class SampleCodeSnippetFactory
    {
      private static SampleCodeGenerator.SampleCodeSnippets dataStoreCSharp;
      private static SampleCodeGenerator.SampleCodeSnippets dataStoreVB;
      private static SampleCodeGenerator.SampleCodeSnippets cSharp;
      private static SampleCodeGenerator.SampleCodeSnippets vb;

      public static SampleCodeGenerator.SampleCodeSnippets DataStoreCSharp
      {
        get
        {
          if (SampleCodeGenerator.SampleCodeSnippetFactory.dataStoreCSharp == null)
          {
            SampleCodeGenerator.SampleCodeSnippets storeCodeSnippets = SampleCodeGenerator.SampleCodeSnippetFactory.CreateDataStoreCodeSnippets("DataStoreCode.cs", "//");
            SampleCodeGenerator.SampleCodeSnippetFactory.InitializeForCSharp(storeCodeSnippets);
            SampleCodeGenerator.SampleCodeSnippetFactory.dataStoreCSharp = storeCodeSnippets;
          }
          return SampleCodeGenerator.SampleCodeSnippetFactory.dataStoreCSharp;
        }
      }

      public static SampleCodeGenerator.SampleCodeSnippets DataStoreVB
      {
        get
        {
          if (SampleCodeGenerator.SampleCodeSnippetFactory.dataStoreVB == null)
          {
            SampleCodeGenerator.SampleCodeSnippets storeCodeSnippets = SampleCodeGenerator.SampleCodeSnippetFactory.CreateDataStoreCodeSnippets("DataStoreCode.vb", "'");
            SampleCodeGenerator.SampleCodeSnippetFactory.InitializeForVB(storeCodeSnippets);
            SampleCodeGenerator.SampleCodeSnippetFactory.dataStoreVB = storeCodeSnippets;
          }
          return SampleCodeGenerator.SampleCodeSnippetFactory.dataStoreVB;
        }
      }

      public static SampleCodeGenerator.SampleCodeSnippets CSharp
      {
        get
        {
          if (SampleCodeGenerator.SampleCodeSnippetFactory.cSharp == null)
          {
            SampleCodeGenerator.SampleCodeSnippets dataCodeSnippets = SampleCodeGenerator.SampleCodeSnippetFactory.CreateSampleDataCodeSnippets("SampleDataCode.cs", "//");
            SampleCodeGenerator.SampleCodeSnippetFactory.InitializeForCSharp(dataCodeSnippets);
            SampleCodeGenerator.SampleCodeSnippetFactory.cSharp = dataCodeSnippets;
          }
          return SampleCodeGenerator.SampleCodeSnippetFactory.cSharp;
        }
      }

      public static SampleCodeGenerator.SampleCodeSnippets VB
      {
        get
        {
          if (SampleCodeGenerator.SampleCodeSnippetFactory.vb == null)
          {
            SampleCodeGenerator.SampleCodeSnippets dataCodeSnippets = SampleCodeGenerator.SampleCodeSnippetFactory.CreateSampleDataCodeSnippets("SampleDataCode.vb", "'");
            SampleCodeGenerator.SampleCodeSnippetFactory.InitializeForVB(dataCodeSnippets);
            SampleCodeGenerator.SampleCodeSnippetFactory.vb = dataCodeSnippets;
          }
          return SampleCodeGenerator.SampleCodeSnippetFactory.vb;
        }
      }

      private static void InitializeForVB(SampleCodeGenerator.SampleCodeSnippets codeSnippets)
      {
        codeSnippets.NewInstanceFieldValue = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "New {0}", new object[1]
        {
          (object) SampleCodeGenerator.PropertyTypeTemplate
        });
        codeSnippets.BasicTypeValues = new Dictionary<SampleBasicType, string>(5);
        codeSnippets.BasicTypeValues[SampleBasicType.String] = "String.Empty";
        codeSnippets.BasicTypeValues[SampleBasicType.Date] = "DateTime.Today";
        codeSnippets.BasicTypeValues[SampleBasicType.Number] = "0";
        codeSnippets.BasicTypeValues[SampleBasicType.Boolean] = "False";
        codeSnippets.BasicTypeValues[SampleBasicType.Image] = "Nothing";
        codeSnippets.BasicTypeNames = new Dictionary<SampleBasicType, string>(5);
        codeSnippets.BasicTypeNames[SampleBasicType.String] = "String";
        codeSnippets.BasicTypeNames[SampleBasicType.Date] = "System.DateTime";
        codeSnippets.BasicTypeNames[SampleBasicType.Number] = "Double";
        codeSnippets.BasicTypeNames[SampleBasicType.Boolean] = "Boolean";
        codeSnippets.BasicTypeNames[SampleBasicType.Image] = "System.Windows.Media.ImageSource";
      }

      private static void InitializeForCSharp(SampleCodeGenerator.SampleCodeSnippets codeSnippets)
      {
        codeSnippets.NewInstanceFieldValue = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "new {0}()", new object[1]
        {
          (object) SampleCodeGenerator.PropertyTypeTemplate
        });
        codeSnippets.BasicTypeValues = new Dictionary<SampleBasicType, string>(5);
        codeSnippets.BasicTypeValues[SampleBasicType.String] = "string.Empty";
        codeSnippets.BasicTypeValues[SampleBasicType.Date] = "DateTime.Today";
        codeSnippets.BasicTypeValues[SampleBasicType.Number] = "0";
        codeSnippets.BasicTypeValues[SampleBasicType.Boolean] = "false";
        codeSnippets.BasicTypeValues[SampleBasicType.Image] = "null";
        codeSnippets.BasicTypeNames = new Dictionary<SampleBasicType, string>(5);
        codeSnippets.BasicTypeNames[SampleBasicType.String] = "string";
        codeSnippets.BasicTypeNames[SampleBasicType.Date] = "System.DateTime";
        codeSnippets.BasicTypeNames[SampleBasicType.Number] = "double";
        codeSnippets.BasicTypeNames[SampleBasicType.Boolean] = "bool";
        codeSnippets.BasicTypeNames[SampleBasicType.Image] = "System.Windows.Media.ImageSource";
      }

      private static SampleCodeGenerator.SampleCodeSnippets CreateDataStoreCodeSnippets(string templateFileName, string comment)
      {
        SampleCodeGenerator.SampleCodeSnippets sampleCodeSnippets = new SampleCodeGenerator.SampleCodeSnippets();
        sampleCodeSnippets.Comment = comment;
        string codeTemplate;
        using (StreamReader streamReader = new StreamReader(Path.Combine(TemplateManager.TranslatedFolder("Templates"), templateFileName), true))
          codeTemplate = streamReader.ReadToEnd();
        sampleCodeSnippets.NamespaceHeader = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "ClrNamespaceHeader", comment);
        sampleCodeSnippets.NamespaceFooter = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "ClrNamespaceFooter", comment);
        sampleCodeSnippets.GlobalStorageTypeHeader = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "GlobalStorageTypeHeader", comment);
        sampleCodeSnippets.GlobalStorageTypeFooter = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "GlobalStorageTypeFooter", comment);
        sampleCodeSnippets.GlobalStorageGetSetProperty = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "GlobalStorageGetSetProperty", comment);
        sampleCodeSnippets.DataStoreTypeHeader = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "DataStoreTypeHeader", comment);
        sampleCodeSnippets.DataStoreTypeFooter = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "DataStoreTypeFooter", comment);
        sampleCodeSnippets.DataStoreTypeConstructor = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "DataStoreTypeConstructor", comment);
        sampleCodeSnippets.DataStoreGetSetProperty = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "DataStoreGetSetProperty", comment);
        return sampleCodeSnippets;
      }

      private static SampleCodeGenerator.SampleCodeSnippets CreateSampleDataCodeSnippets(string templateFileName, string comment)
      {
        SampleCodeGenerator.SampleCodeSnippets sampleCodeSnippets = new SampleCodeGenerator.SampleCodeSnippets();
        sampleCodeSnippets.Comment = comment;
        string codeTemplate;
        using (StreamReader streamReader = new StreamReader(Path.Combine(TemplateManager.TranslatedFolder("Templates"), templateFileName), true))
          codeTemplate = streamReader.ReadToEnd();
        sampleCodeSnippets.NamespaceHeader = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "ClrNamespaceHeader", comment);
        sampleCodeSnippets.NamespaceFooter = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "ClrNamespaceFooter", comment);
        sampleCodeSnippets.CompositeTypeHeader = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "CompositeTypeHeader", comment);
        sampleCodeSnippets.CompositeTypeFooter = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "CompositeTypeFooter", comment);
        sampleCodeSnippets.RootTypeConstructor = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "RootTypeConstructor", comment);
        sampleCodeSnippets.GetSetProperty = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "GetSetProperty", comment);
        sampleCodeSnippets.GetProperty = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "GetProperty", comment);
        sampleCodeSnippets.CollectionType = SampleCodeGenerator.SampleCodeSnippetFactory.ExtractSnippet(codeTemplate, "CollectionType", comment);
        return sampleCodeSnippets;
      }

      private static string ExtractSnippet(string codeTemplate, string snippetName, string comment)
      {
        string str1 = comment + snippetName;
        int num1 = codeTemplate.IndexOf(str1, StringComparison.Ordinal);
        int startIndex1 = codeTemplate.LastIndexOf(SampleCodeGenerator.EOL, num1, num1, StringComparison.Ordinal);
        string str2 = codeTemplate.Substring(startIndex1, num1 - startIndex1).TrimEnd();
        int num2 = codeTemplate.IndexOf(str1, num1 + str1.Length, StringComparison.Ordinal);
        string str3;
        if (num2 < 0)
        {
          str3 = str2;
        }
        else
        {
          int startIndex2 = codeTemplate.IndexOf(SampleCodeGenerator.EOL, num1, StringComparison.Ordinal);
          string str4 = codeTemplate.Substring(startIndex2, num2 - startIndex2).TrimEnd();
          str3 = str2 + str4;
        }
        return str3.TrimStart(SampleCodeGenerator.EOL.ToCharArray());
      }
    }
  }
}
