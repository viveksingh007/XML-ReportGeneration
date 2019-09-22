using System.Collections.Generic;
using System.Xml.Serialization;

namespace ReportGeneration.Model.Output
{
    /// <summary>
    /// GenerationOutput
    /// </summary>
    [XmlRoot("GenerationOutput")]
    public class GenerationOutput
    {
        /// <summary>
        /// Gets or sets the totals.
        /// </summary>
        /// <value>
        /// The totals.
        /// </value>
        [XmlElement("Totals")]
        public Totals Totals { get; set; }

        /// <summary>
        /// Gets or sets the maximum emission generators.
        /// </summary>
        /// <value>
        /// The maximum emission generators.
        /// </value>
        [XmlElement("MaxEmissionGenerators")]
        public MaxEmissionGenerators MaxEmissionGenerators { get; set; }

        /// <summary>
        /// Gets or sets the actual heat rates.
        /// </summary>
        /// <value>
        /// The actual heat rates.
        /// </value>
        [XmlElement("ActualHeatRates")]
        public List<ActualHeatRates> ActualHeatRates { get; set; }
    }
}
