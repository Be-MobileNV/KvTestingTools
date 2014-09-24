using System.Collections.Generic;
using CommandLine;

namespace Spookfiles.Testing.Common
{
    public class Options
    {
        public const string DefaultApiKeyHeaderName = "ApiKey";
        public const bool AllowValidSelfSignedCertificates = true;
        public const string InvalidApiKey = "nowaythisisavalidkey";
        public const string InvalidUserName = "wronguser";
        public const string InvalidPassword = "wrongpass";
        public const string DefaultSegmentCsv = "1662180";
        public const string DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.fff";

#if DEBUG
        /// <summary>
        ///     Performance duration, in seconds
        /// </summary>
        public const int PerformanceTestDuration = 8;

        /// <summary>
        ///     Performance test interval in milliseconds
        /// </summary>
        public const int PerformanceTestInterval = 15000;
#else
    /// <summary>
    /// Performance duration, in seconds
    /// </summary>
        public const int PerformanceTestDuration = 300;
        /// <summary>
        /// Performance test interval in milliseconds
        /// </summary>
        public const int PerformanceTestInterval = 30 * 1000;
#endif

#if DEBUG

        /// <summary>
        ///     Performance duration, in seconds
        /// </summary>
        public const int ContinuityTestDuration = 600;

        /// <summary>
        ///     Performance test interval in milliseconds
        /// </summary>
        public const int ContinuityTestInterval = 5000;
#else
    /// <summary>
    /// Performance duration, in seconds
    /// </summary>
        public const int ContinuityTestDuration = 3600;
        /// <summary>
        /// Performance test interval in milliseconds
        /// </summary>
        public const int ContinuityTestInterval = 30 * 1000;
#endif

#if DEBUG
        /// <summary>
        /// History calls: Set number of minutes back in time for start datetime
        /// </summary>
        public const int HistoryStartBackInTime = 30;
        /// <summary>
        /// History calls: Set number of minutes back in time for end datetime
        /// </summary>
        public const int HistoryEndBackInTime = 15;
#else
        /// <summary>
        /// History calls: Set number of minutes back in time for start datetime
        /// </summary>
        public const int HistoryStartBackInTime = 30;
        /// <summary>
        /// History calls: Set number of minutes back in time for end datetime
        /// </summary>
        public const int HistoryEndBackInTime = 15;
#endif

        [Option("Connectivity", DefaultValue = false, HelpText = "Run the connectivity tests")]
        public bool Connectivity { get; set; }

        [Option("Functionality", DefaultValue = false, HelpText = "Run the functionality tests")]
        public bool Functionality { get; set; }

        [Option("Security", DefaultValue = false, HelpText = "Run the security tests")]
        public bool Security { get; set; }

        [Option("Performance", DefaultValue = false, HelpText = "Run the performance tests")]
        public bool Performance { get; set; }

        [Option("Continuity", DefaultValue = false, HelpText = "Run the continuity tests")]
        public bool Continuity { get; set; }

        [Option("All", DefaultValue = false, HelpText = "Run all tests")]
        public bool All { get; set; }

        [Option("Url", Required = true, HelpText = "The base url of the host to check/connect to")]
        public string Url { get; set; }

        [Option("ApiKey", HelpText = "The actual api key")]
        public string ApiKey { get; set; }

        [Option("ApiKeyHeader",
            HelpText =
                "The header name of the api key. By default the program will try to use " + DefaultApiKeyHeaderName)]
        public string ApiKeyHeader { get; set; }

        [Option("User",
            HelpText = "The username to use for basic Auth. Note: you can combine this with api key if you want.")]
        public string User { get; set; }

        [Option("Pass", HelpText = "The pass to use for basic Auth")]
        public string Pass { get; set; }

        [Option("SegmentCsv", HelpText = "CommaSeperated list of segment ids to test")]
        public string SegmentCsv { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }
}