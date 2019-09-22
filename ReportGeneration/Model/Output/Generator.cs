using System.Xml.Serialization;

namespace ReportGeneration.Model.Output
{
    /// <summary>
    /// Generator
    /// </summary>
    public class Generator
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
        /// Gets or sets the total.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        [XmlElement("Total")]
        public double Total { get; set; }
    }
}
