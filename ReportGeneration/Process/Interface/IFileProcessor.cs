using System.Threading.Tasks;

namespace ReportGeneration.Process.Interface
{
    /// <summary>
    /// IFileProcessor
    /// </summary>
    public interface IFileProcessor
    {
        /// <summary>
        /// Determines whether the specified full path is processed.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        Task<bool> IsProcessed(string fullPath);
    }
}
