using AuthorizationServer.Filters;
using Data.Entities;
using Data.Models;
using Data.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace AuthorizationServer.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AuthorizeController : Controller
    {
        private ClientRepository clientRepo = new ClientRepository();
        private AuthorizationCodeRepository codeRepo = new AuthorizationCodeRepository();

        //
        // GET: /Authorize/
        public ActionResult Index()
        {
            const string RESPONSE_TYPE = "response_type";
            const string CLIENT_ID = "client_id";
            const string REDIRECT_URI = "redirect_uri";
            const string SCOPE = "scope";
            const string STATE = "state";

            var query = Request.QueryString;

            AuthorizationModel model = new AuthorizationModel();
            model.response_type = query[RESPONSE_TYPE];
            model.client_id = query[CLIENT_ID];
            model.redirect_uri = query[REDIRECT_URI];
            model.scope = query[SCOPE]; //Optional
            model.state = query[STATE];  //Recommended

            if (string.IsNullOrEmpty(model.response_type) || string.IsNullOrEmpty(model.client_id) || string.IsNullOrEmpty(model.redirect_uri))
                return Redirect(string.Format("{0}?error={1}", model.redirect_uri, "invalid_request"));

            var client = clientRepo.GetClient(model.client_id);

            if (client == null)
                return Redirect(string.Format("{0}?error={1}", model.redirect_uri, "unauthorized_client"));

            if (model.response_type != "code")
                return Redirect(string.Format("{0}?error={1}", model.redirect_uri, "unsupported_response_type"));

            //Agregar mas errores, especificados en el RFC6749

            ViewBag.AppName = client.ClientName;

            //Everything ok
            return View(model);
        }

        //
        // POST: /Authorize/
        [HttpPost]
        public ActionResult Index(AuthorizationModel model)
        {
            if (string.IsNullOrEmpty(model.response_type) || string.IsNullOrEmpty(model.client_id) || string.IsNullOrEmpty(model.redirect_uri))
                return Redirect(string.Format("{0}?error={1}", model.redirect_uri, "invalid_request"));

            var client = clientRepo.GetClient(model.client_id);

            if (client == null)
                return Redirect(string.Format("{0}?error={1}", model.redirect_uri, "unauthorized_client"));

            if (model.response_type != "code")
                return Redirect(string.Format("{0}?error={1}", model.redirect_uri, "unsupported_response_type"));

            if(model.Deny)
                return Redirect(string.Format("{0}?error={1}", model.redirect_uri, "access_denied"));

            //Agregar mas errores, especificados en el RFC6749

            //Generate the code hash, valid for max (recommended) 10 minutes
            var key = Convert.FromBase64String(client.ClientSecret);
            var provider = new System.Security.Cryptography.HMACSHA256(key);

            var UserID = WebSecurity.CurrentUserId;

            var rawCodeInfo = string.Concat(client.ClientID, client.ClientSecret, UserID, DateTime.UtcNow.ToString("d"));
            var rawCodeByte = Encoding.UTF8.GetBytes(rawCodeInfo);
            var code = provider.ComputeHash(rawCodeByte);

            var authorizationCode = new AuthorizationCode()
            {
                ClientID = model.client_id,
                UserID = UserID,
                Code = Convert.ToBase64String(code),
                Expiration = DateTime.Now.AddMinutes(10),
                Redirect_Uri = model.redirect_uri
            };

            if (codeRepo.Save(authorizationCode))
                return Redirect(string.Format("{0}?code={1}", model.redirect_uri, HttpUtility.UrlEncode(authorizationCode.Code)));

            return Redirect(string.Format("{0}?error={1}", model.redirect_uri, "server_error"));
        }
    }
}
