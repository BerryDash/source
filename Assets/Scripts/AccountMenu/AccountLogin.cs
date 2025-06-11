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
        WWWForm dataForm = new();
        dataForm.AddField("username", SensitiveInfo.Encrypt(loginUsernameInput.text, SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
        dataForm.AddField("password", SensitiveInfo.Encrypt(loginPasswordInput.text, SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
        dataForm.AddField("currentHighScore", SensitiveInfo.Encrypt(PlayerPrefs.GetString("HighScoreV2", "0"), SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
        dataForm.AddField("loginType", SensitiveInfo.Encrypt("0", SensitiveInfo.SERVER_SEND_TRANSFER_KEY)); //Yes.
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "loginAccount.php", dataForm);
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            AccountHandler.UpdateStatusText(loginPanelStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response != "-1")
        {
            if (response == "-2")
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
                AccountHandler.instance.SwitchPanel(0);
                AccountHandler.UpdateStatusText(loginPanelStatusText, "", Color.red);
            }
            else
            {
                AccountHandler.UpdateStatusText(loginPanelStatusText, "Unknown server response", Color.red);
            }
        }
        else
        {
            AccountHandler.UpdateStatusText(loginPanelStatusText, "Internal login server error", Color.red);
        }
    }
}
