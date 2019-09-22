using System;
using System.Xml.Serialization;

namespace ReportGeneration.Model.Input
{
    /// <summary>
    /// Represents the daily energy consumption
    /// </summary>
    public class Day
    {
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [XmlElement("Date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the energy.
        /// </summary>
        /// <value>
        /// The energy.
        /// </value>
        [XmlElement("Energy")]
        public double Energy { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        [XmlElement("Price")]
        public double Price { get; set; }
    }
}
