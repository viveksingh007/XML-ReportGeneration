using ReportGeneration.Model.Input;
using ReportGeneration.Model.Output;
using System;
using System.IO;
using System.Xml.Serialization;

namespace ReportGeneration.Helpers
{
    /// <summary>
    /// XmlParser
    /// </summary>
    public static class XmlParser
    {
        private static readonly string _outputFilePath = AppConfigHelper.GetSectionsConfig("ReportConfig", "OutputFilePath", string.Empty);

        /// <summary>
        /// Deserializes the XML data.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        public static GenerationReport DeserializeXmlData(string fullPath)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(GenerationReport));
                StreamReader streamReader = new StreamReader(fullPath);
                GenerationReport generationReport = (GenerationReport)xmlSerializer.Deserialize(streamReader);
                streamReader.Dispose();
                streamReader.Close();
                return generationReport;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Serializes the object to XML.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool SerializeObjectToXml(GenerationOutput result)
        {
            bool isFileGotSerialize = false;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(GenerationOutput));
                StreamWriter streamWriter = new StreamWriter(_outputFilePath);
                xmlSerializer.Serialize(streamWriter, result);
                streamWriter.Dispose();
                streamWriter.Close();
                isFileGotSerialize = true;
            }
            catch (Exception)
            {
                throw;
            }
            return isFileGotSerialize;
        }
    }
}
