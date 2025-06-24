using System;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour
{
    public TMP_Text text;
    public Button updateButton;
    public Button continueButton;

    void Awake()
    {
        if (PlayerPrefs.GetString("latestVersion", Application.version) == "1.4.0-beta1")
        {
            PlayerPrefs.DeleteKey("Setting2");
            PlayerPrefs.DeleteKey("Setting3");
            PlayerPrefs.SetInt("Setting2", PlayerPrefs.GetInt("Setting4", 0));
            PlayerPrefs.SetInt("Setting3", PlayerPrefs.GetInt("Setting5", 0));
            PlayerPrefs.SetString("latestVersion", "1.4.0");
        }
        if (PlayerPrefs.HasKey("HighScore"))
        {
            PlayerPrefs.SetString("HighScoreV2", Math.Max(PlayerPrefs.GetInt("HighScore"), 0).ToString());
            PlayerPrefs.DeleteKey("HighScore");
        }
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("Setting3", 1) == 1 ? 1 : -1;
        if (!Application.isMobilePlatform && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            SetIfNone("Setting1", 1);
            SetIfNone("Setting2", 0);
            SetIfNone("Setting3", 1);
            SetIfNone("Setting4", 0);
            Screen.fullScreen = PlayerPrefs.GetInt("Setting1", 1) == 1;
        }
        else
        {
            SetIfNone("Setting1", 1, true);
            SetIfNone("Setting2", 0);
            SetIfNone("Setting3", 1, true);
            SetIfNone("Setting4", 0);
            Application.targetFrameRate = 360;
            QualitySettings.vSyncCount = 0;
            if (Application.platform == RuntimePlatform.WebGLPlayer) {
                Screen.fullScreen = false;
            }
        }
        PlayerPrefs.SetString("latestVersion", Application.version);
        updateButton.onClick.AddListener(() =>
        {
            Application.OpenURL("https://berrydash.lncvrt.xyz/download");
        });
        continueButton.onClick.AddListener(async () =>
        {
            await SceneManager.LoadSceneAsync("MainMenu");
        });
    }

    void Start()
    {
        CheckUpdate();
    }

    async void CheckUpdate()
    {
        string response;
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            using UnityWebRequest request = UnityWebRequest.Get(SensitiveInfo.SERVER_DATABASE_PREFIX + "canLoadClient.php");
            request.SetRequestHeader("Requester", "BerryDashClient");
            request.SetRequestHeader("ClientVersion", Application.version);
            request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                text.text = "Failed to check version";
                return;
            }
            response = request.downloadHandler.text;
        }
        else
        {
            response = "1";
        }
        if (response == "1")
        {
            await SceneManager.LoadSceneAsync("MainMenu");
        }
        else if (response == "2")
        {
            text.text = "Outdated client! You can still play the game and access the servers, but it isn't recommended.";

            var updateButtonPos = updateButton.transform.localPosition;
            updateButtonPos.x = -135;
            updateButton.transform.localPosition = updateButtonPos;

            updateButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(true);
        }
        else if (response == "3")
        {
            text.text = "Outdated client! You can still load into the game, but online features may not be available.";

            var updateButtonPos = updateButton.transform.localPosition;
            updateButtonPos.x = -135;
            updateButton.transform.localPosition = updateButtonPos;

            updateButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(true);
        }
        else
        {
            text.text = "Outdated client! Please update your client to play";
            updateButton.gameObject.SetActive(true);
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