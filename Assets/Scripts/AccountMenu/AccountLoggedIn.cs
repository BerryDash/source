using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AccountLoggedIn : MonoBehaviour
{
    public TMP_Text loggedInText;
    public Button loggedInChangeUsernameButton;
    public Button loggedInChangePasswordButton;
    public Button loggedInSaveButton;
    public Button loggedInLoadButton;
    public Button loggedInRefreshLoginButton;
    public Button loggedInLogoutButton;
    public Button loggedInBackButton;

    void Awake()
    {
        loggedInChangeUsernameButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(4));
        loggedInChangePasswordButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(5));
        loggedInSaveButton.onClick.AddListener(() => SaveAccount());
        loggedInLoadButton.onClick.AddListener(() => LoadAccount());
        loggedInRefreshLoginButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(6));
        loggedInLogoutButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(1));
        loggedInBackButton.onClick.AddListener(async () => await SceneManager.LoadSceneAsync("MainMenu"));
    }

    void OnEnable()
    {
        loggedInSaveButton.interactable = true;
        loggedInLoadButton.interactable = true;
        loggedInText.text = "Logged in as: " + PlayerPrefs.GetString("userName");
    }

    async void SaveAccount()
    {
        loggedInLoadButton.interactable = false;
        loggedInSaveButton.interactable = false;
        WWWForm dataForm = new();
        dataForm.AddField("userName", SensitiveInfo.Encrypt(PlayerPrefs.GetString("userName", ""), SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
        dataForm.AddField("gameSession", SensitiveInfo.Encrypt(PlayerPrefs.GetString("gameSession", ""), SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
        dataForm.AddField("highScore", SensitiveInfo.Encrypt(PlayerPrefs.GetString("HighScoreV2", "0"), SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
        dataForm.AddField("icon", SensitiveInfo.Encrypt(PlayerPrefs.GetInt("icon", 1).ToString(), SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
        dataForm.AddField("overlay", SensitiveInfo.Encrypt(PlayerPrefs.GetInt("overlay", 0).ToString(), SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "saveAccount.php", dataForm);
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            AccountHandler.UpdateStatusText(loggedInText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        switch (response)
        {
            case "1":
                AccountHandler.UpdateStatusText(loggedInText, "Synced account", Color.green);
                break;
            case "-1":
                AccountHandler.UpdateStatusText(loggedInText, "Internal login server error", Color.red);
                break;
            case "-2":
                AccountHandler.instance.SwitchPanel(0);
                break;
            default:
                AccountHandler.UpdateStatusText(loggedInText, "Unknown server response", Color.red);
                break;
        }
        loggedInLoadButton.interactable = true;
        loggedInSaveButton.interactable = true;
    }

    async void LoadAccount()
    {
        loggedInLoadButton.interactable = false;
        loggedInSaveButton.interactable = false;
        WWWForm dataForm = new();
        dataForm.AddField("userName", SensitiveInfo.Encrypt(PlayerPrefs.GetString("userName", ""), SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
        dataForm.AddField("gameSession", SensitiveInfo.Encrypt(PlayerPrefs.GetString("gameSession", ""), SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "loadAccount.php", dataForm);
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            AccountHandler.UpdateStatusText(loggedInText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        switch (response)
        {
            case "-1":
                AccountHandler.UpdateStatusText(loggedInText, "Internal login server error", Color.red);
                break;
            case "-2":
                AccountHandler.instance.SwitchPanel(0);
                break;
            default:
                var split = response.Split(":");
                if (split[0] == "1")
                {
                    PlayerPrefs.SetString("HighScoreV2", split[1]);
                    PlayerPrefs.SetInt("icon", int.Parse(split[2]));
                    PlayerPrefs.SetInt("overlay", int.Parse(split[3]));
                    AccountHandler.UpdateStatusText(loggedInText, "Loaded account data", Color.green);
                }
                else
                {
                    AccountHandler.UpdateStatusText(loggedInText, "Unknown server response", Color.red);
                }
                break;
        }
        loggedInLoadButton.interactable = true;
        loggedInSaveButton.interactable = true;
    }
}
