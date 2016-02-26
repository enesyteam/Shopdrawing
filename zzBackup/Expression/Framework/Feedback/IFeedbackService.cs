// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Feedback.IFeedbackService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Feedback
{
  public interface IFeedbackService
  {
    string GroupFeedbackRegistryPath { get; set; }

    string CustomerFeedbackRegistryPath { get; set; }

    bool CanSetFeedbackPolicy { get; }

    bool? AggregateFeedbackPolicy { get; }

    bool? CustomerFeedbackPolicy { get; set; }

    bool ShouldPromptAtStartup { get; }

    string LoggingFileName { get; set; }

    bool IsLogging { get; }

    void Start();

    void Stop();

    void AddCommandStringToValueTable(Dictionary<string, int> commandStringToFeedbackValue);

    int GetFeedbackValue(string commandName);

    void SetData(int dataId, int value);

    void AddDataToStream(int dataId, int value);

    void AddDataToStream2(int dataId, int value1, int value2);
  }
}
