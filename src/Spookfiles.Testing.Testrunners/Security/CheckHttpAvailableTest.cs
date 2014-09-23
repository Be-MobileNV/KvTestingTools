using System;
using System.Net;
using System.Text;
using Spookfiles.Testing.Common;

namespace Spookfiles.Testing.Testrunners.Security
{
    public class CheckHttpAvailableTest : HttpTestBase, ITest
    {
        public string RelativeUrl { get; set; }
        public Type TypeToDeserialize { get; set; }

        public TestResultBase Test(Options o)
        {
            var res = new GenericTestResult
            {
                ShortDescription = "Calling HTTP (no SSL)",
                Status = TestResult.INCONCLUSIVE
            };
            try
            {
                WebClient c = SetupWebClient(o);
                string data = Encoding.UTF8.GetString(c.DownloadData((o.Url + RelativeUrl).Replace("https", "http")));
                if (HttpValidationTests.IsValidJson(c.ResponseHeaders) && HttpValidationTests.IsValidCharsetUtf8(c.ResponseHeaders))
                {
                    if (data.Contains("generation_time") && data.Contains("ref_position_lon") &&
                        data.Contains("ref_position_lat") && data.Contains("speed")
                        && data.Contains("heading") && data.Contains("provider_id") &&
                        data.Contains("user_id_anonymous"))
                    {
                        res.Status = TestResult.FAIL;
                        res.ExtraInformation = "Received http 200 ok with all data fields present.";
                    }
                    else
                    {
                        res.Status = TestResult.FAIL;
                        res.ExtraInformation = "Received http 200 ok, but required data fields were not present.";
                    }
                }
                else
                {
                    res.Status = TestResult.OK;
                    res.ExtraInformation = "No json data received. ";
                }
            }
            catch (WebException wex)
            {
                var response = (HttpWebResponse) wex.Response;
                if (response.StatusCode != HttpStatusCode.Accepted && response.StatusCode != HttpStatusCode.NotModified)
                {
                    res.Status = TestResult.OK;
                    res.ExtraInformation = "Ok - received " + response.StatusCode + " (expected behaviour)";
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