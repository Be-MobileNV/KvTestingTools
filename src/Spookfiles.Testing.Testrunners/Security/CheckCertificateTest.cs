using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Spookfiles.Testing.Common;

namespace Spookfiles.Testing.Testrunners.Security
{
    public class CheckCertificateTest : HttpTestBase, ITest
    {
        public string RelativeUrl { get; set; }

        public TestResultBase Test(Options o)
        {
            var res = new GenericTestResult
            {
                ShortDescription = "Validity server-side certificate",
                Status = TestResult.OK
            };

            try
            {
                ServicePointManager.MaxServicePoints = 0;
                ServicePointManager.MaxServicePointIdleTime = 0;

                ServicePointManager.ServerCertificateValidationCallback =
                    (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        res.ExtraInformation = certificate.ToString();
                        // CHECK CERTIFICATE

                        // If the certificate is a valid, signed certificate, return true.
                        if (sslPolicyErrors == SslPolicyErrors.None)
                        {
                            res.Status = TestResult.OK;
                            return true;
                        }

                        // If there are errors in the certificate chain, look at each error to determine the cause.
                        if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
                        {
                            if (chain != null && chain.ChainStatus != null)
                            {
                                foreach (
                                    X509ChainStatus status in
                                        chain.ChainStatus)
                                {
                                    if ((certificate.Subject == certificate.Issuer) &&
                                        (status.Status ==
                                         X509ChainStatusFlags
                                             .UntrustedRoot))
                                    {
                                        // Self-signed certificates with an untrusted root are valid. 
                                        if (Options.AllowValidSelfSignedCertificates)
                                            continue;
                                        res.Status = TestResult.FAIL;
                                        res.CauseOfFailure =
                                            "Self signed certificate. While valid - is not approved.";
                                        return false;
                                    }
                                    if (status.Status !=
                                        X509ChainStatusFlags.NoError)
                                    {
                                        // If there are any other errors in the certificate chain, the certificate is invalid,
                                        // so the method returns false.
                                        res.Status = TestResult.FAIL;
                                        res.CauseOfFailure = status.StatusInformation;
                                        return false;
                                    }
                                }
                            }

                            // When processing reaches this line, the only errors in the certificate chain are 
                            // untrusted root errors for self-signed certificates. These certificates are valid
                            // for default Exchange server installations, so return true.
                            if (Options.AllowValidSelfSignedCertificates)
                            {
                                res.Status = TestResult.OK;
                                return true;
                            }
                            res.Status = TestResult.FAIL;
                            res.CauseOfFailure =
                                "Self signed certificate. While valid - is not approved.";
                            return false;
                        }
                        res.Status = TestResult.FAIL;
                        res.CauseOfFailure =
                            "Invalid certificate";
                        return false;
                    };
                HttpWebRequest req = SetupWebRequest(o, o.Url + RelativeUrl);
                var response = (HttpWebResponse) req.GetResponse();
                response.Close();
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