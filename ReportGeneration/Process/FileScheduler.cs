using ReportGeneration.Helpers;
using ReportGeneration.Logger.Interface;
using ReportGeneration.Process.Interface;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace ReportGeneration.Process
{
    public class FileScheduler : IFileScheduler
    {
        /// <summary>
        /// The incoming file path
        /// </summary>
        private static readonly string _incomingFilePath = AppConfigHelper.GetSectionsConfig("ReportConfig", "IncomingFilePath", string.Empty);
        /// <summary>
        /// The allowed file type
        /// </summary>
        private static readonly string _allowedFileType = AppConfigHelper.GetSectionsConfig("ReportConfig", "AllowedFileType", "*.xml");

        /// <summary>
        /// The data processor
        /// </summary>
        private readonly IFileProcessor _fileProcessor;

        /// <summary>
        /// The i log
        /// </summary>
        private readonly ILog _iLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileScheduler"/> class.
        /// </summary>
        /// <param name="fileProcessor">The file processor.</param>
        public FileScheduler(IFileProcessor fileProcessor, ILog iLog)
        {
            _fileProcessor = fileProcessor;
            _iLog = iLog;
        }

        /// <summary>
        /// Runs the schedule.
        /// </summary>
        public void Run()
        {
            _iLog.Info("File scheduler initiated.");
            var watcher = new FileSystemWatcher
            {
                Path = _incomingFilePath
            };
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = _allowedFileType;
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Called when [changed event fire].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private async void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                _iLog.Info("Recived the file into incoming folder.");

                bool isFileProcessed = false;
                var isReady = false;
                while (!isReady)
                {
                    isReady = FileWatcherHelper.IsFileReady(e.FullPath);
                    if (isReady)
                    {
                        isFileProcessed = await _fileProcessor.IsProcessed(e.FullPath);
                        break;
                    }
                }
                if (isFileProcessed)
                    _iLog.Info("Report generated successfully!!");
            }
            catch (Exception ex)
            {
                _iLog.Error($"Error while processing the file. Error: {ex.Message}");
            }
        }
    }
}

