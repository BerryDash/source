using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardsMenu : MonoBehaviour
{
    public GameObject content;
    public TMP_Dropdown showAmountDropdown;
    public TMP_Text statusText;
    public Button backButton;
    public Button refreshButton;
    public GameObject sampleObject;

    private void Awake()
    {
        GetTopPlayers(0);
        showAmountDropdown.onValueChanged.AddListener(value =>
        {
            GetTopPlayers(value);
        });
        backButton.onClick.AddListener(async () =>
        {
            await SceneManager.LoadSceneAsync("MainMenu");
        });
        refreshButton.onClick.AddListener(() =>
        {
            GetTopPlayers(showAmountDropdown.value);
        });
    }

    async void GetTopPlayers(int showAmount)
    {
        refreshButton.interactable = false;
        showAmountDropdown.interactable = false;
        foreach (Transform item in content.transform)
        {
            if (item.gameObject.activeSelf)
            {
                Destroy(item.gameObject);
            }
        }
        UpdateStatus(true, "Loading...");
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("showAmount", showAmount.ToString());
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
                return;
            }
            else if (response == "-998")
            {
                UpdateStatus(true, "Client version too outdated to access servers");
                return;
            }
            else if (response == "-1")
            {
                UpdateStatus(true, "Server returned no results");
                return;
            }
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

                var entryInfo = Instantiate(sampleObject, content.transform);
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
        else
        {
            UpdateStatus(true, "Failed to fetch leaderboard stats");
        }
        refreshButton.interactable = true;
        showAmountDropdown.interactable = true;
    }

    private void UpdateStatus(bool enabled, string message = "")
    {
        statusText.gameObject.SetActive(enabled);
        statusText.text = message;
    }
}