using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountRefreshLogin : MonoBehaviour
{
    public TMP_Text refreshLoginStatusText;
    public TMP_InputField refreshLoginUsernameInput;
    public TMP_InputField refreshLoginPasswordInput;
    public Button refreshLoginBackButton;
    public Button refreshLoginSubmitButton;

    void Awake()
    {
        refreshLoginBackButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(0));
        refreshLoginSubmitButton.onClick.AddListener(() => RefreshLogin());
    }

    void OnEnable()
    {
        refreshLoginUsernameInput.text = "";
        refreshLoginPasswordInput.text = "";
    }

    async void RefreshLogin()
    {
        WWWForm dataForm = new();
        dataForm.AddField("username", refreshLoginUsernameInput.text);
        dataForm.AddField("password", refreshLoginPasswordInput.text);
        dataForm.AddField("loginType", "1");
        using UnityWebRequest request = UnityWebRequest.Post("https://berrydash.lncvrt.xyz/database/loginAccount.php", dataForm);
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            AccountHandler.UpdateStatusText(refreshLoginStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = request.downloadHandler.text;
        if (response != "-1")
        {
            if (response == "-2")
            {
                AccountHandler.UpdateStatusText(refreshLoginStatusText, "Incorrect username or password", Color.red);
            }
            else if (response.Split(":")[0] == "1")
            {
                string[] array = response.Split(':');
                string session = array[1];
                string userName = array[2];
                int userId = int.Parse(array[3]);
                PlayerPrefs.SetString("gameSession", session);
                PlayerPrefs.SetString("userName", userName);
                PlayerPrefs.SetInt("userId", userId);
                AccountHandler.instance.SwitchPanel(0);
                AccountHandler.UpdateStatusText(refreshLoginStatusText, "", Color.red);
            }
            else
            {
                AccountHandler.UpdateStatusText(refreshLoginStatusText, "Unknown server response", Color.red);
            }
        }
        else
        {
            AccountHandler.UpdateStatusText(refreshLoginStatusText, "Internal login server error", Color.red);
        }
    }
}
