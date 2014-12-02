using AuthorizationServer.Filters;
using Data.Entities;
using Data.Models;
using Data.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace AuthorizationServer.Controllers
{
    public class TokenController : Controller
    {
        private ClientRepository clientRepo = new ClientRepository();
        private AuthorizationCodeRepository codeRepo = new AuthorizationCodeRepository();
        private AccessTokenRepository tokenRepo = new AccessTokenRepository();
        private UserRepository userRepo = new UserRepository();

        //
        // POST: /Token/
        [HttpPost]
        public JsonResult Index(AuthorizationGrantModel model)
        {
            try
            {
                if (model.grant_type == "authorization_code")
                {
                    //Validate the code is used only once

                    var client = clientRepo.GetClient(model.client_id);

                    if (client == null)
                        return Json(new { error = "invalid_request" }, JsonRequestBehavior.AllowGet);

                    if (!client.ClientSecret.Equals(model.client_secret))
                        return Json(new { error = "invalid_client" }, JsonRequestBehavior.AllowGet);

                    var code = codeRepo.GetAuthorizationCode(model.code);

                    if (code == null)
                        return Json(new { error = "invalid_grant" }, JsonRequestBehavior.AllowGet);

                    if (!code.ClientID.Equals(client.ClientID))
                        return Json(new { error = "unauthorized_client" }, JsonRequestBehavior.AllowGet);

                    //Validate Uri match the client url registered

                    if (!string.IsNullOrEmpty(code.Redirect_Uri) && !code.Redirect_Uri.Equals(model.redirect_uri))
                        return Json(new { error = "invalid_request" }, JsonRequestBehavior.AllowGet);

                    //Valid
                    var key = Convert.FromBase64String(client.ClientSecret);
                    var provider = new System.Security.Cryptography.HMACSHA256(key);

                    var UserID = code.UserID;

                    var rawTokenInfo = string.Concat(code.Code, client.ClientSecret, UserID, DateTime.UtcNow.ToString("hhmmss"));
                    var rawTokenByte = Encoding.UTF8.GetBytes(rawTokenInfo);
                    var token = provider.ComputeHash(rawTokenByte);

                    var rawRefreeshInfo = string.Concat(code.Code, client.ClientSecret, DateTime.UtcNow.ToString("hhmmss"));
                    var rawRefreshByte = Encoding.UTF8.GetBytes(rawRefreeshInfo);
                    var refresh = provider.ComputeHash(rawRefreshByte);

                    var accessToken = new AccessToken()
                    {
                        ClientID = model.client_id,
                        UserID = UserID,
                        Token = Convert.ToBase64String(token),
                        Type = "bearer",
                        Expiration = DateTime.Now.AddDays(1),
                        RefreshToken = Convert.ToBase64String(refresh),
                        Scope = string.Empty
                    };

                    if (tokenRepo.Save(accessToken))
                        return Json(new AccessTokenModel(accessToken), JsonRequestBehavior.AllowGet);
                }

                if (model.grant_type == "refresh_token" && !string.IsNullOrEmpty(model.refresh_token))
                {
                    //Validate the code is used only once

                    var client = clientRepo.GetClient(model.client_id);

                    if (client == null)
                        return Json(new { error = "invalid_request" }, JsonRequestBehavior.AllowGet);

                    if (!client.ClientSecret.Equals(model.client_secret))
                        return Json(new { error = "invalid_client" }, JsonRequestBehavior.AllowGet);

                    var oldToken = tokenRepo.GetRefreshToken(model.refresh_token);

                    if (oldToken == null)
                        return Json(new { error = "invalid_grant" }, JsonRequestBehavior.AllowGet);

                    if (!oldToken.ClientID.Equals(client.ClientID))
                        return Json(new { error = "unauthorized_client" }, JsonRequestBehavior.AllowGet);

                    //Valid
                    var key = Convert.FromBase64String(client.ClientSecret);
                    var provider = new System.Security.Cryptography.HMACSHA256(key);

                    var UserID = oldToken.UserID;

                    var rawTokenInfo = string.Concat(oldToken.Token, client.ClientSecret, DateTime.UtcNow.ToString("hhmmss"));
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

            }
            catch
            {
                return Json(new { error = "invalid_request" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = "invalid_request" }, JsonRequestBehavior.AllowGet);
        }

    }
}
