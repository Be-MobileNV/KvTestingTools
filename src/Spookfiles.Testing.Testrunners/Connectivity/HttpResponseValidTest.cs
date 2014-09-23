using System;
using System.Net;
using Spookfiles.Testing.Common;

namespace Spookfiles.Testing.Testrunners.Connectivity
{
    public class HttpResponseValidTest : HttpTestBase, ITest
    {
        public string RelativeUrl { get; set; }

        public TestResultBase Test(Options o)
        {
            var res = new GenericTestResult
            {
                ShortDescription = "Call " + RelativeUrl,
                Status = TestResult.INCONCLUSIVE
            };

            try
            {
                WebClient c = SetupWebClient(o, AuthenticationMode.UseValidCredentials);
                byte[] data = c.DownloadData(o.Url + RelativeUrl);
                if (HttpValidationTests.IsValidJson(c.ResponseHeaders) && HttpValidationTests.IsValidCharsetUtf8(c.ResponseHeaders))
                {
                    res.Status = TestResult.OK;
                }

                else
                {
                    res.Status = TestResult.FAIL;
                    res.ExtraInformation = "No response header Content-Type. Invalid HTTP";
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

    public class HttpResponseValidTests : ITest
    {
        public string RelativeUrl { get; set; }
        public Func<dynamic, bool> ValidatorFunction { get; set; }

        public TestResultBase Test(Options o)
        {
            var res = new GenericTestResult
            {
                ShortDescription = "HTTP response valid",
                Status = TestResult.INCONCLUSIVE
            };

            try
            {
                var c = new WebClient();
                byte[] data = c.DownloadData(o.Url + RelativeUrl);
                if (HttpValidationTests.IsValidJson(c.ResponseHeaders) && HttpValidationTests.IsValidCharsetUtf8(c.ResponseHeaders))
                        res.Status = TestResult.OK;
                else
                {
                    res.Status = TestResult.FAIL;
                    res.ExtraInformation = "No response header Content-Type. Invalid HTTP";
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