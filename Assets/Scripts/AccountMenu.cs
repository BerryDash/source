using System.Text.RegularExpressions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    void Awake()
    {
        if (PlayerPrefs.HasKey("gameSession") && PlayerPrefs.HasKey("userName"))
        {
            SwitchPanel(0);
        }
        else
        {
            SwitchPanel(1);
        }

        loggedInLogoutButton.onClick.AddListener(() =>
        {
            PlayerPrefs.DeleteKey("gameSession");
            PlayerPrefs.DeleteKey("userName");
            PlayerPrefs.SetInt("HighScore", 0);
            PlayerPrefs.SetInt("icon", 1);
            PlayerPrefs.SetInt("overlay", 0);
            SwitchPanel(1);
        });
        loggedInBackButton.onClick.AddListener(() => SceneManager.LoadSceneAsync("MainMenu"));
        loggedOutLoginButton.onClick.AddListener(() => SwitchPanel(2));
        loggedOutRegisterButton.onClick.AddListener(() => SwitchPanel(3));
        loginRegisterButton.onClick.AddListener(() => SwitchPanel(3));
        loginBackButton.onClick.AddListener(() => SwitchPanel(1));
        loginSubmitButton.onClick.AddListener(() => SubmitLogin());
        registerLoginButton.onClick.AddListener(() => SwitchPanel(2));
        registerBackButton.onClick.AddListener(() => SwitchPanel(1));
        registerSubmitButton.onClick.AddListener(() => SubmitRegister());
    }

    void SwitchPanel(int panel)
    {
        switch (panel)
        {
            case 0:
                loggedInText.text = "Logged in as: " + PlayerPrefs.GetString("userName");
                loggedInPanel.SetActive(true);
                loggedOutPanel.SetActive(false);
                loginPanel.SetActive(false);
                registerPanel.SetActive(false);
                break;
            case 1:
                loggedInPanel.SetActive(false);
                loggedOutPanel.SetActive(true);
                loginPanel.SetActive(false);
                registerPanel.SetActive(false);
                break;
            case 2:
                loginUsernameInput.text = "";
                loginPasswordInput.text = "";
                loginPanelStatusText.text = "";
                loggedInPanel.SetActive(false);
                loggedOutPanel.SetActive(false);
                loginPanel.SetActive(true);
                registerPanel.SetActive(false);
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
        WWWForm wWWForm = new();
        wWWForm.AddField("username", registerUsernameInput.text);
        wWWForm.AddField("email", registerEmailInput.text);
        wWWForm.AddField("password", registerPasswordInput.text);
        using UnityWebRequest request = UnityWebRequest.Post("https://berrydash.lncvrt.xyz/database/registerAccount.php", wWWForm);
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", PlayerPrefs.GetFloat("clientVersion").ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            UpdateStatusText(registerPanelStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string text = request.downloadHandler.text;
        switch (text)
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
                UpdateStatusText(registerPanelStatusText, "Unknown server response \"" + text + "\"", Color.red);
                break;
        }
    }

    async void SubmitLogin()
    {
        WWWForm wWWForm = new();
        wWWForm.AddField("username", loginUsernameInput.text);
        wWWForm.AddField("password", loginPasswordInput.text);
        wWWForm.AddField("currentHighScore", PlayerPrefs.GetInt("HighScore", 0).ToString());
        using UnityWebRequest request = UnityWebRequest.Post("https://berrydash.lncvrt.xyz/database/loginAccount.php", wWWForm);
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", PlayerPrefs.GetFloat("clientVersion").ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            UpdateStatusText(loginPanelStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string text = request.downloadHandler.text;
        if (!(text == "-1"))
        {
            if (text == "-2")
            {
                UpdateStatusText(loginPanelStatusText, "Incorrect username or password", Color.red);
            }
            else if (text.Split(":")[0] == "1")
            {
                string[] array = text.Split(':');
                string session = array[1];
                string userName = array[2];
                int highScore = int.Parse(array[3]);
                int iconId = int.Parse(array[4]);
                int overlayId = int.Parse(array[5]);
                PlayerPrefs.SetString("gameSession", session);
                PlayerPrefs.SetString("userName", userName);
                PlayerPrefs.SetInt("HighScore", highScore);
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

    void UpdateStatusText(TMP_Text statusText, string message, Color color)
    {
        statusText.text = message;
        statusText.color = color;
    }
}
