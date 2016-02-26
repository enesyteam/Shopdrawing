// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.VisualStudioAutomation.VSAutomation
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Project;
using System;
using System.Reflection;

namespace Microsoft.Expression.VisualStudioAutomation
{
  internal static class VSAutomation
  {
    private static bool solutionModelProviderInitialized;
    private static ISolutionModelProvider solutionModelProvider;

    public static bool UseVisualStudioIfAvailable { get; set; }

    public static ISolutionModelProvider SolutionModelProvider
    {
      get
      {
        if (!VSAutomation.solutionModelProviderInitialized)
        {
          VSAutomation.solutionModelProviderInitialized = true;
          try
          {
            AssemblyName assemblyName1 = AssemblyHelper.GetAssemblyName(Assembly.GetExecutingAssembly());
            AssemblyName assemblyName2 = new AssemblyName();
            assemblyName2.Name = "Microsoft.Expression.VisualStudioAutomation";
            assemblyName2.CultureInfo = assemblyName1.CultureInfo;
            assemblyName2.Version = assemblyName1.Version;
            assemblyName2.SetPublicKeyToken(assemblyName1.GetPublicKeyToken());
            Assembly assembly = ProjectAssemblyHelper.Load(assemblyName2);
            if (assembly != (Assembly) null)
            {
              ISolutionModelProvider solutionModelProvider = (ISolutionModelProvider) Activator.CreateInstance(assembly.GetType("Microsoft.Expression.VisualStudioAutomation.SolutionModelProvider"));
              if (solutionModelProvider.IsAvailable)
                VSAutomation.solutionModelProvider = solutionModelProvider;
            }
          }
          catch (Exception ex)
          {
          }
        }
        return VSAutomation.solutionModelProvider;
      }
    }
  }
}
