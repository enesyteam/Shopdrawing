using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Microsoft.Expression.Project.Templates
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
    public class VSTemplateTemplateData
    {
        private NameDescriptionIcon nameField;

        private NameDescriptionIcon descriptionField;

        private NameDescriptionIcon iconField;

        private string previewImageField;

        private string projectTypeField;

        private string projectSubTypeField;

        private string templateIDField;

        private string templateGroupIDField;

        private string sortOrderField;

        private bool createNewFolderField;

        private bool createNewFolderFieldSpecified;

        private string defaultNameField;

        private bool provideDefaultNameField;

        private bool provideDefaultNameFieldSpecified;

        private bool promptForSaveOnCreationField;

        private bool promptForSaveOnCreationFieldSpecified;

        private bool enableLocationBrowseButtonField;

        private bool enableLocationBrowseButtonFieldSpecified;

        private bool enableEditOfLocationFieldField;

        private bool enableEditOfLocationFieldFieldSpecified;

        private bool hiddenField;

        private bool hiddenFieldSpecified;

        private string locationFieldMRUPrefixField;

        private string numberOfParentCategoriesToRollUpField;

        private object createInPlaceField;

        private object buildOnLoadField;

        private object showByDefaultField;

        private VSTemplateTemplateDataLocationField locationFieldField;

        private bool locationFieldFieldSpecified;

        private bool supportsMasterPageField;

        private bool supportsMasterPageFieldSpecified;

        private bool supportsCodeSeparationField;

        private bool supportsCodeSeparationFieldSpecified;

        private bool supportsLanguageDropDownField;

        private bool supportsLanguageDropDownFieldSpecified;

        private VSTemplateTemplateDataRequiredFrameworkVersion requiredFrameworkVersionField;

        private bool requiredFrameworkVersionFieldSpecified;

        private string frameworkVersionField;

        private VSTemplateTemplateDataMaxFrameworkVersion maxFrameworkVersionField;

        private bool maxFrameworkVersionFieldSpecified;

        private string customDataSignatureField;

        public object BuildOnLoad
        {
            get
            {
                return this.buildOnLoadField;
            }
            set
            {
                this.buildOnLoadField = value;
            }
        }

        public object CreateInPlace
        {
            get
            {
                return this.createInPlaceField;
            }
            set
            {
                this.createInPlaceField = value;
            }
        }

        public bool CreateNewFolder
        {
            get
            {
                return this.createNewFolderField;
            }
            set
            {
                this.createNewFolderField = value;
            }
        }

        [XmlIgnore]
        public bool CreateNewFolderSpecified
        {
            get
            {
                return this.createNewFolderFieldSpecified;
            }
            set
            {
                this.createNewFolderFieldSpecified = value;
            }
        }

        public string CustomDataSignature
        {
            get
            {
                return this.customDataSignatureField;
            }
            set
            {
                this.customDataSignatureField = value;
            }
        }

        public string DefaultName
        {
            get
            {
                return this.defaultNameField;
            }
            set
            {
                this.defaultNameField = value;
            }
        }

        public NameDescriptionIcon Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public bool EnableEditOfLocationField
        {
            get
            {
                return this.enableEditOfLocationFieldField;
            }
            set
            {
                this.enableEditOfLocationFieldField = value;
            }
        }

        [XmlIgnore]
        public bool EnableEditOfLocationFieldSpecified
        {
            get
            {
                return this.enableEditOfLocationFieldFieldSpecified;
            }
            set
            {
                this.enableEditOfLocationFieldFieldSpecified = value;
            }
        }

        public bool EnableLocationBrowseButton
        {
            get
            {
                return this.enableLocationBrowseButtonField;
            }
            set
            {
                this.enableLocationBrowseButtonField = value;
            }
        }

        [XmlIgnore]
        public bool EnableLocationBrowseButtonSpecified
        {
            get
            {
                return this.enableLocationBrowseButtonFieldSpecified;
            }
            set
            {
                this.enableLocationBrowseButtonFieldSpecified = value;
            }
        }

        public bool ExpressionBlendPrototypingEnabled
        {
            get;
            set;
        }

        public string FrameworkVersion
        {
            get
            {
                return this.frameworkVersionField;
            }
            set
            {
                this.frameworkVersionField = value;
            }
        }

        public bool Hidden
        {
            get
            {
                return this.hiddenField;
            }
            set
            {
                this.hiddenField = value;
            }
        }

        [XmlIgnore]
        public bool HiddenSpecified
        {
            get
            {
                return this.hiddenFieldSpecified;
            }
            set
            {
                this.hiddenFieldSpecified = value;
            }
        }

        public NameDescriptionIcon Icon
        {
            get
            {
                return this.iconField;
            }
            set
            {
                this.iconField = value;
            }
        }

        public VSTemplateTemplateDataLocationField LocationField
        {
            get
            {
                return this.locationFieldField;
            }
            set
            {
                this.locationFieldField = value;
            }
        }

        public string LocationFieldMRUPrefix
        {
            get
            {
                return this.locationFieldMRUPrefixField;
            }
            set
            {
                this.locationFieldMRUPrefixField = value;
            }
        }

        [XmlIgnore]
        public bool LocationFieldSpecified
        {
            get
            {
                return this.locationFieldFieldSpecified;
            }
            set
            {
                this.locationFieldFieldSpecified = value;
            }
        }

        public VSTemplateTemplateDataMaxFrameworkVersion MaxFrameworkVersion
        {
            get
            {
                return this.maxFrameworkVersionField;
            }
            set
            {
                this.maxFrameworkVersionField = value;
            }
        }

        [XmlIgnore]
        public bool MaxFrameworkVersionSpecified
        {
            get
            {
                return this.maxFrameworkVersionFieldSpecified;
            }
            set
            {
                this.maxFrameworkVersionFieldSpecified = value;
            }
        }

        public NameDescriptionIcon Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        [XmlElement(DataType = "integer")]
        public string NumberOfParentCategoriesToRollUp
        {
            get
            {
                return this.numberOfParentCategoriesToRollUpField;
            }
            set
            {
                this.numberOfParentCategoriesToRollUpField = value;
            }
        }

        public string PreviewImage
        {
            get
            {
                return this.previewImageField;
            }
            set
            {
                this.previewImageField = value;
            }
        }

        public string ProjectSubType
        {
            get
            {
                return this.projectSubTypeField;
            }
            set
            {
                this.projectSubTypeField = value;
            }
        }

        public string ProjectSubTypes
        {
            get;
            set;
        }

        public string ProjectType
        {
            get
            {
                return this.projectTypeField;
            }
            set
            {
                this.projectTypeField = value;
            }
        }

        public bool PromptForSaveOnCreation
        {
            get
            {
                return this.promptForSaveOnCreationField;
            }
            set
            {
                this.promptForSaveOnCreationField = value;
            }
        }

        [XmlIgnore]
        public bool PromptForSaveOnCreationSpecified
        {
            get
            {
                return this.promptForSaveOnCreationFieldSpecified;
            }
            set
            {
                this.promptForSaveOnCreationFieldSpecified = value;
            }
        }

        public bool ProvideDefaultName
        {
            get
            {
                return this.provideDefaultNameField;
            }
            set
            {
                this.provideDefaultNameField = value;
            }
        }

        [XmlIgnore]
        public bool ProvideDefaultNameSpecified
        {
            get
            {
                return this.provideDefaultNameFieldSpecified;
            }
            set
            {
                this.provideDefaultNameFieldSpecified = value;
            }
        }

        public VSTemplateTemplateDataRequiredFrameworkVersion RequiredFrameworkVersion
        {
            get
            {
                return this.requiredFrameworkVersionField;
            }
            set
            {
                this.requiredFrameworkVersionField = value;
            }
        }

        [XmlIgnore]
        public bool RequiredFrameworkVersionSpecified
        {
            get
            {
                return this.requiredFrameworkVersionFieldSpecified;
            }
            set
            {
                this.requiredFrameworkVersionFieldSpecified = value;
            }
        }

        public object ShowByDefault
        {
            get
            {
                return this.showByDefaultField;
            }
            set
            {
                this.showByDefaultField = value;
            }
        }

        [XmlElement(DataType = "integer")]
        public string SortOrder
        {
            get
            {
                return this.sortOrderField;
            }
            set
            {
                this.sortOrderField = value;
            }
        }

        public bool SupportsCodeSeparation
        {
            get
            {
                return this.supportsCodeSeparationField;
            }
            set
            {
                this.supportsCodeSeparationField = value;
            }
        }

        [XmlIgnore]
        public bool SupportsCodeSeparationSpecified
        {
            get
            {
                return this.supportsCodeSeparationFieldSpecified;
            }
            set
            {
                this.supportsCodeSeparationFieldSpecified = value;
            }
        }

        public bool SupportsLanguageDropDown
        {
            get
            {
                return this.supportsLanguageDropDownField;
            }
            set
            {
                this.supportsLanguageDropDownField = value;
            }
        }

        [XmlIgnore]
        public bool SupportsLanguageDropDownSpecified
        {
            get
            {
                return this.supportsLanguageDropDownFieldSpecified;
            }
            set
            {
                this.supportsLanguageDropDownFieldSpecified = value;
            }
        }

        public bool SupportsMasterPage
        {
            get
            {
                return this.supportsMasterPageField;
            }
            set
            {
                this.supportsMasterPageField = value;
            }
        }

        [XmlIgnore]
        public bool SupportsMasterPageSpecified
        {
            get
            {
                return this.supportsMasterPageFieldSpecified;
            }
            set
            {
                this.supportsMasterPageFieldSpecified = value;
            }
        }

        public string TemplateGroupID
        {
            get
            {
                return this.templateGroupIDField;
            }
            set
            {
                this.templateGroupIDField = value;
            }
        }

        public string TemplateID
        {
            get
            {
                return this.templateIDField;
            }
            set
            {
                this.templateIDField = value;
            }
        }

        public VSTemplateTemplateData()
        {
        }
    }
}