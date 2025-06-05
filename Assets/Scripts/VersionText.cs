using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    void Awake()
    {
        gameObject.GetComponent<TMP_Text>().text = "Current: v" + Application.version;
    }
}
