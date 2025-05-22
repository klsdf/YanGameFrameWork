using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

/// <summary>
/// 这个类用于管理API密钥的加密和解密。
/// </summary>
public static class ApiKeyManager
{
    /// <summary>
    /// 加密API密钥并保存到文件。
    /// </summary>
    /// <param name="apiKey">要加密的API密钥。</param>
    /// <param name="filePath">保存加密密钥的文件路径。</param>
    /// <param name="password">用于加密的密码。</param>
    public static void EncryptAndSaveApiKey(string apiKey, string filePath, string password)
    {
        byte[] encryptedData = EncryptAes(apiKey, password);
        File.WriteAllBytes(filePath, encryptedData);
    }

    /// <summary>
    /// 从文件中加载并解密API密钥。
    /// </summary>
    /// <param name="filePath">保存加密密钥的文件路径。</param>
    /// <param name="password">用于解密的密码。</param>
    /// <returns>解密后的API密钥。</returns>
    public static string LoadAndDecryptApiKey(string filePath, string password)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("找不到加密密钥文件: " + filePath);
            return null;
        }

        byte[] encryptedData = File.ReadAllBytes(filePath);
        return DecryptAes(encryptedData, password);
    }

    /// <summary>
    /// 使用AES加密字符串。
    /// </summary>
    private static byte[] EncryptAes(string plainText, string password)
    {
        using Aes aesAlg = Aes.Create();
        using var sha = SHA256.Create();
        aesAlg.Key = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        aesAlg.GenerateIV();

        using var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        using var msEncrypt = new MemoryStream();
        msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using var swEncrypt = new StreamWriter(csEncrypt);
        swEncrypt.Write(plainText);
        swEncrypt.Close();
        return msEncrypt.ToArray();
    }

    /// <summary>
    /// 使用AES解密字节数组。
    /// </summary>
    private static string DecryptAes(byte[] cipherTextCombined, string password)
    {
        try
        {
            using Aes aesAlg = Aes.Create();
            using var sha = SHA256.Create();
            aesAlg.Key = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

            byte[] iv = new byte[16];
            Array.Copy(cipherTextCombined, 0, iv, 0, iv.Length);

            using var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv);
            using var msDecrypt = new MemoryStream(cipherTextCombined, 16, cipherTextCombined.Length - 16);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
        catch (Exception e)
        {
            Debug.LogError("解密异常: " + e.Message);
            return null;
        }
    }
}
