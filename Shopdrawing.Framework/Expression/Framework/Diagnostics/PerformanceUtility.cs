using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.Diagnostics
{
    public sealed class PerformanceUtility
    {
        private static StatisticsLogger statisticsLogger;

        private static ProfileLogger profileLogger;

        private static PerformanceLogger[] builtInLoggers;

        private static PerformanceLogger[] emptyLoggers;

        private static PerformanceLogger[] Loggers
        {
            get
            {
                if (PerformanceUtility.LoggingEnabled)
                {
                    return PerformanceUtility.builtInLoggers;
                }
                return PerformanceUtility.emptyLoggers;
            }
        }

        public static bool LoggingEnabled
        {
            get;
            set;
        }

        public static IEnumerable<PerformanceEventStatistics> Statistics
        {
            get
            {
                return PerformanceUtility.statisticsLogger.Statistics;
            }
        }

        static PerformanceUtility()
        {
            PerformanceUtility.statisticsLogger = new StatisticsLogger();
            PerformanceUtility.profileLogger = new ProfileLogger();
            PerformanceLogger[] eventTracingLogger = new PerformanceLogger[] { new EventTracingLogger(), PerformanceUtility.statisticsLogger, PerformanceUtility.profileLogger };
            PerformanceUtility.builtInLoggers = eventTracingLogger;
            PerformanceUtility.emptyLoggers = new PerformanceLogger[0];
        }

        private PerformanceUtility()
        {
        }

        public static void EnableEventProfile(PerformanceEvent targetEvent)
        {
            PerformanceUtility.profileLogger.EnableEvent(targetEvent);
        }

        public static void EndPerformanceSequence(PerformanceEvent performanceEvent)
        {
            PerformanceLogger[] loggers = PerformanceUtility.Loggers;
            for (int i = 0; i < (int)loggers.Length; i++)
            {
                loggers[i].AddEndEvent(performanceEvent);
            }
        }

        public static void EndPerformanceSequence(PerformanceEvent performanceEvent, string additionalInformation)
        {
            PerformanceLogger[] loggers = PerformanceUtility.Loggers;
            for (int i = 0; i < (int)loggers.Length; i++)
            {
                loggers[i].AddEndEvent(performanceEvent, additionalInformation);
            }
        }

        public static void EndPerformanceSequenceAfterRender(PerformanceEvent performanceEvent)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new DispatcherOperationCallback((object arg) =>
                {
                    PerformanceUtility.EndPerformanceSequence(performanceEvent);
                    return null;
                }), null);
            }
        }

        public static void EndPerformanceSequenceAfterRender(PerformanceEvent performanceEvent, string additionalInformation)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new DispatcherOperationCallback((object arg) =>
                {
                    PerformanceUtility.EndPerformanceSequence(performanceEvent, additionalInformation);
                    return null;
                }), null);
            }
        }

        public static void EndPerformanceSequenceOnIdle(PerformanceEvent performanceEvent)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback((object arg) =>
                {
                    PerformanceUtility.EndPerformanceSequence(performanceEvent);
                    return null;
                }), null);
            }
        }

        public static void LogInfoEvent(string info)
        {
            PerformanceLogger[] loggers = PerformanceUtility.Loggers;
            for (int i = 0; i < (int)loggers.Length; i++)
            {
                loggers[i].AddInfoEvent(info);
            }
        }

        public static void LogPerformanceEvent(PerformanceEvent performanceEvent)
        {
            PerformanceLogger[] loggers = PerformanceUtility.Loggers;
            for (int i = 0; i < (int)loggers.Length; i++)
            {
                loggers[i].AddLogEvent(performanceEvent);
            }
        }

        public static void MarkInterimStep(PerformanceEvent performanceEvent, string stepName)
        {
            PerformanceLogger[] loggers = PerformanceUtility.Loggers;
            for (int i = 0; i < (int)loggers.Length; i++)
            {
                loggers[i].AddInterimStep(performanceEvent, stepName);
            }
        }

        public static void MeasurePerformanceUntilIdle(PerformanceEvent performanceEvent)
        {
            PerformanceLogger[] loggers = PerformanceUtility.Loggers;
            for (int i = 0; i < (int)loggers.Length; i++)
            {
                loggers[i].AddStartEvent(performanceEvent);
            }
            PerformanceUtility.EndPerformanceSequenceOnIdle(performanceEvent);
        }

        public static void MeasurePerformanceUntilRender(PerformanceEvent performanceEvent)
        {
            PerformanceLogger[] loggers = PerformanceUtility.Loggers;
            for (int i = 0; i < (int)loggers.Length; i++)
            {
                loggers[i].AddStartEvent(performanceEvent);
            }
            PerformanceUtility.EndPerformanceSequenceAfterRender(performanceEvent);
        }

        public static void MeasurePerformanceUntilRender(PerformanceEvent performanceEvent, string additionalInformation)
        {
            PerformanceLogger[] loggers = PerformanceUtility.Loggers;
            for (int i = 0; i < (int)loggers.Length; i++)
            {
                loggers[i].AddStartEvent(performanceEvent, additionalInformation);
            }
            PerformanceUtility.EndPerformanceSequenceAfterRender(performanceEvent, additionalInformation);
        }

        public static IDisposable PerformanceSequence(PerformanceEvent performanceEvent)
        {
            return new PerformanceUtility.PerformanceSequenceToken(performanceEvent);
        }

        public static void Reset()
        {
            PerformanceLogger[] loggers = PerformanceUtility.Loggers;
            for (int i = 0; i < (int)loggers.Length; i++)
            {
                loggers[i].Reset();
            }
        }

        public static void StartPerformanceSequence(PerformanceEvent performanceEvent)
        {
            PerformanceLogger[] loggers = PerformanceUtility.Loggers;
            for (int i = 0; i < (int)loggers.Length; i++)
            {
                loggers[i].AddStartEvent(performanceEvent);
            }
        }

        public static void StartPerformanceSequence(PerformanceEvent performanceEvent, string additionalInformation)
        {
            PerformanceLogger[] loggers = PerformanceUtility.Loggers;
            for (int i = 0; i < (int)loggers.Length; i++)
            {
                loggers[i].AddStartEvent(performanceEvent, additionalInformation);
            }
        }

        [CompilerGenerated]
        // <>c__DisplayClass2
        private sealed class u003cu003ec__DisplayClass2
        {
            public PerformanceEvent performanceEvent;

            public u003cu003ec__DisplayClass2()
            {
            }

            // <EndPerformanceSequenceOnIdle>b__0
            public object u003cEndPerformanceSequenceOnIdleu003eb__0(object arg)
            {
                PerformanceUtility.EndPerformanceSequence(this.performanceEvent);
                return null;
            }
        }

        [CompilerGenerated]
        // <>c__DisplayClass6
        private sealed class u003cu003ec__DisplayClass6
        {
            public PerformanceEvent performanceEvent;

            public u003cu003ec__DisplayClass6()
            {
            }

            // <EndPerformanceSequenceAfterRender>b__4
            public object u003cEndPerformanceSequenceAfterRenderu003eb__4(object arg)
            {
                PerformanceUtility.EndPerformanceSequence(this.performanceEvent);
                return null;
            }
        }

        [CompilerGenerated]
        // <>c__DisplayClassa
        private sealed class u003cu003ec__DisplayClassa
        {
            public PerformanceEvent performanceEvent;

            public string additionalInformation;

            public u003cu003ec__DisplayClassa()
            {
            }

            // <EndPerformanceSequenceAfterRender>b__8
            public object u003cEndPerformanceSequenceAfterRenderu003eb__8(object arg)
            {
                PerformanceUtility.EndPerformanceSequence(this.performanceEvent, this.additionalInformation);
                return null;
            }
        }

        private class PerformanceSequenceToken : IDisposable
        {
            private PerformanceEvent performanceEvent;

            public PerformanceSequenceToken(PerformanceEvent performanceEvent)
            {
                this.performanceEvent = performanceEvent;
                PerformanceUtility.StartPerformanceSequence(this.performanceEvent);
            }

            public void Dispose()
            {
                PerformanceUtility.EndPerformanceSequence(this.performanceEvent);
            }
        }
    }
}