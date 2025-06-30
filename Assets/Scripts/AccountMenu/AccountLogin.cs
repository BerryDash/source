using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountLogin : MonoBehaviour
{
    public TMP_Text loginPanelStatusText;
    public TMP_InputField loginUsernameInput;
    public TMP_InputField loginPasswordInput;
    public Button loginBackButton;
    public Button loginSubmitButton;

    void Awake()
    {
        loginBackButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(1));
        loginSubmitButton.onClick.AddListener(() => SubmitLogin());
    }

    void OnEnable()
    {
        loginUsernameInput.text = "";
        loginPasswordInput.text = "";
        loginPanelStatusText.text = "";
    }

    async void SubmitLogin()
    {
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("username", loginUsernameInput.text);
        dataForm.AddField("password", loginPasswordInput.text);
        dataForm.AddField("currentHighScore", PlayerPrefs.GetString("HighScoreV2", "0"));
        dataForm.AddField("loginType", "0");
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "loginAccount.php", dataForm.GetWWWForm());
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            AccountHandler.UpdateStatusText(loginPanelStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            AccountHandler.UpdateStatusText(loginPanelStatusText, "Server error while fetching data", Color.red);
            return;
        }
        else if (response == "-998")
        {
            AccountHandler.UpdateStatusText(loginPanelStatusText, "Client version too outdated to access servers", Color.red);
            return;
        }
        else if (response == "-997")
        {
            AccountHandler.UpdateStatusText(loginPanelStatusText, "Encryption/decryption issues", Color.red);
            return;
        }
        else if (response == "-996")
        {
            AccountHandler.UpdateStatusText(loginPanelStatusText, "Can't send requests on self-built instance", Color.red);
            return;
        }
        else if (response == "-1")
        {
            AccountHandler.UpdateStatusText(loginPanelStatusText, "Incorrect username or password", Color.red);
        }
        else if (response.Split(":")[0] == "1")
        {
            string[] array = response.Split(':');
            string session = array[1];
            string userName = array[2];
            int userId = int.Parse(array[3]);
            BigInteger highScore = BigInteger.Parse(array[4]);
            int iconId = int.Parse(array[5]);
            int overlayId = int.Parse(array[6]);
            PlayerPrefs.SetString("gameSession", session);
            PlayerPrefs.SetString("userName", userName);
            PlayerPrefs.SetInt("userId", userId);
            PlayerPrefs.SetString("HighScoreV2", highScore.ToString());
            PlayerPrefs.SetInt("icon", iconId);
            PlayerPrefs.SetInt("overlay", overlayId);
            PlayerPrefs.SetString("TotalNormalBerries", array[7]);
            PlayerPrefs.SetString("TotalPoisonBerries", array[8]);
            PlayerPrefs.SetString("TotalSlowBerries", array[9]);
            PlayerPrefs.SetString("TotalUltraBerries", array[10]);
            PlayerPrefs.SetString("TotalSpeedyBerries", array[11]);
            PlayerPrefs.SetString("TotalAttempts", array[12]);
            PlayerPrefs.SetString("BirdColor", $"{array[13]};{array[14]};{array[15]}");
            PlayerPrefs.SetString("OverlayColor", $"{array[16]};{array[17]};{array[18]}");
            AccountHandler.instance.SwitchPanel(0);
            AccountHandler.UpdateStatusText(loginPanelStatusText, "", Color.red);
        }
        else
        {
            AccountHandler.UpdateStatusText(loginPanelStatusText, "Unknown server response", Color.red);
        }
    }
}