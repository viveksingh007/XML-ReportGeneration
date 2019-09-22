using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ReportGeneration.Helpers
{
    /// <summary>
    /// FileWatcherHelper
    /// </summary>
    public static class FileWatcherHelper
    {
        /// <summary>
        /// Check whether file is ready to processed or not
        /// </summary>
        /// <param name="sFilename"></param>
        /// <returns></returns>
        public static bool IsFileReady(string sFilename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return inputStream.Length > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Generates the short unique identifier.
        /// </summary>
        /// <param name="maxlength">The maxlength.</param>
        /// <returns></returns>
        public static string GenerateShortGUID(int maxlength = 10)
        {
            var shortGuid = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");
            if (shortGuid.Length > maxlength)
                shortGuid = shortGuid.Substring(0, maxlength);
            return shortGuid;
        }
    }
}
