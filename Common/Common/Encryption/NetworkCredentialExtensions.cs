using System;using System.Net;

namespace OculiService.Common.Encryption
{
  public static class NetworkCredentialExtensions
  {
    public static NetworkCredential EncryptUserScope(this NetworkCredential toEncrypt)
    {
      return toEncrypt.ApplyCryptor(new Func<string, string>(Cryptor.Encrypt));
    }

    public static NetworkCredential DecryptUserScope(this NetworkCredential toDecrypt)
    {
      return toDecrypt.ApplyCryptor(new Func<string, string>(Cryptor.Decrypt));
    }

    public static NetworkCredential EncryptMachineScope(this NetworkCredential toEncrypt)
    {
      return toEncrypt.ApplyCryptor(new Func<string, string>(Cryptor.EncryptMachineScope));
    }

    public static NetworkCredential DecryptMachineScope(this NetworkCredential toDecrypt)
    {
      return toDecrypt.ApplyCryptor(new Func<string, string>(Cryptor.DecryptMachineScope));
    }

    public static NetworkCredential EncryptTripleDES(this NetworkCredential toEncrypt)
    {
      return toEncrypt.ApplyCryptor(new Func<string, string>(Cryptor.EncryptTripleDES));
    }

    public static NetworkCredential DecryptTripleDES(this NetworkCredential toDecrypt)
    {
      return toDecrypt.ApplyCryptor(new Func<string, string>(Cryptor.DecryptTripleDES));
    }

    public static NetworkCredential ApplyCryptor(this NetworkCredential toEncrypt, Func<string, string> cryptor)
    {
      Invariant.ArgumentNotNull((object) toEncrypt, "toEncrypt");
      Invariant.ArgumentNotNull((object) cryptor, "cryptor");
      return new NetworkCredential() { Domain = toEncrypt.Domain, Password = cryptor(toEncrypt.Password), UserName = toEncrypt.UserName };
    }
  }
}
