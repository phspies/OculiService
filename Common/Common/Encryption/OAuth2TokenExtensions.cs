using OculiService.CloudProviders.Oculi.Contracts;
using System;
using System.Net;

namespace OculiService.Common.Encryption
{
  public static class OculiOAuth2TokenExtensions
  {
    public static OculiOAuth2Token EncryptUserScope(this OculiOAuth2Token toEncrypt)
    {
      return toEncrypt.ApplyCryptor(new Func<string, string>(Cryptor.Encrypt));
    }

    public static OculiOAuth2Token DecryptUserScope(this OculiOAuth2Token toDecrypt)
    {
      return toDecrypt.ApplyCryptor(new Func<string, string>(Cryptor.Decrypt));
    }

    public static OculiOAuth2Token EncryptMachineScope(this OculiOAuth2Token toEncrypt)
    {
      return toEncrypt.ApplyCryptor(new Func<string, string>(Cryptor.EncryptMachineScope));
    }

    public static OculiOAuth2Token DecryptMachineScope(this OculiOAuth2Token toDecrypt)
    {
      return toDecrypt.ApplyCryptor(new Func<string, string>(Cryptor.DecryptMachineScope));
    }

    public static OculiOAuth2Token EncryptTripleDES(this OculiOAuth2Token toEncrypt)
    {
      return toEncrypt.ApplyCryptor(new Func<string, string>(Cryptor.EncryptTripleDES));
    }

    public static OculiOAuth2Token DecryptTripleDES(this OculiOAuth2Token toDecrypt)
    {
      return toDecrypt.ApplyCryptor(new Func<string, string>(Cryptor.DecryptTripleDES));
    }

    public static OculiOAuth2Token ApplyCryptor(this OculiOAuth2Token toEncrypt, Func<string, string> cryptor)
    {
      Invariant.ArgumentNotNull((object) toEncrypt, "toEncrypt");
      Invariant.ArgumentNotNull((object) cryptor, "cryptor");
      return new OculiOAuth2Token() {  accesstoken = cryptor(toEncrypt.accesstoken), uid = toEncrypt.uid, coreengine_id = toEncrypt.coreengine_id };
    }
  }
}
