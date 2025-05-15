using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[Serializable]
public class LarkConfig
{
    public string app_id;
    public string app_secret;
    public string app_token;
    public string table_id;
}



public static class LarkConfigUtil
{
    /// <summary>
    /// 解密并加载 LarkConfig
    /// </summary>
    /// <param name="encFilePath">加密文件路径（绝对路径或 StreamingAssets 下的相对路径）</param>
    /// <param name="password">解密密码</param>
    /// <returns>解密后的 LarkConfig 对象，失败返回 null</returns>
    public static LarkConfig LoadConfig(string encFilePath, string password)
    {
        string path = encFilePath;
        if (!File.Exists(path))
        {
            Debug.LogError("找不到加密配置文件: " + path);
            return null;
        }

        byte[] encryptedData = File.ReadAllBytes(path);
        string json = DecryptAes(encryptedData, password);
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("解密失败，请检查密码是否正确！");
            return null;
        }
        return JsonUtility.FromJson<LarkConfig>(json);
    }

    /// <summary>
    /// AES 解密
    /// </summary>
    static string DecryptAes(byte[] cipherTextCombined, string password)
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
