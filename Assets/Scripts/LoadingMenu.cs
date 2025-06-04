using System;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour
{
    public TMP_Text text;
    public Button button;

    void Awake()
    {
        if (PlayerPrefs.HasKey("HighScore"))
        {
            PlayerPrefs.SetString("HighScoreV2", Math.Max(PlayerPrefs.GetInt("HighScore"), 0).ToString());
            PlayerPrefs.DeleteKey("HighScore");
        }
        Application.targetFrameRate = 360;
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("Setting5", 1);
        Screen.fullScreen = PlayerPrefs.GetInt("Setting1", 1) == 1;
        if (!Application.isMobilePlatform)
        {
            SetIfNone("Setting1", 1);
            SetIfNone("Setting2", 0);
            SetIfNone("Setting3", 0);
            SetIfNone("Setting4", 0);
            SetIfNone("Setting5", 1);
        }
        else
        {
            SetIfNone("Setting1", 1, true);
            SetIfNone("Setting2", 1, true);
            SetIfNone("Setting3", 0);
            SetIfNone("Setting4", 0);
            SetIfNone("Setting5", 0, true);
            QualitySettings.vSyncCount = 0;
        }
        PlayerPrefs.SetString("latestVersion", Application.version);
        button.onClick.AddListener(() =>
        {
            Application.OpenURL("https://berrydash.lncvrt.xyz/download");
        });
    }

    void Start()
    {
        CheckUpdate();
    }

    async void CheckUpdate()
    {
        using UnityWebRequest request = UnityWebRequest.Get("https://berrydash.lncvrt.xyz/database/canLoadClient.php");
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            text.text = "Failed to check version";
            return;
        }
        string response = request.downloadHandler.text;
        if (response == "1")
        {
            await SceneManager.LoadSceneAsync("MainMenu");
        } else
        {
            text.text = "Outdated client! Please update your client to play";
            button.gameObject.SetActive(true);
        }
    }

    void SetIfNone(string key, int value, bool overrideValue = false)
    {
        if (!PlayerPrefs.HasKey(key) || overrideValue)
        {
            PlayerPrefs.SetInt(key, value);
        }
    }
}
