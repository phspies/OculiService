using System.Net;

namespace OculiService.Common.Encryption
{
  public class TripleDESCredentialCryptor : ICredentialCryptor
  {
    public NetworkCredential Encrypt(NetworkCredential plain)
    {
      return plain.EncryptTripleDES();
    }

    public NetworkCredential Decrypt(NetworkCredential cipher)
    {
      return cipher.DecryptTripleDES();
    }
  }
}
