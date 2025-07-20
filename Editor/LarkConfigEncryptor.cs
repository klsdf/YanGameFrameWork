#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;


/// <summary>
/// 飞书配置的加密工具
/// </summary>
public class LarkConfigEncryptor : EditorWindow
{
    string app_id = "";
    string app_secret = "";
    string app_token = "";
    string table_id = "";
    string password = "";

    [MenuItem("😁YanGameFrameWork😁/飞书配置加密工具")]
    public static void ShowWindow()
    {
        GetWindow<LarkConfigEncryptor>("飞书配置加密工具");
    }

    void OnGUI()
    {
        GUILayout.Label("输入飞书配置信息", EditorStyles.boldLabel);
        app_id = EditorGUILayout.TextField("App ID", app_id);
        app_secret = EditorGUILayout.TextField("App Secret", app_secret);
        app_token = EditorGUILayout.TextField("App Token", app_token);
        table_id = EditorGUILayout.TextField("Table ID", table_id);
        password = EditorGUILayout.PasswordField("加密密码", password);

        if (GUILayout.Button("加密并导出"))
        {
            EncryptAndSave();
        }
    }

    void EncryptAndSave()
    {
        if (string.IsNullOrEmpty(password))
        {
            EditorUtility.DisplayDialog("错误", "加密密码不能为空！", "OK");
            return;
        }

        var config = new LarkConfig
        {
            app_id = app_id,
            app_secret = app_secret,
            app_token = app_token,
            table_id = table_id
        };

        string json = JsonConvert.SerializeObject(config, Formatting.Indented);
        byte[] encrypted = EncryptStringToBytes_Aes(json, password);

        string dir = Application.dataPath + "/StreamingAssets";
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string path = dir + "/lark_config.enc";
        File.WriteAllBytes(path, encrypted);

        EditorUtility.DisplayDialog("完成", "加密文件已生成: " + path, "OK");
        AssetDatabase.Refresh();
    }

    byte[] EncryptStringToBytes_Aes(string plainText, string password)
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
