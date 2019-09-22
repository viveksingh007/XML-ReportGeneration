using ReportGeneration.Helpers;
using ReportGeneration.Logger.Interface;
using ReportGeneration.Model.Output;
using ReportGeneration.Process.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Input = ReportGeneration.Model.Input;

namespace ReportGeneration.Process
{
    /// <summary>
    /// FileProcessor
    /// </summary>
    public class FileProcessor : IFileProcessor
    {
        /// <summary>
        /// The archive incoming folder path
        /// </summary>
        private static readonly string _archiveIncomingFolderPath = AppConfigHelper.GetSectionsConfig("ReportConfig", "ArchiveIncomingFolderPath", string.Empty);

        /// <summary>
        /// The i log
        /// </summary>
        private readonly ILog _iLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProcessor"/> class.
        /// </summary>
        /// <param name="iLog">The i log.</param>
        public FileProcessor(ILog iLog)
        {
            _iLog = iLog;
        }

        #region Public   

        /// <summary>
        /// Determines whether the specified full path is processed.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> IsProcessed(string fullPath)
        {
            bool isFileProcessed = false;
            if (!string.IsNullOrWhiteSpace(fullPath))
            {
                try
                {
                    var deserializedGenerationReportData = XmlParser.DeserializeXmlData(fullPath);
                    if (deserializedGenerationReportData != null)
                    {

                        var result = await FileProcessing(deserializedGenerationReportData);
                        if (result != null)
                        {
                            isFileProcessed = XmlParser.SerializeObjectToXml(result);
                        }
                        if (isFileProcessed)
                            ArchiveIncomingFile(fullPath);
                    }
                }
                catch (Exception ex)
                {
                    _iLog.Error($"Error while processing the incoming file. Error: {ex.Message}");
                }
            }
            return isFileProcessed;
        }

        #endregion

        #region Private

        /// <summary>
        /// Processing the incoming datas
        /// </summary>
        /// <param name="generationReport">The generation report.</param>
        /// <returns></returns>
        private async Task<GenerationOutput> FileProcessing(Input.GenerationReport generationReport)
        {
            if (generationReport == null) return null;
            _iLog.Info("File processing started.");
            var generationOutput = new GenerationOutput();
            var actualHeatRates = new List<ActualHeatRates>();
            var generators = new List<Generator>();
            var days = new List<Day>();
            try
            {
                foreach (var item in generationReport.Wind?.WindGenerators)
                {
                    generators.Add(await CalculateWindGeneratorTotal(item));
                };
                foreach (var item in generationReport.Coal?.CoalGenerators)
                {
                    generators.Add(await CalculateCoalGeneratorTotal(item));
                    days.AddRange(await CalculateCoalMaxEmission(item));
                    actualHeatRates.Add(await CalculateActualHeatRate(item));
                };
                foreach (var item in generationReport.Gas.GasGenerators)
                {
                    generators.Add(await CalculateGasGeneratorTotal(item));
                    days.AddRange(await CalculateGasMaxEmission(item));
                };

                generationOutput.Totals = new Totals
                {
                    Generators = new List<Generator>()
                };
                generationOutput.Totals.Generators.AddRange(generators);
                generationOutput.MaxEmissionGenerators = new MaxEmissionGenerators
                {
                    Days = new List<Day>()
                };
                generationOutput.MaxEmissionGenerators.Days.AddRange(days);
                generationOutput.ActualHeatRates = new List<ActualHeatRates>();
                generationOutput.ActualHeatRates.AddRange(actualHeatRates);
            }
            catch (Exception ex)
            {
                _iLog.Error($"Error in calculation. Error: {ex.Message}");
            }
            return generationOutput;
        }

        /// <summary>
        /// Deletes the local files.
        /// </summary>
        /// <param name="incomingFolderFilePath">The incoming folder file path.</param>
        private void ArchiveIncomingFile(string incomingFolderFilePath)
        {
            if (string.IsNullOrWhiteSpace(incomingFolderFilePath)) return;

            try
            {
                _iLog.Info("Archive file process started.");
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(incomingFolderFilePath);
                var directoryName = Path.GetDirectoryName(incomingFolderFilePath);
                var files = Directory.GetFiles(directoryName, fileNameWithoutExtension + "*.*", SearchOption.TopDirectoryOnly);
                foreach (var item in files)
                {

                    File.Copy(item, Path.Combine(_archiveIncomingFolderPath, Path.GetFileName(item)?.Replace(fileNameWithoutExtension,
                        $"{fileNameWithoutExtension}_{FileWatcherHelper.GenerateShortGUID()}")), true);
                    File.Delete(item);
                    _iLog.Info("Incoming file archived successfully.");
                }
            }
            catch (Exception ex)
            {
                _iLog.Error($"Error file archiving the incoming file. Error: {ex.Message}");
            }
        }

        #region Gas    

        /// <summary>
        /// Calculates the gas maximum emission.
        /// </summary>
        /// <param name="gasGenerator">The gas generator.</param>
        /// <returns></returns>
        private async Task<List<Day>> CalculateGasMaxEmission(Input.GasGenerator gasGenerator)
        {
            if (gasGenerator == null) return null;
            List<Day> days = new List<Day>();
            try
            {
                _iLog.Info("Calculate gas max emission process started.");
                foreach (var day in gasGenerator.Generations?.Days)
                {
                    days.Add(await CalculateGasDailyMaxEmission(day, gasGenerator.Name, gasGenerator.EmissionsRating));
                }
            }
            catch (Exception ex)
            {
                _iLog.Error($"Error while calculating gas max emission. Error: {ex.Message}");
            }
            return days;
        }

        /// <summary>
        /// Calculates the gas generator total.
        /// </summary>
        /// <param name="gasGenerator">The gas generator.</param>
        /// <returns></returns>
        private async Task<Generator> CalculateGasGeneratorTotal(Input.GasGenerator gasGenerator)
        {
            if (gasGenerator == null) return null;
            try
            {
                _iLog.Info("Calculate gas generator total process started.");
                double gasTotal = 0;
                double _valueFactorMedium = AppConfigHelper.GetSectionsConfig("ValueFactor", "Medium", 0.0);
                if (!(gasGenerator.Generations?.Days == null) && gasGenerator.Generations?.Days?.Count > 0)
                {
                    foreach (var item in gasGenerator.Generations?.Days)
                    {
                        gasTotal += (item.Energy * item.Price * _valueFactorMedium);
                    }
                }

                return new Generator
                {
                    Name = gasGenerator.Name,
                    Total = gasTotal
                };
            }
            catch (Exception ex)
            {
                _iLog.Error($"Error while calculating gas generator total. Error: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Calculates the gas daily maximum emission.
        /// </summary>
        /// <param name="day">The day.</param>
        /// <param name="name">The name.</param>
        /// <param name="emissionsRating">The emissions rating.</param>
        /// <returns></returns>
        private async Task<Day> CalculateGasDailyMaxEmission(Input.Day day, string name, double emissionsRating)
        {
            if (day == null) return null;
            try
            {
                _iLog.Info("Calculate gas daily max emission process started.");
                return new Day
                {
                    Name = name,
                    Date = day.Date,
                    Emission = (day.Energy * emissionsRating * AppConfigHelper.GetSectionsConfig("EmissionFactor", "Medium", 0.0))
                };
            }
            catch (Exception ex)
            {
                _iLog.Error($"Error while calculating gas daily max emission. Error: {ex.Message}");
            }
            return null;
        }

        #endregion Gas

        #region Coal

        /// <summary>
        /// Calculates the coal maximum emission.
        /// </summary>
        /// <param name="coalGenerator">The coal generator.</param>
        /// <returns></returns>
        private async Task<List<Day>> CalculateCoalMaxEmission(Input.CoalGenerator coalGenerator)
        {
            if (coalGenerator == null) return null;
            List<Day> days = new List<Day>();

            try
            {
                _iLog.Info("Calculate coal max emission process started.");
                foreach (var day in coalGenerator.Generations?.Days)
                {
                    days.Add(await CalculateCoalDailyMaxEmission(day, coalGenerator.Name, coalGenerator.EmissionsRating));
                }
            }
            catch (Exception ex)
            {
                _iLog.Error($"Error while calculating coal max emission. Error: {ex.Message}");
            }
            return days;
        }

        /// <summary>
        /// Calculates the coal daily maximum emission.
        /// </summary>
        /// <param name="day">The day.</param>
        /// <param name="name">The name.</param>
        /// <param name="emissionsRating">The emissions rating.</param>
        /// <returns></returns>
        private async Task<Day> CalculateCoalDailyMaxEmission(Input.Day day, string name, double emissionsRating)
        {
            if (day == null) return null;
            try
            {
                _iLog.Info("Calculate coal daily max emission process started.");
                return new Day
                {
                    Name = name,
                    Date = day.Date,
                    Emission = (day.Energy * emissionsRating * AppConfigHelper.GetSectionsConfig("EmissionFactor", "High", 0.0))
                };
            }
            catch (Exception ex)
            {
                _iLog.Error($"Error while calculating coal daily max emission. Error: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Calculates the coal generator total.
        /// </summary>
        /// <param name="coalGenerator">The coal generator.</param>
        /// <returns></returns>
        private async Task<Generator> CalculateCoalGeneratorTotal(Input.CoalGenerator coalGenerator)
        {
            if (coalGenerator == null) return null;
            try
            {
                _iLog.Info($"Calculate coal generator process started.");
                double coalTotal = 0;
                double _valueFactorMedium = AppConfigHelper.GetSectionsConfig("ValueFactor", "Medium", 0.0);
                if (!(coalGenerator.Generations?.Days == null) && coalGenerator.Generations?.Days?.Count > 0)
                {
                    foreach (var item in coalGenerator.Generations?.Days)
                    {
                        coalTotal += (item.Energy * item.Price * _valueFactorMedium);
                    }
                }

                return new Generator
                {
                    Name = coalGenerator.Name,
                    Total = coalTotal
                };
            }
            catch (Exception ex)
            {
                _iLog.Error($"Error while calculating coal generator total. Error: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Calculates the actual heat rate.
        /// </summary>
        /// <param name="coalGenerator">The coal generator.</param>
        /// <returns></returns>
        private async Task<ActualHeatRates> CalculateActualHeatRate(Input.CoalGenerator coalGenerator)
        {
            if (coalGenerator == null) return null;
            try
            {
                _iLog.Info("Actual heat rate calculation started.");
                return new ActualHeatRates
                {
                    Name = coalGenerator.Name,
                    HeatRate = (coalGenerator.TotalHeatInput / coalGenerator.ActualNetGeneration)
                };
            }
            catch (Exception ex)
            {
                _iLog.Error($"Error while calculating coal actual heat rate. Error: {ex.Message}");
            }
            return null;
        }
        #endregion Coal

        #region Wind
        /// <summary>
        /// Calculates the wind generator total.
        /// </summary>
        /// <param name="windGenerator">The wind generator.</param>
        /// <returns></returns>
        private async Task<Generator> CalculateWindGeneratorTotal(Input.WindGenerator windGenerator)
        {
            if (windGenerator == null) return null;
            try
            {
                _iLog.Info("Calculate wind generator total processing started.");
                double windTotal = 0;
                if (!(windGenerator.Generations?.Days == null) && windGenerator.Generations?.Days?.Count > 0)
                {
                    double valueFactor = string.Equals(windGenerator.Location, "ONSHORE", StringComparison.InvariantCultureIgnoreCase) ? AppConfigHelper.GetSectionsConfig("ValueFactor", "High", 0.0)
                                                                : AppConfigHelper.GetSectionsConfig("ValueFactor", "Low", 0.0);

                    foreach (var item in windGenerator.Generations.Days)
                    {
                        windTotal += (item.Energy * item.Price * valueFactor);
                    }
                }
                return new Generator
                {
                    Name = windGenerator.Name,
                    Total = windTotal
                };
            }
            catch (Exception ex)
            {
                _iLog.Error($"Error while calculating wind generator total. Error: {ex.Message}");
            }
            return null;
        }
        #endregion Wind

        #endregion
    }
}
