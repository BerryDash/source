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
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("username", refreshLoginUsernameInput.text);
        dataForm.AddField("password", refreshLoginPasswordInput.text);
        dataForm.AddField("loginType", "1");
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "loginAccount.php", dataForm.GetWWWForm());
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            AccountHandler.UpdateStatusText(refreshLoginStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            AccountHandler.UpdateStatusText(refreshLoginStatusText, "Server error while fetching data", Color.red);
            return;
        }
        else if (response == "-998")
        {
            AccountHandler.UpdateStatusText(refreshLoginStatusText, "Client version too outdated to access servers", Color.red);
            return;
        }
        else if (response == "-997")
        {
            AccountHandler.UpdateStatusText(refreshLoginStatusText, "Encryption/decryption issues", Color.red);
            return;
        }
        else if (response == "-1")
        {
            AccountHandler.UpdateStatusText(refreshLoginStatusText, "Incorrect username or password", Color.red);
        }
        else if (response == "1")
        {
            AccountHandler.instance.SwitchPanel(0);
        }
        else
        {
            AccountHandler.UpdateStatusText(refreshLoginStatusText, "Unknown server response", Color.red);
        }
    }
}
