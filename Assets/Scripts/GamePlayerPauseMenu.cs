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
        backButton.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");
        });
        continueButton.onClick.AddListener(Unpause);
        musicSlider.onValueChanged.AddListener((float value) =>
        {
            PlayerPrefs.SetFloat("musicVolume", value);
            PlayerPrefs.Save();
            songLoop.volume = value;
        });
        sfxSlider.onValueChanged.AddListener((float value) =>
        {
            PlayerPrefs.SetFloat("sfxVolume", value);
            PlayerPrefs.Save();
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }

    void Unpause()
    {
        songLoop.UnPause();
        gameObject.SetActive(false);
        GameObject[] berries = GameObject.FindGameObjectsWithTag("Berry");
        GameObject[] poisonberries = GameObject.FindGameObjectsWithTag("PoisonBerry");
        GameObject[] ultraberries = GameObject.FindGameObjectsWithTag("UltraBerry");
        GameObject[] slownessberries = GameObject.FindGameObjectsWithTag("SlowBerry");

        foreach (GameObject b in berries)
        {
            b.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, -3);
        }
        foreach (GameObject pb in poisonberries)
        {
            pb.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, -3);
        }
        foreach (GameObject ub in ultraberries)
        {
            ub.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, -3);
        }
        foreach (GameObject sb in slownessberries)
        {
            sb.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, -3);
        }
    }
}
