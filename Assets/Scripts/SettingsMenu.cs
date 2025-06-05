using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Toggle setting1toggle;
    public Toggle setting2toggle;
    public Toggle setting3toggle;
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
            setting3toggle.isOn = PlayerPrefs.GetInt("Setting3", 1) == 1;
        }
        else
        {
            setting1toggle.interactable = false;
            setting2toggle.isOn = PlayerPrefs.GetInt("Setting2", 0) == 1;
            setting3toggle.interactable = false;
        }
        setting1toggle.onValueChanged.AddListener(value =>
        {
            Screen.fullScreen = value;
            PlayerPrefs.SetInt("Setting1", value ? 1 : 0);
        });
        setting2toggle.onValueChanged.AddListener(value =>
        {
            PlayerPrefs.SetInt("Setting2", value ? 1 : 0);
        });
        setting3toggle.onValueChanged.AddListener(value =>
        {
            PlayerPrefs.SetInt("Setting3", value ? 1 : 0);
            QualitySettings.vSyncCount = value ? 1 : -1;
        });
        musicSlider.onValueChanged.AddListener(value =>
        {
            PlayerPrefs.SetFloat("musicVolume", value);
            PlayerPrefs.Save();
        });
        sfxSlider.onValueChanged.AddListener(value =>
        {
            PlayerPrefs.SetFloat("sfxVolume", value);
            PlayerPrefs.Save();
        });
    }
}