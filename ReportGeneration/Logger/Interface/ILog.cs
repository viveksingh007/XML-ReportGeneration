namespace ReportGeneration.Logger.Interface
{
    /// <summary>
    /// ILog
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Informations the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(string message);

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Error(string message);
    }
}
