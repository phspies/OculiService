using System.Net;

namespace OculiService.Common.Encryption
{
    public interface ICredentialCryptor
    {
        NetworkCredential Encrypt(NetworkCredential plain);

        NetworkCredential Decrypt(NetworkCredential cipher);
    }
}
