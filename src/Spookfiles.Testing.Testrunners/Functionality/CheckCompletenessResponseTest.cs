using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using Spookfiles.Testing.Common;

namespace Spookfiles.Testing.Testrunners.Functionality
{
    public class CheckCompletenessResponseTest : HttpTestBase, ITest
    {
        public string RelativeUrl { get; set; }
        public Type TypeToDeserialize { get; set; }
        //public Func<object, bool> CheckValidDataInsideFunctionHandler { get; set; } 
        public Action<WebClient> OnSetupWebClient { get; set; }

        /// <summary>
        ///     After calling succesfully, you can retrieve the result here.
        /// </summary>
        public string ResponseData { get; private set; }

        /// <summary>
        ///     Sets the total milliseconds the request took.
        /// </summary>
        public long RequestDuration { get; private set; }

        public List<string> FieldsThatShouldBePresent { get; set; }

        public TestResult? TestResultWhenNoData { get; set; }

        public TestResultBase Test(Options o)
        {
            var res = new GenericTestResult
            {
                ShortDescription = "Check completeness response",
                Status = TestResult.INCONCLUSIVE
            };
            try
            {
                WebClientEx c = SetupWebClient(o);

                if (OnSetupWebClient != null)
                    OnSetupWebClient(c);

                var watch = new Stopwatch();
                watch.Start();
                string data = Encoding.UTF8.GetString(c.DownloadData(o.Url + RelativeUrl));
              
                
                if (data == "[]" || c.StatusCode == HttpStatusCode.NoContent)
                {

                    res.Status = TestResultWhenNoData.HasValue ? TestResultWhenNoData.Value : TestResult.INCONCLUSIVE;
                    res.ExtraInformation = "NO DATA at " + RelativeUrl;
                    return res;
                }
                watch.Stop();

                ResponseData = data;
                RequestDuration = watch.ElapsedMilliseconds;

                if (c.ResponseHeaders["Content-Type"].ToLowerInvariant().Contains("application/json")
                    && c.ResponseHeaders["Content-Type"].ToLowerInvariant().Contains("charset=utf-8")
                    )
                {
                    if (FieldsThatShouldBePresent.All(data.Contains))
                        res.Status = TestResult.OK;
                    else
                    {
                        res.Status = TestResult.FAIL;
                        res.ExtraInformation = "Response DTO did not contain all the required fields";
                    }
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
            finally
            {
                res.ExtraInformation += " Url: " + RelativeUrl;
            }

            return res;
        }
    }
}