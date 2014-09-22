using System;
using System.Net;
using System.Text;
using Spookfiles.Testing.Common;

namespace Spookfiles.Testing.Testrunners.Functionality
{
    public class SanityCheck : HttpTestBase, ITest
    {
        public string RelativeUrl { get; set; }
        public Type TypeToDeserialize { get; set; }
        public Func<object, bool> CheckValidDataInsideFunctionHandler { get; set; } 

        public TestResultBase Test(Options o)
        {
            var res = new GenericTestResult
            {
                ShortDescription = "Sanity light check",
                Status = TestResult.INCONCLUSIVE
            };
            try
            {
                WebClientEx webClient = SetupWebClient(o);
                string responseData = Encoding.UTF8.GetString(webClient.DownloadData(o.Url + RelativeUrl));
                if (responseData == "[]" || webClient.StatusCode == HttpStatusCode.NoContent)
                {
                    res.Status = TestResult.INCONCLUSIVE;
                    res.ExtraInformation = "NO DATA at " + RelativeUrl;
                    return res;
                }
                var responseObj = ServiceStack.Text.JsonSerializer.DeserializeFromString(responseData, TypeToDeserialize);

                // check if response is ok according to the handler
                res.Status = CheckValidDataInsideFunctionHandler(responseObj) ? TestResult.OK : TestResult.FAIL;
            }
            catch (Exception ex)
            {
                res.Status = TestResult.FAIL;
                res.CauseOfFailure = ex.Message;
            }
            finally
            {
                res.ExtraInformation += " Url: " + RelativeUrl;
            }

            return res;
        }
    }
}