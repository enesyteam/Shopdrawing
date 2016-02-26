// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DataViewTemplate
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.DesignSurface.SampleData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public class DataViewTemplate
  {
    private static readonly string labelPrefix = "Label_";
    private static readonly string fieldPrefix = "Field_";

    public DocumentCompositeNode RootNode { get; private set; }

    public List<DataViewTemplateEntry> TemplateEntries { get; private set; }

    public DataViewCategory Category { get; private set; }

    public DataViewTemplate(IPlatform platform, DataViewCategory category)
    {
      this.Category = category;
      this.TemplateEntries = new List<DataViewTemplateEntry>();
      this.Load(platform);
    }

    private void Load(IPlatform platform)
    {
      SimpleTextBuffer simpleTextBuffer = new SimpleTextBuffer();
      using (StreamReader streamReader = new StreamReader(this.GetDataViewTemplateFile()))
      {
        string text = streamReader.ReadToEnd();
        simpleTextBuffer.SetText(0, text.Length, text);
      }
      this.Initialize((DocumentCompositeNode) XamlParserResults.Parse((IDocumentContext) new DocumentContext((IProjectContext) new DefaultProjectContext(platform), (IDocumentLocator) null, true), PlatformTypes.Panel, (IReadableSelectableTextBuffer) simpleTextBuffer).RootNode);
    }

    private string GetDataViewTemplateFile()
    {
      return Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof (DataViewTemplate)).Location), "DataBinding"), (string) (object) this.Category + (object) ".xaml");
    }

    private void Initialize(DocumentCompositeNode rootNode)
    {
      IProperty defaultContentProperty = rootNode.Type.Metadata.DefaultContentProperty;
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) rootNode.Properties[(IPropertyId) defaultContentProperty];
      for (int index = documentCompositeNode.Children.Count - 1; index >= 0; --index)
      {
        DocumentCompositeNode entryNode = documentCompositeNode.Children[index] as DocumentCompositeNode;
        if (entryNode != null)
        {
          string name = entryNode.Name;
          if (!string.IsNullOrEmpty(name))
          {
            if (name.StartsWith(DataViewTemplate.labelPrefix, StringComparison.Ordinal))
            {
              string typeName = name.Substring(DataViewTemplate.labelPrefix.Length);
              this.UpdateTemplateEntry(entryNode, typeName, true);
              documentCompositeNode.Children.RemoveAt(index);
            }
            else if (name.StartsWith(DataViewTemplate.fieldPrefix, StringComparison.Ordinal))
            {
              string typeName = name.Substring(DataViewTemplate.fieldPrefix.Length);
              this.UpdateTemplateEntry(entryNode, typeName, false);
              documentCompositeNode.Children.RemoveAt(index);
            }
          }
        }
      }
      rootNode.SourceContext = (INodeSourceContext) null;
      this.RootNode = rootNode;
    }

    private void UpdateTemplateEntry(DocumentCompositeNode entryNode, string typeName, bool isLabel)
    {
      DataViewTemplateEntry createTemplateEntry = this.GetOrCreateTemplateEntry(entryNode.PlatformMetadata, typeName);
      IProperty property = Enumerable.First<IProperty>((IEnumerable<IProperty>) entryNode.Properties.Keys, (Func<IProperty, bool>) (p => entryNode.Properties[(IPropertyId) p].Type.IsBinding));
      entryNode.Name = (string) null;
      entryNode.SourceContext = (INodeSourceContext) null;
      if (isLabel)
      {
        createTemplateEntry.LabelNode = entryNode;
        createTemplateEntry.LabelValueProperty = property;
        createTemplateEntry.LabelNode.ClearValue((IPropertyId) property);
      }
      else
      {
        createTemplateEntry.FieldNode = entryNode;
        createTemplateEntry.FieldValueProperty = property;
      }
    }

    private DataViewTemplateEntry GetOrCreateTemplateEntry(IPlatformMetadata platformMetadata, string typeName)
    {
      IType platformType = ((IPlatformTypes) platformMetadata).GetPlatformType(typeName);
      DataViewTemplateEntry viewTemplateEntry = this.TemplateEntries.Find((Predicate<DataViewTemplateEntry>) (t => t.DataType == platformType));
      if (viewTemplateEntry == null)
      {
        viewTemplateEntry = new DataViewTemplateEntry((ITypeId) platformType);
        this.TemplateEntries.Add(viewTemplateEntry);
      }
      return viewTemplateEntry;
    }
  }
}
