using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SettingsMenu : MonoBehaviour
{
    public Toggle setting1toggle;
    public Toggle setting2toggle;
    public Toggle setting3toggle;
    public Toggle setting4toggle;
    public Toggle setting5toggle;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Awake()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 1f);
        if (!Application.isMobilePlatform)
        {
            setting1toggle.isOn = PlayerPrefs.GetInt("Setting1", 1) == 1;
            setting2toggle.isOn = PlayerPrefs.GetInt("Setting2", 0) == 1;
            setting3toggle.isOn = PlayerPrefs.GetInt("Setting3", 0) == 1;
            setting3toggle.interactable = PlayerPrefs.GetInt("Setting2", 0) == 1;
            setting4toggle.isOn = PlayerPrefs.GetInt("Setting4", 0) == 1;
            setting5toggle.isOn = PlayerPrefs.GetInt("Setting5", 1) == 1;
        }
        else
        {
            setting1toggle.interactable = false;
            setting2toggle.interactable = false;
            setting3toggle.isOn = PlayerPrefs.GetInt("Setting3", 0) == 1;
            setting4toggle.isOn = PlayerPrefs.GetInt("Setting4", 0) == 1;
            setting5toggle.interactable = false;
        }
        setting1toggle.onValueChanged.AddListener((bool value) =>
        {
            Screen.fullScreen = value;
            PlayerPrefs.SetInt("Setting1", value ? 1 : 0);
        });
        setting2toggle.onValueChanged.AddListener((bool value) =>
        {
            PlayerPrefs.SetInt("Setting2", value ? 1 : 0);
            setting3toggle.interactable = value;
            setting3toggle.isOn = value && setting3toggle.isOn;
            PlayerPrefs.SetInt("Setting3", setting3toggle.isOn ? 1 : 0);
        });
        setting3toggle.onValueChanged.AddListener((bool value) =>
        {
            PlayerPrefs.SetInt("Setting3", value ? 1 : 0);
        });
        setting4toggle.onValueChanged.AddListener((bool value) =>
        {
            PlayerPrefs.SetInt("Setting4", value ? 1 : 0);
        });
        setting5toggle.onValueChanged.AddListener((bool value) =>
        {
            PlayerPrefs.SetInt("Setting5", value ? 1 : 0);
            QualitySettings.vSyncCount = (value ? 1 : 0);
        });
        musicSlider.onValueChanged.AddListener((float value) =>
        {
            PlayerPrefs.SetFloat("musicVolume", value);
            PlayerPrefs.Save();
        });
        sfxSlider.onValueChanged.AddListener((float value) =>
        {
            PlayerPrefs.SetFloat("sfxVolume", value);
            PlayerPrefs.Save();
        });
    }
}