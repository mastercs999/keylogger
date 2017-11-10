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
