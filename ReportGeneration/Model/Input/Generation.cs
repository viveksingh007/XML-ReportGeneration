using System.Collections.Generic;
using System.Xml.Serialization;

namespace ReportGeneration.Model.Input
{
    /// <summary>Generation class</summary>
    public class Generation
    {
        /// <summary>
        /// Gets or sets the day.
        /// </summary>
        /// <value>
        /// The day.
        /// </value>
        [XmlElement("Day")]
        public List<Day> Days { get; set; }
    }
}
