using System.Xml.Serialization;

namespace ReportGeneration.Model.Output
{
    /// <summary>
    /// ActualHeatRates
    /// </summary>
    public class ActualHeatRates
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
        /// Gets or sets the heat rate.
        /// </summary>
        /// <value>
        /// The heat rate.
        /// </value>
        [XmlElement("HeatRate")]
        public double HeatRate { get; set; }
    }
}
