// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DesignDataSchemaManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface
{
  internal sealed class DesignDataSchemaManager : IDisposable
  {
    private Dictionary<IProjectItem, DesignDataSchemaManager.SchemaAndOpenState> designDataCache = new Dictionary<IProjectItem, DesignDataSchemaManager.SchemaAndOpenState>();
    private IProjectContext projectContext;

    public DesignDataSchemaManager(IProjectContext projectContext)
    {
      this.projectContext = projectContext;
      IProject project = (IProject) this.projectContext.GetService(typeof (IProject));
      project.ItemRemoved += new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
      project.ItemChanged += new EventHandler<ProjectItemEventArgs>(this.Project_ItemChanged);
    }

    public ISchema GetSchemaForDesignDataFile(IProjectItem designDataFile, IProjectContext projectContext)
    {
      DocumentNode documentNode = (DocumentNode) null;
      bool flag = false;
      DesignDataSchemaManager.SchemaAndOpenState schemaAndOpenState;
      if (this.designDataCache.TryGetValue(designDataFile, out schemaAndOpenState))
      {
        if (!designDataFile.IsOpen && !schemaAndOpenState.IsOpen)
          return schemaAndOpenState.Schema;
        schemaAndOpenState.IsOpen = designDataFile.IsOpen;
        ClrObjectSchema clrObjectSchema = schemaAndOpenState.Schema as ClrObjectSchema;
        if (clrObjectSchema != null)
        {
          documentNode = DesignDataHelper.GetRootDesignDataNode(designDataFile, projectContext);
          flag = true;
          if (clrObjectSchema.DataSource.UpdateDocumentNode(documentNode))
            return (ISchema) clrObjectSchema;
        }
      }
      if (!flag)
        documentNode = DesignDataHelper.GetRootDesignDataNode(designDataFile, projectContext);
      ISchema schema = documentNode == null || documentNode.Type.RuntimeType == (Type) null ? (ISchema) new EmptySchema() : (ISchema) new ClrObjectSchema(documentNode.Type.RuntimeType, documentNode);
      this.designDataCache[designDataFile] = new DesignDataSchemaManager.SchemaAndOpenState(schema, designDataFile.IsOpen);
      return schema;
    }

    public void Refresh(SampleDataSet changedSampleData)
    {
      List<IProjectItem> list = new List<IProjectItem>();
      foreach (KeyValuePair<IProjectItem, DesignDataSchemaManager.SchemaAndOpenState> keyValuePair in this.designDataCache)
      {
        ClrObjectSchema clrObjectSchema = keyValuePair.Value.Schema as ClrObjectSchema;
        bool flag = false;
        if (clrObjectSchema != null)
        {
          DocumentNode rootDesignDataNode = DesignDataHelper.GetRootDesignDataNode(keyValuePair.Key, (IProjectContext) clrObjectSchema.DataSource.DocumentNode.TypeResolver);
          if (rootDesignDataNode == null || rootDesignDataNode != clrObjectSchema.DataSource.DocumentNode)
            flag = true;
          else if (changedSampleData != null)
          {
            SampleNonBasicType sampleNonBasicType = rootDesignDataNode.Type as SampleNonBasicType;
            if (sampleNonBasicType != null && sampleNonBasicType.DeclaringDataSet == changedSampleData)
              flag = true;
          }
        }
        else
          flag = true;
        if (flag)
          list.Add(keyValuePair.Key);
      }
      foreach (IProjectItem key in list)
        this.designDataCache.Remove(key);
    }

    private void Project_ItemChanged(object sender, ProjectItemEventArgs e)
    {
      this.designDataCache.Remove(e.ProjectItem);
    }

    private void Project_ItemRemoved(object sender, ProjectItemEventArgs e)
    {
      this.designDataCache.Remove(e.ProjectItem);
    }

    public void Dispose()
    {
      if (this.projectContext != null)
      {
        IProject project = (IProject) this.projectContext.GetService(typeof (IProject));
        project.ItemRemoved -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
        project.ItemChanged -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemChanged);
        this.projectContext = (IProjectContext) null;
      }
      GC.SuppressFinalize((object) this);
    }

    private class SchemaAndOpenState
    {
      public ISchema Schema { get; private set; }

      public bool IsOpen { get; set; }

      public SchemaAndOpenState(ISchema schema, bool isOpen)
      {
        this.Schema = schema;
        this.IsOpen = isOpen;
      }
    }
  }
}
