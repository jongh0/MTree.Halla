using MTree.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace MTree.Utility
{
    public enum EventID
    {
        Default = 10000,


        Exception = 99999
    }

    class Logger
    {
        private static readonly string LogName = "MTree";
        private static readonly string SourcName = "MTree";

        static Logger()
        {
            try
            {
                if (EventLog.SourceExists(SourcName) == false)
                    EventLog.CreateEventSource(SourcName, LogName);

                using (EventLog eventLog = new EventLog())
                {
                    eventLog.Source = SourcName;
                    eventLog.MaximumKilobytes = 4194240;
                    eventLog.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, 10);
                    eventLog.WriteEntry("Logger started", EventLogEntryType.Information, (int)EventID.Default);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        private static void WriteWindowsEventLog(string message, int eventID, EventLogEntryType logType)
        {
            try
            {
                using (EventLog eventLog = new EventLog())
                {
                    eventLog.Source = SourcName;
                    eventLog.WriteEntry(message, logType, eventID);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        public static void WriteEventLog(string message, 
            EventID eventID = EventID.Default, 
            EventLogEntryType logType = EventLogEntryType.Information,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                string now = DateTime.Now.ToString(DefaultConfiguration.Instance.DateTimeFormat);
                string traceLogMessage = $"[{now}/{Path.GetFileName(sourceFilePath)}/{memberName}/{sourceLineNumber}] {message}";
                string eventLogMessage = $"[{now}/{Path.GetFileName(sourceFilePath)}/{memberName}/{sourceLineNumber}]\n{message}";

                Trace.WriteLine(traceLogMessage);
                WriteWindowsEventLog(eventLogMessage, (int)eventID, logType);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        public static void WriteEventLog(Exception ex, 
            EventID eventID = EventID.Exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            WriteEventLog(ex.ToString(), eventID, EventLogEntryType.Error, memberName, sourceFilePath, sourceLineNumber);
            //Debugger.Break();
        }

        public static void WriteTraceLog(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            string now = DateTime.Now.ToString(DefaultConfiguration.Instance.DateTimeFormat);
            string traceLogMessage = $"[{now}/{Path.GetFileName(sourceFilePath)}/{memberName}/{sourceLineNumber}] {message}";

            Trace.WriteLine(traceLogMessage);
        }
    }
}
