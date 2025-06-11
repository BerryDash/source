using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountChangeUsername : MonoBehaviour
{
    public TMP_Text changeUsernameStatusText;
    public TMP_InputField changeUsernameCurrentUsernameInput;
    public TMP_InputField changeUsernameNewUsernameInput;
    public Button changeUsernameBackButton;
    public Button changeUsernameSubmitButton;

    void Awake()
    {
        changeUsernameBackButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(0));
        changeUsernameSubmitButton.onClick.AddListener(() => ChangeUsername());
    }

    void OnEnable()
    {
        changeUsernameCurrentUsernameInput.text = "";
        changeUsernameNewUsernameInput.text = "";
        changeUsernameStatusText.text = "";
    }

    async void ChangeUsername()
    {
        WWWForm dataForm = new();
        dataForm.AddField("inputUserName", SensitiveInfo.Encrypt(changeUsernameCurrentUsernameInput.text, SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
        dataForm.AddField("inputNewUserName", SensitiveInfo.Encrypt(changeUsernameNewUsernameInput.text, SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
        dataForm.AddField("session", SensitiveInfo.Encrypt(PlayerPrefs.GetString("gameSession"), SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
        dataForm.AddField("userName", SensitiveInfo.Encrypt(PlayerPrefs.GetString("userName"), SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "changeAccountUsername.php", dataForm);
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            AccountHandler.UpdateStatusText(changeUsernameStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        switch (response)
        {
            case "1":
                PlayerPrefs.SetString("userName", changeUsernameNewUsernameInput.text);
                AccountHandler.instance.SwitchPanel(0);
                AccountHandler.UpdateStatusText(AccountHandler.instance.accountLoggedIn.loggedInText, "Username changed successfully", Color.green);
                break;
            case "-1":
                AccountHandler.UpdateStatusText(changeUsernameStatusText, "Internal login server error", Color.red);
                break;
            case "-2":
                AccountHandler.UpdateStatusText(changeUsernameStatusText, "New Username is too short or too long", Color.red);
                break;
            case "-3":
                AccountHandler.UpdateStatusText(changeUsernameStatusText, "New Username does not match the required format", Color.red);
                break;
            case "-4":
                AccountHandler.UpdateStatusText(changeUsernameStatusText, "New username already exists", Color.red);
                break;
            default:
                AccountHandler.UpdateStatusText(changeUsernameStatusText, "Unknown server response", Color.red);
                break;
        }
    }
}
