using System;
using System.Collections.Specialized;
using System.Configuration;
namespace ReportGeneration.Helpers
{
    public class AppConfigHelper
    {
        /// <summary>
        /// Gets the sections configuration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionname">Section name.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Config Value</returns>
        public static T GetSectionsConfig<T>(string sectionname, string key, T defaultValue)
        {
            try
            {
                return GetSectionsConfig<T>(sectionname, key);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets the sections configuration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionname">Section name.</param>
        /// <param name="key">key name.</param>
        /// <returns>Config Value</returns>
        /// <exception cref="System.Exception">Config not found</exception>
        public static T GetSectionsConfig<T>(string sectionname, string key)
        {
            var section = ConfigurationManager.GetSection(sectionname) as NameValueCollection;
            {
                if (section == null)
                {
                    throw new Exception("Incorrect Section name," + sectionname + " is not found in app.config");
                }

                if (string.IsNullOrWhiteSpace(section[key]))
                {
                    throw new Exception("Incorrect key name," + key + " is not found in app.config");
                }
            }
            var retVal = section[key];
            return (T)Convert.ChangeType(retVal, typeof(T));
        }
    }
}
