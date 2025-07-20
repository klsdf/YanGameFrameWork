#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// API密钥加密工具窗口
/// </summary>
public class ApiKeyEncryptor : EditorWindow
{
    private string apiKey = "";
    private string password = "";

    [MenuItem("😁YanGameFrameWork😁/API密钥加密工具")]
    public static void ShowWindow()
    {
        GetWindow<ApiKeyEncryptor>("API密钥加密工具");
    }

    void OnGUI()
    {
        GUILayout.Label("输入API密钥信息", EditorStyles.boldLabel);
        apiKey = EditorGUILayout.TextField("API Key", apiKey);
        password = EditorGUILayout.PasswordField("加密密码", password);

        if (GUILayout.Button("加密并导出"))
        {
            EncryptAndSave();
        }
    }

    /// <summary>
    /// 加密API密钥并保存到文件
    /// </summary>
    private void EncryptAndSave()
    {
        if (string.IsNullOrEmpty(password))
        {
            EditorUtility.DisplayDialog("错误", "加密密码不能为空！", "OK");
            return;
        }

        byte[] encrypted = EncryptStringToBytes_Aes(apiKey, password);

        string dir = Application.dataPath + "/StreamingAssets";
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string path = dir + "/api_key.enc";
        File.WriteAllBytes(path, encrypted);

        EditorUtility.DisplayDialog("完成", "加密文件已生成: " + path, "OK");
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 使用AES加密字符串
    /// </summary>
    private byte[] EncryptStringToBytes_Aes(string plainText, string password)
    {
        using Aes aesAlg = Aes.Create();
        using var sha = SHA256.Create();
        aesAlg.Key = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        aesAlg.GenerateIV();
        var iv = aesAlg.IV;

        using var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv);
        using var msEncrypt = new MemoryStream();
        msEncrypt.Write(iv, 0, iv.Length); // 先写入 IV
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
            swEncrypt.Write(plainText);

        return msEncrypt.ToArray();
    }
}
#endif
