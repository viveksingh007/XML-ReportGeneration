using System.Collections.Generic;
using System.Xml.Serialization;

namespace ReportGeneration.Model.Input
{
    /// <summary>
    /// Wind
    /// </summary>
    public class Wind
    {
        /// <summary>
        /// Gets or sets the wind generators.
        /// </summary>
        /// <value>
        /// The wind generators.
        /// </value>
        [XmlElement("WindGenerator")]
        public List<WindGenerator> WindGenerators { get; set; }
    }
}
