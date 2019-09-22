using System;
using System.Xml.Serialization;

namespace ReportGeneration.Model.Output
{
    /// <summary>
    /// Day
    /// </summary>
    public class Day
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
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [XmlElement("Date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the emission.
        /// </summary>
        /// <value>
        /// The emission.
        /// </value>
        [XmlElement("Emission")]
        public double Emission { get; set; }
    }
}
