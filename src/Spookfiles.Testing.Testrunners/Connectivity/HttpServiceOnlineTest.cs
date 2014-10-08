using System;
using System.Linq;
using System.Net;
using Spookfiles.Testing.Common;

namespace Spookfiles.Testing.Testrunners.Connectivity
{
    public class HttpServiceOnlineTest : ITest
    {
        public TestResultBase Test(Options o)
        {
            var res = new GenericTestResult
            {
                ShortDescription = "HTTP service running",
                Status = TestResult.INCONCLUSIVE
            };

            try
            {
                var c = new WebClient();
                byte[] data = c.DownloadData(o.Url);
                if (c.ResponseHeaders.AllKeys.Contains("Content-Type"))
                    res.Status = TestResult.OK;
                else
                {
                    res.Status = TestResult.FAIL;
                    res.ExtraInformation = "No response header Content-Type. Invalid HTTP";
                }
            }
            catch (WebException wex)
            {
                var response = (HttpWebResponse) wex.Response;
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    res.Status = TestResult.OK;
                    res.ExtraInformation = "Ok - HTTP confirmed. Received " + response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                res.Status = TestResult.FAIL;
                res.CauseOfFailure = ex.Message;
            }

            return res;
        }
    }
}