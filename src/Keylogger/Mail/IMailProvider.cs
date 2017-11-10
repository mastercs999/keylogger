using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keylogger.Mail
{
    /// <summary>
    /// Common interface for sending mails
    /// </summary>
    public interface IMailProvider
    {
        /// <summary>
        /// This function sends mail specified by arguments
        /// </summary>
        /// <param name="address">Recipient of the email</param>
        /// <param name="subject">Email's subject</param>
        /// <param name="message">Text of the email</param>
        void SendMail(string address, string subject, string message);
    }
}
