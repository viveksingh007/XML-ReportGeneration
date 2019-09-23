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
    /// To run the solution, we need to create a few folders in c: drive or you can change it according to your wish.
    /// If you want to change the folder name and location for the incoming and output file,
    /// please update the same into App.config else please created the below the folder structure in c: drive.
    /// IncomingFilePath = C:\Personal\ReportGeneration\Incoming
    /// OutputFilePath = C:\Personal\ReportGeneration\Output\GenerationOutput.xml
    /// ArchiveIncomingFolderPath = C:\Personal\ReportGeneration\Incoming\Archive
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
