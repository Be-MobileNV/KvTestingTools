using System;
using Spookfiles.Koppelvlak.G;
using Spookfiles.Testing.Common;
using Spookfiles.Testing.Testrunners;
using Spookfiles.Testing.Testrunners.Connectivity;
using Spookfiles.Testing.Testrunners.Functionality;
using Spookfiles.Testing.Testrunners.Security;

namespace Spookfiles.Testing.CLI
{
    public class Tester : RunTesterBase
    {
        /// <summary>
        /// General todo: refactor the url getters to a static method for flexibility purposes.
        /// </summary>
        /// <param name="options"></param>
        internal static void RunConnectivityTests(Options options)
        {
            RunTests(options, "Connectivity", Out.Info,
                new PingTest(),
                new TcpConnectionTest(),
                new HttpServiceOnlineTest(),
                new HttpResponseValidTest {RelativeUrl = "/fcd"});
        }   

        internal static void RunFunctionalityTests(Options options)
        {
            RunTests(options, "Functionality", Out.Info,
                new HttpResponseValidTest {RelativeUrl = "/fcd?last_message_time=0"},
                new CheckCompletenessResponseTest
                {
                    RelativeUrl = "/fcd?last_message_time=0",
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInFCD()
                },
                new SanityCheck()
                {
                    RelativeUrl = "/fcd?last_message_time=0",
                    TypeToDeserialize = typeof (FcdMessage),
                    CheckValidDataInsideFunctionHandler = o =>
                    {
                        var fcd = (FcdMessage) o;
                        if (fcd.fcd_records.Count == 0)
                            return false;
                        // todo : further specify checks
                        return true;
                    }
                }
                );
        }

        internal static void RunSecurityTests(Options options)
        {
            RunTests(options, "Security", Out.Info,
                new CheckCompletenessResponseTest
                {
                    RelativeUrl = "/fcd?last_message_time=0",
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInFCD()
                },
                // 1                 // note: this is the same test as runfunctionality - the last one.
                new CheckCertificateTest {RelativeUrl = "/fcd"}, // 2    
                new CallingWithInvalidCredentials
                {
                    RelativeUrl = "/fcd",
                    UseCredentials = HttpTestBase.AuthenticationMode.UseInvalidCredentials
                }, // 3
                new CallingWithInvalidCredentials
                {
                    RelativeUrl = "/fcd",
                    UseCredentials = HttpTestBase.AuthenticationMode.UseNoCredentials
                }, // 4
                new CheckHttpAvailableTest {RelativeUrl = "/fcd"} // 5
                );
        }

        internal static void RunPerformanceTests(Options options)
        {
            var test = new KvGPerformanceTest
            {
                RelativeUrl = "/fcd",
                IntervalTime = Options.PerformanceTestInterval,
                TestDuration = Options.PerformanceTestDuration
            };
            TestResultBase result1 = test.Test(options);
            result1.StepNr = 1;
            result1.SubTest = "Performance";

            Out.Info(result1.ToString());

            // test 2
            if (test.GenerationTimeValid.Value)
            {
                Out.Info(new GenericTestResult
                {
                    SubTest = "Performance",
                    StepNr = 2,
                    ShortDescription = "Update interval /fcd",
                    Status = TestResult.OK
                }.ToString());
            }
            else
            {
                Out.Info(new GenericTestResult
                {
                    SubTest = "Performance",
                    StepNr = 2,
                    ShortDescription = "Update interval /fcd",
                    Status = TestResult.FAIL,
                    CauseOfFailure = "Generation time did not update properly."
                }.ToString());
            }
        }

        internal static void RunContinuityTests(Options options)
        {
            var test = new KvGPerformanceTest
            {
                RelativeUrl = "/fcd",
                IntervalTime = Options.PerformanceTestInterval,
                TestDuration = Options.PerformanceTestDuration
            };
            TestResultBase result1 = test.Test(options);
            result1.StepNr = 1;
            result1.SubTest = "Continuity";
            Out.Info(result1.ToString());
        }
    }
}