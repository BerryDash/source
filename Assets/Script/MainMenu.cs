using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Button exitButton;

    void Awake()
    {
        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
