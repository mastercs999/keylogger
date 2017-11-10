using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keylogger.Mail
{
    public interface IMailProvider
    {
        void SendMail(string address, string subject, string message);
    }
}
