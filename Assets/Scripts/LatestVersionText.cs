using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LatestVersionText : MonoBehaviour
{
    public static LatestVersionText Instance;
    public TMP_Text text;
    public Button updateButton;
    private string latest = null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (updateButton != null)
        {
            updateButton.onClick.AddListener(() =>
            {
                Application.OpenURL("https://berrydash.lncvrt.xyz/download");
            });
        }
    }

    void Start()
    {
        RefreshText();
        if (Application.platform != RuntimePlatform.WebGLPlayer && latest == null) GetLatestVersion();
    }

    void OnEnable()
    {
        RefreshText();
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
            latest = "-1";
        }
        else
        {
            latest = request.downloadHandler.text;
        }
        RefreshText();
        if (latest != Application.version && updateButton != null) updateButton.gameObject.SetActive(true);
    }

    public void RefreshText()
    {
        if (text == null) return;
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            text.text = "";
        else if (latest == null)
            text.text = "Latest: Loading...";
        else if (latest == "-1")
            text.text = "Latest: N/A";
        else
            text.text = "Latest: v" + latest;
    }
}
