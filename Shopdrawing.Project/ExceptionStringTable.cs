// Decompiled with JetBrains decompiler
// Type: ExceptionStringTable
// Assembly: Microsoft.Expression.Project, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 80357D9B-A7D7-4011-8FBC-3E1052652ADC
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Project.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

[CompilerGenerated]
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
[DebuggerNonUserCode]
internal class ExceptionStringTable
{
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
        get
        {
            if (object.ReferenceEquals((object)ExceptionStringTable.resourceMan, (object)null))
                ExceptionStringTable.resourceMan = new ResourceManager("ExceptionStringTable", typeof(ExceptionStringTable).Assembly);
            return ExceptionStringTable.resourceMan;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
        get
        {
            return ExceptionStringTable.resourceCulture;
        }
        set
        {
            ExceptionStringTable.resourceCulture = value;
        }
    }

    internal static string CouldNotFindFile
    {
        get
        {
            return ExceptionStringTable.ResourceManager.GetString("CouldNotFindFile", ExceptionStringTable.resourceCulture);
        }
    }

    internal static string MethodOrOperationIsNotImplemented
    {
        get
        {
            return ExceptionStringTable.ResourceManager.GetString("MethodOrOperationIsNotImplemented", ExceptionStringTable.resourceCulture);
        }
    }

    internal static string OneChildAllowed
    {
        get
        {
            return ExceptionStringTable.ResourceManager.GetString("OneChildAllowed", ExceptionStringTable.resourceCulture);
        }
    }

    internal static string ProjectCollectionNoProject
    {
        get
        {
            return ExceptionStringTable.ResourceManager.GetString("ProjectCollectionNoProject", ExceptionStringTable.resourceCulture);
        }
    }

    internal static string ProjectNoStartupSceneAllowed
    {
        get
        {
            return ExceptionStringTable.ResourceManager.GetString("ProjectNoStartupSceneAllowed", ExceptionStringTable.resourceCulture);
        }
    }

    internal static string SettingBuildItemNameNoMSBuildItem
    {
        get
        {
            return ExceptionStringTable.ResourceManager.GetString("SettingBuildItemNameNoMSBuildItem", ExceptionStringTable.resourceCulture);
        }
    }

    internal static string SettingBuildItemNameOnWrongProjectItemType
    {
        get
        {
            return ExceptionStringTable.ResourceManager.GetString("SettingBuildItemNameOnWrongProjectItemType", ExceptionStringTable.resourceCulture);
        }
    }

    internal static string TargetFrameworkChanged
    {
        get
        {
            return ExceptionStringTable.ResourceManager.GetString("TargetFrameworkChanged", ExceptionStringTable.resourceCulture);
        }
    }

    internal static string UnknownFrameworkEncountered
    {
        get
        {
            return ExceptionStringTable.ResourceManager.GetString("UnknownFrameworkEncountered", ExceptionStringTable.resourceCulture);
        }
    }

    internal static string UnsupportedProjectStore
    {
        get
        {
            return ExceptionStringTable.ResourceManager.GetString("UnsupportedProjectStore", ExceptionStringTable.resourceCulture);
        }
    }

    internal ExceptionStringTable()
    {
    }
}
