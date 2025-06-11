using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class SensitiveInfo
{
    public static readonly string SERVER_DATABASE_PREFIX = "https://berrydash.lncvrt.xyz/database/";
    private static readonly string SERVER_TRANSFER_KEY = "";

    public static string Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(SERVER_TRANSFER_KEY);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.GenerateIV();

        using MemoryStream ms = new();
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cryptoStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        using (var writer = new StreamWriter(cryptoStream))
        {
            writer.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public static string Decrypt(string dataB64)
    {
        var data = Convert.FromBase64String(dataB64);
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(SERVER_TRANSFER_KEY);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        byte[] iv = new byte[16];
        Array.Copy(data, 0, iv, 0, iv.Length);
        aes.IV = iv;

        using MemoryStream ms = new(data, iv.Length, data.Length - iv.Length);
        using var cryptoStream = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using StreamReader reader = new(cryptoStream);

        return reader.ReadToEnd();
    }
}