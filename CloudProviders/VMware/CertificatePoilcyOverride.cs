using OculiService.Common.Diagnostics;
using OculiService.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace OculiService.CloudProviders.VMware
{
    public class CertificatePoilcyOverride : IDisposable
    {
        private static readonly Tracer _tracer = Tracer.GetTracer(typeof(CertificatePoilcyOverride));
        private readonly ILogger _logger;
        private readonly RemoteCertificateValidationCallback _callback;

        public X509Certificate2 FailedCertificate { get; set; }

        public CertificatePoilcyOverride(VMwareCertificatePolicy policy, Func<object, bool> filterSender, ILogger logger)
        {
            CertificatePoilcyOverride certificatePoilcyOverride = this;
            this._logger = logger;
            this._callback = (RemoteCertificateValidationCallback)((sender, certificate, chain, errors) => certificatePoilcyOverride.ProcessCertCallback(sender, certificate, chain, errors, policy, filterSender));
            ServicePointManager.ServerCertificateValidationCallback += this._callback;
        }

        public void Dispose()
        {
            ServicePointManager.ServerCertificateValidationCallback -= this._callback;
        }

        private bool ProcessCertCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors, VMwareCertificatePolicy policyCert, Func<object, bool> filterSender)
        {
            CertificatePoilcyOverride._tracer.TraceInformation("Processing certificate {0} for URI {1}", (object)certificate, (object)(sender is HttpWebRequest ? ((WebRequest)sender).RequestUri.ToString() : "(unknown)"));
            if (!filterSender(sender))
            {
                CertificatePoilcyOverride._tracer.TraceInformation("Callback is not intended to process the certificate: {0}", (object)certificate.Subject);
                return sslPolicyErrors == SslPolicyErrors.None;
            }
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                CertificatePoilcyOverride._tracer.TraceInformation("Certificate {0} is valid", (object)certificate.Subject);
                return true;
            }
            IEnumerable<X509ChainStatus> array = (IEnumerable<X509ChainStatus>)((chain != null ? (IEnumerable<X509ChainStatus>)chain.ChainStatus : (IEnumerable<X509ChainStatus>)(X509ChainStatus[])null) ?? Enumerable.Empty<X509ChainStatus>()).Where<X509ChainStatus>((Func<X509ChainStatus, bool>)(s => (uint)s.Status > 0U)).ToArray<X509ChainStatus>();
            this._logger.FormatWarning("Issues with certificate {0}: {1}:{2}{3}", (object)certificate, (object)sslPolicyErrors, (object)Environment.NewLine, (object)string.Join(Environment.NewLine, array.Select<X509ChainStatus, string>((Func<X509ChainStatus, string>)(s => s.StatusInformation))));
            switch (policyCert)
            {
                case VMwareCertificatePolicy.AllowAll:
                    this._logger.FormatInformation("Allowing certificate {0}", (object)certificate.Subject);
                    return true;
                case VMwareCertificatePolicy.AllowSelfSigned:
                    if (SslPolicyErrors.RemoteCertificateChainErrors == sslPolicyErrors)
                    {
                        IEnumerable<X509ChainStatus> source = array;
                        Func<X509ChainStatus, bool> func = (Func<X509ChainStatus, bool>)(e => (X509ChainStatusFlags.UntrustedRoot | X509ChainStatusFlags.PartialChain) == (e.Status | X509ChainStatusFlags.UntrustedRoot | X509ChainStatusFlags.PartialChain));
                        Func<X509ChainStatus, bool> predicate;
                        if (source.All<X509ChainStatus>(predicate))
                        {
                            this._logger.FormatInformation("Allowing self-signed certificate {0}", (object)certificate.Subject);
                            return true;
                        }
                    }
                    this.FailedCertificate = new X509Certificate2(certificate);
                    return false;
                case VMwareCertificatePolicy.AllowKnownOrValid:
                    if (CertificatePoilcyOverride.IsCertInstalled(new X509Certificate2(certificate)))
                    {
                        this._logger.FormatInformation("Allowing known certificate {0}", (object)certificate.Subject);
                        return true;
                    }
                    this.FailedCertificate = new X509Certificate2(certificate);
                    return false;
                default:
                    this.FailedCertificate = new X509Certificate2(certificate);
                    return false;
            }
        }

        private static bool IsCertInstalled(X509Certificate2 certificate)
        {
            X509Store certificateStore = VCService.GetCertificateStore();
            try
            {
                certificateStore.Open(OpenFlags.ReadOnly);
                return certificateStore.Certificates.Contains(certificate);
            }
            finally
            {
                certificateStore.Close();
            }
        }
    }
}
