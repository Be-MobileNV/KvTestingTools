using System;
using System.Runtime.InteropServices;
using Spookfiles.Testing.Common;

namespace Spookfiles.Testing.Testrunners
{
    public abstract class RunTesterBase
    {
        /// <summary>
        ///     Run these tests for the category 'subTest', given Options o.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="subTest"></param>
        /// <param name="tests"></param>
        protected static void RunTests(Options o, string subTest, Action<string> logger, params ITest[] tests)
        {
            for (int i = 1; i <= tests.Length; i++)
            {
                var test = tests[i - 1];
                TestResultBase result = null;
                try
                {
                    result = test.Test(o);
                    result.StepNr = i;
                    result.SubTest = subTest;
                    if (logger != null)
                        logger(result.ToString());
                }
                catch (Exception ex)
                {
                    if (result == null)
                    {
                        result = new GenericTestResult();
                    }
                    result.StepNr = i;
                    result.SubTest = subTest;
                    result.Status = TestResult.FAIL;
                    result.CauseOfFailure = ex.Message;
                    result.ExtraInformation = ex.StackTrace;
                }
            }
        }
    }
}