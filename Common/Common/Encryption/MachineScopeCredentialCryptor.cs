using System.Net;

namespace OculiService.Common.Encryption
{
  public class MachineScopeCredentialCryptor : ICredentialCryptor
  {
    public NetworkCredential Encrypt(NetworkCredential plain)
    {
      return plain.EncryptMachineScope();
    }

    public NetworkCredential Decrypt(NetworkCredential cipher)
    {
      return cipher.DecryptMachineScope();
    }
  }
}
