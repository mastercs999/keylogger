using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keylogger.Upload
{
    public interface IOnlineService
    {
        string Upload(string zipFile);
    }
}
