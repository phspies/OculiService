using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;

namespace OculiService.Common.Net
{
    [DataContract]
    public class NetworkConnectionInfo
    {
        private string networkId;
        private int? port;
        private NetworkCredential credential;

        public string NetworkId
        {
            get
            {
                return this.networkId;
            }
            set
            {
                IPAddress address;
                if (IPAddress.TryParse(value, out address) && address.AddressFamily == AddressFamily.InterNetworkV6)
                    value = string.Format((IFormatProvider)CultureInfo.InvariantCulture, "[{0}]", new object[1]
                    {
            (object) value
                    });
                if (!(this.networkId != value))
                    return;
                this.networkId = value;
            }
        }

        public int? Port
        {
            get
            {
                return this.port;
            }
            set
            {
                int? port = this.port;
                int? nullable = value;
                if ((port.GetValueOrDefault() == nullable.GetValueOrDefault() ? (port.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
                    return;
                this.port = value;
            }
        }

        [DataMember]
        public string UserName
        {
            get
            {
                return this.credential.UserName;
            }
            set
            {
                if (!(this.UserName != value))
                    return;
                this.credential.UserName = value;
            }
        }

        [DataMember]
        public string Domain
        {
            get
            {
                return this.credential.Domain;
            }
            set
            {
                if (!(this.credential.Domain != value))
                    return;
                this.credential.Domain = value;
            }
        }

        [DataMember]
        public string Password
        {
            get
            {
                return this.credential.Password;
            }
            set
            {
                if (!(this.credential.Password != value))
                    return;
                this.credential.Password = value;
            }
        }

        public NetworkConnectionInfo()
          : this((string)null, new int?(), (NetworkCredential)null)
        {
            this.credential = CredentialUtils.Empty;
        }

        public NetworkConnectionInfo(string networkId)
          : this(networkId, new int?(), (NetworkCredential)null)
        {
        }

        public NetworkConnectionInfo(string networkId, int? port)
          : this(networkId, port, (NetworkCredential)null)
        {
        }

        public NetworkConnectionInfo(string networkId, int? port, NetworkCredential credential)
        {
            this.networkId = this.NormalizeNetworkId(networkId);
            this.port = port;
            this.credential = credential ?? CredentialUtils.Empty;
        }

        public NetworkCredential GetCredential()
        {
            return this.credential;
        }

        public void SetCredential(NetworkCredential credential)
        {
            this.credential = credential ?? CredentialUtils.Empty;
        }

        private string NormalizeNetworkId(string networkId)
        {
            IPAddress address;
            if (!IPAddress.TryParse(networkId, out address) || address.AddressFamily != AddressFamily.InterNetworkV6)
                return networkId;
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, "[{0}]", new object[1] { (object)address });
        }
    }
}
