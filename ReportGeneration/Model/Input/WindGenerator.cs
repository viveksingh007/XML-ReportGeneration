using System.Collections.Generic;
using System.Xml.Serialization;

namespace ReportGeneration.Model.Input
{
    /// <summary>
    /// WindGenerator
    /// </summary>
    public class WindGenerator
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
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        [XmlElement("Location")]
        public string Location { get; set; }
    }
}
