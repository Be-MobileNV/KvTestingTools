using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Spookfiles.Testing.Common;
using Spookfiles.Testing.Testrunners;
using Spookfiles.Testing.Testrunners.Functionality;

namespace Spookfiles.Testing.KvA.CLI
{
    public class KvAPerformanceTest : ITest
    {
        public string RelativeUrl { get; set; }

        /// <summary>
        ///     while we're here, just validate if the generation time changes every time.
        /// </summary>
        public bool? GenerationTimeValid { get; private set; }

        /// <summary>
        ///     Value in ms
        /// </summary>
        public int IntervalTime { get; set; }

        /// <summary>
        ///     Value in seconds
        /// </summary>
        public int TestDuration { get; set; }

        public List<string> FieldsThatShouldBePresent { get; set; }

        public TestResultBase Test(Options o)
        {
            var testResult = new GenericTestResult
            {
                ShortDescription = "Performance (uptime) " + RelativeUrl,
                Status = TestResult.INCONCLUSIVE
            };


            DateTime current = DateTime.Now; // to check if we elapsed total seconds.
            var results = new List<Tuple<long, TestResult>>();
            //long lastMessageTime = 0;

            while (true)
            {
                var test = new CheckCompletenessResponseTest
                {
                    RelativeUrl = RelativeUrl, //string.Format(RelativeUrl, lastMessageTime)
                    FieldsThatShouldBePresent = FieldsThatShouldBePresent
                };
                TestResultBase res = test.Test(o);
                results.Add(new Tuple<long, TestResult>(test.RequestDuration, res.Status));

                //if (test.ResponseData != null)
                //{
                //    // now store the last updated time from the previous
                //    string data = test.ResponseData;
                //}
                Thread.Sleep(IntervalTime);
                if (DateTime.Now.Subtract(current).TotalSeconds >= TestDuration)
                    break;
            }

            IEnumerable<Tuple<long, TestResult>> succesCount = results.Where(t => t.Item2 == TestResult.OK).ToList();

            var totalResultCount = results.Count;
            double percentageSuccess = totalResultCount == 0 ? 0 : succesCount.Count() / (double)totalResultCount;
            double avgTime = succesCount.Any() ? succesCount.Select(t => t.Item1).Average() : -1;

            testResult.ExtraInformation = Math.Round(avgTime, 0) + " ms average time";

            if (percentageSuccess >= 0.95)
            {
                testResult.Status = TestResult.OK;
            }
            else
            {
                testResult.Status = TestResult.FAIL;
                testResult.CauseOfFailure = "Percentage success: " + Math.Round(percentageSuccess * 100)  + "%";
            }

            return testResult;
        }
    }
}