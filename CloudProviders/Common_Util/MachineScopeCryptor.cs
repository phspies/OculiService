using System;
using System.Security.Cryptography;
using System.Text;

public class MachineScopeCryptor
{
    private byte[] Entropy;

    public MachineScopeCryptor()
    {
        this.Entropy = new byte[12]
        {
      (byte) 1,
      (byte) 1,
      (byte) 2,
      (byte) 3,
      (byte) 5,
      (byte) 8,
      (byte) 13,
      (byte) 21,
      (byte) 34,
      (byte) 55,
      (byte) 89,
      (byte) 144
        };
    }

    public MachineScopeCryptor(string entropy)
    {
        this.Entropy = Encoding.Unicode.GetBytes(entropy);
    }

    public string Encrypt(string msg)
    {
        try
        {
            return Convert.ToBase64String(ProtectedData.Protect(Encoding.Unicode.GetBytes(msg), this.Entropy, DataProtectionScope.LocalMachine));
        }
        catch (CryptographicException ex)
        {
            throw new EsxException("Error encrypting password", (Exception)ex, false);
        }
    }

    public string Decrypt(string msg)
    {
        try
        {
            return Encoding.Unicode.GetString(ProtectedData.Unprotect(Convert.FromBase64String(msg), this.Entropy, DataProtectionScope.LocalMachine));
        }
        catch (CryptographicException ex)
        {
            throw new EsxException("Error decrypting password", (Exception)ex, false);
        }
    }
}
