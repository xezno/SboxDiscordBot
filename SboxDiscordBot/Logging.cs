using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SboxDiscordBot
{
    public static class Logging
    {
        public struct LogEntry
        {
            public DateTime timestamp;
            public StackTrace stackTrace;
            public string str;
            public Severity severity;

            public LogEntry(DateTime timestamp, StackTrace stackTrace, string str, Logging.Severity severity)
            {
                this.timestamp = timestamp;
                this.stackTrace = stackTrace;
                this.str = str;
                this.severity = severity;
            }

            public override string ToString() => $"[{severity} | {timestamp.ToLongTimeString()} | {stackTrace.GetFrame(1)?.GetMethod()?.ReflectedType?.Name}] {str}";
        }

        private static object lockObject = new object();

        public static List<LogEntry> LogEntries = new List<LogEntry>();

        public delegate void DebugLogHandler(LogEntry logEntry);
        public static DebugLogHandler onDebugLog;

        /// <summary>
        /// The available severity levels for a debug message.
        /// </summary>
        public enum Severity
        {
            Low,
            Medium,
            High,
            Fatal
        }

        private static void WriteLog(StackTrace stackTrace, string logString = "", Severity severity = Severity.Low)
        {
            var logEntry = new LogEntry(DateTime.Now, stackTrace, logString, severity);

            Console.WriteLine(logEntry.ToString());

            LogEntries.Add(logEntry);
            onDebugLog?.Invoke(logEntry);
        }

        /// <summary>
        /// Display a message to the console.
        /// </summary>
        /// <param name="str">The message to output.</param>
        /// <param name="severity">The severity of the message, determining its color.</param>
        public static void Log(string str, Severity severity = Severity.Low)
        {
            // Prepare stack trace
            var stackTrace = new StackTrace();

            lock (lockObject)
            {
                Console.ForegroundColor = SeverityToConsoleColor(severity);

                var logTextNoSeverity = str;

                WriteLog(stackTrace, logTextNoSeverity, severity);
            }
        }

        /// <summary>
        /// Maps a particular debug severity to a console color.
        /// </summary>
        /// <param name="severity"></param>
        /// <returns></returns>
        public static ConsoleColor SeverityToConsoleColor(Severity severity) =>
            severity switch
            {
                Severity.Fatal => ConsoleColor.DarkRed,
                Severity.High => ConsoleColor.Red,
                Severity.Low => ConsoleColor.DarkGray,
                Severity.Medium => ConsoleColor.DarkYellow,
                _ => ConsoleColor.DarkGray,
            };
    }
}