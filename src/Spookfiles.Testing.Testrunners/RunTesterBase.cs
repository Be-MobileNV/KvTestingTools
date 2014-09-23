using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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
                RunTest(o, subTest, logger, test, i);
            }
        }

        /// <summary>
        ///     Run these tests for the category 'subTest', given Options o.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="subTest"></param>
        /// <param name="tests"></param>
        protected static void RunTestsParallel(Options o, string subTest, Action<string> logger, params ITest[] tests)
        {
            var tasks = new List<Task>();
            for (int i = 1; i <= tests.Length; i++)
            {
                var test = tests[i - 1];
                int i1 = i;
                var task = Task.Factory.StartNew(() => RunTest(o, subTest, logger, test, i1));
                tasks.Add(task);
                // We sleep 30 seconds after every test. This gives us a slow and sequential start up
                Thread.Sleep(TimeSpan.FromSeconds(30));
            }
            Task.WaitAll(tasks.ToArray());
        }

        private static void RunTest(Options o, string subTest, Action<string> logger, ITest test, int i)
        {
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