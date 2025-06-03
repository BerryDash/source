using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour
{
    public TMP_Text text;
    public Button button;

    void Awake() {
        button.onClick.AddListener(() =>
        {
            Application.OpenURL("https://berrydash.lncvrt.xyz/download");
        });
    }

    void Start()
    {
        CheckUpdate();
    }

    async void CheckUpdate()
    {
        using UnityWebRequest request = UnityWebRequest.Get("https://berrydash.lncvrt.xyz/database/canLoadClient.php");
        request.SetRequestHeader("User-Agent", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            text.text = "Failed to check version";
            return;
        }
        string response = request.downloadHandler.text;
        if (response == "1")
        {
            await SceneManager.LoadSceneAsync("MainMenu");
        } else
        {
            text.text = "Outdated client! Please update your client to play";
            button.gameObject.SetActive(true);
        }
    }
}
