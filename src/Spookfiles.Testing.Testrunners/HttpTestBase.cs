using System;
using System.Net;
using Spookfiles.Testing.Common;

namespace Spookfiles.Testing.Testrunners
{
    public abstract class HttpTestBase
    {
        public enum AuthenticationMode
        {
            UseValidCredentials,
            UseInvalidCredentials,
            UseNoCredentials
        }

        protected WebClientEx SetupWebClient(Options o,
            AuthenticationMode validCredentials = AuthenticationMode.UseValidCredentials)
        {
            var c = new WebClientEx();
            c.Headers.Add(HttpRequestHeader.Accept, "application/json");

            if (!String.IsNullOrEmpty(o.ApiKey) && validCredentials != AuthenticationMode.UseNoCredentials)
            {
                string headerName = !String.IsNullOrEmpty(o.ApiKeyHeader)
                    ? o.ApiKeyHeader
                    : Options.DefaultApiKeyHeaderName;
                c.Headers.Add(headerName,
                    validCredentials == AuthenticationMode.UseValidCredentials ? o.ApiKey : Options.InvalidApiKey);
            }
            if (!String.IsNullOrEmpty(o.User) && validCredentials != AuthenticationMode.UseNoCredentials)
            {
                c.Credentials = validCredentials == AuthenticationMode.UseValidCredentials
                    ? new NetworkCredential(o.User, o.Pass)
                    : new NetworkCredential(Options.InvalidUserName, Options.InvalidPassword);
            }

            return c;
        }


        protected HttpWebRequest SetupWebRequest(Options o, string url,
            AuthenticationMode validCredentials = AuthenticationMode.UseValidCredentials)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "application/json";
            if (!String.IsNullOrEmpty(o.ApiKey) && validCredentials != AuthenticationMode.UseNoCredentials)
            {
                string headerName = !String.IsNullOrEmpty(o.ApiKeyHeader)
                    ? o.ApiKeyHeader
                    : Options.DefaultApiKeyHeaderName;
                request.Headers.Add(headerName,
                    validCredentials == AuthenticationMode.UseValidCredentials ? o.ApiKey : Options.InvalidApiKey);
            }
            if (!String.IsNullOrEmpty(o.User) && validCredentials != AuthenticationMode.UseNoCredentials)
            {
                request.Credentials = validCredentials == AuthenticationMode.UseValidCredentials
                    ? new NetworkCredential(o.User, o.Pass)
                    : new NetworkCredential(Options.InvalidUserName, Options.InvalidPassword);
            }
            return request;
        }
    }
}