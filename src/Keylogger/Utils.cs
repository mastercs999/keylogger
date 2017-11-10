using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keylogger
{
    public static class Utils
    {
        /// <summary>
        /// Gets all values of given enum
        /// </summary>
        /// <typeparam name="T">Enum whose values we want</typeparam>
        /// <returns>Values of the enum</returns>
        public static IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
