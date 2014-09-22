using System;
using System.Collections.Generic;
using System.Reflection;
using CommandLine;
using CommandLine.Text;
using log4net;
using Spookfiles.Testing.Common;
using Spookfiles.Testing.Testrunners;

namespace Spookfiles.Testing.KvA.CLI
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static void Main(string[] args)
        {
#if DEBUG
            //args = new[]
            //{
            //    "--all",
            //    "--url", "http://yourtesturl",
            //    "--apikey", "MyFamousApiKey",
            //    "--user", "TheOneAndOnlyKvAUser",
            //    "--pass", "MyVeryHardToGuesspassword"
            //};

#endif
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                var stuffToTest = new Queue<Action<Options>>(5);
                if (options.All)
                {
                    stuffToTest.Enqueue(Tester.RunConnectivityTests);
                    stuffToTest.Enqueue(Tester.RunFunctionalityTests);
                    stuffToTest.Enqueue(Tester.RunSecurityTests);
                    stuffToTest.Enqueue(Tester.RunPerformanceTests);
                    stuffToTest.Enqueue(Tester.RunContinuityTests);
                }
                else
                {
                    // check for each option individually
                    if (options.Connectivity) stuffToTest.Enqueue(Tester.RunConnectivityTests);
                    if (options.Functionality) stuffToTest.Enqueue(Tester.RunFunctionalityTests);
                    if (options.Security) stuffToTest.Enqueue(Tester.RunSecurityTests);
                    if (options.Performance) stuffToTest.Enqueue(Tester.RunPerformanceTests);
                    if (options.Continuity) stuffToTest.Enqueue(Tester.RunContinuityTests);
                }
                if (stuffToTest.Count > 0)
                {
                    Out.Info(TestResultBase.WriteHeader());
                    foreach (var methodPointer in stuffToTest)
                        methodPointer(options);
                }
            }
            else
            {
                HelpText txt = HelpText.AutoBuild(options);
                Out.Info(txt.ToString());
            }
        }
    }
}