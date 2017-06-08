using System;using System.Security.Cryptography;
using System.Text;

namespace OculiService.Common
{
  internal class TripleDesCryptor : IDisposable
  {
    private static byte[] Hash = new byte[24]{ (byte) 161, (byte) 81, (byte) 31, (byte) 40, (byte) 55, (byte) 73, (byte) 227, (byte) 98, (byte) 99, (byte) 193, (byte) 22, (byte) 133, (byte) 160, (byte) 11, (byte) 10, (byte) 240, (byte) 21, (byte) 82, (byte) 93, (byte) 131, (byte) 207, (byte) 101, (byte) 236, (byte) 128 };
    private static byte[] IV = new byte[8]{ (byte) 178, (byte) 180, (byte) 182, (byte) 134, (byte) 149, (byte) 159, (byte) 11, (byte) 222 };
    private TripleDESCryptoServiceProvider _tripleDesProvider;
    private ICryptoTransform _encryptor;
    private ICryptoTransform _decryptor;

    public TripleDesCryptor()
    {
      this._tripleDesProvider = new TripleDESCryptoServiceProvider();
      this._tripleDesProvider.Key = TripleDesCryptor.Hash;
      this._tripleDesProvider.IV = TripleDesCryptor.IV;
      this._tripleDesProvider.Mode = CipherMode.CBC;
      this._tripleDesProvider.Padding = PaddingMode.Zeros;
      this._encryptor = this._tripleDesProvider.CreateEncryptor();
      this._decryptor = this._tripleDesProvider.CreateDecryptor();
    }

    public string Encrypt(string toEncrypt)
    {
      byte[] inArray = this.Encrypt(Encoding.Unicode.GetBytes(toEncrypt));
      char[] outArray = new char[(int) (Math.Ceiling((double) inArray.Length / 3.0) * 4.0)];
      Convert.ToBase64CharArray(inArray, 0, inArray.Length, outArray, 0);
      return new string(outArray);
    }

    public string Decrypt(string toDecrypt)
    {
      if (toDecrypt == string.Empty)
        return toDecrypt;
      char[] charArray = toDecrypt.ToCharArray();
      return Encoding.Unicode.GetString(this.Decrypt(Convert.FromBase64CharArray(charArray, 0, charArray.Length))).TrimEnd(new char[1]);
    }

    public byte[] Encrypt(byte[] bytesToEncrypt)
    {
      return this._encryptor.TransformFinalBlock(bytesToEncrypt, 0, bytesToEncrypt.Length);
    }

    public byte[] Decrypt(byte[] bytesToDecrypt)
    {
      return this._decryptor.TransformFinalBlock(bytesToDecrypt, 0, bytesToDecrypt.Length);
    }

    public void Dispose()
    {
      if (this._tripleDesProvider == null)
        return;
      this._tripleDesProvider.Clear();
    }
  }
}
