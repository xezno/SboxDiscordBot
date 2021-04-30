using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SboxDiscordBot
{
    public static class Logging
    {
        public delegate void DebugLogHandler(LogEntry logEntry);

        /// <summary>
        ///     The available severity levels for a debug message.
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
        }

        /// <summary>
        ///     Display a message to the console.
        /// </summary>
        /// <param name="str">The message to output.</param>
        /// <param name="severity">The severity of the message, determining its color.</param>
        public static void Log(string str, Severity severity = Severity.Low)
        {
            // Prepare stack trace
            var stackTrace = new StackTrace();

            Console.ForegroundColor = SeverityToConsoleColor(severity);
            var logTextNoSeverity = str;
            WriteLog(stackTrace, logTextNoSeverity, severity);
        }

        /// <summary>
        ///     Maps a particular debug severity to a console color.
        /// </summary>
        /// <param name="severity"></param>
        /// <returns></returns>
        public static ConsoleColor SeverityToConsoleColor(Severity severity)
        {
            return severity switch
            {
                Severity.Fatal => ConsoleColor.DarkRed,
                Severity.High => ConsoleColor.Red,
                Severity.Low => ConsoleColor.DarkGray,
                Severity.Medium => ConsoleColor.DarkYellow,
                _ => ConsoleColor.DarkGray
            };
        }

        public struct LogEntry
        {
            public DateTime timestamp;
            public StackTrace stackTrace;
            public string str;
            public Severity severity;

            public LogEntry(DateTime timestamp, StackTrace stackTrace, string str, Severity severity)
            {
                this.timestamp = timestamp;
                this.stackTrace = stackTrace;
                this.str = str;
                this.severity = severity;
            }

            public override string ToString()
            {
                return
                    $"[{severity} | {timestamp.ToLongTimeString()} | {stackTrace.GetFrame(1)?.GetMethod()?.ReflectedType?.Name}] {str}";
            }
        }
    }
}