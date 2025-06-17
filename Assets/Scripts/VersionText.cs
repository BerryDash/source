using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    private TMP_Text text;
    void Awake()
    {
        text = gameObject.GetComponent<TMP_Text>();
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            text.text = "Current: v" + "2.11";
        }
        else
        {
            text.text = "v" + "2.11";
        }
    }
}