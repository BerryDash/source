using UnityEngine;
using UnityEngine.UI;

public class GamePlayerPauseMenu : MonoBehaviour
{
    public Button backButton;
    public Button continueButton;
    public AudioSource songLoop;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Awake()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 1f);
        backButton.onClick.AddListener(async () =>
        {
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");
        });
        continueButton.onClick.AddListener(GamePlayer.instance.DisablePause);
        musicSlider.onValueChanged.AddListener(value =>
        {
            PlayerPrefs.SetFloat("musicVolume", value);
            PlayerPrefs.Save();
            songLoop.volume = value;
        });
        sfxSlider.onValueChanged.AddListener(value =>
        {
            PlayerPrefs.SetFloat("sfxVolume", value);
            PlayerPrefs.Save();
        });
    }
}