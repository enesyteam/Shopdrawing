//using System;
//using System.CodeDom.Compiler;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Globalization;
//using System.Resources;
//using System.Runtime.CompilerServices;

//namespace MS.Internal.Properties
//{
//    [CompilerGenerated]
//    [DebuggerNonUserCode]
//    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
//    internal class Resources
//    {
//        private static ResourceManager resourceMan;

//        private static CultureInfo resourceCulture;

//        internal static string AdornerNodeAutomationPeer_HelpText
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("AdornerNodeAutomationPeer_HelpText", Resources.resourceCulture);
//            }
//        }

//        internal static string AdornerNodeAutomationPeer_ItemType
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("AdornerNodeAutomationPeer_ItemType", Resources.resourceCulture);
//            }
//        }

//        internal static string AdornerNodeAutomationPeer_Name
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("AdornerNodeAutomationPeer_Name", Resources.resourceCulture);
//            }
//        }

//        internal static string AdornerPlacement_ToString
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("AdornerPlacement_ToString", Resources.resourceCulture);
//            }
//        }

//        [EditorBrowsable(EditorBrowsableState.Advanced)]
//        internal static CultureInfo Culture
//        {
//            get
//            {
//                return Resources.resourceCulture;
//            }
//            set
//            {
//                Resources.resourceCulture = value;
//            }
//        }

//        internal static string DesignerItemAutomationPeer_LocalizedControlType
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("DesignerItemAutomationPeer_LocalizedControlType", Resources.resourceCulture);
//            }
//        }

//        internal static string DesignerViewAutomationPeer_HelpText
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("DesignerViewAutomationPeer_HelpText", Resources.resourceCulture);
//            }
//        }

//        internal static string DesignerViewAutomationPeer_ItemType
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("DesignerViewAutomationPeer_ItemType", Resources.resourceCulture);
//            }
//        }

//        internal static string DesignerViewAutomationPeer_Name
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("DesignerViewAutomationPeer_Name", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_AdornerHasParent
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_AdornerHasParent", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_AdornerNotParentedToThisAdornerLayer
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_AdornerNotParentedToThisAdornerLayer", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_ArgIncorrectType
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_ArgIncorrectType", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_ArgIncorrectTypeValue
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_ArgIncorrectTypeValue", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_CannotConvertValueToString
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_CannotConvertValueToString", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_CannotUpdateValueFromStringValue
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_CannotUpdateValueFromStringValue", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_CantDeactivateActiveTransactedTask
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_CantDeactivateActiveTransactedTask", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_ContextHasView
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_ContextHasView", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_DerivedContextItem
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_DerivedContextItem", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_DesignerActionItemSharing
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_DesignerActionItemSharing", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_DisposingDuringCall
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_DisposingDuringCall", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_DuplicateItem
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_DuplicateItem", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_DuplicateService
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_DuplicateService", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_EditingScopeReverted
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_EditingScopeReverted", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_EdtingScopeCompleted
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_EdtingScopeCompleted", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_EffectsNotAllowed
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_EffectsNotAllowed", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_EnumerationNotReady
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_EnumerationNotReady", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_IncompatibleGestureData
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_IncompatibleGestureData", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_IncompatiblePositionReference
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_IncompatiblePositionReference", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_IncorrectFocusedTask
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_IncorrectFocusedTask", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_IncorrectServiceType
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_IncorrectServiceType", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_IncorrectTypePassed
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_IncorrectTypePassed", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_InvalidArrayIndex
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_InvalidArrayIndex", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_InvalidFactoryType
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_InvalidFactoryType", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_InvalidRedirectParent
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_InvalidRedirectParent", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_InvalidToolboxExampleFactoryType
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_InvalidToolboxExampleFactoryType", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_LocalAssemblyNameChanged
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_LocalAssemblyNameChanged", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_MissingContext
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_MissingContext", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_NoCreationType
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_NoCreationType", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_NoDesignerView
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_NoDesignerView", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_NoGestureData
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_NoGestureData", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_NoPropertyValue
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_NoPropertyValue", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_NullImplementation
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_NullImplementation", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_NullService
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_NullService", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_ObjectAlreadyActive
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_ObjectAlreadyActive", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_ObjectNotActive
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_ObjectNotActive", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_ParentNotSupported
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_ParentNotSupported", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_PropertyIsReadOnly
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_PropertyIsReadOnly", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_PropertyNotFound
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_PropertyNotFound", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_PropertyValueEditor_InvalidDialogValueEditorCommandInvocation
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_PropertyValueEditor_InvalidDialogValueEditorCommandInvocation", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_PropertyValueEditor_InvalidDialogValueEditorEditorValue
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_PropertyValueEditor_InvalidDialogValueEditorEditorValue", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_RecursionResolvingService
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_RecursionResolvingService", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_RequiredService
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_RequiredService", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_TableValidationFailed
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_TableValidationFailed", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_TaskAlreadyFocused
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_TaskAlreadyFocused", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_ToolAlreadyActive
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_ToolAlreadyActive", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_UnknownMemberDescriptor
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_UnknownMemberDescriptor", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_ValidationAmbiguousMember
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_ValidationAmbiguousMember", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_ValidationNoMatchingMember
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_ValidationNoMatchingMember", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_ValueGetFailed
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_ValueGetFailed", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_ValueSetFailed
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_ValueSetFailed", Resources.resourceCulture);
//            }
//        }

//        internal static string Error_VisualNotInDesigner
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("Error_VisualNotInDesigner", Resources.resourceCulture);
//            }
//        }

//        [EditorBrowsable(EditorBrowsableState.Advanced)]
//        internal static ResourceManager ResourceManager
//        {
//            get
//            {
//                if (object.ReferenceEquals(Resources.resourceMan, null))
//                {
//                    Resources.resourceMan = new ResourceManager("MS.Internal.Properties.Resources", typeof(Resources).Assembly);
//                }
//                return Resources.resourceMan;
//            }
//        }

//        internal static string ToolDescription_CreateInstance
//        {
//            get
//            {
//                return Resources.ResourceManager.GetString("ToolDescription_CreateInstance", Resources.resourceCulture);
//            }
//        }

//        internal Resources()
//        {
//        }
//    }
//}