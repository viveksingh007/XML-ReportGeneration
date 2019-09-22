using System.Collections.Generic;
using System.Xml.Serialization;

namespace ReportGeneration.Model.Input
{
    /// <summary>
    /// CoalGenerator
    /// </summary>
    public class CoalGenerator
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlElement("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the generation.
        /// </summary>
        /// <value>
        /// The generation.
        /// </value>
        [XmlElement("Generation")]
        public Generation Generations { get; set; }

        /// <summary>
        /// Gets or sets the total heat input.
        /// </summary>
        /// <value>
        /// The total heat input.
        /// </value>
        [XmlElement("TotalHeatInput")]
        public double TotalHeatInput { get; set; }

        /// <summary>
        /// Gets or sets the actual net generation.
        /// </summary>
        /// <value>
        /// The actual net generation.
        /// </value>
        [XmlElement("ActualNetGeneration")]
        public double ActualNetGeneration { get; set; }

        /// <summary>
        /// Gets or sets the emissions rating.
        /// </summary>
        /// <value>
        /// The emissions rating.
        /// </value>
        [XmlElement("EmissionsRating")]
        public double EmissionsRating { get; set; }
    }
}
