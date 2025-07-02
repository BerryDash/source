using System;
using System.IO;
using System.Linq;
using System.Net.Security;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class BazookaManager : MonoBehaviour
{
    public static BazookaManager Instance;
    private bool firstLoadDone = false;
    public JObject saveFile = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (!firstLoadDone)
            {
                firstLoadDone = true;
                Load();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save();
        }
    }

    public void Load()
    {
        string path = Path.Join(Application.persistentDataPath, "BazookaManager.dat");
        if (!File.Exists(path))
        {
            File.Create(path).Dispose();
        }
        else
        {
            try
            {
                var tempSaveFile = JObject.Parse(SensitiveInfo.DecryptRaw(File.ReadAllBytes(path), SensitiveInfo.BAZOOKA_MANAGER_KEY));
                if (tempSaveFile != null) saveFile = tempSaveFile;
            }
            catch
            {
                Debug.LogWarning("Failed to load save file");
            }
        }
    }

    public void Save()
    {
        string path = Path.Join(Application.persistentDataPath, "BazookaManager.dat");
        var encoded = SensitiveInfo.EncryptRaw(saveFile.ToString(Newtonsoft.Json.Formatting.None), SensitiveInfo.BAZOOKA_MANAGER_KEY);
        if (encoded == null) return;
        using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        fileStream.Write(encoded, 0, encoded.Length);
        fileStream.Flush(true);
    }
}
