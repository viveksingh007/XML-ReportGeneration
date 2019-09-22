using System.Collections.Generic;
using System.Xml.Serialization;

namespace ReportGeneration.Model.Output
{
    /// <summary>
    /// MaxEmissionGenerators
    /// </summary>
    public class MaxEmissionGenerators
    {
        /// <summary>
        /// Gets or sets the days.
        /// </summary>
        /// <value>
        /// The days.
        /// </value>
        [XmlElement("Day")]
        public List<Day> Days { get; set; }
    }
}
