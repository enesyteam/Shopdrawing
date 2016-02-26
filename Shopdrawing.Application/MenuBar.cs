// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.MenuBar
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;

namespace Shopdrawing.App
{
  internal sealed class MenuBar
  {
    private MenuBar()
    {
    }

    private static KeyBinding CreateKeyBinding(Key key, ModifierKeys modifierKeys)
    {
      return new KeyBinding()
      {
        Modifiers = modifierKeys,
        Key = key
      };
    }

    public static void Create(ICommandBar menuBar, IServices services)
    {
      ICommandBarMenu commandBarMenu1 = menuBar.Items.AddMenu("File", StringTable.MenuBarFile);
      ICommandBarMenu commandBarMenu2 = menuBar.Items.AddMenu("Edit", StringTable.MenuBarEdit);
      ICommandBarMenu commandBarMenu3 = menuBar.Items.AddMenu("View", StringTable.MenuBarView);
      ICommandBarMenu commandBarMenu4 = menuBar.Items.AddMenu("Object", StringTable.MenuBarObject);
      ICommandBarMenu commandBarMenu5 = menuBar.Items.AddMenu("Project", StringTable.MenuBarProject);
      ICommandBarMenu commandBarMenu6 = menuBar.Items.AddMenu("Tools", StringTable.MenuBarTools);
      ICommandBarMenu commandBarMenu7 = menuBar.Items.AddMenu("Window", StringTable.MenuBarWindow);
      ICommandBarMenu commandBarMenu8 = menuBar.Items.AddMenu("Help", StringTable.MenuBarHelp);
      commandBarMenu1.Items.AddButton("Application_AddNewItem", StringTable.MenuBarFileMenuNewItem, MenuBar.CreateKeyBinding(Key.N, ModifierKeys.Control));
      commandBarMenu1.Items.AddButton("Application_NewProject", StringTable.MenuBarFileMenuNewProject, MenuBar.CreateKeyBinding(Key.N, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu1.Items.AddSeparator();
      commandBarMenu1.Items.AddButton("Project_OpenProject", StringTable.MenuBarFileMenuOpenProjectSolution, MenuBar.CreateKeyBinding(Key.O, ModifierKeys.Control | ModifierKeys.Shift));
      ICommandBarMenu commandBarMenu9 = commandBarMenu1.Items.AddMenu("RecentProjects", StringTable.MenuBarFileRecentProjects);
      for (int index = 0; index < 10; ++index)
        commandBarMenu9.Items.AddButton("Project_OpenRecentProject_" + index.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      commandBarMenu1.Items.AddSeparator();
      commandBarMenu1.Items.AddButton("Application_FileClose", StringTable.MenuBarFileMenuClose, MenuBar.CreateKeyBinding(Key.W, ModifierKeys.Control));
      commandBarMenu1.Items.AddButton("Application_CloseOtherDocuments", StringTable.MenuBarFileMenuCloseOther);
      commandBarMenu1.Items.AddButton("Application_CloseAllDocuments", StringTable.MenuBarFileMenuCloseAll, MenuBar.CreateKeyBinding(Key.W, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu1.Items.AddButton("Project_CloseProject", "");
      commandBarMenu1.Items.AddSeparator();
      commandBarMenu1.Items.AddButton("Application_FileSave", StringTable.MenuBarFileMenuSave, MenuBar.CreateKeyBinding(Key.S, ModifierKeys.Control));
      commandBarMenu1.Items.AddButton("Application_SaveAll", StringTable.MenuBarFileMenuSaveAll, MenuBar.CreateKeyBinding(Key.S, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu1.Items.AddButton("Project_SaveSolutionCopy", "", MenuBar.CreateKeyBinding(Key.P, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu1.Items.AddSeparator();
      commandBarMenu1.Items.AddButton("Application_Import_Photoshop", StringTable.MenuBarFileMenuImportPhotoshop);
      commandBarMenu1.Items.AddButton("Application_Import_Illustrator", StringTable.MenuBarFileMenuImportIllustrator);
      commandBarMenu1.Items.AddSeparator();
      commandBarMenu1.Items.AddButton("Application_Exit", StringTable.MenuBarFileMenuExit, MenuBar.CreateKeyBinding(Key.Q, ModifierKeys.Control));
      commandBarMenu2.Items.AddButton("Edit_Undo", StringTable.MenuBarEditMenuUndo, MenuBar.CreateKeyBinding(Key.Z, ModifierKeys.Control));
      commandBarMenu2.Items.AddButton("Edit_Redo", StringTable.MenuBarEditMenuRedo, MenuBar.CreateKeyBinding(Key.Y, ModifierKeys.Control), MenuBar.CreateKeyBinding(Key.Z, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu2.Items.AddSeparator();
      commandBarMenu2.Items.AddButton("Edit_Cut", StringTable.MenuBarEditMenuCut, MenuBar.CreateKeyBinding(Key.X, ModifierKeys.Control));
      commandBarMenu2.Items.AddButton("Edit_Copy", StringTable.MenuBarEditMenuCopy, MenuBar.CreateKeyBinding(Key.C, ModifierKeys.Control));
      commandBarMenu2.Items.AddButton("Edit_Paste", StringTable.MenuBarEditMenuPaste, MenuBar.CreateKeyBinding(Key.V, ModifierKeys.Control));
      commandBarMenu2.Items.AddButton("Edit_Delete", StringTable.MenuBarEditMenuDelete, MenuBar.CreateKeyBinding(Key.Delete, ModifierKeys.None));
      commandBarMenu2.Items.AddSeparator();
      commandBarMenu2.Items.AddButton("Edit_Find", StringTable.MenuBarEditMenuFind, MenuBar.CreateKeyBinding(Key.F, ModifierKeys.Control));
      commandBarMenu2.Items.AddButton("Edit_FindNext", StringTable.MenuBarEditMenuFindNext, MenuBar.CreateKeyBinding(Key.F3, ModifierKeys.None));
      commandBarMenu2.Items.AddButton("Edit_FindInFiles", StringTable.FindInFiles, MenuBar.CreateKeyBinding(Key.F, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu2.Items.AddButton("Edit_Replace", StringTable.MenuBarEditMenuReplace, MenuBar.CreateKeyBinding(Key.H, ModifierKeys.Control));
      commandBarMenu2.Items.AddButton("Edit_GoToLine", StringTable.MenuBarEditMenuGoTo);
      commandBarMenu2.Items.AddSeparator();
      commandBarMenu2.Items.AddButton("Edit_SelectAll", StringTable.MenuBarEditMenuSelectAll, MenuBar.CreateKeyBinding(Key.A, ModifierKeys.Control));
      commandBarMenu2.Items.AddButton("Edit_SelectNone", StringTable.MenuBarEditMenuSelectNone, MenuBar.CreateKeyBinding(Key.A, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu2.Items.AddSeparator();
      ICommandService service = services.GetService<ICommandService>();
      service.SetCommandPropertyDefault("Edit_CommentLines", "IsVisible", (object) false);
      service.SetCommandPropertyDefault("Edit_UncommentLines", "IsVisible", (object) false);
      commandBarMenu2.Items.AddButton("Edit_CommentLines", StringTable.MenuBarEditMenuCommentSelection, MenuBar.CreateKeyBinding(Key.C, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu2.Items.AddButton("Edit_UncommentLines", StringTable.MenuBarEditMenuUncommentSelection, MenuBar.CreateKeyBinding(Key.U, ModifierKeys.Control | ModifierKeys.Shift));
      ICommandBarMenu commandBarMenu10 = commandBarMenu3.Items.AddMenu("DocumentView", StringTable.MenuBarViewMenuDocumentMenu);
      commandBarMenu10.Items.AddCheckBox("View_ShowDesign", StringTable.MenuBarViewMenuDesign);
      commandBarMenu10.Items.AddCheckBox("View_ShowSplit", StringTable.MenuBarViewMenuSplit);
      commandBarMenu10.Items.AddCheckBox("View_ShowXaml", StringTable.MenuBarViewMenuXaml);
      commandBarMenu10.Items.AddSeparator();
      commandBarMenu10.Items.AddButton("View_CycleView", StringTable.MenuBarViewMenuCycle, MenuBar.CreateKeyBinding(Key.F11, ModifierKeys.None));
      ICommandBarMenu commandBarMenu11 = commandBarMenu3.Items.AddMenu("SplitView", StringTable.MenuBarViewMenuSplitMenu);
      commandBarMenu11.Items.AddCheckBox("View_SplitView_SplitHorizontally", StringTable.MenuBarViewMenuSplitHorizontally);
      commandBarMenu11.Items.AddCheckBox("View_SplitView_SplitVertically", StringTable.MenuBarViewMenuSplitVertically);
      commandBarMenu11.Items.AddSeparator();
      commandBarMenu11.Items.AddButton("View_SplitView_DesignOnTop", StringTable.MenuBarViewMenuSplitDesignOnTop);
      commandBarMenu3.Items.AddSeparator();
      commandBarMenu3.Items.AddButton("View_ZoomIn", StringTable.MenuBarViewMenuZoomIn, MenuBar.CreateKeyBinding(Key.OemPlus, ModifierKeys.Control));
      commandBarMenu3.Items.AddButton("View_ZoomOut", StringTable.MenuBarViewMenuZoomOut, MenuBar.CreateKeyBinding(Key.OemMinus, ModifierKeys.Control));
      commandBarMenu3.Items.AddButton("View_FitToAll", StringTable.MenuBarViewMenuFitToAll, MenuBar.CreateKeyBinding(Key.D0, ModifierKeys.Control));
      commandBarMenu3.Items.AddButton("View_FitToSelection", StringTable.MenuBarViewMenuFitToSelection, MenuBar.CreateKeyBinding(Key.D9, ModifierKeys.Control));
      commandBarMenu3.Items.AddButton("View_ActualSize", StringTable.MenuBarViewMenuActualSize, MenuBar.CreateKeyBinding(Key.D1, ModifierKeys.Control));
      commandBarMenu3.Items.AddSeparator();
      commandBarMenu3.Items.AddButton("View_3D_ToggleLights", StringTable.MenuBarViewMenuToggleLights);
      commandBarMenu3.Items.AddSeparator();
      commandBarMenu3.Items.AddCheckBox("View_ToggleAdorners", StringTable.MenuBarViewMenuShowAdorners, MenuBar.CreateKeyBinding(Key.F9, ModifierKeys.None));
      commandBarMenu3.Items.AddCheckBox("View_ShowSelectionPreview", StringTable.MenuBarViewMenuShowSelectionPreview);
      commandBarMenu3.Items.AddCheckBox("View_ShowActiveContainer", StringTable.MenuBarViewMenuShowActiveContainer);
      commandBarMenu3.Items.AddCheckBox("View_ShowAlignmentAdorners", StringTable.MenuBarViewMenuShowAlignmentAdorners);
      commandBarMenu3.Items.AddCheckBox("View_ShowBoundaries", StringTable.MenuBarViewMenuShowBoundaries, MenuBar.CreateKeyBinding(Key.H, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu3.Items.AddCheckBox("View_ShowAnnotations", StringTable.MenuBarViewMenuShowAnnotations);
      commandBarMenu3.Items.AddCheckBox("View_ApplyProjections", StringTable.MenuBarViewMenuShowObjectsWithProjections);
      commandBarMenu4.Items.AddButton("Edit_EditText", StringTable.MenuBarFormatMenuEditText, MenuBar.CreateKeyBinding(Key.F2, ModifierKeys.None));
      commandBarMenu4.Items.AddButton("Edit_EditControl", StringTable.MenuBarFormatMenuEditControl, MenuBar.CreateKeyBinding(Key.E, ModifierKeys.Control));
      commandBarMenu4.Items.AddSeparator();
      ICommandBarMenu commandBarMenu12 = commandBarMenu4.Items.AddMenu("Order", StringTable.MenuBarArrangeMenuOrder);
      commandBarMenu12.Items.AddButton("Edit_Order_BringToFront", StringTable.MenuBarArrangeMenuOrderBringToFront, MenuBar.CreateKeyBinding(Key.Oem6, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu12.Items.AddButton("Edit_Order_BringForward", StringTable.MenuBarArrangeMenuOrderBringForward, MenuBar.CreateKeyBinding(Key.Oem6, ModifierKeys.Control));
      commandBarMenu12.Items.AddButton("Edit_Order_SendToBack", StringTable.MenuBarArrangeMenuOrderSendToBack, MenuBar.CreateKeyBinding(Key.Oem4, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu12.Items.AddButton("Edit_Order_SendBackward", StringTable.MenuBarArrangeMenuOrderSendBackward, MenuBar.CreateKeyBinding(Key.Oem4, ModifierKeys.Control));
      ICommandBarMenu commandBarMenu13 = commandBarMenu4.Items.AddMenu("Align", StringTable.MenuBarArrangeMenuAlign);
      commandBarMenu13.Items.AddButton("Edit_Align_AlignLeft", StringTable.MenuBarArrangeMenuAlignLeft);
      commandBarMenu13.Items.AddButton("Edit_Align_AlignCenter", StringTable.MenuBarArrangeMenuAlignCenterHorizontally);
      commandBarMenu13.Items.AddButton("Edit_Align_AlignRight", StringTable.MenuBarArrangeMenuAlignRight);
      commandBarMenu13.Items.AddSeparator();
      commandBarMenu13.Items.AddButton("Edit_Align_AlignTop", StringTable.MenuBarArrangeMenuAlignTop);
      commandBarMenu13.Items.AddButton("Edit_Align_AlignMiddle", StringTable.MenuBarArrangeMenuAlignMiddleVertically);
      commandBarMenu13.Items.AddButton("Edit_Align_AlignBottom", StringTable.MenuBarArrangeMenuAlignBottom);
      ICommandBarMenu commandBarMenu14 = commandBarMenu4.Items.AddMenu("AutoSize", StringTable.MenuBarArrangeMenuAutoSize);
      commandBarMenu14.Items.AddCheckBox("Edit_AutoSize_Horizontal", StringTable.MenuBarArrangeMenuAutoSizeWidth, MenuBar.CreateKeyBinding(Key.D5, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu14.Items.AddCheckBox("Edit_AutoSize_Vertical", StringTable.MenuBarArrangeMenuAutoSizeHeight, MenuBar.CreateKeyBinding(Key.D6, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu14.Items.AddCheckBox("Edit_AutoSize_Both", StringTable.MenuBarArrangeMenuAutoSizeBoth);
      commandBarMenu14.Items.AddButton("Edit_AutoSize_Fill", StringTable.MenuBarArrangeMenuAutoSizeFill);
      ICommandBarMenu commandBarMenu15 = commandBarMenu4.Items.AddMenu("Size", StringTable.MenuBarArrangeMenuSize);
      commandBarMenu15.Items.AddButton("Edit_Size_MakeSameWidth", StringTable.MenuBarArrangeMenuMakeSameWidth, MenuBar.CreateKeyBinding(Key.D1, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu15.Items.AddButton("Edit_Size_MakeSameHeight", StringTable.MenuBarArrangeMenuMakeSameHeight, MenuBar.CreateKeyBinding(Key.D2, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu15.Items.AddButton("Edit_Size_MakeSameSize", StringTable.MenuBarArrangeMenuMakeSameSize, MenuBar.CreateKeyBinding(Key.D9, ModifierKeys.Control | ModifierKeys.Shift));
      ICommandBarMenu commandBarMenu16 = commandBarMenu4.Items.AddMenu("Flip", StringTable.MenuBarArrangeMenuFlip);
      commandBarMenu16.Items.AddButton("Edit_Flip_Horizontal", StringTable.MenuBarArrangeMenuFlipHorizontal, MenuBar.CreateKeyBinding(Key.D3, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu16.Items.AddButton("Edit_Flip_Vertical", StringTable.MenuBarArrangeMenuFlipVertical, MenuBar.CreateKeyBinding(Key.D4, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu4.Items.AddSeparator();
      commandBarMenu4.Items.AddButton("Edit_Group", StringTable.MenuBarArrangeMenuGroup, MenuBar.CreateKeyBinding(Key.G, ModifierKeys.Control));
      commandBarMenu4.Items.AddDynamicMenu("Edit_GroupInto", StringTable.MenuBarArrangeMenuGroupInto, MenuBar.CreateKeyBinding(Key.G, ModifierKeys.Control));
      commandBarMenu4.Items.AddButton("Edit_Ungroup", StringTable.MenuBarArrangeMenuUngroup, MenuBar.CreateKeyBinding(Key.G, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu4.Items.AddCheckBox("Edit_LockInsertionPoint", StringTable.MenuBarLockInsertionPoint, MenuBar.CreateKeyBinding(Key.D, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu4.Items.AddSeparator();
      ICommandBarMenu commandBarMenu17 = commandBarMenu4.Items.AddMenu("Combine", StringTable.MenuBarToolsMenuCombine);
      commandBarMenu17.Items.AddButton("Tools_Combine_Unite", StringTable.MenuBarToolsMenuUnite);
      commandBarMenu17.Items.AddButton("Tools_Combine_Divide", StringTable.MenuBarToolsMenuDivide);
      commandBarMenu17.Items.AddButton("Tools_Combine_Intersect", StringTable.MenuBarToolsMenuIntersect);
      commandBarMenu17.Items.AddButton("Tools_Combine_Subtract", StringTable.MenuBarToolsMenuSubtract);
      commandBarMenu17.Items.AddButton("Tools_Combine_ExcludeOverlap", StringTable.MenuBarToolsMenuExcludeOverlap);
      ICommandBarMenu commandBarMenu18 = commandBarMenu4.Items.AddMenu("Path", StringTable.MenuBarObjectMenuPath);
      commandBarMenu18.Items.AddButton("Edit_ConvertToPath", StringTable.MenuBarObjectMenuConvertToPath);
      commandBarMenu18.Items.AddButton("Edit_ConvertToMotionPath", StringTable.MenuBarObjectMenuConvertToMotionPath);
      commandBarMenu18.Items.AddButton("Edit_MakeLayoutPath", StringTable.MenuBarObjectMenuMakeLayoutPath);
      commandBarMenu18.Items.AddButton("Edit_MakeClippingPath", StringTable.MenuBarObjectMenuMakeClippingPath, MenuBar.CreateKeyBinding(Key.D7, ModifierKeys.Control));
      commandBarMenu18.Items.AddButton("Edit_RemoveClippingPath", StringTable.MenuBarObjectMenuRemoveClippingPath, MenuBar.CreateKeyBinding(Key.D7, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu18.Items.AddButton("Edit_MakeCompoundPath", StringTable.MenuBarObjectMenuMakeCompoundPath, MenuBar.CreateKeyBinding(Key.D8, ModifierKeys.Control));
      commandBarMenu18.Items.AddButton("Edit_ReleaseCompoundPath", StringTable.MenuBarObjectMenuReleaseCompoundPath, MenuBar.CreateKeyBinding(Key.D8, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu4.Items.AddSeparator();
      ICommandBarMenu commandBarMenu19 = commandBarMenu4.Items.AddMenu("Style", StringTable.MenuBarFormatMenuEditStyleMenuName, true);
      commandBarMenu19.Items.AddButton("Edit_Style_EditExisting", StringTable.MenuBarFormatMenuEditExistingStyle);
      commandBarMenu19.Items.AddButton("Edit_Style_EditCopy", StringTable.MenuBarFormatMenuEditCopyStyle);
      commandBarMenu19.Items.AddButton("Edit_Style_EditNew", StringTable.MenuBarFormatMenuEditNewStyle);
      commandBarMenu19.Items.AddSeparator();
      commandBarMenu19.Items.AddDynamicMenu("Edit_Style_LocalResource", StringTable.MenuBarFormatMenuEditLocalResource);
      commandBarMenu4.Items.AddDynamicMenu("Edit_EditStyles", StringTable.MenuBarFormatMenuEditStyles);
      commandBarMenu4.Items.AddSeparator();
      ICommandBarMenu commandBarMenu20 = commandBarMenu4.Items.AddMenu("Template", StringTable.MenuBarFormatMenuEditTemplateMenuName, true);
      commandBarMenu20.Items.AddButton("Edit_Template_EditExisting", StringTable.MenuBarFormatMenuEditExistingTemplate);
      commandBarMenu20.Items.AddButton("Edit_Template_EditCopy", StringTable.MenuBarFormatMenuEditCopyStyle);
      commandBarMenu20.Items.AddButton("Edit_Template_EditNew", StringTable.MenuBarFormatMenuEditNewStyle);
      commandBarMenu20.Items.AddSeparator();
      commandBarMenu20.Items.AddDynamicMenu("Edit_Template_LocalResource", StringTable.MenuBarFormatMenuEditLocalResource);
      commandBarMenu4.Items.AddDynamicMenu("Edit_EditTemplates", StringTable.MenuBarFormatMenuEditTemplates);
      commandBarMenu4.Items.AddSeparator();
      ICommandBarMenu commandBarMenu21 = commandBarMenu4.Items.AddMenu("Lock", StringTable.MenuBarFormatMenuLockMenuName);
      commandBarMenu21.Items.AddButton("Edit_Format_LockObjects", StringTable.MenuBarFormatMenuLockObjects, MenuBar.CreateKeyBinding(Key.L, ModifierKeys.Control));
      commandBarMenu21.Items.AddButton("Edit_Format_LockAllObjects", StringTable.MenuBarFormatMenuLockAllObjects);
      commandBarMenu21.Items.AddButton("Edit_Format_UnlockAllObjects", StringTable.MenuBarFormatMenuUnlockAllObjects, MenuBar.CreateKeyBinding(Key.L, ModifierKeys.Control | ModifierKeys.Shift));
      ICommandBarMenu commandBarMenu22 = commandBarMenu4.Items.AddMenu("Visibility", StringTable.MenuBarFormatMenuVisibilityMenuName);
      commandBarMenu22.Items.AddButton("Edit_Format_ShowObjects", StringTable.MenuBarFormatMenuShowObjects, MenuBar.CreateKeyBinding(Key.T, ModifierKeys.Control));
      commandBarMenu22.Items.AddButton("Edit_Format_ShowAllObjects", StringTable.MenuBarFormatMenuShowAllObjects);
      commandBarMenu22.Items.AddButton("Edit_Format_HideObjects", StringTable.MenuBarFormatMenuHideObjects, MenuBar.CreateKeyBinding(Key.D3, ModifierKeys.Control));
      commandBarMenu22.Items.AddButton("Edit_Format_HideAllObjects", StringTable.MenuBarFormatMenuHideAllObjects);
      commandBarMenu5.Items.AddButton("Project_NewFolder", StringTable.MenuBarProjectMenuNewFolder);
      commandBarMenu5.Items.AddSeparator();
      commandBarMenu5.Items.AddButton("Project_AddNewProject", StringTable.MenuBarProjectAddNewProject);
      commandBarMenu5.Items.AddButton("Project_AddExistingProject", StringTable.MenuBarProjectAddExistingProject);
      commandBarMenu5.Items.AddButton("Project_AddExistingWebsite", StringTable.MenuBarProjectAddExistingWebsite);
      commandBarMenu5.Items.AddSeparator();
      commandBarMenu5.Items.AddButton("Application_AddNewItem", StringTable.MenuBarProjectMenuAddNewItem);
      commandBarMenu5.Items.AddButton("Project_AddExistingItem", StringTable.MenuBarProjectMenuAddExistingItem, MenuBar.CreateKeyBinding(Key.I, ModifierKeys.Control));
      commandBarMenu5.Items.AddButton("Project_LinkToExistingItem", StringTable.MenuBarProjectMenuLinkToExistingItem);
      commandBarMenu5.Items.AddSeparator();
      commandBarMenu5.Items.AddButton("Project_AddReference", StringTable.MenuBarProjectMenuAddReference, MenuBar.CreateKeyBinding(Key.R, ModifierKeys.Alt | ModifierKeys.Shift));
      commandBarMenu5.Items.AddDynamicMenu("Project_AddProjectReference", StringTable.MenuBarProjectMenuAddProjectReference);
      commandBarMenu5.Items.AddSeparator();
      commandBarMenu5.Items.AddDynamicMenu("Project_SetStartupSceneMenu", StringTable.MenuBarEditMenuSetSetStartupScene);
      commandBarMenu5.Items.AddSeparator();
      ICommandBarMenu commandBarMenu23 = commandBarMenu5.Items.AddMenu("Project_SilverlightMenu", StringTable.MenuBarProjectMenuSilverlightFlyoutMenu, true);
      commandBarMenu23.Items.AddCheckBox("Project_EnablePlatformExtensions", StringTable.MenuBarProjectMenuUsePlatformExtensions);
      commandBarMenu23.Items.AddCheckBox("Project_EnableOutOfBrowser", StringTable.MenuBarProjectMenuEnableOutOfBrowser);
      commandBarMenu23.Items.AddCheckBox("Project_EnableElevatedOutOfBrowser", StringTable.MenuBarProjectMenuRequireElevatedPermissions);
      commandBarMenu23.Items.AddSeparator();
      commandBarMenu23.Items.AddCheckBox("Project_EnablePreviewOutOfBrowser", StringTable.MenuBarProjectMenuEnablePreviewOutOfBrowser);
      commandBarMenu5.Items.AddSeparator();
      commandBarMenu5.Items.AddButton("Project_Build", "", MenuBar.CreateKeyBinding(Key.B, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu5.Items.AddButton("Project_Rebuild");
      commandBarMenu5.Items.AddButton("Project_Clean");
      commandBarMenu5.Items.AddButton("Project_TestProject", "", MenuBar.CreateKeyBinding(Key.F5, ModifierKeys.None), MenuBar.CreateKeyBinding(Key.F5, ModifierKeys.Control));
      commandBarMenu6.Items.AddButton("Edit_MakeButton", StringTable.MenuBarToolsMenuMakeControl);
      commandBarMenu6.Items.AddButton("Edit_MakeUserControl", StringTable.MenuBarToolsMenuMakeUserControl, MenuBar.CreateKeyBinding(Key.F8, ModifierKeys.None));
      commandBarMenu6.Items.AddButton("Edit_MakeCompositionScreen", StringTable.MenuBarToolsMenuMakeCompositionScreen, MenuBar.CreateKeyBinding(Key.F8, ModifierKeys.Shift));
      commandBarMenu6.Items.AddSeparator();
      commandBarMenu6.Items.AddDynamicMenu("Edit_CopyAllStateProperties", StringTable.MenuBarToolsMenuCopyAllStateProperties);
      commandBarMenu6.Items.AddDynamicMenu("Edit_CopySelectedStateProperties", StringTable.MenuBarToolsMenuCopySelectedStateProperties);
      commandBarMenu6.Items.AddDynamicMenu("Edit_MakePart", StringTable.MenuBarToolsMenuMakePart);
      commandBarMenu6.Items.AddButton("Edit_ClearPart", StringTable.MenuBarToolsMenuClearPartAssignment);
      commandBarMenu6.Items.AddSeparator();
      ICommandBarMenu commandBarMenu24 = commandBarMenu6.Items.AddMenu("MakeTileBrush", StringTable.MenuBarToolsMenuMakeTileBrush, true);
      commandBarMenu24.Items.AddButton("Edit_MakeTileBrush_MakeImageBrush", StringTable.MenuBarToolsMenuMakeImageBrush);
      commandBarMenu24.Items.AddButton("Edit_MakeTileBrush_MakeVisualBrush", StringTable.MenuBarToolsMenuMakeVisualBrush);
      commandBarMenu24.Items.AddButton("Edit_MakeTileBrush_MakeDrawingBrush", StringTable.MenuBarToolsMenuMakeDrawingBrush);
      commandBarMenu24.Items.AddButton("Edit_MakeTileBrush_MakeVideoBrush", StringTable.MenuBarToolsMenuMakeVideoBrush);
      ICommandBarMenu commandBarMenu25 = commandBarMenu6.Items.AddMenu("EditTileBrush", StringTable.MenuBarToolsMenuEditTileBrush, true);
      commandBarMenu25.Items.AddButton("Edit_MakeTileBrush_CopyToResource", StringTable.CopyToResourceMenu);
      commandBarMenu25.Items.AddButton("Edit_MakeTileBrush_MoveToResource", StringTable.MoveToResourceMenu);
      commandBarMenu6.Items.AddButton("Edit_MakeImage3D", StringTable.MenuBarToolsMenuMakeImage3D);
      commandBarMenu6.Items.AddSeparator();
      commandBarMenu6.Items.AddButton("Annotations_CreateAnnotationCommand", StringTable.MenuBarCreateAnnotation, MenuBar.CreateKeyBinding(Key.T, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu6.Items.AddButton("Annotations_CopyAnnotationsInDocumentAsText", StringTable.MenuBarCopyAnnotationsInDocumentAsText);
      commandBarMenu6.Items.AddButton("Annotations_DeleteAnnotationsInDocument", StringTable.MenuBarDeleteAnnotationsInDocument);
      commandBarMenu6.Items.AddSeparator();
      commandBarMenu6.Items.AddButton("Tools_SetupFontEmbedding", StringTable.MenuBarToolsSetupFontEmbedding);
      commandBarMenu6.Items.AddButton("Tools_NameInteractiveElements", StringTable.MenuBarToolsNameInteractiveElements);
      commandBarMenu6.Items.AddSeparator();
      commandBarMenu6.Items.AddButton("Application_Options", StringTable.MenuBarToolsOptions);
      ICommandBarMenu commandBarMenu26 = commandBarMenu7.Items.AddMenu("Workspace", StringTable.MenuBarWindowsWorkspace);
      commandBarMenu26.Items.AddCheckBox("Windows_Workspace_0");
      commandBarMenu26.Items.AddCheckBox("Windows_Workspace_1");
      commandBarMenu26.Items.AddSeparator();
      commandBarMenu26.Items.AddCheckBox("Windows_CustomWorkspace_0");
      commandBarMenu26.Items.AddCheckBox("Windows_CustomWorkspace_1");
      commandBarMenu26.Items.AddCheckBox("Windows_CustomWorkspace_2");
      commandBarMenu26.Items.AddCheckBox("Windows_CustomWorkspace_3");
      commandBarMenu26.Items.AddCheckBox("Windows_CustomWorkspace_4");
      commandBarMenu26.Items.AddCheckBox("Windows_CustomWorkspace_5");
      commandBarMenu26.Items.AddCheckBox("Windows_CustomWorkspace_6");
      commandBarMenu26.Items.AddCheckBox("Windows_CustomWorkspace_7");
      commandBarMenu26.Items.AddCheckBox("Windows_CustomWorkspace_8");
      commandBarMenu26.Items.AddCheckBox("Windows_CustomWorkspace_9");
      commandBarMenu26.Items.AddButton("Windows_AllWorkspaces", StringTable.MenuBarWindowMenuAllWorkspaces);
      commandBarMenu26.Items.AddSeparator();
      commandBarMenu26.Items.AddButton("Windows_CycleWorkspace", StringTable.MenuBarWindowCycleWorkspaces, MenuBar.CreateKeyBinding(Key.F6, ModifierKeys.None));
      commandBarMenu7.Items.AddButton("Windows_ResetWorkspace", StringTable.MenuBarWindowMenuResetWorkspace, MenuBar.CreateKeyBinding(Key.R, ModifierKeys.Control | ModifierKeys.Shift));
      commandBarMenu7.Items.AddButton("Windows_SaveAsNewWorkspace", StringTable.MenuBarWindowMenuSaveAsNewWorkspace);
      commandBarMenu7.Items.AddButton("Windows_ManageWorkspaces", StringTable.MenuBarWindowMenuManageWorkspaces);
      commandBarMenu7.Items.AddSeparator();
      foreach (PaletteRegistryEntry paletteRegistryEntry in (IEnumerable<PaletteRegistryEntry>) services.GetService<IWindowService>().PaletteRegistry.PaletteRegistryEntries)
      {
        if (paletteRegistryEntry.KeyBinding == null)
          commandBarMenu7.Items.AddCheckBox(paletteRegistryEntry.CommandName);
        else
          commandBarMenu7.Items.AddCheckBox(paletteRegistryEntry.CommandName, (string) null, paletteRegistryEntry.KeyBinding);
        service.SetCommandPropertyDefault(paletteRegistryEntry.CommandName, "IsVisible", (object) false);
      }
      commandBarMenu7.Items.AddSeparator();
      commandBarMenu7.Items.AddCheckBox("Windows_ToggleWorkspacePalettes", StringTable.MenuBarWindowMenuHideWorkspacePalettes, MenuBar.CreateKeyBinding(Key.Tab, ModifierKeys.None), MenuBar.CreateKeyBinding(Key.F4, ModifierKeys.None));
      commandBarMenu7.Items.AddSeparator();
      commandBarMenu7.Items.AddButton("Windows_Empty", StringTable.MenuBarWindowsEmpty);
      commandBarMenu7.Items.AddCheckBox("Windows_View_0");
      commandBarMenu7.Items.AddCheckBox("Windows_View_1");
      commandBarMenu7.Items.AddCheckBox("Windows_View_2");
      commandBarMenu7.Items.AddCheckBox("Windows_View_3");
      commandBarMenu7.Items.AddCheckBox("Windows_View_4");
      commandBarMenu7.Items.AddCheckBox("Windows_View_5");
      commandBarMenu7.Items.AddCheckBox("Windows_View_6");
      commandBarMenu7.Items.AddCheckBox("Windows_View_7");
      commandBarMenu7.Items.AddCheckBox("Windows_View_8");
      commandBarMenu7.Items.AddCheckBox("Windows_View_9");
      commandBarMenu7.Items.AddButton("Windows_More", StringTable.MenuBarWindowsMore);
      commandBarMenu8.Items.AddButton("Application_HelpTopics", StringTable.MenuBarHelpMenuHelpTopics, MenuBar.CreateKeyBinding(Key.F1, ModifierKeys.None));
      commandBarMenu8.Items.AddButton("Application_SdkHelpTopics", StringTable.MenuBarHelpMenuSdkHelpTopics);
      commandBarMenu8.Items.AddSeparator();
      ICommandBarMenu commandBarMenu27 = commandBarMenu8.Items.AddMenu("Help_Community", StringTable.MenuBarHelpMenuWebsite);
      commandBarMenu27.Items.AddButton("Application_Website", StringTable.MenuBarHelpMenuWebsiteCommunityHome);
      commandBarMenu27.Items.AddButton("Application_OnlineForums", StringTable.MenuBarHelpMenuWebsiteOnlineForums);
      commandBarMenu27.Items.AddSeparator();
      commandBarMenu27.Items.AddButton("Application_Learn", StringTable.MenuBarHelpMenuWebsiteLearn);
      commandBarMenu27.Items.AddButton("Application_Downloads", StringTable.MenuBarHelpMenuWebsiteDownloads);
      commandBarMenu27.Items.AddButton("Application_CommunityNews", StringTable.MenuBarHelpMenuWebsiteCommunityNews);
      commandBarMenu27.Items.AddButton("Application_Gallery", StringTable.MenuBarHelpMenuWebsiteGallery);
      commandBarMenu27.Items.AddSeparator();
      commandBarMenu27.Items.AddButton("Application_InsideExpression", StringTable.MenuBarHelpMenuWebsiteInsideExpression);
      commandBarMenu27.Items.AddButton("Application_TeamBlog", StringTable.MenuBarHelpMenuWebsiteTeamBlog);
      commandBarMenu8.Items.AddButton("Application_WelcomeScreen", StringTable.MenuBarHelpMenuWelcomeScreen);
      commandBarMenu8.Items.AddButton("Application_KeyboardShortcuts", StringTable.MenuBarHelpMenuKeyboardShortcuts);
      commandBarMenu8.Items.AddButton("Application_PrivacyStatement", StringTable.MenuBarHelpMenuPrivacyStatement);
      commandBarMenu8.Items.AddSeparator();
      commandBarMenu8.Items.AddButton("Application_FeedbackOptions", StringTable.MenuBarHelpMenuFeedbackOptions);
      commandBarMenu8.Items.AddButton("Application_RegisterProduct", StringTable.MenuBarHelpMenuRegisterProduct);
      commandBarMenu8.Items.AddButton("Application_EnterProductKey", StringTable.MenuBarHelpMenuEnterProductKey);
      commandBarMenu8.Items.AddButton("Application_Activate", StringTable.MenuBarHelpMenuActivateProduct);
      commandBarMenu8.Items.AddSeparator();
      string text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.MenuBarHelpMenuAbout, new object[1]
      {
        (object) services.GetService<IExpressionInformationService>().LongApplicationName
      });
      commandBarMenu8.Items.AddButton("Application_About", text);
    }
  }
}
