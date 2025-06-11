using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountChangePassword : MonoBehaviour
{
    public TMP_Text changePasswordStatusText;
    public TMP_InputField changePasswordCurrentPasswordInput;
    public TMP_InputField changePasswordNewPasswordInput;
    public TMP_InputField changePasswordRetypeNewPasswordInput;
    public Button changePasswordBackButton;
    public Button changePasswordSubmitButton;

    void Awake()
    {
        changePasswordBackButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(0));
        changePasswordSubmitButton.onClick.AddListener(() => ChangePassword());
    }

    void OnEnable()
    {
        changePasswordCurrentPasswordInput.text = "";
        changePasswordNewPasswordInput.text = "";
        changePasswordRetypeNewPasswordInput.text = "";
        changePasswordStatusText.text = "";
    }

    async void ChangePassword()
    {
        if (changePasswordNewPasswordInput.text != changePasswordRetypeNewPasswordInput.text)
        {
            AccountHandler.UpdateStatusText(changePasswordStatusText, "Passwords do not match", Color.red);
            return;
        }
        WWWForm dataForm = new();
        dataForm.AddField("inputPassword", SensitiveInfo.Encrypt(changePasswordCurrentPasswordInput.text));
        dataForm.AddField("inputNewPassword", SensitiveInfo.Encrypt(changePasswordNewPasswordInput.text));
        dataForm.AddField("session", SensitiveInfo.Encrypt(PlayerPrefs.GetString("gameSession")));
        dataForm.AddField("userName", SensitiveInfo.Encrypt(PlayerPrefs.GetString("userName")));
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "changeAccountPassword.php", dataForm);
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            AccountHandler.UpdateStatusText(changePasswordStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = request.downloadHandler.text;
        switch (response)
        {
            case "-1":
                AccountHandler.UpdateStatusText(changePasswordStatusText, "Internal login server error", Color.red);
                break;
            case "-2":
                AccountHandler.UpdateStatusText(changePasswordStatusText, "New Password, Password, or username is empty", Color.red);
                break;
            case "-3":
                AccountHandler.UpdateStatusText(changePasswordStatusText, "New Password is too short or too long", Color.red);
                break;
            case "-4":
                AccountHandler.UpdateStatusText(changePasswordStatusText, "Username must be 3-16 characters, letters and numbers only", Color.red);
                break;
            case "-5":
                AccountHandler.UpdateStatusText(changePasswordStatusText, "Incorrect current password", Color.red);
                break;
            case "-6":
                AccountHandler.UpdateStatusText(changePasswordStatusText, "Current username is incorrect", Color.red);
                break;
            case "-7":
                AccountHandler.UpdateStatusText(changePasswordStatusText, "New password cannot be the same as your old password", Color.red);
                break;
        }
        if (Regex.IsMatch(response, "^[a-zA-Z0-9]{512}$"))
        {
            PlayerPrefs.SetString("gameSession", response);
            AccountHandler.instance.SwitchPanel(0);
            AccountHandler.UpdateStatusText(AccountHandler.instance.accountLoggedIn.loggedInText, "Password changed successfully", Color.green);
        }
        else
        {
            AccountHandler.UpdateStatusText(changePasswordStatusText, "Unknown server response " + response, Color.red);
        }
    }
}
