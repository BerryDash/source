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
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("userName", PlayerPrefs.GetString("userName", ""));
        dataForm.AddField("gameSession", PlayerPrefs.GetString("gameSession", ""));
        dataForm.AddField("highScore", PlayerPrefs.GetString("HighScoreV2", "0"));
        dataForm.AddField("icon", PlayerPrefs.GetInt("icon", 1).ToString());
        dataForm.AddField("overlay", PlayerPrefs.GetInt("overlay", 0).ToString());
        dataForm.AddField("totalNormalBerries", PlayerPrefs.GetString("TotalNormalBerries", "0"));
        dataForm.AddField("totalPoisonBerries", PlayerPrefs.GetString("TotalPoisonBerries", "0"));
        dataForm.AddField("totalSlowBerries", PlayerPrefs.GetString("TotalSlowBerries", "0"));
        dataForm.AddField("totalUltraBerries", PlayerPrefs.GetString("TotalUltraBerries", "0"));
        dataForm.AddField("totalSpeedyBerries", PlayerPrefs.GetString("TotalSpeedyBerries", "0"));
        dataForm.AddField("totalAttempts", PlayerPrefs.GetString("TotalAttempts", "0"));
        dataForm.AddField("birdR", PlayerPrefs.GetString("BirdColor", "255;255;255").Split(';')[0]);
        dataForm.AddField("birdG", PlayerPrefs.GetString("BirdColor", "255;255;255").Split(';')[1]);
        dataForm.AddField("birdB", PlayerPrefs.GetString("BirdColor", "255;255;255").Split(';')[2]);
        dataForm.AddField("overlayR", PlayerPrefs.GetString("OverlayColor", "255;255;255").Split(';')[0]);
        dataForm.AddField("overlayG", PlayerPrefs.GetString("OverlayColor", "255;255;255").Split(';')[1]);
        dataForm.AddField("overlayB", PlayerPrefs.GetString("OverlayColor", "255;255;255").Split(';')[2]);
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "saveAccount.php", dataForm.GetWWWForm());
        request.SetRequestHeader("Requester", "BerryDashClient");
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
            case "-999":
                AccountHandler.UpdateStatusText(loggedInText, "Server error while fetching data", Color.red);
                break;
            case "-998":
                AccountHandler.UpdateStatusText(loggedInText, "Client version too outdated to access servers", Color.red);
                break;
            case "-997":
                AccountHandler.UpdateStatusText(loggedInText, "Encryption/decryption issues", Color.red);
                break;
            case "1":
                AccountHandler.UpdateStatusText(loggedInText, "Synced account", Color.green);
                break;
            case "-1":
                AccountHandler.UpdateStatusText(loggedInText, "Failed to find info about your user (refresh login?)", Color.red);
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
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("userName", PlayerPrefs.GetString("userName", ""));
        dataForm.AddField("gameSession", PlayerPrefs.GetString("gameSession", ""));
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "loadAccount.php", dataForm.GetWWWForm());
        request.SetRequestHeader("Requester", "BerryDashClient");
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
            case "-999":
                AccountHandler.UpdateStatusText(loggedInText, "Server error while fetching data", Color.red);
                break;
            case "-998":
                AccountHandler.UpdateStatusText(loggedInText, "Client version too outdated to access servers", Color.red);
                break;
            case "-997":
                AccountHandler.UpdateStatusText(loggedInText, "Encryption/decryption issues", Color.red);
                break;
            case "-1":
                AccountHandler.UpdateStatusText(loggedInText, "Failed to find info about your user (refresh login?)", Color.red);
                break;
            default:
                var split = response.Split(":");
                if (split[0] == "1")
                {
                    PlayerPrefs.SetString("HighScoreV2", split[1]);
                    PlayerPrefs.SetInt("icon", int.Parse(split[2]));
                    PlayerPrefs.SetInt("overlay", int.Parse(split[3]));
                    PlayerPrefs.SetString("TotalNormalBerries", split[4]);
                    PlayerPrefs.SetString("TotalPoisonBerries", split[5]);
                    PlayerPrefs.SetString("TotalSlowBerries", split[6]);
                    PlayerPrefs.SetString("TotalUltraBerries", split[7]);
                    PlayerPrefs.SetString("TotalSpeedyBerries", split[8]);
                    PlayerPrefs.SetString("TotalAttempts", split[9]);
                    PlayerPrefs.SetString("BirdColor", $"{split[10]};{split[11]};{split[12]}");
                    PlayerPrefs.SetString("OverlayColor", $"{split[13]};{split[14]};{split[15]}");
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
