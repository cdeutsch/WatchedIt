using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Infrastructure.OAuth
{
    internal class InMemoryTokenManager
    {
        private Dictionary<string, string> tokensAndSecrets = new Dictionary<string, string>();

        public InMemoryTokenManager(string consumerKey, string consumerSecret)
        {
            if (String.IsNullOrEmpty(consumerKey))
            {
                throw new ArgumentNullException("consumerKey");
            }

            this.ConsumerKey = consumerKey;
            this.ConsumerSecret = consumerSecret;
        }

        public string ConsumerKey { get; private set; }
        public string ConsumerSecret { get; private set; }

        public string GetTokenSecret(string Token)
        {
            return this.tokensAndSecrets[Token];
        }

        public void StoreNewRequestToken(string Token, string TokenSecret)
        {
            this.tokensAndSecrets[Token] = TokenSecret;
        }

        public void ExpireRequestTokenAndStoreNewAccessToken(string requestToken, string accessToken, string accessTokenSecret)
        {
            this.tokensAndSecrets.Remove(requestToken);
            this.tokensAndSecrets[accessToken] = accessTokenSecret;
        }
    }
}