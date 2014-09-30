using Data.Entities;
using Data.Models;
using Data.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace AuthorizationServer.Controllers
{
    public class TokenController : Controller
    {
        private ClientRepository clientRepo = new ClientRepository();
        private AuthorizationCodeRepository codeRepo = new AuthorizationCodeRepository();
        private AccessTokenRepository tokenRepo = new AccessTokenRepository();

        //
        // POST: /Token/
        [HttpPost]
        public JsonResult Index(AuthorizationGrantModel model)
        {
            try
            {
                //Validate the code is used only once

                var client = clientRepo.GetClient(model.client_id);

                if (client == null)
                    return Json(new { error = "invalid_request" }, JsonRequestBehavior.AllowGet);

                if(!client.ClientSecret.Equals(model.client_secret))
                    return Json(new { error = "invalid_client" }, JsonRequestBehavior.AllowGet);

                var code = codeRepo.GetAuthorizationCode(model.code);

                if(code == null)
                    return Json(new { error = "invalid_grant" }, JsonRequestBehavior.AllowGet);

                if(!code.ClientID.Equals(client.ClientID))
                    return Json(new { error = "unauthorized_client" }, JsonRequestBehavior.AllowGet);

                //Validate Uri match the client url registered

                if(!string.IsNullOrEmpty(code.Redirect_Uri) && !code.Redirect_Uri.Equals(model.redirect_uri))
                    return Json(new { error = "invalid_request" }, JsonRequestBehavior.AllowGet);

                //Valid
                var key = Convert.FromBase64String(client.ClientSecret);
                var provider = new System.Security.Cryptography.HMACSHA256(key);

                var UserID = WebSecurity.CurrentUserId;

                var rawTokenInfo = string.Concat(code.Code, client.ClientSecret, UserID, DateTime.UtcNow.ToString("d"));
                var rawTokenByte = Encoding.UTF8.GetBytes(rawTokenInfo);
                var token = provider.ComputeHash(rawTokenByte);

                var accessToken = new AccessToken()
                {
                    ClientID = model.client_id,
                    UserID = UserID,
                    Token = Convert.ToBase64String(token),
                    Type = "bearer",
                    Expiration = DateTime.Now.AddDays(1),
                    RefreshToken = string.Empty,
                    Scope = string.Empty
                };

                if (tokenRepo.Save(accessToken))
                    return Json(new AccessTokenModel(accessToken), JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json(new { error = "invalid_request" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = "invalid_request" }, JsonRequestBehavior.AllowGet);
        }

    }
}
