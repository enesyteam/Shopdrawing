// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.GoToXamlCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class GoToXamlCommand : SceneCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled && (!this.SceneViewModel.ElementSelectionSet.IsEmpty || !this.SceneViewModel.BehaviorSelectionSet.IsEmpty || !this.SceneViewModel.ChildPropertySelectionSet.IsEmpty && this.SceneViewModel.ChildPropertySelectionSet.PrimarySelection is EffectNode))
          return this.SceneView.IsDesignSurfaceVisible;
        return false;
      }
    }

    public GoToXamlCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      List<DocumentNode> targetNodes = new List<DocumentNode>();
      foreach (SceneElement sceneElement in this.SceneViewModel.ElementSelectionSet.Selection)
        targetNodes.Add(sceneElement.DocumentNode);
      foreach (BehaviorBaseNode behaviorBaseNode in this.SceneViewModel.BehaviorSelectionSet.Selection)
        targetNodes.Add(behaviorBaseNode.DocumentNode);
      GoToXamlCommand.GoToXaml(this.SceneViewModel.XamlDocument, targetNodes);
    }

    public static void GoToXaml(SceneXamlDocument document, List<DocumentNode> targetNodes)
    {
      GoToXamlCommand.GoToXaml((SceneView) null, document, targetNodes, false, true);
    }

    public static void GoToXaml(SceneView sceneView, SceneXamlDocument document, List<DocumentNode> targetNodes, bool selectElementNameOnly, bool setFocusToXamlEditor)
    {
      ITextRange selectionSpan = TextRange.Null;
      bool flag = false;
      ProjectXamlContext projectXamlContext = ProjectXamlContext.FromProjectContext(document.ProjectContext);
      if (projectXamlContext == null)
        return;
      DocumentNode rootNode = document.RootNode;
      if (rootNode != null && targetNodes != null)
      {
        foreach (DocumentNode documentNode in targetNodes)
        {
          DocumentNode node = GoToXamlCommand.GetCorrespondingDocumentNode(documentNode, rootNode);
          if (node != null)
          {
            ITextRange nodeSpan;
            for (nodeSpan = DocumentNodeHelper.GetNodeSpan(node); TextRange.IsNull(nodeSpan) && node != null && node != rootNode; nodeSpan = DocumentNodeHelper.GetNodeSpan(node))
              node = (DocumentNode) node.Parent;
            if (!TextRange.IsNull(nodeSpan))
            {
              if (!flag)
              {
                flag = true;
                selectionSpan = nodeSpan;
              }
              else
                selectionSpan = TextRange.Union(selectionSpan, nodeSpan);
            }
          }
        }
      }
      if (sceneView == null)
        sceneView = projectXamlContext.OpenView((IDocumentRoot) document, true);
      using (sceneView.DisableSelectionSynchronization())
      {
        sceneView.EnsureXamlEditorVisible();
        if (!flag || sceneView.CodeEditor == null)
          return;
        ITextEditor textEditor = sceneView.CodeEditor;
        textEditor.ClearSelection();
        if (selectElementNameOnly)
          selectionSpan = GoToXamlCommand.GetElementNameSelectionSpan((IReadableSelectableTextBuffer) document.TextBuffer, selectionSpan);
        textEditor.Select(selectionSpan.Offset, selectionSpan.Length);
        textEditor.CaretPosition = selectionSpan.Offset + selectionSpan.Length;
        Action action = (Action) (() =>
        {
          textEditor.EnsureSpanVisible(selectionSpan.Offset, selectionSpan.Length);
          textEditor.EnsureCaretVisible();
          textEditor.MoveLineToCenterOfView(textEditor.GetLineNumberFromPosition(selectionSpan.Offset));
          if (!setFocusToXamlEditor)
            return;
          textEditor.Focus();
        });
        if (SceneViewUpdateScheduleTask.Synchronous)
          action();
        else
          UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Render, action);
      }
    }

    public static ITextRange GetElementNameSelectionSpan(IReadableSelectableTextBuffer textBuffer, ITextRange elementSpan)
    {
      int start = elementSpan.Offset + 1;
      int num = elementSpan.Length - 1;
      int offset = start;
      while (offset < textBuffer.Length)
      {
        int length = Math.Min(32, textBuffer.Length - offset);
        string text = textBuffer.GetText(offset, length);
        for (int index = 0; index < text.Length; ++index)
        {
          if (char.IsWhiteSpace(text[index]) || (int) text[index] == 47 || (int) text[index] == 62)
          {
            num = index + offset - start;
            offset = textBuffer.Length;
            break;
          }
        }
        offset += length;
      }
      return (ITextRange) new TextRange(start, start + num);
    }

    private static DocumentNode GetCorrespondingDocumentNode(DocumentNode documentNode, DocumentNode rootNode)
    {
      if (documentNode.Parent == null)
        return rootNode;
      DocumentCompositeNode documentCompositeNode = GoToXamlCommand.GetCorrespondingDocumentNode((DocumentNode) documentNode.Parent, rootNode) as DocumentCompositeNode;
      if (documentCompositeNode != null)
      {
        if (documentNode.IsProperty)
          return documentCompositeNode.Properties[(IPropertyId) documentNode.SitePropertyKey];
        if (documentCompositeNode.SupportsChildren && documentCompositeNode.Children.Count > documentNode.SiteChildIndex && documentNode.SiteChildIndex >= 0)
          return documentCompositeNode.Children[documentNode.SiteChildIndex];
      }
      return (DocumentNode) null;
    }
  }
}
