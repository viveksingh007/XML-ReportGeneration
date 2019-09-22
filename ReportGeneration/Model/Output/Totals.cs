using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ReportGeneration.Model.Output
{
    /// <summary>
    /// Totals
    /// </summary>
    public class Totals
    {
        /// <summary>
        /// Gets or sets the generators.
        /// </summary>
        /// <value>
        /// The generators.
        /// </value>
        [XmlElement("Generator")]
        public List<Generator> Generators { get; set; }
    }
}
