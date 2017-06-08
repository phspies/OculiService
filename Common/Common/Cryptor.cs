using System;
using System.Security.Cryptography;
using System.Text;

namespace OculiService.Common
{
  public static class Cryptor
  {
    private static byte[] _entropy = new byte[11]{ (byte) 1, (byte) 2, (byte) 4, (byte) 9, (byte) 16, (byte) 25, (byte) 36, (byte) 49, (byte) 64, (byte) 81, (byte) 99 };

    public static string Encrypt(string clear)
    {
      return Cryptor.EncryptHelper(clear, Cryptor._entropy, DataProtectionScope.CurrentUser);
    }

    public static byte[] Encrypt(byte[] clear)
    {
      return ProtectedData.Protect(clear, Cryptor._entropy, DataProtectionScope.CurrentUser);
    }

    public static byte[] Decrypt(byte[] encrypted)
    {
      return ProtectedData.Unprotect(encrypted, Cryptor._entropy, DataProtectionScope.CurrentUser);
    }

    public static string Encrypt(string clear, byte[] entropy)
    {
      return Cryptor.EncryptHelper(clear, entropy, DataProtectionScope.CurrentUser);
    }

    public static string Decrypt(string encrypted)
    {
      return Cryptor.DecryptHelper(encrypted, Cryptor._entropy, DataProtectionScope.CurrentUser);
    }

    public static string Decrypt(string encrypted, byte[] entropy)
    {
      return Cryptor.DecryptHelper(encrypted, entropy, DataProtectionScope.CurrentUser);
    }

    private static string EncryptHelper(string clear, byte[] entropy, DataProtectionScope scope)
    {
      return Convert.ToBase64String(ProtectedData.Protect(Encoding.Unicode.GetBytes(clear), entropy, scope));
    }

    private static string DecryptHelper(string encrypted, byte[] entropy, DataProtectionScope scope)
    {
      return Encoding.Unicode.GetString(ProtectedData.Unprotect(Convert.FromBase64String(encrypted), entropy, scope));
    }

    public static string EncryptMachineScope(string clear)
    {
      return Cryptor.EncryptHelper(clear, Cryptor._entropy, DataProtectionScope.LocalMachine);
    }

    public static string DecryptMachineScope(string encrypted)
    {
      return Cryptor.DecryptHelper(encrypted, Cryptor._entropy, DataProtectionScope.LocalMachine);
    }

    public static string EncryptTripleDES(string clear)
    {
      using (TripleDesCryptor tripleDesCryptor = new TripleDesCryptor())
        return tripleDesCryptor.Encrypt(clear);
    }

    public static byte[] EncryptTripleDES(byte[] clear)
    {
      using (TripleDesCryptor tripleDesCryptor = new TripleDesCryptor())
        return tripleDesCryptor.Encrypt(clear);
    }

    public static string DecryptTripleDES(string encrypted)
    {
      using (TripleDesCryptor tripleDesCryptor = new TripleDesCryptor())
        return tripleDesCryptor.Decrypt(encrypted);
    }

    public static byte[] DecryptTripleDES(byte[] encrypted)
    {
      using (TripleDesCryptor tripleDesCryptor = new TripleDesCryptor())
        return tripleDesCryptor.Decrypt(encrypted);
    }
  }
}
