using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestSharp;
using RestSharp.Authenticators;
using System.Configuration;
using Web.Infrastructure.OAuth;
using RestSharp.Deserializers;
using Web.Models;
using Web.Infrastructure.FormsAuthenticationService;

namespace Web.Controllers
{
    public class TwitterController : Controller
    {
        public IFormsAuthenticationService FormsAuthService { get; set; }

        string baseUrl = "http://api.twitter.com";
        SiteDB _db;
        UserActivity _log;

        static private readonly InMemoryTokenManager TokenManager = new InMemoryTokenManager(
            ConfigurationManager.AppSettings["Twitter_Consumer_Key"],
            ConfigurationManager.AppSettings["Twitter_Consumer_Secret"]
            );



        public TwitterController(IFormsAuthenticationService FormsAuthService)
        {
            _db = new SiteDB();
            _log = new UserActivity(_db);

            this.FormsAuthService = FormsAuthService;
        }

        //
        // GET: /Twitter/

        public ActionResult Login()
        {
            //TODO: move all this logic outside the controller?

            var client = new RestClient(baseUrl);

            //get a token
            client.Authenticator = OAuth1Authenticator.ForRequestToken(TokenManager.ConsumerKey, TokenManager.ConsumerSecret);
            var request = new RestRequest("oauth/request_token", Method.POST);
            var response = client.Execute(request);

            //parse response.
            var qs = HttpUtility.ParseQueryString(response.Content);
            var oauth_token = qs["oauth_token"];
            var oauth_token_secret = qs["oauth_token_secret"];

            //store secret in memory
            TokenManager.StoreNewRequestToken(oauth_token, oauth_token_secret);

            //get authorize token
            request = new RestRequest("oauth/authenticate");
            request.AddParameter("oauth_token", oauth_token);
            var url = client.BuildUri(request).ToString();
            
            //redirect to Twitter
            return Redirect(url);

            //return View((object)response.Content);
        }

        public ActionResult Callback(string oauth_token, string oauth_verifier)
        {
            //TODO: move all this logic outside the controller?

            var client = new RestClient(baseUrl);

            //verify callback from twitter.
            var request = new RestRequest("oauth/access_token", Method.POST);
            client.Authenticator = OAuth1Authenticator.ForAccessToken(
                TokenManager.ConsumerKey, TokenManager.ConsumerSecret, oauth_token, TokenManager.GetTokenSecret(oauth_token), oauth_verifier
            );
            var response = client.Execute(request);

            var qs = HttpUtility.ParseQueryString(response.Content);
            var access_oauth_token = qs["oauth_token"];
            var access_oauth_token_secret = qs["oauth_token_secret"];
            
            request = new RestRequest("account/verify_credentials.xml");
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(
                TokenManager.ConsumerKey, TokenManager.ConsumerSecret, access_oauth_token, access_oauth_token_secret
            );
            response = client.Execute(request);

            var xml = new XmlDeserializer();
            var profile = xml.Deserialize<TwitterProfile>(response);
            var twitterId = profile.id.ToString();

            //get or create user based on username.
            var user = UserRepository.GetUserByLoginId(_db, twitterId, (short)UserTypes.Twitter);
            if (user == null)
            {
                //user not found, create a new one.
                user = UserRepository.Create3rdPartyAuthUser(_db, twitterId, access_oauth_token, (short)UserTypes.Twitter, profile.screen_name);

                //log that user registered.
                _log.LogIt(user.UserId, "User registered");

                this.FlashInfo("Thank you for signing up!");
            }
            //singin user with forms auth.
            FormsAuthService.SignIn(user.UserId.ToString(), true /* createPersistentCookie */);
            return SaveFriendlyInfoAndRedirect(user, "~/");
        }

        protected ActionResult SaveFriendlyInfoAndRedirect(User user, string returnUrl)
        {
            //save a friendly name for view use.
            Response.Cookies["friendly"].Value = user.Username;
            Response.Cookies["friendly"].Expires = DateTime.Now.AddDays(30);
            Response.Cookies["friendly"].HttpOnly = true;

            //redirect to specified url or default.
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
