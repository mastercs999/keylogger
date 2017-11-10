using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keylogger.Upload
{
    /// <summary>
    /// Common interface for online storage services
    /// </summary>
    public interface IOnlineService
    {
        /// <summary>
        /// This function should upload given zip file onto online storage and return link for file download
        /// </summary>
        /// <param name="zipFile">Path to the zip file to upload</param>
        /// <returns>Url where the zip file can be downloaded</returns>
        string Upload(string zipFile);
    }
}
