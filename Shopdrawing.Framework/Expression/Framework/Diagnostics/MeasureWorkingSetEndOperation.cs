// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.MeasureWorkingSetEndOperation
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Diagnostics;
using System.IO;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public class MeasureWorkingSetEndOperation : PerformanceTestOperation
  {
    private string resultDirectory;

    public MeasureWorkingSetEndOperation(string resultDirectory)
    {
      this.resultDirectory = resultDirectory;
    }

    public override void Execute()
    {
      Process.Start("xperfinfo/xperfinfo.exe", "-mf Finish_Run1").WaitForExit();
      string str1 = Environment.GetEnvironmentVariable("memorytoolsdir");
      string str2 = Environment.GetEnvironmentVariable("resultroot");
      if (string.IsNullOrEmpty(str1))
        str1 = "d:\\memorytools";
      if (string.IsNullOrEmpty(str2))
        str2 = "d:\\perfdata\\myroot";
      Process.Start(str1 + "\\StopTrace.bat", str2 + "\\Scenario\\" + this.resultDirectory + " Group").WaitForExit();
      Process.Start(str1 + "\\AnalyzeScenario.bat", this.resultDirectory).WaitForExit();
      File.WriteAllText("workingset2.txt", string.Concat((object) Environment.WorkingSet));
      this.FinishExecution();
    }
  }
}
