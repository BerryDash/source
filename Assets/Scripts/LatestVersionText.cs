using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LatestVersionText : MonoBehaviour
{
    public Button updateButton;
    private TMP_Text text;

    void Awake()
    {
        updateButton.onClick.AddListener(() =>
        {
            Application.OpenURL("https://berrydash.lncvrt.xyz/download");
        });
        text = gameObject.GetComponent<TMP_Text>();
    }

    void Start()
    {
        GetLatestVersion();
    }

    async void GetLatestVersion()
    {
        using UnityWebRequest request = UnityWebRequest.Get("https://berrydash.lncvrt.xyz/database/getLatestVersion.php");
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            text.text = "Latest: N/A";
            return;
        }
        string response = request.downloadHandler.text;
        text.text = "Latest: v" + response;
        if (response != Application.version)
        {
            updateButton.gameObject.SetActive(true);
        }
    }
}
