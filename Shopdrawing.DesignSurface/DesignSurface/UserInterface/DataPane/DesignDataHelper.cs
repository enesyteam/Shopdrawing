// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DesignDataHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public static class DesignDataHelper
  {
    public static IProjectItem GetDesignDataFile(DocumentNode documentNode)
    {
      IProject project = (IProject) ((IServiceProvider) documentNode.TypeResolver).GetService(typeof (IProject));
      string path = (string) null;
      if (documentNode.Type.RuntimeType == typeof (DesignDataExtension))
        path = DesignDataInstanceBuilder.GetSourceFilePath((DocumentCompositeNode) documentNode);
      else if (documentNode.Parent == null)
        path = documentNode.Context.DocumentUrl;
      if (!string.IsNullOrEmpty(path))
      {
        IProjectItem projectItem = project.FindItem(DocumentReference.Create(path));
        if (projectItem != null && DocumentContextHelper.GetDesignDataMode(projectItem) != DesignDataMode.None)
          return projectItem;
      }
      return (IProjectItem) null;
    }

    public static DocumentNode GetRootDesignDataNode(DocumentNode documentNode)
    {
      return DesignDataHelper.GetRootDesignDataNode(DesignDataHelper.GetDesignDataFile(documentNode), (IProjectContext) documentNode.TypeResolver);
    }

    public static DocumentNode GetRootDesignDataNode(IProjectItem designDataFile, IProjectContext projectContext)
    {
      if (designDataFile == null)
        return (DocumentNode) null;
      return DocumentContextHelper.GetParsedOrSniffedRootNode(designDataFile, projectContext);
    }

    public static DocumentNode CreateDesignDataExtension(IProjectItem designDataFile, IDocumentContext documentContext)
    {
      DesignDataExtension designDataExtension = new DesignDataExtension(DesignDataHelper.GetDesignDataPath(designDataFile));
      return documentContext.CreateNode(designDataExtension.GetType(), (object) designDataExtension);
    }

    public static IProjectItem PromptAndCreateDesignData(SceneViewModel viewModel)
    {
      DesignObjectDataSourceDialog dataSourceDialog = new DesignObjectDataSourceDialog(viewModel.DataPanelModel);
      bool? nullable = dataSourceDialog.ShowDialog();
      if (nullable.HasValue)
      {
        if (nullable.Value)
        {
          try
          {
            using (TemporaryCursor.SetWaitCursor())
              return DesignDataHelper.CreateDesignDataFile(dataSourceDialog.ObjectType, dataSourceDialog.DataSourceName.Trim(), viewModel.ProjectContext, false);
          }
          catch (Exception ex)
          {
            viewModel.DesignerContext.MessageDisplayService.ShowError(ex.Message);
          }
        }
      }
      return (IProjectItem) null;
    }

    public static IProjectItem CreateDesignDataFile(Type type, string dataSourceName, IProjectContext projectContext, bool isDesignTimeCreatable)
    {
      string path1 = Path.Combine(Path.GetDirectoryName(projectContext.ProjectPath), DataSetContext.SampleData.DataRootFolder);
      string str = dataSourceName ?? type.Name;
      string path2_1 = str + ".xaml";
      string path2 = Path.Combine(path1, path2_1);
      int num = 1;
      while (File.Exists(path2))
      {
        string path2_2 = str + num.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ".xaml";
        path2 = Path.Combine(path1, path2_2);
        ++num;
      }
      string path3 = Path.GetTempFileName() + ".xaml";
      IProjectContext projectContext1 = projectContext;
      if (!isDesignTimeCreatable)
        projectContext1 = (IProjectContext) (projectContext as TypeReflectingProjectContext) ?? (IProjectContext) new TypeReflectingProjectContext(projectContext);
      DocumentContext documentContext = new DocumentContext(projectContext1, (IDocumentLocator) new DocumentLocator(path2), false);
      IType type1 = documentContext.TypeResolver.GetType(type);
      DocumentNode node = new DesignDataGenerator(type1, (IDocumentContext) documentContext).Build();
      try
      {
        using (StreamWriter text = File.CreateText(path3))
        {
          using (XamlDocument xamlDocument = new XamlDocument((IDocumentContext) documentContext, (ITypeId) type1, (ITextBuffer) new SimpleTextBuffer(), DocumentEncodingHelper.DefaultEncoding, (IXamlSerializerFilter) new DefaultXamlSerializerFilter()))
            new XamlSerializer((IDocumentRoot) xamlDocument, (IXamlSerializerFilter) new DefaultXamlSerializerFilter()).Serialize(node, (TextWriter) text);
        }
        BuildTaskInfo buildTaskInfo = new BuildTaskInfo(DocumentContextHelper.DesignDataBuildTask, (IDictionary<string, string>) new Dictionary<string, string>());
        DocumentCreationInfo creationInfo = new DocumentCreationInfo()
        {
          BuildTaskInfo = buildTaskInfo,
          TargetFolder = path1,
          TargetPath = path2,
          SourcePath = path3,
          CreationOptions = CreationOptions.SilentlyOverwrite | CreationOptions.SilentlyOverwriteReadOnly | CreationOptions.DoNotSelectCreatedItems | CreationOptions.DoNotSetDefaultImportPath
        };
        return ((IProject) projectContext.GetService(typeof (IProject))).AddItem(creationInfo);
      }
      finally
      {
        try
        {
          File.Delete(path3);
        }
        catch
        {
        }
      }
    }

    public static string GetDesignDataPath(IProjectItem designDataFile)
    {
      return designDataFile.ProjectRelativeDocumentReference.Replace('\\', '/');
    }

    public static bool RemoveDesignData(IProjectItem designDataFile, IProjectContext projectContext, IExpressionInformationService service)
    {
      IProject project = (IProject) projectContext.GetService(typeof (IProject));
      string path = designDataFile.DocumentReference.Path;
      if (!project.RemoveItems(1 != 0, designDataFile))
        return false;
      new DesignDataRemovalProcessor((IAsyncMechanism) new CurrentDispatcherAsyncMechanism(DispatcherPriority.Background), path, projectContext, ChangeProcessingModes.CollectChanges | ChangeProcessingModes.ApplyChanges).Process(service);
      return true;
    }

    public static IType GetSourceType(IType type, ITypeResolver typeResolver)
    {
      if (typeResolver.PlatformMetadata.IsNullType((ITypeId) type))
        return type;
      DesignTypeResult designTypeResult = DesignTypeGenerator.LookupGetDesignTypeResult(type.RuntimeType);
      if (designTypeResult != null && !designTypeResult.IsFailed && designTypeResult.SourceType != type.RuntimeType)
      {
        ITypeResolver typeResolver1 = (ITypeResolver) ProjectXamlContext.FromProjectContext(typeResolver as IProjectContext) ?? typeResolver;
        IType type1 = typeResolver1.GetType(designTypeResult.SourceType);
        if (!typeResolver1.PlatformMetadata.IsNullType((ITypeId) type1))
          return type1;
      }
      return type;
    }

    public static bool CompareDataSources(DocumentNode leftSource, DocumentNode rightSource)
    {
      return leftSource == rightSource || leftSource != null && rightSource != null && DesignDataHelper.CompareDesignDataSources(leftSource, rightSource);
    }

    private static bool CompareDesignDataSources(DocumentNode leftSource, DocumentNode rightSource)
    {
      if (leftSource == null || rightSource == null)
        return false;
      IProjectItem designDataFile1 = DesignDataHelper.GetDesignDataFile(leftSource);
      if (designDataFile1 == null)
        return false;
      IProjectItem designDataFile2 = DesignDataHelper.GetDesignDataFile(rightSource);
      return designDataFile1 == designDataFile2;
    }
  }
}
