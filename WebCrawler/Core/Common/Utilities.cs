using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Utilities
    {
        private static readonly object _syncObject = new object();
        public static bool IsValidURL(string urlToCrawl, out Uri uriResult)
        {
            if (Uri.TryCreate(urlToCrawl, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp)
            {
                return true;
            }
            return false;
        }
        public static string GetHttpResponse(string inputUrl)
        {
            var responseValue = string.Empty;
            var request = (HttpWebRequest)WebRequest.Create(inputUrl);
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        throw new ApplicationException(message);
                    }

                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                LogExceptions("Failed to get response from " + inputUrl);
            }
            return responseValue;
        }
        public static void LogExceptions(string errorMessgae)
        {
            string errorLogPath = ConfigManager.errorLogPath;
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("*** Exception Begins ***");
            stringBuilder.AppendLine("Message = " + errorMessgae);
            stringBuilder.AppendLine("*** Exception Ends ***");
            lock (_syncObject)
            {
                using (StreamWriter sw = File.AppendText(errorLogPath))
                {
                    sw.WriteLine(stringBuilder.ToString());
                }
            }
        }
    }
}
