using ReportGeneration.Logger.Interface;
using System;

namespace ReportGeneration.Logger
{
    /// <summary>
    /// ConsoleLogger class
    /// </summary>
    /// <seealso cref="ReportGeneration.Logger.Interface.ILog" />
    public class ConsoleLogger : ILog
    {
        /// <summary>
        /// Log the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }
    }
}
