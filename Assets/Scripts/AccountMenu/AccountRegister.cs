using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountRegister : MonoBehaviour
{
    public TMP_Text registerPanelStatusText;
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerRetypeEmailInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField registerRetypePasswordInput;
    public Button registerBackButton;
    public Button registerSubmitButton;

    void Awake()
    {
        registerBackButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(1));
        registerSubmitButton.onClick.AddListener(() => SubmitRegister());
    }

    void OnEnable()
    {
        registerUsernameInput.text = "";
        registerEmailInput.text = "";
        registerRetypeEmailInput.text = "";
        registerPasswordInput.text = "";
        registerRetypePasswordInput.text = "";
        registerPanelStatusText.text = "";
    }

    async void SubmitRegister()
    {
        if (!registerEmailInput.text.Trim().Equals(registerRetypeEmailInput.text.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            AccountHandler.UpdateStatusText(registerPanelStatusText, "Email doesn't match", Color.red);
            return;
        }
        if (!registerPasswordInput.text.Trim().Equals(registerRetypePasswordInput.text.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            AccountHandler.UpdateStatusText(registerPanelStatusText, "Password doesn't match", Color.red);
            return;
        }
        if (!Regex.IsMatch(registerUsernameInput.text, "^[a-zA-Z0-9]{3,16}$"))
        {
            AccountHandler.UpdateStatusText(registerPanelStatusText, "Username must be 3-16 characters, letters and numbers only", Color.red);
            return;
        }
        WWWForm dataForm = new();
        dataForm.AddField("username", SensitiveInfo.Encrypt(registerUsernameInput.text));
        dataForm.AddField("email", SensitiveInfo.Encrypt(registerEmailInput.text));
        dataForm.AddField("password", SensitiveInfo.Encrypt(registerPasswordInput.text));
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "registerAccount.php", dataForm);
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            AccountHandler.UpdateStatusText(registerPanelStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = request.downloadHandler.text;
        switch (response)
        {
            case "1":
                AccountHandler.instance.SwitchPanel(2);
                break;
            case "-1":
                AccountHandler.UpdateStatusText(registerPanelStatusText, "Internal login server error", Color.red);
                break;
            case "-2":
                AccountHandler.UpdateStatusText(registerPanelStatusText, "Incomplete form data", Color.red);
                break;
            case "-3":
                AccountHandler.UpdateStatusText(registerPanelStatusText, "Username not valid", Color.red);
                break;
            case "-4":
                AccountHandler.UpdateStatusText(registerPanelStatusText, "Email not valid", Color.red);
                break;
            case "-5":
                AccountHandler.UpdateStatusText(registerPanelStatusText, "Password must have 8 characters, one number and one letter", Color.red);
                break;
            case "-6":
                AccountHandler.UpdateStatusText(registerPanelStatusText, "Username too long or short", Color.red);
                break;
            case "-7":
                AccountHandler.UpdateStatusText(registerPanelStatusText, "Username must be 3-16 characters, letters and numbers only", Color.red);
                break;
            case "-8":
                AccountHandler.UpdateStatusText(registerPanelStatusText, "Username or email already exists", Color.red);
                break;
            default:
                AccountHandler.UpdateStatusText(registerPanelStatusText, "Unknown server response", Color.red);
                break;
        }
    }
}
