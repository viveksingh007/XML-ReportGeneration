using ReportGeneration.Logger;
using ReportGeneration.Logger.Interface;
using ReportGeneration.Process;
using ReportGeneration.Process.Interface;
using StructureMap;

namespace ReportGeneration.DependencyResolution
{
    /// <summary>
    /// DefaultRegistry
    /// </summary>
    /// <seealso cref="StructureMap.Registry" />
    public class DefaultRegistry : Registry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRegistry"/> class.
        /// </summary>
        public DefaultRegistry()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.WithDefaultConventions();
            });
            // requires explicit registration
            For<IFileProcessor>().Use<FileProcessor>();
            For<IFileScheduler>().Use<FileScheduler>();
            For<ILog>().Use<ConsoleLogger>();
        }
    }
}
