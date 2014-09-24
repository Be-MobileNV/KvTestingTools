using System;
using System.Collections.Generic;
using System.Linq;
using Spookfiles.Koppelvlak.A;
using Spookfiles.Testing.Common;
using Spookfiles.Testing.Testrunners;
using Spookfiles.Testing.Testrunners.Connectivity;
using Spookfiles.Testing.Testrunners.Functionality;
using Spookfiles.Testing.Testrunners.Security;

namespace Spookfiles.Testing.KvA.CLI
{
    public class Tester : RunTesterBase
    {
        internal static void RunConnectivityTests(Options options)
        {
            RunTests(options, "Connectivity", Out.Info,
                new PingTest(),
                new TcpConnectionTest(),
                new HttpServiceOnlineTest(),
                new HttpResponseValidTest { RelativeUrl = GetTrafficStateLatest() });
        }

        internal static void RunFunctionalityTests(Options options)
        {
            RunTests(options, "Functionality", Out.Info,
                new HttpResponseValidTest { RelativeUrl = GetTrafficStateLatest() }, // traffic state / latest
                new CheckCompletenessResponseTest
                {
                    RelativeUrl = GetTrafficStateLatest(),
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInTrafficState()
                },
                new SanityCheck()
                {
                    RelativeUrl = GetTrafficStateLatest(),
                    TypeToDeserialize = typeof(List<Trafficstate>),
                    CheckValidDataInsideFunctionHandler = o =>
                    {
                        foreach (var i in (List<Trafficstate>)o)
                        {
                            if (i.publication_time.ToUniversalTime() > DateTime.UtcNow)
                                return false;
                            if (i.speed >= 150 || i.speed < 0)
                                return false;
                            if (i.max_speed > 130 || i.max_speed < 0)
                                return false;
                            if (i.freeflow_speed > 130 || i.freeflow_speed < 0)
                                return false;
                        }
                        return true;
                    }
                },

                new HttpResponseValidTest { RelativeUrl = GetTrafficStateHistoric() }, // traffic state historic
                new CheckCompletenessResponseTest
                {
                    RelativeUrl = GetTrafficStateHistoric(),
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInTrafficState()
                },
                new SanityCheck()
                {
                    RelativeUrl = GetTrafficStateHistoric(),
                    TypeToDeserialize = typeof(List<Trafficstate>),
                    CheckValidDataInsideFunctionHandler = o =>
                    {
                        foreach (var i in (List<Trafficstate>)o)
                        {
                            if (i.publication_time.ToUniversalTime() > DateTime.UtcNow)
                                return false;
                            if (i.speed >= 150 || i.speed < 0)
                                return false;
                            if (i.max_speed > 130 || i.max_speed < 0)
                                return false;
                            if (i.freeflow_speed > 130 || i.freeflow_speed < 0)
                                return false;
                        }
                        return true;
                    }
                },
                new HttpResponseValidTest { RelativeUrl = GetEventsLatest() }, // Events/latest
                new CheckCompletenessResponseTest
                {
                    RelativeUrl = GetEventsLatest(),
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInEvents()
                },
                new SanityCheck()
                {
                    RelativeUrl = GetEventsLatest(),
                    TypeToDeserialize = typeof(List<TrafficEvent>),
                    CheckValidDataInsideFunctionHandler = o =>
                    {
                        foreach (var i in (List<TrafficEvent>)o)
                        {
                            if (i.start_time.Value.ToUniversalTime() >= DateTime.UtcNow)
                                return false;

                            if (i.event_code.Any(eventCode => eventCode < 1 || eventCode > 255))
                                return false;

                            if (i.start_time > i.expected_end_time)
                                //    // note: if traffic events in the future are allowed, this should also be removed.
                                return false;

                            if (i.expected_end_time < DateTime.UtcNow)
                                return false;
                        }
                        return true;
                    }
                },

                // mandatory: start_time, end_time
                new HttpResponseValidTest { RelativeUrl = GetEventsHistoric() },
                new CheckCompletenessResponseTest
                {
                    RelativeUrl = GetEventsHistoric(),
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInEvents()
                },
                new SanityCheck()
                {
                    RelativeUrl = GetEventsHistoric(),
                    TypeToDeserialize = typeof(List<TrafficEvent>),
                    CheckValidDataInsideFunctionHandler = o =>
                    {
                        foreach (var i in (List<TrafficEvent>)o)
                        {
                            if (i.start_time.Value.ToUniversalTime() >= DateTime.UtcNow)
                                return false;

                            if (i.event_code.Any(eventCode => eventCode < 1 || eventCode > 255))
                                return false;

                            if (i.start_time > i.expected_end_time)
                                //    // note: if traffic events in the future are allowed, this should also be removed.
                                return false;

                            //if (i.expected_end_time < DateTime.UtcNow) -> should be ok for historic
                            //    return false;
                        }
                        return true;
                    }
                },

                new HttpResponseValidTest { RelativeUrl = GetWeatherStateLatest() },
                new CheckCompletenessResponseTest
                {
                    RelativeUrl = GetWeatherStateLatest(),
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInWeatherState()
                },
                new SanityCheck()
                {
                    RelativeUrl = GetWeatherStateLatest(),
                    TypeToDeserialize = typeof(List<Weatherstate>),
                    CheckValidDataInsideFunctionHandler = o =>
                    {
                        foreach (var i in (List<Weatherstate>)o)
                        {
                            if (i.measurement_time.ToUniversalTime() > DateTime.UtcNow)
                                return false;

                            if (i.segment_id.Count == 0)
                                return false;
                        }
                        return true;
                    }
                },

                // mandatory: start_time, end_time
                new HttpResponseValidTest { RelativeUrl = GetWeatherStateHistoric() },
                new CheckCompletenessResponseTest
                {
                    RelativeUrl = GetWeatherStateHistoric(),
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInWeatherState()
                },
                new SanityCheck()
                {
                    RelativeUrl = GetWeatherStateHistoric(),
                    TypeToDeserialize = typeof(List<Weatherstate>),
                    CheckValidDataInsideFunctionHandler = o =>
                    {
                        foreach (var i in (List<Weatherstate>)o)
                        {
                            if (i.measurement_time.ToUniversalTime() > DateTime.UtcNow)
                                return false;

                            if (i.segment_id.Count == 0)
                                return false;
                        }
                        return true;
                    }
                },

                new HttpResponseValidTest { RelativeUrl = GetWeatherForecastLatest() },
                new CheckCompletenessResponseTest
                {
                    RelativeUrl = GetWeatherForecastLatest(),
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInWeatherForecast()
                },
                new SanityCheck()
                {
                    RelativeUrl = GetWeatherForecastLatest(),
                    TypeToDeserialize = typeof(List<Weatherforecast>),
                    CheckValidDataInsideFunctionHandler = o =>
                    {
                        foreach (var i in (List<Weatherforecast>)o)
                        {
                            //if (i.measurement_time.ToUniversalTime() > DateTime.UtcNow)
                            //    return false;

                            if (i.segment_id.Count == 0)
                                return false;

                            //if (i.measurement_time.ToUniversalTime() < DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(15)))
                            //    return false;
                        }
                        return true;
                    }
                },

                // mandatory: start_time, end_time
                new HttpResponseValidTest { RelativeUrl = GetWeatherForecastHistoric() },
                new CheckCompletenessResponseTest
                {
                    RelativeUrl = GetWeatherForecastHistoric(),
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInWeatherForecast()
                },
                new SanityCheck()
                {
                    RelativeUrl = GetWeatherForecastHistoric(),
                    TypeToDeserialize = typeof(List<Weatherforecast>),
                    CheckValidDataInsideFunctionHandler = o =>
                    {
                        foreach (var i in (List<Weatherforecast>)o)
                        {
                            //if (i.measurement_time.ToUniversalTime() > DateTime.UtcNow)
                            //    return false;

                            if (i.segment_id.Count == 0)
                                return false;

                            //if (i.measurement_time.ToUniversalTime() < DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(15)))
                            //    return false;
                        }
                        return true;
                    }
                },

                new HttpResponseValidTest { RelativeUrl = GetSegmentState() },
                new CheckCompletenessResponseTest
                {
                    RelativeUrl = GetSegmentState(),
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInSegmentState()
                },
                new SanityCheck()
                {
                    RelativeUrl = GetSegmentState(),
                    TypeToDeserialize = typeof(List<Segmentstate>),
                    CheckValidDataInsideFunctionHandler = o =>
                    {
                        foreach (var i in (List<Segmentstate>)o)
                        {
                            if (i.update_time.ToUniversalTime() > DateTime.UtcNow)
                                return false;

                            // todo: add more sanity checks here.

                        }
                        return true;
                    }
                },

                // mandatory: start_time, end_time
                new HttpResponseValidTest { RelativeUrl = GetSegmentStateHistoric() },
                new CheckCompletenessResponseTest
                {
                    RelativeUrl = GetSegmentStateHistoric(),
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInSegmentState()
                },
                new SanityCheck()
                {
                    RelativeUrl = GetSegmentStateHistoric(),
                    TypeToDeserialize = typeof(List<Segmentstate>),
                    CheckValidDataInsideFunctionHandler = o =>
                    {
                        foreach (var i in (List<Segmentstate>)o)
                        {
                            if (i.update_time.ToUniversalTime() > DateTime.UtcNow)
                                return false;

                            // todo: add more sanity checks here.

                        }
                        return true;
                    }
                },

                new HttpResponseValidTest { RelativeUrl = GetRoadSegments() },
                new CheckCompletenessResponseTest
                {
                    RelativeUrl = GetRoadSegments(),
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInRoadSegments()
                },
                new SanityCheck()
                {
                    RelativeUrl = GetRoadSegments(),
                    TypeToDeserialize = typeof(List<Roadsegments>),
                    CheckValidDataInsideFunctionHandler = o =>
                    {
                        foreach (var i in (List<Roadsegments>)o)
                        {
                            if (i.max_speed > 130)
                                return false;
                            if (i.length > 100)
                                return false;

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
                    RelativeUrl = GetEventsLatest(),
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInEvents()
                },
                // 1                 // note: this is the same test as runfunctionality - the last one.
                new CheckCertificateTest { RelativeUrl = GetEventsLatest() }, // 2    
                new CallingWithInvalidCredentials
                {
                    RelativeUrl = GetEventsLatest(),
                    UseCredentials = HttpTestBase.AuthenticationMode.UseInvalidCredentials
                }, // 3
                new CallingWithInvalidCredentials
                {
                    RelativeUrl = GetEventsLatest(),
                    UseCredentials = HttpTestBase.AuthenticationMode.UseNoCredentials
                }, // 4
                new CheckHttpAvailableTest { RelativeUrl = GetEventsLatest() } // 5
                );
        }

        internal static void RunPerformanceTests(Options options)
        {
            RunTestsParallel(options, "Performance", Out.Info,
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInTrafficState(),
                    RelativeUrl = GetTrafficStateLatest(),
                    IntervalTime = Options.PerformanceTestInterval,
                    TestDuration = Options.PerformanceTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInTrafficState(),
                    RelativeUrl = GetTrafficStateHistoric(),
                    IntervalTime = Options.PerformanceTestInterval,
                    TestDuration = Options.PerformanceTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInEvents(),
                    RelativeUrl = GetEventsLatest(),
                    IntervalTime = Options.PerformanceTestInterval,
                    TestDuration = Options.PerformanceTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInEvents(),
                    RelativeUrl = GetEventsHistoric(),
                    IntervalTime = Options.PerformanceTestInterval,
                    TestDuration = Options.PerformanceTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInWeatherState(),
                    RelativeUrl = GetWeatherStateLatest(),
                    IntervalTime = Options.PerformanceTestInterval,
                    TestDuration = Options.PerformanceTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInWeatherState(),
                    RelativeUrl = GetWeatherStateHistoric(),
                    IntervalTime = Options.PerformanceTestInterval,
                    TestDuration = Options.PerformanceTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInWeatherForecast(),
                    RelativeUrl = GetWeatherForecastLatest(),
                    IntervalTime = Options.PerformanceTestInterval,
                    TestDuration = Options.PerformanceTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInWeatherForecast(),
                    RelativeUrl = GetWeatherForecastHistoric(),
                    IntervalTime = Options.PerformanceTestInterval,
                    TestDuration = Options.PerformanceTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInSegmentState(),
                    RelativeUrl = GetSegmentState(),
                    IntervalTime = Options.PerformanceTestInterval,
                    TestDuration = Options.PerformanceTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInSegmentState(),
                    RelativeUrl = GetSegmentStateHistoric(),
                    IntervalTime = Options.PerformanceTestInterval,
                    TestDuration = Options.PerformanceTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInRoadSegments(),
                    RelativeUrl = GetRoadSegments(),
                    IntervalTime = Options.PerformanceTestInterval,
                    TestDuration = Options.PerformanceTestDuration
                }
                );
        }

        internal static void RunContinuityTests(Options options)
        {
            RunTestsParallel(options, "Continuity", Out.Info,
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInTrafficState(),
                    RelativeUrl = GetTrafficStateLatest(),
                    IntervalTime = Options.ContinuityTestInterval,
                    TestDuration = Options.ContinuityTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInTrafficState(),
                    RelativeUrl = GetTrafficStateHistoric(),
                    IntervalTime = Options.ContinuityTestInterval,
                    TestDuration = Options.ContinuityTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInEvents(),
                    RelativeUrl = GetEventsLatest(),
                    IntervalTime = Options.ContinuityTestInterval,
                    TestDuration = Options.ContinuityTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInEvents(),
                    RelativeUrl = GetEventsHistoric(),
                    IntervalTime = Options.ContinuityTestInterval,
                    TestDuration = Options.ContinuityTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInWeatherState(),
                    RelativeUrl = GetWeatherStateLatest(),
                    IntervalTime = Options.ContinuityTestInterval,
                    TestDuration = Options.ContinuityTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInWeatherState(),
                    RelativeUrl = GetWeatherStateHistoric(),
                    IntervalTime = Options.ContinuityTestInterval,
                    TestDuration = Options.ContinuityTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInWeatherForecast(),
                    RelativeUrl = GetWeatherForecastLatest(),
                    IntervalTime = Options.ContinuityTestInterval,
                    TestDuration = Options.ContinuityTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInWeatherForecast(),
                    RelativeUrl = GetWeatherForecastHistoric(),
                    IntervalTime = Options.ContinuityTestInterval,
                    TestDuration = Options.ContinuityTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInSegmentState(),
                    RelativeUrl = GetSegmentState(),
                    IntervalTime = Options.ContinuityTestInterval,
                    TestDuration = Options.ContinuityTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInSegmentState(),
                    RelativeUrl = GetSegmentStateHistoric(),
                    IntervalTime = Options.ContinuityTestInterval,
                    TestDuration = Options.ContinuityTestDuration
                },
                new KvAPerformanceTest
                {
                    FieldsThatShouldBePresent = FieldTester.FieldsThatShouldBePresentInRoadSegments(),
                    RelativeUrl = GetRoadSegments(),
                    IntervalTime = Options.ContinuityTestInterval,
                    TestDuration = Options.ContinuityTestDuration
                }
                );
        }

        #region Helpers

        private static string GetStartAndEndTime()
        {
            return string.Format("start_time={0}&end_time={1}",
                //DateTime.UtcNow.AddHours(-4).ToString("O"), 
                //"0001-01-01T01:00:00.0000000",
                DateTime.UtcNow.AddMinutes(-Options.HistoryStartBackInTime).ToString(Options.DateTimeFormat),
                DateTime.UtcNow.AddMinutes(-Options.HistoryEndBackInTime).ToString(Options.DateTimeFormat));
        }

        private static string GetTrafficStateLatest()
        {
            return "/trafficstate/latest";
        }

        private static string GetTrafficStateHistoric()
        {
            return string.Format("/trafficstate/historic?{0}&segment_id={1}", GetStartAndEndTime(), Options.SegmentCSV);
        }

        private static string GetWeatherStateLatest()
        {
            return "/weatherstate/latest";
        }

        private static string GetWeatherStateHistoric()
        {
            return string.Format("/weatherstate/historic?{0}", GetStartAndEndTime());
        }

        private static string GetWeatherForecastLatest()
        {
            return "/weatherforecast/latest";
        }

        private static string GetWeatherForecastHistoric()
        {
            return string.Format("/weatherforecast/historic?{0}", GetStartAndEndTime());
        }

        private static string GetRoadSegments()
        {
            return "/roadsegments";
        }

        private static string GetSegmentState()
        {
            return "/segmentstate/latest";
        }
        
        private static string GetSegmentStateHistoric()
        {
            return string.Format("/segmentstate/historic?{0}", GetStartAndEndTime());
        }

        private static string GetEventsLatest()
        {
            return "/events/latest";
        }

        private static string GetEventsHistoric()
        {
            return string.Format("/events/historic?{0}", GetStartAndEndTime());
        }

        #endregion
    }
}