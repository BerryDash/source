using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    void Awake()
    {
        gameObject.GetComponent<TMP_Text>().text = "v" + Application.version;
    }
}
