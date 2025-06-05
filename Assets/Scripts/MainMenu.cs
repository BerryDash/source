using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Button exitButton;
    public TMP_Text updateText;
    public Button updateButton;

    void Awake()
    {
        LatestVersionText.Instance.text = updateText;
        LatestVersionText.Instance.updateButton = updateButton;
        LatestVersionText.Instance.RefreshText();

        if (Application.isMobilePlatform || Application.isEditor || Application.platform == RuntimePlatform.WebGLPlayer)
        {
            exitButton.gameObject.SetActive(false);
        }
        else
        {
            exitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
    }
}
