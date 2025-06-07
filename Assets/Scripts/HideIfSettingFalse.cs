using UnityEngine;

public class HideIfSettingFalse : MonoBehaviour
{
    public string setting;
    public bool reverse;

    void Awake()
    {
        gameObject.SetActive(PlayerPrefs.GetInt(setting, 0) == (reverse ? 0 : 1));
    }
}