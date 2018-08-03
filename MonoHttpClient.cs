using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MonoUtilities.Http
{
    public class MonoHttpClient: HttpClient 
    {
        /*
        * TODO:
        * Add method to add headers (referer, etc)
        * Expose more of the hidden HttpClient and HttpClientHandler methods
        */
        private HttpClient fClient;
        private HttpClientHandler fClientHandler;
        private CookieContainer fCookieContainer;
        private List<KeyValuePair<string, string>> fPostParams;
        private List<KeyValuePair<string, string>> fGetParams;
        public MonoHttpClient(TimeSpan requestTimeout, bool allowAutoRedirect = true)
        {
            fCookieContainer = new CookieContainer();
            fClientHandler = new HttpClientHandler() {AllowAutoRedirect = allowAutoRedirect, CookieContainer = fCookieContainer};
            fClient = new HttpClient(fClientHandler) { Timeout = requestTimeout};
            fPostParams = new List<KeyValuePair<string, string>>();
            fGetParams = new List<KeyValuePair<string, string>>();
        }

        public void AddPostParameter(string paramName, object paramValue)
        {
            fPostParams.Add(new KeyValuePair<string, string>(paramName, paramValue.ToString()));
        }
        public void ClearPostParameters()
        {
            fPostParams.Clear();
        }

        public void AddGetParameter(string paramName, object paramValue)
        {
            fGetParams.Add(new KeyValuePair<string, string>(paramName, paramValue.ToString()));
        }
        public void ClearGetParameters()
        {
            fGetParams.Clear();
        }

        public void AddCookie(string url, string key, object value)
        {
            Cookie c = new Cookie(key, value.ToString());
            fCookieContainer.Add(new Uri(url), c);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, bool clearParamsAfterResponse = true)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(fPostParams);
            HttpResponseMessage result = await fClient.PostAsync(url, content);
            if (clearParamsAfterResponse)
                fPostParams.Clear();
            return result;
        }

        public async Task<HttpResponseMessage> GetAsync(string url, bool clearParamsAfterResponse = true)
        {
            for (int i = 0; i > fGetParams.Count; i++)
            {
                if (i == 0)
                    url += "?";
                url += fGetParams[i].Key + "=" + fGetParams[i].Value;
                if (i + 1 > fGetParams.Count)
                    url += "&";
            }
            HttpResponseMessage result = await fClient.GetAsync(url);
            if (clearParamsAfterResponse)
                fGetParams.Clear();
            return result;
        }
    }
}
