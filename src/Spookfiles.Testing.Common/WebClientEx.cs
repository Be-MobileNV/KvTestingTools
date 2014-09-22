using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Spookfiles.Testing.Common
{
    public class WebClientEx : WebClient
    {
        private WebResponse m_Resp = null;

        protected override WebResponse GetWebResponse(WebRequest Req)
        {
            return m_Resp = base.GetWebResponse(Req);
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                if (m_Resp != null && m_Resp is HttpWebResponse)
                    return (m_Resp as HttpWebResponse).StatusCode;
                else
                    return HttpStatusCode.OK;
            }
        }
    }
}
