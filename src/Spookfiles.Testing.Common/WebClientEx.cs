using System.Net;

namespace Spookfiles.Testing.Common
{
    public class WebClientEx : WebClient
    {
        private WebResponse _resp;

        protected override WebResponse GetWebResponse(WebRequest req)
        {
            return _resp = base.GetWebResponse(req);
        }

        protected override WebRequest GetWebRequest(System.Uri address)
        {
            WebRequest request = (WebRequest)base.GetWebRequest(address);

            request.PreAuthenticate = true;
            return request;
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                if (_resp != null && _resp is HttpWebResponse)
                    return (_resp as HttpWebResponse).StatusCode;
                return HttpStatusCode.OK;
            }
        }
    }
}
