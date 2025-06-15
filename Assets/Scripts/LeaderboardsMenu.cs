using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardsMenu : MonoBehaviour
{
    public TMP_Text statusText;

    public GameObject selectionPanel;
    public Button selectionScoreButton;
    public Button selectionBerryButton;
    public Button selectionBackButton;

    public GameObject scorePanel;
    public GameObject scoreContent;
    public Button scoreBackButton;
    public Button scoreRefreshButton;
    public GameObject scoreSampleObject;

    public GameObject berryPanel;
    public GameObject berryContent;
    public TMP_Dropdown berryShowTypeDropdown;
    public Button berryBackButton;
    public Button berryRefreshButton;
    public GameObject berrySampleObject;

    void Awake()
    {
        selectionScoreButton.onClick.AddListener(() => SwitchMenu(1));
        selectionBerryButton.onClick.AddListener(() => SwitchMenu(2));
        selectionBackButton.onClick.AddListener(async () => await SceneManager.LoadSceneAsync("MainMenu"));

        scoreBackButton.onClick.AddListener(() => SwitchMenu(0));
        scoreRefreshButton.onClick.AddListener(() => GetTopPlayersScore());

        berryShowTypeDropdown.onValueChanged.AddListener(value => GetTopPlayersBerry(value));
        berryBackButton.onClick.AddListener(() => SwitchMenu(0));
        berryRefreshButton.onClick.AddListener(() => GetTopPlayersBerry(berryShowTypeDropdown.value));
    }

    void SwitchMenu(int menu) {
        UpdateStatus(false, "");
        foreach (Transform item in scoreContent.transform)
        {
            if (item.gameObject.activeSelf)
            {
                Destroy(item.gameObject);
            }
        }
        foreach (Transform item in berryContent.transform)
        {
            if (item.gameObject.activeSelf)
            {
                Destroy(item.gameObject);
            }
        }

        switch (menu)
        {
            case 0:
                selectionPanel.SetActive(true);
                scorePanel.SetActive(false);
                berryPanel.SetActive(false);
                break;
            case 1:
                GetTopPlayersScore();
                selectionPanel.SetActive(false);
                scorePanel.SetActive(true);
                berryPanel.SetActive(false);
                break;
            case 2:
                berryShowTypeDropdown.value = 0;
                GetTopPlayersBerry(0);
                selectionPanel.SetActive(false);
                scorePanel.SetActive(false);
                berryPanel.SetActive(true);
                break;
        }
    }

    async void GetTopPlayersScore()
    {
        scoreBackButton.interactable = false;
        scoreRefreshButton.interactable = false;
        foreach (Transform item in scoreContent.transform)
        {
            if (item.gameObject.activeSelf)
            {
                Destroy(item.gameObject);
            }
        }
        UpdateStatus(true, "Loading...");
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("type", "0");
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "getTopPlayers.php", dataForm.GetWWWForm());
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            UpdateStatus(false);
            string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
            if (response == "-999")
            {
                UpdateStatus(true, "Server error while fetching data");
            }
            else if (response == "-998")
            {
                UpdateStatus(true, "Client version too outdated to access servers");
            }
            else if (response == "-997")
            {
                UpdateStatus(true, "Encryption/decryption issues");
            }
            else if (response == "-1")
            {
                UpdateStatus(true, "No entries for this leaderboard found!");
            }
            else
            {
                var splitResponse = response.Split(';');
                for (int i = 0; i < splitResponse.Length; i++)
                {
                    var entry = splitResponse[i];
                    var split = entry.Split(":");
                    var username = Encoding.UTF8.GetString(Convert.FromBase64String(split[0]));
                    var highScore = split[1];
                    var icon = split[2];
                    var overlay = split[3];
                    var uid = split[4];

                    var entryInfo = Instantiate(scoreSampleObject, scoreContent.transform);
                    var usernameText = entryInfo.transform.GetChild(0).GetComponent<TMP_Text>();
                    var playerIcon = usernameText.transform.GetChild(0).GetComponent<Image>();
                    var playerOverlayIcon = playerIcon.transform.GetChild(0).GetComponent<Image>();
                    var highScoreText = entryInfo.transform.GetChild(1).GetComponent<TMP_Text>();

                    usernameText.text = $"{username} (#{i + 1})";
                    highScoreText.text += Tools.FormatWithCommas(highScore);
                    playerIcon.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + icon);
                    if (icon == "1")
                    {
                        playerIcon.sprite = Tools.GetIconForUser(int.Parse(uid));
                    }
                    playerOverlayIcon.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + overlay);
                    if (overlay == "0")
                    {
                        playerOverlayIcon.gameObject.SetActive(false);
                    }
                    else if (overlay == "8")
                    {
                        playerOverlayIcon.transform.localPosition = new Vector2(-16.56f, 14.81f);
                    }
                    else if (overlay == "11")
                    {
                        playerOverlayIcon.transform.localPosition = new Vector2(-14.74451f, 20.39122f);
                    }
                    else if (overlay == "13")
                    {
                        playerOverlayIcon.transform.localPosition = new Vector2(-16.54019f, 14.70365f);
                    }
                    entryInfo.SetActive(true);
                }
            }
        }
        else
        {
            UpdateStatus(true, "Failed to fetch leaderboard stats");
        }
        scoreBackButton.interactable = true;
        scoreRefreshButton.interactable = true;
    }

    async void GetTopPlayersBerry(int showAmount)
    {
        berryShowTypeDropdown.interactable = false;
        berryBackButton.interactable = false;
        berryRefreshButton.interactable = false;
        foreach (Transform item in berryContent.transform)
        {
            if (item.gameObject.activeSelf)
            {
                Destroy(item.gameObject);
            }
        }
        UpdateStatus(true, "Loading...");
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("showType", showAmount.ToString());
        dataForm.AddField("type", "1");
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "getTopPlayers.php", dataForm.GetWWWForm());
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            UpdateStatus(false);
            string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
            if (response == "-999")
            {
                UpdateStatus(true, "Server error while fetching data");
            }
            else if (response == "-998")
            {
                UpdateStatus(true, "Client version too outdated to access servers");
            }
            else if (response == "-997")
            {
                UpdateStatus(true, "Encryption/decryption issues");
            }
            else if (response == "-1")
            {
                UpdateStatus(true, "No entries for this leaderboard found!");
            }
            else
            {
                var splitResponse = response.Split(';');
                for (int i = 0; i < splitResponse.Length; i++)
                {
                    var entry = splitResponse[i];
                    var split = entry.Split(":");
                    var username = Encoding.UTF8.GetString(Convert.FromBase64String(split[0]));
                    var highScore = split[1];
                    var icon = split[2];
                    var overlay = split[3];
                    var uid = split[4];

                    var entryInfo = Instantiate(berrySampleObject, berryContent.transform);
                    var usernameText = entryInfo.transform.GetChild(0).GetComponent<TMP_Text>();
                    var playerIcon = usernameText.transform.GetChild(0).GetComponent<Image>();
                    var playerOverlayIcon = playerIcon.transform.GetChild(0).GetComponent<Image>();
                    var highScoreText = entryInfo.transform.GetChild(1).GetComponent<TMP_Text>();

                    usernameText.text = $"{username} (#{i + 1})";
                    highScoreText.text += Tools.FormatWithCommas(highScore);
                    playerIcon.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + icon);
                    if (icon == "1")
                    {
                        playerIcon.sprite = Tools.GetIconForUser(int.Parse(uid));
                    }
                    playerOverlayIcon.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + overlay);
                    if (overlay == "0")
                    {
                        playerOverlayIcon.gameObject.SetActive(false);
                    }
                    else if (overlay == "8")
                    {
                        playerOverlayIcon.transform.localPosition = new Vector2(-16.56f, 14.81f);
                    }
                    else if (overlay == "11")
                    {
                        playerOverlayIcon.transform.localPosition = new Vector2(-14.74451f, 20.39122f);
                    }
                    else if (overlay == "13")
                    {
                        playerOverlayIcon.transform.localPosition = new Vector2(-16.54019f, 14.70365f);
                    }
                    entryInfo.SetActive(true);
                }
            }
        }
        else
        {
            UpdateStatus(true, "Failed to fetch leaderboard stats");
        }
        berryShowTypeDropdown.interactable = true;
        berryBackButton.interactable = true;
        berryRefreshButton.interactable = true;
    }

    private void UpdateStatus(bool enabled, string message = "")
    {
        statusText.gameObject.SetActive(enabled);
        statusText.text = message;
    }
}