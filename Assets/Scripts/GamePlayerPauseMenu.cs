using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayerPauseMenu : MonoBehaviour
{
    public static GamePlayerPauseMenu Instance;
    public Button backButton;
    public Button continueButton;
    public Button editUiButton;
    public Button resetUiButton;
    public AudioSource songLoop;
    public Slider musicSlider;
    public Slider sfxSlider;
    public TMP_Text fpsText;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text boostText;
    public bool editingUI = false;

    void Awake()
    {
        Instance = this;
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
        editUiButton.onClick.AddListener(() =>
        {
            ToggleEditingUI();
        });
        resetUiButton.onClick.AddListener(() =>
        {
            fpsText.transform.localPosition = new Vector2(-432.5f, 375f);
            scoreText.transform.localPosition = new Vector2(0f, 290f);
            highScoreText.transform.localPosition = new Vector2(0f, 220f);
            boostText.transform.localPosition = new Vector2(0f, 170f);
            PlayerPrefs.DeleteKey("DraggedUIFPSText");
            PlayerPrefs.DeleteKey("DraggedUIScoreText");
            PlayerPrefs.DeleteKey("DraggedUIHighScoreText");
            PlayerPrefs.DeleteKey("DraggedUIBoostText");
        });
    }

    public void ToggleEditingUI()
    {
        editingUI = !editingUI;
        musicSlider.gameObject.SetActive(!musicSlider.gameObject.activeSelf);
        sfxSlider.gameObject.SetActive(!sfxSlider.gameObject.activeSelf);
        backButton.gameObject.SetActive(!backButton.gameObject.activeSelf);
        continueButton.gameObject.SetActive(!continueButton.gameObject.activeSelf);
        editUiButton.transform.GetChild(0).GetComponent<TMP_Text>().text = editUiButton.transform.GetChild(0).GetComponent<TMP_Text>().text == "Edit UI" ? "Done" : "Edit UI";
        resetUiButton.gameObject.SetActive(!resetUiButton.gameObject.activeSelf);
        fpsText.GetComponent<DraggableUI>().canDrag = !fpsText.GetComponent<DraggableUI>().canDrag;
        scoreText.GetComponent<DraggableUI>().canDrag = !scoreText.GetComponent<DraggableUI>().canDrag;
        highScoreText.GetComponent<DraggableUI>().canDrag = !highScoreText.GetComponent<DraggableUI>().canDrag;
        boostText.GetComponent<DraggableUI>().canDrag = !boostText.GetComponent<DraggableUI>().canDrag;
    }
}