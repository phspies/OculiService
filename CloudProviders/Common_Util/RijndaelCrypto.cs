using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class RijndaelCrypto
{
    private string _password;

    public RijndaelCrypto(string password)
    {
        this._password = password;
    }

    private RijndaelManaged Initialize()
    {
        Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(this._password, new byte[12] { (byte)1, (byte)1, (byte)2, (byte)3, (byte)5, (byte)8, (byte)13, (byte)21, (byte)34, (byte)55, (byte)89, (byte)144 }, 1000);
        RijndaelManaged rijndaelManaged = new RijndaelManaged();
        rijndaelManaged.KeySize = 256;
        rijndaelManaged.Mode = CipherMode.CBC;
        byte[] bytes = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
        rijndaelManaged.Key = bytes;
        rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(16);
        return rijndaelManaged;
    }

    public string Encrypt(string input)
    {
        try
        {
            RijndaelManaged rijndaelManaged = this.Initialize();
            int keySize = rijndaelManaged.KeySize;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, rijndaelManaged.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] bytes = new UTF8Encoding(false).GetBytes(input);
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cryptoStream.Close();
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException("AES encryption failed: " + ex.Message);
        }
    }

    public string Decrypt(string input)
    {
        try
        {
            RijndaelManaged rijndaelManaged = this.Initialize();
            int keySize = rijndaelManaged.KeySize;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    byte[] buffer = Convert.FromBase64String(input);
                    cryptoStream.Write(buffer, 0, buffer.Length);
                    cryptoStream.FlushFinalBlock();
                    cryptoStream.Close();
                    return new UTF8Encoding(false).GetString(memoryStream.ToArray());
                }
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException("AES decryption failed: " + ex.Message);
        }
    }
}
