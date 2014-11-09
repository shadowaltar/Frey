using System;
using System.IO;
using System.Net;

namespace Trading.Common.Utils
{
    public static class Websites
    {
        public static string GetString(string url)
        {
            var result = "";
            var req = WebRequest.Create(url);
            req.Timeout = 20 * 1000;
            try
            {
                using (var response = req.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (var reader = new StreamReader(responseStream))
                            {
                                result = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return result;
        }
    }
}