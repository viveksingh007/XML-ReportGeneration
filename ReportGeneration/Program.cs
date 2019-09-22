using ReportGeneration.DependencyResolution;
using ReportGeneration.Logger;
using ReportGeneration.Logger.Interface;
using ReportGeneration.Process;
using ReportGeneration.Process.Interface;
using StructureMap;
using System;

namespace ReportGeneration
{
    /// <summary>
    /// Program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The file scheduler
        /// </summary>
        private static readonly IFileScheduler _fileProcessor;

        /// <summary>
        /// The i log
        /// </summary>
        private static readonly ILog _iLog;

        /// <summary>
        /// Initializes the <see cref="Program"/> class.
        /// </summary>
        static Program()
        {
            var container = Container.For<DefaultRegistry>();
            _fileProcessor = container.GetInstance<FileScheduler>();
            _iLog = container.GetInstance<ConsoleLogger>();
        }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            _iLog.Info("Application started.");
            _fileProcessor.Run();
            _iLog.Info("Application is in idle state.");
            _iLog.Info("********************************");
            Console.ReadLine();
        }
    }
}
