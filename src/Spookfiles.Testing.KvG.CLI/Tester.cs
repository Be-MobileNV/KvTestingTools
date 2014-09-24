using System;
using System.Collections.Generic;
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
                new HttpResponseValidTest { RelativeUrl = FcdRetrieveRequestWithTime() });
        }

        internal static void RunFunctionalityTests(Options options)
        {
            RunTests(options, "Functionality", Out.Info,
                new HttpResponseValidTest { RelativeUrl = FcdRetrieveRequestWithTime() },
                new CheckCompletenessResponseTest
                {
                    RelativeUrl = FcdRetrieveRequestWithTime(),
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInFCD()
                },
                new SanityCheck()
                {
                    RelativeUrl = FcdRetrieveRequestWithTime(),
                    TypeToDeserialize = typeof(FcdMessage),
                    CheckValidDataInsideFunctionHandler = o =>
                    {
                        var fcd = (FcdMessage)o;
                        if (fcd.provider_id == null) return false;
                        if (fcd.fcd_records.Count == 0) return false;
                        foreach (var fcdrecord in fcd.fcd_records)
                        {
                            if (fcdrecord.user_id_anonymous == 0) return false;
                            foreach (var trail in fcdrecord.trail)
                            {
                                if (trail.generation_time.ToUniversalTime() > DateTime.UtcNow)
                                {
                                    return false;
                                }
                                if (trail.speed < 0 || trail.speed > 150)
                                {
                                    return false;
                                }
                                if (trail.ref_position_lat < 51.482733 || trail.ref_position_lat > 51.555198)
                                {
                                    return false;
                                }
                                if (trail.ref_position_lon < 5.116459 || trail.ref_position_lon > 5.393983)
                                {
                                    return false;
                                }
                                if (trail.heading < 0 || trail.heading > 360)
                                {
                                    return false;
                                }

                                // Check Not implimented no valid time to countermeasure with
                                //if (trail.generation_time.ToUniversalTime() < DateTime.UtcNow.AddMinutes(-2))
                                //{
                                //    return false;
                                //}
                            }
                        }


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
                    RelativeUrl = FcdRetrieveRequestWithTime(),
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInFCD()
                },
                // 1                 // note: this is the same test as runfunctionality - the last one.
                new CheckCertificateTest { RelativeUrl = FcdRetrieveRequestWithTime() }, // 2    
                new CallingWithInvalidCredentials
                {
                    RelativeUrl = FcdRetrieveRequestWithTime(),
                    UseCredentials = HttpTestBase.AuthenticationMode.UseInvalidCredentials
                }, // 3
                new CallingWithInvalidCredentials
                {
                    RelativeUrl = FcdRetrieveRequestWithTime(),
                    UseCredentials = HttpTestBase.AuthenticationMode.UseNoCredentials
                }, // 4
                new CheckHttpAvailableTest { RelativeUrl = FcdRetrieveRequestWithTime() } // 5
                );
        }

        internal static void RunPerformanceTests(Options options)
        {
            var test = new KvGPerformanceTest
            {
                RelativeUrl = FcdRetrieveRequestWithTime(),
                IntervalTime = Options.PerformanceTestInterval,
                TestDuration = Options.PerformanceTestDuration,
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
                    ShortDescription = "Update interval " + FcdRetrieveRequest(),
                    Status = TestResult.OK
                }.ToString());
            }
            else
            {
                Out.Info(new GenericTestResult
                {
                    SubTest = "Performance",
                    StepNr = 2,
                    ShortDescription = "Update interval " + FcdRetrieveRequest(),
                    Status = TestResult.FAIL,
                    CauseOfFailure = "Generation time did not update properly."
                }.ToString());
            }
        }

        internal static void RunContinuityTests(Options options)
        {
            var test = new KvGPerformanceTest
            {
                RelativeUrl = FcdRetrieveRequest(),
                IntervalTime = Options.PerformanceTestInterval,
                TestDuration = Options.PerformanceTestDuration
            };
            TestResultBase result1 = test.Test(options);
            result1.StepNr = 1;
            result1.SubTest = "Continuity";
            Out.Info(result1.ToString());
        }


        private static string FcdRetrieveRequest()
        {
            return "/fcd";
        }

        private static string FcdRetrieveRequestWithTime()
        {
            return "/fcd?last_message_time=0";
        }





    }
}