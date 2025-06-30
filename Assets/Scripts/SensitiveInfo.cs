using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SensitiveInfo
{
    public static readonly string SERVER_DATABASE_PREFIX = "https://berrydash.lncvrt.xyz/database/";
    public static readonly string SERVER_RECEIVE_TRANSFER_KEY = "";
    public static readonly string SERVER_SEND_TRANSFER_KEY = "";

    public static string Encrypt(string plainText, string key)
    {
        if (SERVER_RECEIVE_TRANSFER_KEY.Trim() == string.Empty || SERVER_SEND_TRANSFER_KEY.Trim() == string.Empty) return null;
        try
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
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
        catch
        {
            Application.Quit();
            return "-997"; //the server returns this if theres an issue with encryption/decryption so the client will too ig
        }
    }

    public static string Decrypt(string dataB64, string key)
    {
        if (SERVER_RECEIVE_TRANSFER_KEY.Trim() == string.Empty || SERVER_SEND_TRANSFER_KEY.Trim() == string.Empty)
        {
            return "-996";
        }
        try
        {
            var data = Convert.FromBase64String(dataB64);
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
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
        catch
        {
            Application.Quit();
            return "-997"; //the server returns this if theres an issue with encryption/decryption so the client will too ig
        }
    }
}