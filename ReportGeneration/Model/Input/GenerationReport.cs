using System.Xml.Serialization;

namespace ReportGeneration.Model.Input
{

    /// <summary>
    /// GenerationReport
    /// </summary>
    [XmlRoot("GenerationReport")]
    public class GenerationReport
    {
        /// <summary>
        /// Gets or sets the wind.
        /// </summary>
        /// <value>
        /// The wind.
        /// </value>
        [XmlElement("Wind")]
        public Wind Wind { get; set; }

        /// <summary>
        /// Gets or sets the gas.
        /// </summary>
        /// <value>
        /// The gas.
        /// </value>
        [XmlElement("Gas")]
        public Gas Gas { get; set; }

        /// <summary>
        /// Gets or sets the coal.
        /// </summary>
        /// <value>
        /// The coal.
        /// </value>
        [XmlElement("Coal")]
        public Coal Coal { get; set; }
    }
}
