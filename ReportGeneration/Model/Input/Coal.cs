using System.Collections.Generic;
using System.Xml.Serialization;

namespace ReportGeneration.Model.Input
{
    /// <summary>
    /// Coal
    /// </summary>
    public class Coal
    {
        /// <summary>
        /// Gets or sets the coal generator.
        /// </summary>
        /// <value>
        /// The coal generator.
        /// </value>
        [XmlElement("CoalGenerator")]
        public List<CoalGenerator> CoalGenerators { get; set; }
    }
}
