#if !(SILVERLIGHT || WINDOWS_PHONE)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using HtmlAgilityPack;
using VkToolkit.Exception;
using System.Web;

namespace VkToolkit.Utils
{
    public class Browser : IBrowser
    {
        internal static string InvalidLoginOrPassword = "Invalid login or password";
        internal static string InvalidLoginOrPasswordRu = "Указан неверный логин или пароль";

        private readonly WebHttpClient _client;

        public Browser(WebHttpClient client)
        {
            _client = client;
        }

        public string Authorize(string url, string email, string password)
        {
            // Get authorize html page form
            var response = _client.GetContentStream(url);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new VkApiException(response.StatusDescription);

            var html = new HtmlDocument();
            html.Load(response.Resource, Encoding.UTF8);

            var form = GetFormNode(html);
            var inputs = GetHtmlDocumentInputs(form);

            if (inputs.ContainsKey("email"))
                inputs["email"] = HttpUtility.UrlEncode(email);

            if (inputs.ContainsKey("pass"))
                inputs["pass"] = HttpUtility.UrlEncode(password);

            var actionUrl = form.Attributes["action"] != null
                                ? form.Attributes["action"].Value
                                : url;

            var uri = string.Join("&", inputs.Select(x => string.Format("{0}={1}", x.Key, x.Value)));
            var bytes = Encoding.UTF8.GetBytes(uri);
            
            // post authorize information
            response = _client.Post<Stream>(actionUrl, new MemoryStream(bytes), new MediaTypeHeaderValue("application/x-www-form-urlencoded"));
            html.Load(response.Resource, Encoding.UTF8);

            if (ContainsText(html, InvalidLoginOrPassword) || ContainsText(html, InvalidLoginOrPasswordRu))
                throw new VkApiAuthorizationException(InvalidLoginOrPassword, email, password);

            // we run our application at first time
            // we need gain access
            if (!response.ResponseUrl.Contains("access_token"))
                response = GainAccess(html, response.ResponseUrl);

            if (!response.ResponseUrl.Contains("access_token"))
                throw new VkApiAuthorizationException("Can't authorize.");

            //var req = (HttpWebRequest)WebRequest.Create(actionUrl);
            //req.CookieContainer = new CookieContainer();
            //req.Method = "POST";
            //req.ContentType = "application/x-www-form-urlencoded";
            //req.ContentLength = bytes.Length;
            //req.GetRequestStream().Write(bytes, 0, bytes.Length);
            //req.AllowAutoRedirect = false;

            //HttpWebResponse resp = GetResponse(req);
            //_html.Load(resp.GetResponseStream(), Encoding.UTF8);
            //Url = resp.ResponseUri;

            //SaveCookies(Url, resp.Cookies);

            //if ((int)resp.StatusCode == 302) // redirect
            //{
            //    Redirect(resp.Headers["Location"]);
            //}
            ////else
            ////    throw new VkApiException("Redirect expected!");

            return response.ResponseUrl;
        }

        public string GetJson(string url)
        {
            var json = _client.Get<string>(url);

            return json.Resource;
        }

        private Response<Stream> GainAccess(HtmlDocument html, string url)
        {
            var form = GetFormNode(html);
            var inputs = GetHtmlDocumentInputs(form);

            var actionUrl = form.Attributes["action"] != null
                                ? form.Attributes["action"].Value
                                : url;
            
            var uri = string.Join("&", inputs.Select(x => string.Format("{0}={1}", x.Key, x.Value)));
            var bytes = Encoding.UTF8.GetBytes(uri);

            var response = _client.Post<Stream>(actionUrl, new MemoryStream(bytes), new MediaTypeHeaderValue("application/x-www-form-urlencoded"));

            //var req = (HttpWebRequest)WebRequest.Create(actionUrl);
            //req.Referer = _referer;
            //req.CookieContainer = _cookies;
            //req.CookieContainer = new CookieContainer();
            //req.Method = "POST";
            //req.ContentType = "application/x-www-form-urlencoded";
            //req.ContentLength = bytes.Length;
            //req.GetRequestStream().Write(bytes, 0, bytes.Length);
            //req.AllowAutoRedirect = false;

            //HttpWebResponse resp = GetResponse(req);
            //_html.Load(resp.GetResponseStream(), Encoding.UTF8);
            //Url = resp.ResponseUri;

            //if ((int)resp.StatusCode == 302) // redirect
            //{
            //    Redirect(resp.Headers["Location"]);
            //}

            return response;
        }

        private bool ContainsText(HtmlDocument html, string text)
        {
            if (html == null || html.DocumentNode == null)
                return false;

            var bodyNode = html.DocumentNode.SelectSingleNode("//body");
            if (bodyNode == null)
                return false;

            var body = bodyNode.InnerText;
            return body.Contains(text);
        }

        #region Private Methods

        private Dictionary<string, string> GetHtmlDocumentInputs(HtmlNode form)
        {
            var result = new Dictionary<string, string>();

            foreach (var node in form.SelectNodes("//input"))
            {
                var nameAttribute = node.Attributes["name"];
                var valueAttribute = node.Attributes["value"];

                var name = nameAttribute != null ? nameAttribute.Value : string.Empty;
                var value = valueAttribute != null ? valueAttribute.Value : string.Empty;

                if (string.IsNullOrEmpty(name))
                    continue;

                result.Add(name, HttpUtility.UrlEncode(value));
            }

            return result;
        }

        private HtmlNode GetFormNode(HtmlDocument html)
        {
            HtmlNode.ElementsFlags.Remove("form");
            var form = html.DocumentNode.SelectSingleNode("//form");

            if (form == null)
                throw new VkApiException("Form element not found.");

            return form;
        }

        #endregion
    }
}

#endif