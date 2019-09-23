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
                        //If file processed successfully and report generated, moving the incoming file to the archive folder.
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
                    //Daily wind generation calculation adding it to generator collection
                    generators.Add(await CalculateGenerationTotal(item.Generations, item.Name, item.Location));
                };
                foreach (var item in generationReport.Coal?.CoalGenerators)
                {
                    //Daily coal generation calculation and adding it to generator collection
                    generators.Add(await CalculateGenerationTotal(item.Generations, item.Name, null));
                    //Coal max emission calculation and adding it to days collection. 
                    days.AddRange(await CalculateCoalMaxEmission(item));
                    //Coal actual heat rate calculation.
                    actualHeatRates.Add(await CalculateActualHeatRate(item));
                };
                foreach (var item in generationReport.Gas.GasGenerators)
                {
                    //Daily gas generation calculation and adding it to generator collection
                    generators.Add(await CalculateGenerationTotal(item.Generations, item.Name, null));
                    //Gas max emission calculation and adding it to days collection. 
                    days.AddRange(await CalculateGasMaxEmission(item));
                };

                //Adding items to the GenerationOut object
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
                    days.Add(await CalculateDailyMaxEmission(day, gasGenerator.Name, gasGenerator.EmissionsRating, "Medium"));
                }
            }
            catch (Exception ex)
            {
                _iLog.Error($"Error while calculating gas max emission. Error: {ex.Message}");
            }
            return days;
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
                    days.Add(await CalculateDailyMaxEmission(day, coalGenerator.Name, coalGenerator.EmissionsRating, "High"));
                }
            }
            catch (Exception ex)
            {
                _iLog.Error($"Error while calculating coal max emission. Error: {ex.Message}");
            }
            return days;
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

        /// <summary>
        /// Calculates the generation total.
        /// </summary>
        /// <param name="generation">The generation.</param>
        /// <param name="name">The name.</param>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        private async Task<Generator> CalculateGenerationTotal(Input.Generation generation, string name, string location = null)
        {
            if (generation == null || string.IsNullOrWhiteSpace(name)) return null;
            try
            {
                _iLog.Info("Calculate wind generator total processing started.");

                double valueFactor = 0;
                double total = 0;
                //Decide the value factor for specific generator
                if (Extensions.Contains(name, "Wind[Offshore]") || Extensions.Contains(name, "Wind[Onshore]"))
                {
                    valueFactor = string.Equals(location, "ONSHORE", StringComparison.InvariantCultureIgnoreCase) ? AppConfigHelper.GetSectionsConfig("ValueFactor", "High", 0.0)
                                                                   : AppConfigHelper.GetSectionsConfig("ValueFactor", "Low", 0.0);
                }
                else if (Extensions.Contains(name, "coal") || Extensions.Contains(name, "gas"))
                {
                    valueFactor = AppConfigHelper.GetSectionsConfig("ValueFactor", "Medium", 0.0);
                }

                //Calculate the generation value for each generator
                if (!(generation?.Days == null) && generation?.Days?.Count > 0)
                {
                    foreach (var item in generation.Days)
                    {
                        total += (item.Energy * item.Price * valueFactor);
                    }
                }
                //return the generation value
                return new Generator
                {
                    Name = name,
                    Total = total
                };
            }
            catch (Exception ex)
            {
                _iLog.Error($"Error while calculating generation total. Error: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Calculates the daily maximum emission.
        /// </summary>
        /// <param name="day">The day.</param>
        /// <param name="name">The name.</param>
        /// <param name="emissionsRating">The emissions rating.</param>
        /// <param name="emissionFactor">The emission factor.</param>
        /// <returns></returns>
        private async Task<Day> CalculateDailyMaxEmission(Input.Day day, string name, double emissionsRating, string emissionFactor)
        {
            if (day == null) return null;
            try
            {
                _iLog.Info("Calculate daily max emission process started.");
                return new Day
                {
                    Name = name,
                    Date = day.Date,
                    Emission = day.Energy * emissionsRating * GetEmissionFactorValue(name, emissionFactor)
                };
            }
            catch (Exception ex)
            {
                _iLog.Error($"Error while calculating daily max emission. Error: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Gets the emission factor value.
        /// </summary>
        /// <param name="generatorType">Type of the generator.</param>
        /// <param name="emissionFactor">The emission factor.</param>
        /// <returns></returns>
        private double GetEmissionFactorValue(string generatorType, string emissionFactor)
        {
            double emissionFactorValue = 0;
            if (string.IsNullOrWhiteSpace(generatorType) || string.IsNullOrWhiteSpace(emissionFactor)) return emissionFactorValue;
            char[] delimiterChars = { '[' };
            string[] splitGeneratorType = generatorType.Split(delimiterChars);
            switch (splitGeneratorType[0]?.ToUpper())
            {
                case "COAL":
                    switch (emissionFactor.ToUpper())
                    {
                        case "HIGH":
                            emissionFactorValue = AppConfigHelper.GetSectionsConfig("EmissionFactor", emissionFactor.ToUpper(), 0.0);
                            break;
                        case "MEDIUM":
                            emissionFactorValue = AppConfigHelper.GetSectionsConfig("EmissionFactor", emissionFactor.ToUpper(), 0.0);
                            break;
                        case "LOW":
                            emissionFactorValue = AppConfigHelper.GetSectionsConfig("EmissionFactor", emissionFactor.ToUpper(), 0.0);
                            break;
                        default:
                            break;
                    }
                    break;
                case "GAS":
                    switch (emissionFactor.ToUpper())
                    {
                        case "HIGH":
                            emissionFactorValue = AppConfigHelper.GetSectionsConfig("EmissionFactor", emissionFactor.ToUpper(), 0.0);
                            break;
                        case "MEDIUM":
                            emissionFactorValue = AppConfigHelper.GetSectionsConfig("EmissionFactor", emissionFactor.ToUpper(), 0.0);
                            break;
                        case "LOW":
                            emissionFactorValue = AppConfigHelper.GetSectionsConfig("EmissionFactor", emissionFactor.ToUpper(), 0.0);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return emissionFactorValue;
        }

        #endregion
    }
}
