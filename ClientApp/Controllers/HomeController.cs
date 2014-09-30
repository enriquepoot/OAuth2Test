using ClientApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace ClientApp.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        //
        // Get: /Home/
        public ActionResult Connect(string code)
        {
            try
            {
                // TODO: Add insert logic here
                var urlHelper = new UrlHelper(Request.RequestContext);
                var server = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, urlHelper.Content("~"));
                var returnPath = string.Format("{0}{1}", server, "Home/Connect");
                var redirectUrl = string.Format("http://localhost:63226/oauth2/authorize?client_id={0}&redirect_uri={1}&state=optional-csrf-token&response_type=code"
                    , urlHelper.Encode("bXljbGllbnRpZA==")
                    , urlHelper.Encode(returnPath));

                var result = RequestAccessToken(code, returnPath);

                if (string.IsNullOrEmpty(result.error))
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return View();
            }
        }

        public static AccessToken RequestAccessToken(string code, string rUri)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:63226/");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            NameValueCollection data = new NameValueCollection();
            data.Add("code", code);
            data.Add("client_id", "bXljbGllbnRpZA==");
            data.Add("client_secret", "aXQnc2FzZWNyZXQ=");
            data.Add("redirect_uri", rUri);
            data.Add("grant_type", "authorization_code");

            HttpResponseMessage response = client.PostAsync(string.Format("oauth2/token"), new FormUrlEncodedContent(
                                              data.
                                                  AllKeys.ToDictionary(
                                                      k => k, v => data[v]))).Result;

            return response.Content.ReadAsAsync<AccessToken>().Result;
        }

        //
        // POST: /Home/
        [HttpPost]
        public ActionResult Index(string something)
        {
            try
            {
                // TODO: Add insert logic here
                var urlHelper = new UrlHelper(Request.RequestContext);
                var server = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, urlHelper.Content("~"));
                var returnPath = string.Format("{0}{1}", server, "Home/Connect");
                var redirectUrl = string.Format("http://localhost:63226/oauth2/authorize?client_id={0}&redirect_uri={1}&state=optional-csrf-token&response_type=code"
                    , urlHelper.Encode("bXljbGllbnRpZA==")
                    , urlHelper.Encode(returnPath));

                return Redirect(redirectUrl);
            }
            catch
            {
                return View();
            }
        }
    }
}
