using System.Collections.Generic;
using System.Xml.Serialization;

namespace ReportGeneration.Model.Input
{
    /// <summary>
    /// GasGenerator
    /// </summary>
    public class GasGenerator
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
        /// Gets or sets the emissions rating.
        /// </summary>
        /// <value>
        /// The emissions rating.
        /// </value>
        [XmlElement("EmissionsRating")]
        public double EmissionsRating { get; set; }
    }
}
