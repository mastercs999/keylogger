using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keylogger
{
    public static class Extensions
    {
        /// <summary>
        /// Append given version to the string. If given string is path, version is appended before extension
        /// </summary>
        /// <param name="str">String where the version will be appended</param>
        /// <param name="version">Version to append</param>
        /// <returns>String with version appended. Few examples: Custom->Custom_2 or image.png->image_2.png</returns>
        public static string AddVersion(this string str, int version)
        {
            if (!str.Contains('.'))
                return str + "_" + version;
            else
            {
                string extension = Path.GetExtension(str);

                return str.Substring(0, str.Length - extension.Length) + "_" + version + extension;
            }
        }
    }
}
