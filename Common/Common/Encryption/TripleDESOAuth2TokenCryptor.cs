using OculiService.CloudProviders.Oculi.Contracts;
using System.Net;

namespace OculiService.Common.Encryption
{
  public class TripleDESOAuth2TokenCryptor : IOAuth2TokenCryptor
    {
    public OculiOAuth2Token Encrypt(OculiOAuth2Token plain)
    {
      return plain.EncryptTripleDES();
    }

    public OculiOAuth2Token Decrypt(OculiOAuth2Token cipher)
    {
      return cipher.DecryptTripleDES();
    }
  }
}
