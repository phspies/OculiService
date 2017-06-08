using OculiService.CloudProviders.Oculi.Contracts;
using System.Net;

namespace OculiService.Common.Encryption
{
  public interface IOAuth2TokenCryptor
    {
        OculiOAuth2Token Encrypt(OculiOAuth2Token plain);

        OculiOAuth2Token Decrypt(OculiOAuth2Token cipher);
  }
}
