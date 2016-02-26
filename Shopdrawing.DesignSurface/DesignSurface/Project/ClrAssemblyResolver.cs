// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Project.ClrAssemblyResolver
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.Expression.DesignSurface.Project
{
  internal class ClrAssemblyResolver : IAssemblyResolver
  {
    private static string[][] retargetMapping = new string[71][]
    {
      new string[6]
      {
        "System",
        "b77a5c561934e089",
        "1.0.0.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Xml",
        "b77a5c561934e089",
        "1.0.0.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System",
        "1c9e259686f921e0",
        "1.0.5000.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System",
        "5fd57c543a9c0247",
        "1.0.5000.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Drawing",
        "5fd57c543a9c0247",
        "1.0.5000.0",
        null,
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Web.Services",
        "5fd57c543a9c0247",
        "1.0.5000.0",
        null,
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Windows.Forms",
        "5fd57c543a9c0247",
        "1.0.5000.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Xml",
        "5fd57c543a9c0247",
        "1.0.5000.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System",
        "969db8053d3322ac",
        "1.0.5000.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Data",
        "969db8053d3322ac",
        "1.0.5000.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Drawing",
        "969db8053d3322ac",
        "1.0.5000.0",
        null,
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Web.Services",
        "969db8053d3322ac",
        "1.0.5000.0",
        null,
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Windows.Forms",
        "969db8053d3322ac",
        "1.0.5000.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Xml",
        "969db8053d3322ac",
        "1.0.5000.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "Microsoft.VisualBasic",
        "969db8053d3322ac",
        "7.0.5000.0",
        null,
        "b03f5f7f11d50a3a",
        "10.0.0.0"
      },
      new string[6]
      {
        "System",
        "1c9e259686f921e0",
        "1.0.5500.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System",
        "5fd57c543a9c0247",
        "1.0.5500.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Drawing",
        "5fd57c543a9c0247",
        "1.0.5500.0",
        null,
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Web.Services",
        "5fd57c543a9c0247",
        "1.0.5500.0",
        null,
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Windows.Forms",
        "5fd57c543a9c0247",
        "1.0.5500.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Xml",
        "5fd57c543a9c0247",
        "1.0.5500.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System",
        "969db8053d3322ac",
        "1.0.5500.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Data",
        "969db8053d3322ac",
        "1.0.5500.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Drawing",
        "969db8053d3322ac",
        "1.0.5500.0",
        null,
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Web.Services",
        "969db8053d3322ac",
        "1.0.5500.0",
        null,
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Windows.Forms",
        "969db8053d3322ac",
        "1.0.5500.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Xml",
        "969db8053d3322ac",
        "1.0.5500.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "Microsoft.VisualBasic",
        "969db8053d3322ac",
        "7.0.5500.0",
        null,
        "b03f5f7f11d50a3a",
        "10.0.0.0"
      },
      new string[6]
      {
        "Microsoft.WindowsCE.Forms",
        "1c9e259686f921e0",
        "1.0.5000.0",
        null,
        "969db8053d3322ac",
        "4.0.0.0"
      },
      new string[6]
      {
        "Microsoft.WindowsCE.Forms",
        "1c9e259686f921e0",
        "1.0.5500.0",
        null,
        "969db8053d3322ac",
        "4.0.0.0"
      },
      new string[6]
      {
        "Microsoft.WindowsCE.Forms",
        "5fd57c543a9c0247",
        "1.0.5000.0",
        null,
        "969db8053d3322ac",
        "4.0.0.0"
      },
      new string[6]
      {
        "Microsoft.WindowsCE.Forms",
        "5fd57c543a9c0247",
        "1.0.5500.0",
        null,
        "969db8053d3322ac",
        "4.0.0.0"
      },
      new string[6]
      {
        "Microsoft.WindowsCE.Forms",
        "969db8053d3322ac",
        "1.0.5000.0",
        null,
        "969db8053d3322ac",
        "4.0.0.0"
      },
      new string[6]
      {
        "Microsoft.WindowsCE.Forms",
        "969db8053d3322ac",
        "1.0.5500.0",
        null,
        "969db8053d3322ac",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Data.SqlClient",
        "969db8053d3322ac",
        "1.0.5000.0",
        "System.Data",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Data.SqlClient",
        "969db8053d3322ac",
        "1.0.5500.0",
        "System.Data",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Data.Common",
        "969db8053d3322ac",
        "1.0.5000.0",
        "System.Data",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Data.Common",
        "969db8053d3322ac",
        "1.0.5500.0",
        "System.Data",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Windows.Forms.DataGrid",
        "969db8053d3322ac",
        "1.0.5000.0",
        "System.Windows.Forms",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Windows.Forms.DataGrid",
        "969db8053d3322ac",
        "1.0.5500.0",
        "System.Windows.Forms",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System",
        "969db8053d3322ac",
        "2.0.0.0-2.0.10.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Xml",
        "969db8053d3322ac",
        "2.0.0.0-2.0.10.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Drawing",
        "969db8053d3322ac",
        "2.0.0.0-2.0.10.0",
        null,
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Web.Services",
        "969db8053d3322ac",
        "2.0.0.0-2.0.10.0",
        null,
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Windows.Forms",
        "969db8053d3322ac",
        "2.0.0.0-2.0.10.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Data",
        "969db8053d3322ac",
        "2.0.0.0-2.0.10.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Messaging",
        "969db8053d3322ac",
        "2.0.0.0-2.0.10.0",
        null,
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Data.SqlClient",
        "969db8053d3322ac",
        "2.0.0.0-2.0.10.0",
        "System.Data",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Data.Common",
        "969db8053d3322ac",
        "2.0.0.0-2.0.10.0",
        "System.Data",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Windows.Forms.DataGrid",
        "969db8053d3322ac",
        "2.0.0.0-2.0.10.0",
        "System.Windows.Forms",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "Microsoft.VisualBasic",
        "969db8053d3322ac",
        "8.0.0.0-8.0.10.0",
        null,
        "b03f5f7f11d50a3a",
        "10.0.0.0"
      },
      new string[6]
      {
        "System",
        "969db8053d3322ac",
        "3.5.0.0-3.9.0.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Xml",
        "969db8053d3322ac",
        "3.5.0.0-3.9.0.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Drawing",
        "969db8053d3322ac",
        "3.5.0.0-3.9.0.0",
        null,
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Web.Services",
        "969db8053d3322ac",
        "3.5.0.0-3.9.0.0",
        null,
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Windows.Forms",
        "969db8053d3322ac",
        "3.5.0.0-3.9.0.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Data",
        "969db8053d3322ac",
        "3.5.0.0-3.9.0.0",
        null,
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Messaging",
        "969db8053d3322ac",
        "3.5.0.0-3.9.0.0",
        null,
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Data.SqlClient",
        "969db8053d3322ac",
        "3.5.0.0-3.9.0.0",
        "System.Data",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Windows.Forms.DataGrid",
        "969db8053d3322ac",
        "3.5.0.0-3.9.0.0",
        "System.Windows.Forms",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "Microsoft.VisualBasic",
        "969db8053d3322ac",
        "8.1.0.0-8.1.5.0",
        "Microsoft.VisualBasic",
        "b03f5f7f11d50a3a",
        "10.0.0.0"
      },
      new string[6]
      {
        "System.Data.SqlClient",
        "3be235df1c8d2ad3",
        "3.5.0.0",
        "System.Data",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Data.SqlServerCe",
        "3be235df1c8d2ad3",
        "3.5.0.0",
        null,
        "89845dcd8080cc91",
        "3.5.0.0"
      },
      new string[6]
      {
        "System.Data.SqlServerCe",
        "3be235df1c8d2ad3",
        "3.5.1.0-3.5.200.999",
        null,
        "89845dcd8080cc91",
        "3.5.0.0"
      },
      new string[6]
      {
        "System.Data.SqlClient",
        "3be235df1c8d2ad3",
        "3.0.3600.0",
        "System.Data",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[6]
      {
        "System.Data.SqlServerCe",
        "3be235df1c8d2ad3",
        "3.0.3600.0",
        null,
        "89845dcd8080cc91",
        "9.0.242.0"
      },
      new string[6]
      {
        "system.xml.linq",
        "969db8053d3322ac",
        "3.5.0.0-3.9.0.0",
        null,
        "b77a5c561934e089",
        "3.5.0.0"
      },
      new string[6]
      {
        "system.data.DataSetExtensions",
        "969db8053d3322ac",
        "3.5.0.0-3.9.0.0",
        null,
        "b77a5c561934e089",
        "3.5.0.0"
      },
      new string[6]
      {
        "System.Core",
        "969db8053d3322ac",
        "3.5.0.0-3.9.0.0",
        null,
        "b77a5c561934e089",
        "3.5.0.0"
      },
      new string[6]
      {
        "System.ServiceModel",
        "969db8053d3322ac",
        "3.5.0.0-3.9.0.0",
        null,
        "b77a5c561934e089",
        "3.0.0.0"
      },
      new string[6]
      {
        "System.Runtime.Serialization",
        "969db8053d3322ac",
        "3.5.0.0-3.9.0.0",
        null,
        "b77a5c561934e089",
        "3.0.0.0"
      }
    };
    private static string[][] frameworkMapping = new string[98][]
    {
      new string[3]
      {
        "Accessibility",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "cscompmgd",
        "b03f5f7f11d50a3a",
        "8.0.0.0"
      },
      new string[3]
      {
        "CustomMarshalers",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "IEExecRemote",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "IEHost",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "IIEHost",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "ISymWrapper",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "Microsoft.JScript",
        "b03f5f7f11d50a3a",
        "10.0.0.0"
      },
      new string[3]
      {
        "Microsoft.VisualBasic",
        "b03f5f7f11d50a3a",
        "10.0.0.0"
      },
      new string[3]
      {
        "Microsoft.VisualBasic.Compatibility",
        "b03f5f7f11d50a3a",
        "10.0.0.0"
      },
      new string[3]
      {
        "Microsoft.VisualBasic.Compatibility.Data",
        "b03f5f7f11d50a3a",
        "10.0.0.0"
      },
      new string[3]
      {
        "Microsoft.VisualC",
        "b03f5f7f11d50a3a",
        "10.0.0.0"
      },
      new string[3]
      {
        "mscorlib",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Configuration",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Configuration.Install",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Data",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Data.OracleClient",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Data.SqlXml",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Deployment",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Design",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.DirectoryServices",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.DirectoryServices.Protocols",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Drawing",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Drawing.Design",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.EnterpriseServices",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Management",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Messaging",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Runtime.Remoting",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Runtime.Serialization.Formatters.Soap",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Security",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.ServiceProcess",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Transactions",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Web",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Web.Mobile",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Web.RegularExpressions",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Web.Services",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Windows.Forms",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Xml",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "AspNetMMCExt",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "CppCodeProvider",
        "b03f5f7f11d50a3a",
        "10.0.0.0"
      },
      new string[3]
      {
        "sysglobl",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "Microsoft.Build.Engine",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "Microsoft.Build.Framework",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "Microsoft.VisualC.VSCodeParser",
        "b03f5f7f11d50a3a",
        "10.0.0.0"
      },
      new string[3]
      {
        "Microsoft.VisualC.VSCodeProvider",
        "b03f5f7f11d50a3a",
        "10.0.0.0"
      },
      new string[3]
      {
        "PresentationCFFRasterizer",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "PresentationCore",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "PresentationFramework",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "PresentationFramework.Aero",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "PresentationFramework.Classic",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "PresentationFramework.Luna",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "PresentationFramework.Royale",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "PresentationUI",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "ReachFramework",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Printing",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "UIAutomationClient",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "UIAutomationClientsideProviders",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "UIAutomationProvider",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "UIAutomationTypes",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "WindowsBase",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "WindowsFormsIntegration",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "SMDiagnostics",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.IdentityModel",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.IdentityModel.Selectors",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.IO.Log",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Runtime.Serialization",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.ServiceModel",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.ServiceModel.Install",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.ServiceModel.WasHosting",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Workflow.Activities",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Workflow.ComponentModel",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Workflow.Runtime",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "Microsoft.Transactions.Bridge",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "Microsoft.Transactions.Bridge.Dtc",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.AddIn",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Core",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Data.DataSetExtensions",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Data.Linq",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Xml.Linq",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.DirectoryServices.AccountManagement",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Management.Instrumentation",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Net",
        "b03f5f7f11d50a3a",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.ServiceModel.Web",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Web.Extensions",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Web.Extensions.Design",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Windows.Presentation",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.WorkflowServices",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.ComponentModel.DataAnnotations",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Data.Entity",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Data.Entity.Design",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Web.Abstractions",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Web.DynamicData",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Web.DynamicData.Design",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Web.Entity",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Web.Entity.Design",
        "b77a5c561934e089",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Web.Routing",
        "31bf3856ad364e35",
        "4.0.0.0"
      },
      new string[3]
      {
        "System.Xaml",
        "b77a5c561934e089",
        "4.0.0.0"
      }
    };
    private HashSet<string> resolveAttempts = new HashSet<string>();
    private const string NETCF_PUBLIC_KEY_TOKEN_1 = "1c9e259686f921e0";
    private const string NETCF_PUBLIC_KEY_TOKEN_2 = "5fd57c543a9c0247";
    private const string NETCF_PUBLIC_KEY_TOKEN_3 = "969db8053d3322ac";
    private const string SQL_PUBLIC_KEY_TOKEN = "89845dcd8080cc91";
    private const string SQL_MOBILE_PUBLIC_KEY_TOKEN = "3be235df1c8d2ad3";
    private const string ECMA_PUBLICKEY_STR_L = "b77a5c561934e089";
    private const string MICROSOFT_PUBLICKEY_STR_L = "b03f5f7f11d50a3a";
    private const string SHAREDLIB_PUBLICKEY_STR_L = "31bf3856ad364e35";
    private const string VER_VS_COMPATIBILITY_ASSEMBLYVERSION_STR_L = "8.0.0.0";
    private const string VER_VS_ASSEMBLYVERSION_STR_L = "10.0.0.0";
    private const string VER_SQL_ASSEMBLYVERSION_STR_L = "9.0.242.0";
    private const string VER_LINQ_ASSEMBLYVERSION_STR_L = "3.0.0.0";
    private const string VER_LINQ_ASSEMBLYVERSION_STR_2_L = "3.5.0.0";
    private const string VER_SQL_ORCAS_ASSEMBLYVERSION_STR_L = "3.5.0.0";
    private const string VER_ASSEMBLYVERSION_STR_L = "4.0.0.0";

    public Assembly ResolveAssembly(AssemblyName assemblyName)
    {
      try
      {
        AssemblyName assemblyName1 = ClrAssemblyResolver.Redirect(assemblyName);
        if (assemblyName1 != null)
        {
          string assemblyFullName = ProjectAssemblyHelper.UncachedGetAssemblyFullName(assemblyName1);
          foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
          {
            if (assembly.FullName == assemblyFullName && assembly.GlobalAssemblyCache)
              return assembly;
          }
          if (this.resolveAttempts.Add(assemblyFullName))
            return ProjectAssemblyHelper.Load(assemblyName1);
        }
      }
      catch (BadImageFormatException ex)
      {
      }
      catch (IOException ex)
      {
      }
      return (Assembly) null;
    }

    private static AssemblyName Redirect(AssemblyName assemblyName)
    {
      string name = assemblyName.Name;
      byte[] publicKeyToken = assemblyName.GetPublicKeyToken();
      if (publicKeyToken != null)
      {
        string strA = ClrAssemblyResolver.ByteArrayToString(publicKeyToken);
        for (int index = 0; index < ClrAssemblyResolver.frameworkMapping.GetLength(0); ++index)
        {
          string strB1 = ClrAssemblyResolver.frameworkMapping[index][0];
          string strB2 = ClrAssemblyResolver.frameworkMapping[index][1];
          if (string.Compare(name, strB1, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(strA, strB2, StringComparison.OrdinalIgnoreCase) == 0)
            return new AssemblyName(ProjectAssemblyHelper.CachedGetAssemblyFullName(assemblyName))
            {
              Version = new Version(ClrAssemblyResolver.frameworkMapping[index][2])
            };
        }
        for (int index = 0; index < ClrAssemblyResolver.retargetMapping.GetLength(0); ++index)
        {
          string strB1 = ClrAssemblyResolver.retargetMapping[index][0];
          string strB2 = ClrAssemblyResolver.retargetMapping[index][1];
          if (string.Compare(name, strB1, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(strA, strB2, StringComparison.OrdinalIgnoreCase) == 0)
          {
            Version version1 = assemblyName.Version;
            string[] strArray = ClrAssemblyResolver.retargetMapping[index][2].Split('-');
            Version version2 = new Version(strArray[0]);
            Version version3 = new Version(strArray.Length == 1 ? strArray[0] : strArray[1]);
            if (version1 >= version2 && version1 <= version3)
            {
              AssemblyName assemblyName1 = new AssemblyName(ProjectAssemblyHelper.CachedGetAssemblyFullName(assemblyName));
              assemblyName1.Name = ClrAssemblyResolver.retargetMapping[index][3] == null ? assemblyName1.Name : ClrAssemblyResolver.retargetMapping[index][3];
              assemblyName1.SetPublicKeyToken(ClrAssemblyResolver.ParseByteArray(ClrAssemblyResolver.retargetMapping[index][4]));
              assemblyName1.Version = new Version(ClrAssemblyResolver.retargetMapping[index][5]);
              assemblyName1.Flags &= ~AssemblyNameFlags.Retargetable;
              return assemblyName1;
            }
          }
        }
      }
      return (AssemblyName) null;
    }

    private static string ByteArrayToString(byte[] array)
    {
      string str = string.Empty;
      if (array != null)
      {
        for (int index = 0; index < array.Length; ++index)
          str += array[index].ToString("x2", (IFormatProvider) CultureInfo.InvariantCulture);
      }
      return str;
    }

    private static byte[] ParseByteArray(string value)
    {
      if (string.IsNullOrEmpty(value))
        return (byte[]) null;
      byte[] numArray = new byte[value.Length >> 1];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = byte.Parse(value.Substring(index << 1, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
      return numArray;
    }
  }
}
