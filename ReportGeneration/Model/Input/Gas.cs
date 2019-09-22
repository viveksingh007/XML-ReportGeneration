using System.Collections.Generic;
using System.Xml.Serialization;

namespace ReportGeneration.Model.Input
{
    /// <summary>
    /// Gas
    /// </summary>
    public class Gas
    {
        /// <summary>
        /// Gets or sets the gas generators.
        /// </summary>
        /// <value>
        /// The gas generators.
        /// </value>
        [XmlElement("GasGenerator")]
        public List<GasGenerator> GasGenerators { get; set; }
    }
}
