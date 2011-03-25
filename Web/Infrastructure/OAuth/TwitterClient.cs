//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace Web.Infrastructure.OAuth
//{
//    public class TwitterClient
//    {
//        private string UserName { get; set; }

//        private static readonly ServiceProviderDescription ServiceDescription =
//            new ServiceProviderDescription
//            {
//                RequestTokenEndpoint = new MessageReceivingEndpoint(
//                                           "http://twitter.com/oauth/request_token",
//                                           HttpDeliveryMethods.GetRequest |
//                                           HttpDeliveryMethods.AuthorizationHeaderRequest),
//                UserAuthorizationEndpoint = new MessageReceivingEndpoint(
//                                          "http://twitter.com/oauth/authorize",
//                                          HttpDeliveryMethods.GetRequest |
//                                          HttpDeliveryMethods.AuthorizationHeaderRequest),
//                AccessTokenEndpoint = new MessageReceivingEndpoint(
//                                          "http://twitter.com/oauth/access_token",
//                                          HttpDeliveryMethods.GetRequest |
//                                          HttpDeliveryMethods.AuthorizationHeaderRequest),
//                TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
//            };

//        IConsumerTokenManager _tokenManager;

//        public TwitterClient(IConsumerTokenManager tokenManager)
//        {
//            _tokenManager = tokenManager;
//        }

//        public void StartAuthentication()
//        {
//            var request = HttpContext.Current.Request;
//            using (var twitter = new WebConsumer(ServiceDescription, _tokenManager))
//            {
//                var callBackUrl = new Uri(request.Url.Scheme + "://" +
//                                  request.Url.Authority + "/Account/TwitterCallback");
//                twitter.Channel.Send(
//                    twitter.PrepareRequestUserAuthorization(callBackUrl, null, null)
//                );
//            }
//        }

//        public bool FinishAuthentication()
//        {
//            using (var twitter = new WebConsumer(ServiceDescription, _tokenManager))
//            {
//                var accessTokenResponse = twitter.ProcessUserAuthorization();
//                if (accessTokenResponse != null)
//                {
//                    UserName = accessTokenResponse.ExtraData["screen_name"];
//                    return true;
//                }
//            }

//            return false;
//        }
//    }
//}

//using System.Web.Security;
//public class test
//{
//    private void CreateAuthCookie(string username, string token)
//    {
//        //Get ASP.NET to create a forms authentication cookie (based on settings in web.config)~
//        HttpCookie cookie = FormsAuthentication.GetAuthCookie(username, false);

//        //Decrypt the cookie
//        FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

//        //Create a new ticket using the details from the generated cookie, but store the username &
//        //token passed in from the authentication method
//        FormsAuthenticationTicket newticket = new FormsAuthenticationTicket(
//        ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration,
//        ticket.IsPersistent, token);

//        // Encrypt the ticket & store in the cookie
//        cookie.Value = FormsAuthentication.Encrypt(newticket);

//        // Update the outgoing cookies collection.
//        Response.Cookies.Set(cookie);
//    }
//}