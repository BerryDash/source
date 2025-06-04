using System.Text.RegularExpressions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Numerics;

public class AccountMenu : MonoBehaviour
{
    public GameObject loggedInPanel;
    public TMP_Text loggedInText;
    public Button loggedInChangeUsernameButton;
    public Button loggedInChangePasswordButton;
    public Button loggedInSaveButton;
    public Button loggedInLoadButton;
    public Button loggedInLogoutButton;
    public Button loggedInBackButton;

    public GameObject loggedOutPanel;
    public Button loggedOutLoginButton;
    public Button loggedOutRegisterButton;
    public Button loggedOutBackButton;

    public GameObject loginPanel;
    public TMP_Text loginPanelStatusText;
    public TMP_InputField loginUsernameInput;
    public TMP_InputField loginPasswordInput;
    public Button loginRegisterButton;
    public Button loginBackButton;
    public Button loginSubmitButton;

    public GameObject registerPanel;
    public TMP_Text registerPanelStatusText;
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerRetypeEmailInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField registerRetypePasswordInput;
    public Button registerLoginButton;
    public Button registerBackButton;
    public Button registerSubmitButton;

    public GameObject changeUsernamePanel;
    public TMP_Text changeUsernameStatusText;
    public TMP_InputField changeUsernameCurrentUsernameInput;
    public TMP_InputField changeUsernameNewUsernameInput;
    public Button changeUsernameBackButton;
    public Button changeUsernameSubmitButton;

    public GameObject changePasswordPanel;
    public TMP_Text changePasswordStatusText;
    public TMP_InputField changePasswordCurrentPasswordInput;
    public TMP_InputField changePasswordNewPasswordInput;
    public TMP_InputField changePasswordRetypeNewPasswordInput;
    public Button changePasswordBackButton;
    public Button changePasswordSubmitButton;

    void Awake()
    {
        if (PlayerPrefs.HasKey("gameSession") && PlayerPrefs.HasKey("userName") && PlayerPrefs.HasKey("userId"))
        {
            SwitchPanel(0);
        }
        else
        {
            SwitchPanel(1);
        }

        loggedInChangeUsernameButton.onClick.AddListener(() => SwitchPanel(4));
        loggedInChangePasswordButton.onClick.AddListener(() => SwitchPanel(5));
        loggedInSaveButton.onClick.AddListener(() => SaveAccount());
        loggedInLoadButton.onClick.AddListener(() => LoadAccount());
        loggedInLogoutButton.onClick.AddListener(() => SwitchPanel(1));
        loggedInBackButton.onClick.AddListener(() => SceneManager.LoadSceneAsync("MainMenu"));
        loggedOutLoginButton.onClick.AddListener(() => SwitchPanel(2));
        loggedOutRegisterButton.onClick.AddListener(() => SwitchPanel(3));
        loggedOutBackButton.onClick = loggedInBackButton.onClick;
        loginRegisterButton.onClick.AddListener(() => SwitchPanel(3));
        loginBackButton.onClick.AddListener(() => SwitchPanel(1));
        loginSubmitButton.onClick.AddListener(() => SubmitLogin());
        registerLoginButton.onClick.AddListener(() => SwitchPanel(2));
        registerBackButton.onClick.AddListener(() => SwitchPanel(1));
        registerSubmitButton.onClick.AddListener(() => SubmitRegister());
        changeUsernameBackButton.onClick.AddListener(() => SwitchPanel(0));
        changeUsernameSubmitButton.onClick.AddListener(() => ChangeUsername());
        changePasswordBackButton.onClick.AddListener(() => SwitchPanel(0));
        changePasswordSubmitButton.onClick.AddListener(() => ChangePassword());
    }

    void SwitchPanel(int panel)
    {
        switch (panel)
        {
            case 0:
                loggedInSaveButton.interactable = true;
                loggedInLoadButton.interactable = true;
                loggedInText.text = "Logged in as: " + PlayerPrefs.GetString("userName");
                loggedInPanel.SetActive(true);
                loggedOutPanel.SetActive(false);
                loginPanel.SetActive(false);
                registerPanel.SetActive(false);
                changeUsernamePanel.SetActive(false);
                changePasswordPanel.SetActive(false);
                break;
            case 1:
                PlayerPrefs.DeleteKey("gameSession");
                PlayerPrefs.DeleteKey("userName");
                PlayerPrefs.DeleteKey("userId");
                PlayerPrefs.SetString("HighScoreV2", "0");
                PlayerPrefs.SetInt("icon", 1);
                PlayerPrefs.SetInt("overlay", 0);
                loggedInPanel.SetActive(false);
                loggedOutPanel.SetActive(true);
                loginPanel.SetActive(false);
                registerPanel.SetActive(false);
                changeUsernamePanel.SetActive(false);
                changePasswordPanel.SetActive(false);
                break;
            case 2:
                loginUsernameInput.text = "";
                loginPasswordInput.text = "";
                loginPanelStatusText.text = "";
                loggedInPanel.SetActive(false);
                loggedOutPanel.SetActive(false);
                loginPanel.SetActive(true);
                registerPanel.SetActive(false);
                changeUsernamePanel.SetActive(false);
                changePasswordPanel.SetActive(false);
                break;
            case 3:
                registerUsernameInput.text = "";
                registerEmailInput.text = "";
                registerRetypeEmailInput.text = "";
                registerPasswordInput.text = "";
                registerRetypePasswordInput.text = "";
                registerPanelStatusText.text = "";
                loggedInPanel.SetActive(false);
                loggedOutPanel.SetActive(false);
                loginPanel.SetActive(false);
                registerPanel.SetActive(true);
                changeUsernamePanel.SetActive(false);
                changePasswordPanel.SetActive(false);
                break;
            case 4:
                changeUsernameCurrentUsernameInput.text = "";
                changeUsernameNewUsernameInput.text = "";
                changeUsernameStatusText.text = "";
                loggedInPanel.SetActive(false);
                loggedOutPanel.SetActive(false);
                loginPanel.SetActive(false);
                registerPanel.SetActive(false);
                changeUsernamePanel.SetActive(true);
                changePasswordPanel.SetActive(false);
                break;
            case 5:
                changePasswordCurrentPasswordInput.text = "";
                changePasswordNewPasswordInput.text = "";
                changePasswordRetypeNewPasswordInput.text = "";
                changePasswordStatusText.text = "";
                loggedInPanel.SetActive(false);
                loggedOutPanel.SetActive(false);
                loginPanel.SetActive(false);
                registerPanel.SetActive(false);
                changeUsernamePanel.SetActive(false);
                changePasswordPanel.SetActive(true);
                break;
        }
    }

    async void SubmitRegister()
    {
        if (!registerEmailInput.text.Trim().Equals(registerRetypeEmailInput.text.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            UpdateStatusText(registerPanelStatusText, "Email doesn't match", Color.red);
            return;
        }
        if (!registerPasswordInput.text.Trim().Equals(registerRetypePasswordInput.text.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            UpdateStatusText(registerPanelStatusText, "Password doesn't match", Color.red);
            return;
        }
        if (!Regex.IsMatch(registerUsernameInput.text, "^[a-zA-Z0-9]{3,16}$"))
        {
            UpdateStatusText(registerPanelStatusText, "Username must be 3-16 characters, letters and numbers only", Color.red);
            return;
        }
        WWWForm dataForm = new();
        dataForm.AddField("username", registerUsernameInput.text);
        dataForm.AddField("email", registerEmailInput.text);
        dataForm.AddField("password", registerPasswordInput.text);
        using UnityWebRequest request = UnityWebRequest.Post("https://berrydash.lncvrt.xyz/database/registerAccount.php", dataForm);
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            UpdateStatusText(registerPanelStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = request.downloadHandler.text;
        switch (response)
        {
            case "1":
                SwitchPanel(2);
                break;
            case "-1":
                UpdateStatusText(registerPanelStatusText, "Internal login server error", Color.red);
                break;
            case "-2":
                UpdateStatusText(registerPanelStatusText, "Incomplete form data", Color.red);
                break;
            case "-3":
                UpdateStatusText(registerPanelStatusText, "Username not valid", Color.red);
                break;
            case "-4":
                UpdateStatusText(registerPanelStatusText, "Email not valid", Color.red);
                break;
            case "-5":
                UpdateStatusText(registerPanelStatusText, "Password must have 8 characters, one number and one letter", Color.red);
                break;
            case "-6":
                UpdateStatusText(registerPanelStatusText, "Username too long or short", Color.red);
                break;
            case "-7":
                UpdateStatusText(registerPanelStatusText, "Username must be 3-16 characters, letters and numbers only", Color.red);
                break;
            case "-8":
                UpdateStatusText(registerPanelStatusText, "Username or email already exists", Color.red);
                break;
            default:
                UpdateStatusText(registerPanelStatusText, "Unknown server response", Color.red);
                break;
        }
    }

    async void SubmitLogin()
    {
        WWWForm dataForm = new();
        dataForm.AddField("username", loginUsernameInput.text);
        dataForm.AddField("password", loginPasswordInput.text);
        dataForm.AddField("currentHighScore", PlayerPrefs.GetString("HighScoreV2", "0"));
        using UnityWebRequest request = UnityWebRequest.Post("https://berrydash.lncvrt.xyz/database/loginAccount.php", dataForm);
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            UpdateStatusText(loginPanelStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = request.downloadHandler.text;
        if (response != "-1")
        {
            if (response == "-2")
            {
                UpdateStatusText(loginPanelStatusText, "Incorrect username or password", Color.red);
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
                SwitchPanel(0);
                UpdateStatusText(loginPanelStatusText, "", Color.red);
            }
            else
            {
                UpdateStatusText(loginPanelStatusText, "Unknown server response", Color.red);
            }
        }
        else
        {
            UpdateStatusText(loginPanelStatusText, "Internal login server error", Color.red);
        }
    }

    async void ChangeUsername()
    {
        WWWForm dataForm = new();
        dataForm.AddField("inputUserName", changeUsernameCurrentUsernameInput.text);
        dataForm.AddField("inputNewUserName", changeUsernameNewUsernameInput.text);
        dataForm.AddField("session", PlayerPrefs.GetString("gameSession"));
        dataForm.AddField("userName", PlayerPrefs.GetString("userName"));
        using UnityWebRequest request = UnityWebRequest.Post("https://berrydash.lncvrt.xyz/database/changeAccountUsername.php", dataForm);
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            UpdateStatusText(changeUsernameStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = request.downloadHandler.text;
        switch (response)
        {
            case "1":
                PlayerPrefs.SetString("userName", changeUsernameNewUsernameInput.text);
                SwitchPanel(0);
                UpdateStatusText(loggedInText, "Username changed successfully", Color.green);
                break;
            case "-1":
                UpdateStatusText(changeUsernameStatusText, "Internal login server error", Color.red);
                break;
            case "-2":
                UpdateStatusText(changeUsernameStatusText, "New Username is too short or too long", Color.red);
                break;
            case "-3":
                UpdateStatusText(changeUsernameStatusText, "New Username does not match the required format", Color.red);
                break;
            case "-4":
                UpdateStatusText(changeUsernameStatusText, "New username already exists", Color.red);
                break;
            default:
                UpdateStatusText(changeUsernameStatusText, "Unknown server response", Color.red);
                break;
        }
    }

    async void ChangePassword()
    {
        if (changePasswordNewPasswordInput.text != changePasswordRetypeNewPasswordInput.text)
        {
            UpdateStatusText(changePasswordStatusText, "Passwords do not match", Color.red);
            return;
        }
        WWWForm dataForm = new();
        dataForm.AddField("inputPassword", changePasswordCurrentPasswordInput.text);
        dataForm.AddField("inputNewPassword", changePasswordNewPasswordInput.text);
        dataForm.AddField("session", PlayerPrefs.GetString("gameSession"));
        dataForm.AddField("userName", PlayerPrefs.GetString("userName"));
        using UnityWebRequest request = UnityWebRequest.Post("https://berrydash.lncvrt.xyz/database/changeAccountPassword.php", dataForm);
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            UpdateStatusText(changePasswordStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = request.downloadHandler.text;
        switch (response)
        {
            case "-1":
                UpdateStatusText(changePasswordStatusText, "Internal login server error", Color.red);
                break;
            case "-2":
                UpdateStatusText(changePasswordStatusText, "New Password, Password, or username is empty", Color.red);
                break;
            case "-3":
                UpdateStatusText(changePasswordStatusText, "New Password is too short or too long", Color.red);
                break;
            case "-4":
                UpdateStatusText(changePasswordStatusText, "Username must be 3-16 characters, letters and numbers only", Color.red);
                break;
            case "-5":
                UpdateStatusText(changePasswordStatusText, "Incorrect current password", Color.red);
                break;
            case "-6":
                UpdateStatusText(changePasswordStatusText, "Current username is incorrect", Color.red);
                break;
            case "-7":
                UpdateStatusText(changePasswordStatusText, "New password cannot be the same as your old password", Color.red);
                break;
        }
        if (Regex.IsMatch(response, "^[a-zA-Z0-9]{512}$"))
        {
            PlayerPrefs.SetString("gameSession", response);
            SwitchPanel(0);
            UpdateStatusText(loggedInText, "Password changed successfully", Color.green);
        }
        else
        {
            UpdateStatusText(changePasswordStatusText, "Unknown server response " + response, Color.red);
        }
    }

    async void SaveAccount()
    {
        loggedInLoadButton.interactable = false;
        loggedInSaveButton.interactable = false;
        WWWForm dataForm = new();
        dataForm.AddField("userName", PlayerPrefs.GetString("userName", ""));
        dataForm.AddField("gameSession", PlayerPrefs.GetString("gameSession", ""));
        dataForm.AddField("highScore", PlayerPrefs.GetString("HighScoreV2", "0"));
        dataForm.AddField("icon", PlayerPrefs.GetInt("icon", 1).ToString());
        dataForm.AddField("overlay", PlayerPrefs.GetInt("overlay", 0).ToString());
        using UnityWebRequest request = UnityWebRequest.Post("https://berrydash.lncvrt.xyz/database/saveAccount.php", dataForm);
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            UpdateStatusText(loggedInText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = request.downloadHandler.text;
        switch (response)
        {
            case "1":
                UpdateStatusText(loggedInText, "Synced account", Color.green);
                break;
            case "-1":
                UpdateStatusText(loggedInText, "Internal login server error", Color.red);
                break;
            case "-2":
                SwitchPanel(0);
                break;
            default:
                UpdateStatusText(loggedInText, "Unknown server response", Color.red);
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
        dataForm.AddField("userName", PlayerPrefs.GetString("userName", ""));
        dataForm.AddField("gameSession", PlayerPrefs.GetString("gameSession", ""));
        using UnityWebRequest request = UnityWebRequest.Post("https://berrydash.lncvrt.xyz/database/loadAccount.php", dataForm);
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            UpdateStatusText(loggedInText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = request.downloadHandler.text;
        switch (response)
        {
            case "-1":
                UpdateStatusText(loggedInText, "Internal login server error", Color.red);
                break;
            case "-2":
                SwitchPanel(0);
                break;
            default:
                var split = response.Split(":");
                if (split[0] == "1")
                {
                    PlayerPrefs.SetString("HighScoreV2", split[1]);
                    PlayerPrefs.SetInt("icon", int.Parse(split[2]));
                    PlayerPrefs.SetInt("overlay", int.Parse(split[3]));
                    UpdateStatusText(loggedInText, "Loaded account data", Color.green);
                }
                else
                {
                    UpdateStatusText(loggedInText, "Unknown server response", Color.red);
                }
                break;
        }
        loggedInLoadButton.interactable = true;
        loggedInSaveButton.interactable = true;
    }

    void UpdateStatusText(TMP_Text statusText, string message, Color color)
    {
        statusText.text = message;
        statusText.color = color;
    }
}
