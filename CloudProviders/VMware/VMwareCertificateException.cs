using System;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;

namespace OculiService.CloudProviders.VMware
{
    public class VMwareCertificateException : Exception
    {
        private X509Certificate2 _certificate;
        public X509Certificate2 Certificate
        {
            get
            {
                return this._certificate;
            }
        }
        public VMwareCertificateException(X509Certificate2 certificate)
        {
            this._certificate = certificate;
        }
        public VMwareCertificateException(X509Certificate2 certificate, string message) : base(message)
        {
            this._certificate = certificate;
        }

        public VMwareCertificateException(X509Certificate2 certificate, Exception inner) : base(inner != null ? inner.Message : (string)null, inner)
        {
            this._certificate = certificate;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected VMwareCertificateException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
            this._certificate = (X509Certificate2)info.GetValue("Certificate", typeof(X509Certificate2));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            info.AddValue("Certificate", (object)this._certificate, typeof(X509Certificate2));
        }
    }
}
