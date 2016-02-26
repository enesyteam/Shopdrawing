// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SceneNodeIDHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.CSharp;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  internal sealed class SceneNodeIDHelper
  {
    private static CodeDomProvider codeDomProvider = (CodeDomProvider) new CSharpCodeProvider();
    private DocumentNodeNameScope nameScope;
    private string className;
    private DocumentNodeNameScope newNodeNameScope;

    public DocumentNodeNameScope NameScope
    {
      get
      {
        return this.nameScope;
      }
    }

    public SceneNodeIDHelper(SceneViewModel viewModel, SceneNode root)
    {
      this.nameScope = root.DocumentNode.FindNameScopeForChildren() ?? new DocumentNodeNameScope();
      this.InitializeClassName(viewModel);
    }

    public SceneNodeIDHelper(SceneViewModel viewModel, DocumentNodeNameScope nameScope)
    {
      this.nameScope = nameScope ?? new DocumentNodeNameScope();
      this.InitializeClassName(viewModel);
    }

    public SceneNodeIDHelper()
    {
      this.nameScope = new DocumentNodeNameScope();
    }

    private void InitializeClassName(SceneViewModel viewModel)
    {
      if (viewModel == null)
        return;
      IDocumentRoot documentRoot = viewModel.DocumentRoot;
      if (this.nameScope != documentRoot.RootNode.NameScope)
        return;
      ITypeId typeId = (ITypeId) documentRoot.CodeBehindClass;
      if (typeId == null)
        return;
      this.className = typeId.Name;
    }

    public void SetLocalName(SceneNode node, string newName)
    {
      node.SetLocalValue(node.NameProperty, (object) newName);
    }

    public void SetValidName(SceneNode sceneNode, string namePrefix)
    {
      if (string.IsNullOrEmpty(namePrefix))
        return;
      string validElementId = this.GetValidElementID(sceneNode, namePrefix);
      if (!(validElementId != sceneNode.Name))
        return;
      this.SetLocalName(sceneNode, validElementId);
    }

    public string GetValidElementID(SceneNode sceneNode, string namePrefix)
    {
      namePrefix = SceneNodeIDHelper.ToCSharpID(namePrefix);
      string namePrefix1 = this.GetNamePrefix(namePrefix);
      if (this.NameAlreadyUsed(sceneNode.DocumentNode, namePrefix))
        return this.GetFirstAvailableIndexedName(sceneNode.DocumentNode, namePrefix1);
      return namePrefix;
    }

    public string GetValidCopiedElementID(SceneNode sceneNode, string candidateID)
    {
      if (candidateID != null)
      {
        candidateID = SceneNodeIDHelper.ToCSharpID(candidateID);
        if (candidateID.Length == 0 || this.NameAlreadyUsed(sceneNode.DocumentNode, candidateID))
        {
          string namePrefix = this.GetNamePrefix(sceneNode.Name);
          if (namePrefix == null || !namePrefix.EndsWith(StringTable.SceneModelDuplicateLabelSuffix, StringComparison.Ordinal))
            namePrefix = candidateID + StringTable.SceneModelDuplicateLabelSuffix;
          candidateID = this.GetValidElementID(sceneNode, namePrefix);
        }
      }
      return candidateID;
    }

    public void FixNameConflicts(SceneNode rootNode)
    {
      this.FixNameConflicts(rootNode.DocumentNode);
    }

    public void FixNameConflicts(DocumentNode node)
    {
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      if (documentCompositeNode == null)
        return;
      string name = documentCompositeNode.Name;
      if (name != null)
      {
        if (this.NameAlreadyUsed((DocumentNode) documentCompositeNode, name))
        {
          string namePrefix = this.GetNamePrefix(name);
          documentCompositeNode.Name = this.GetFirstAvailableIndexedName((DocumentNode) documentCompositeNode, namePrefix);
        }
        this.AddNode(documentCompositeNode.Name, (DocumentNode) documentCompositeNode);
      }
      if (node.NameScope != null)
        return;
      if (documentCompositeNode.SupportsChildren)
      {
        for (int index = 0; index < documentCompositeNode.Children.Count; ++index)
          this.FixNameConflicts(documentCompositeNode.Children[index]);
      }
      for (int index = 0; index < documentCompositeNode.Properties.Count; ++index)
        this.FixNameConflicts(documentCompositeNode.Properties[index]);
    }

    public bool IsValidElementID(SceneNode sceneNode, string candidateID)
    {
      if (!SceneNodeIDHelper.IsCSharpID(candidateID))
        return false;
      return !this.NameAlreadyUsed(sceneNode != null ? sceneNode.DocumentNode : (DocumentNode) null, candidateID);
    }

    private bool NameAlreadyUsed(DocumentNode node, string candidateID)
    {
      if (this.className != null && string.Compare(this.className, candidateID, StringComparison.Ordinal) == 0)
        return true;
      DocumentNode node1 = this.FindNode(candidateID);
      return node1 != null && (node == null || node1 != node);
    }

    public static string DefaultNamePrefixForType(ITypeId type)
    {
      string name = type.Name;
      return char.ToLower(name[0], CultureInfo.InvariantCulture).ToString() + name.Substring(1);
    }

    public DocumentNode FindNode(string name)
    {
      DocumentNode node = this.nameScope.FindNode(name);
      if (node == null && this.newNodeNameScope != null)
        node = this.newNodeNameScope.FindNode(name);
      return node;
    }

    private void AddNode(string name, DocumentNode node)
    {
      if (this.nameScope.FindNode(name) != null)
        return;
      if (this.newNodeNameScope == null)
        this.newNodeNameScope = new DocumentNodeNameScope();
      this.newNodeNameScope.AddNode(name, node);
    }

    public static bool IsCSharpID(string original)
    {
      if (string.IsNullOrEmpty(original))
        return false;
      int length = original.Length;
      if (length <= 0)
        return true;
      if (!char.IsLetter(original[0]) && (int) original[0] != 95)
        return false;
      for (int index = 1; index < length; ++index)
      {
        if (!char.IsLetterOrDigit(original[index]) && (int) original[index] != 95)
          return false;
      }
      return SceneNodeIDHelper.codeDomProvider.IsValidIdentifier(original);
    }

    public static string ToCSharpID(string original)
    {
      if (original == null || original.Length <= 0)
        return original;
      int length = original.Length;
      StringBuilder stringBuilder = new StringBuilder(length, length + 1);
      if (!char.IsLetter(original[0]) && (int) original[0] != 95)
        stringBuilder.Append('_');
      for (int index = 0; index < length; ++index)
      {
        if (char.IsLetterOrDigit(original[index]) || (int) original[index] == 95)
          stringBuilder.Append(original[index]);
        else
          stringBuilder.Append('_');
      }
      return SceneNodeIDHelper.codeDomProvider.CreateValidIdentifier(stringBuilder.ToString());
    }

    private string GetFirstAvailableIndexedName(DocumentNode node, string namePrefix)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.GetFirstAvailableElementIDIndex);
      int num = 1;
      string str = namePrefix + num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      for (DocumentNode node1 = this.FindNode(str); node1 != null && node1 != node || string.Compare(str, this.className, StringComparison.Ordinal) == 0; node1 = this.FindNode(str))
      {
        ++num;
        str = namePrefix + num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.GetFirstAvailableElementIDIndex);
      return str;
    }

    private string GetNamePrefix(string name)
    {
      if (string.IsNullOrEmpty(name))
        return (string) null;
      int length1 = name.Length;
      int length2 = length1;
      while (length2 > 0 && char.IsDigit(name[length2 - 1]))
        --length2;
      if (length2 == 0)
        return (string) null;
      if (length2 == length1)
        return name;
      return name.Substring(0, length2);
    }
  }
}
