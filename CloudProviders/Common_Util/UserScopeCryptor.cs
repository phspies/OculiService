using System;
using System.Security.Cryptography;
using System.Text;

public class UserScopeCryptor
{
    private byte[] Entropy;

    public UserScopeCryptor()
    {
        this.Entropy = new byte[12]
        {
      (byte) 1,
      (byte) 2,
      (byte) 4,
      (byte) 7,
      (byte) 11,
      (byte) 16,
      (byte) 22,
      (byte) 29,
      (byte) 37,
      (byte) 46,
      (byte) 56,
      (byte) 67
        };
    }

    public UserScopeCryptor(string entropy)
    {
        this.Entropy = Encoding.Unicode.GetBytes(entropy);
    }

    public string Encrypt(string msg)
    {
        string str = (string)null;
        try
        {
            str = Convert.ToBase64String(ProtectedData.Protect(Encoding.Unicode.GetBytes(msg), this.Entropy, DataProtectionScope.CurrentUser));
        }
        catch (CryptographicException ex)
        {
        }
        return str;
    }

    public string Decrypt(string msg)
    {
        string str = (string)null;
        try
        {
            str = Encoding.Unicode.GetString(ProtectedData.Unprotect(Convert.FromBase64String(msg), this.Entropy, DataProtectionScope.CurrentUser));
        }
        catch (CryptographicException ex)
        {
        }
        return str;
    }
}
