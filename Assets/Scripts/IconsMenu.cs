using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Iconsmenu : MonoBehaviour
{
    public GameObject iconsPanel;
    public GameObject overlaysPanel;
    public Button backButton;
    public Sprite defaultIcon;
    public Button placeholderButton;
    public TMP_Text selectionText;
    public Image previewBird;
    public Image previewOverlay;
    public Button icon1;
    public Button icon2;
    public Button icon3;
    public Button icon4;
    public Button icon5;
    public Button icon6;
    public Button icon7;
    public Button icon8;
    public Button overlay0;
    public Button overlay1;
    public Button overlay2;
    public Button overlay3;
    public Button overlay4;
    public Button overlay5;
    public Button overlay6;
    public Button overlay7;
    public Button overlay8;
    public Button overlay9;
    public GameObject previewBirdObject;

    private void Start()
    {
        SwitchToIcon();
        SelectOverlay(PlayerPrefs.GetInt("overlay", Mathf.Clamp(PlayerPrefs.GetInt("overlay", 0), 0, 9)));
        SelectIcon(PlayerPrefs.GetInt("icon", Mathf.Clamp(PlayerPrefs.GetInt("icon", 0), 1, 8)));
        if (PlayerPrefs.GetInt("icon", 0) == 7)
        {
            SelectOverlay(0);
            placeholderButton.interactable = false;
        }
        if (PlayerPrefs.GetInt("userID", 0) == 1)
        {
            defaultIcon = Resources.Load<Sprite>("Icons/Icons/bird_-1");
        }
        else if (PlayerPrefs.GetInt("userID", 0) == 2)
        {
            defaultIcon = Resources.Load<Sprite>("Icons/Icons/bird_-2");
        }
        else if (PlayerPrefs.GetInt("userID", 0) == 4)
        {
            defaultIcon = Resources.Load<Sprite>("Icons/Icons/bird_-3");
        }
        placeholderButton.onClick.AddListener(ToggleKit);
        backButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt("icon", Mathf.Clamp(PlayerPrefs.GetInt("icon", 0), 1, 8));
            PlayerPrefs.SetInt("overlay", Mathf.Clamp(PlayerPrefs.GetInt("overlay", 0), 0, 9));
            PlayerPrefs.Save();
            SceneManager.LoadSceneAsync("MainMenu");
        });
        icon1.onClick.AddListener(() => SelectIcon(1));
        icon2.onClick.AddListener(() => SelectIcon(2));
        icon3.onClick.AddListener(() => SelectIcon(3));
        icon4.onClick.AddListener(() => SelectIcon(4));
        icon5.onClick.AddListener(() => SelectIcon(5));
        icon6.onClick.AddListener(() => SelectIcon(6));
        icon7.onClick.AddListener(() => SelectIcon(7));
        icon8.onClick.AddListener(() => SelectIcon(8));
        overlay0.onClick.AddListener(() => SelectOverlay(0));
        overlay1.onClick.AddListener(() => SelectOverlay(1));
        overlay2.onClick.AddListener(() => SelectOverlay(2));
        overlay3.onClick.AddListener(() => SelectOverlay(3));
        overlay4.onClick.AddListener(() => SelectOverlay(4));
        overlay5.onClick.AddListener(() => SelectOverlay(5));
        overlay6.onClick.AddListener(() => SelectOverlay(6));
        overlay7.onClick.AddListener(() => SelectOverlay(7));
        overlay8.onClick.AddListener(() => SelectOverlay(8));
        overlay9.onClick.AddListener(() => SelectOverlay(9));
    }

    private void Update()
    {
        if (!Application.isMobilePlatform)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 screenPoint = Input.mousePosition;
                if (RectTransformUtility.RectangleContainsScreenPoint(previewBirdObject.GetComponent<RectTransform>(), screenPoint))
                {
                    float x = previewBirdObject.transform.localScale.x;
                    previewBirdObject.transform.localScale = new Vector3((x != 1f) ? 1 : (-1), 1f, 1f);
                }
            }
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 position = Input.GetTouch(0).position;
            if (RectTransformUtility.RectangleContainsScreenPoint(previewBirdObject.GetComponent<RectTransform>(), position))
            {
                float x2 = previewBirdObject.transform.localScale.x;
                previewBirdObject.transform.localScale = new Vector3((x2 != 1f) ? 1 : (-1), 1f, 1f);
            }
        }
    }

    private void SwitchToIcon()
    {
        iconsPanel.SetActive(value: true);
        overlaysPanel.SetActive(value: false);
        selectionText.text = "Icon selection";
        placeholderButton.GetComponentInChildren<TMP_Text>().text = "Overlays";
    }

    private void SwitchToOverlay()
    {
        iconsPanel.SetActive(value: false);
        overlaysPanel.SetActive(value: true);
        selectionText.text = "Overlay selection";
        placeholderButton.GetComponentInChildren<TMP_Text>().text = "Icons";
    }

    private void ToggleKit()
    {
        if (GetCurrentKit() == 1)
        {
            SwitchToOverlay();
        }
        else if (GetCurrentKit() == 2)
        {
            SwitchToIcon();
        }
    }

    private int GetCurrentKit()
    {
        if (iconsPanel.activeSelf)
        {
            return 1;
        }
        if (overlaysPanel.activeSelf)
        {
            return 2;
        }
        return 0;
    }

    private void SelectIcon(int iconID)
    {
        PlayerPrefs.SetInt("icon", iconID);
        PlayerPrefs.Save();
        icon1.interactable = (iconID != 1);
        icon2.interactable = (iconID != 2);
        icon3.interactable = (iconID != 3);
        icon4.interactable = (iconID != 4);
        icon5.interactable = (iconID != 5);
        icon6.interactable = (iconID != 6);
        icon7.interactable = (iconID != 7);
        icon8.interactable = (iconID != 8);
        previewBird.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + iconID);
        if (iconID == 1)
        {
            if (PlayerPrefs.GetInt("userID", 0) == 1)
            {
                previewBird.sprite = Resources.Load<Sprite>("Icons/Icons/bird_-1");
            }
            else if (PlayerPrefs.GetInt("userID", 0) == 2)
            {
                previewBird.sprite = Resources.Load<Sprite>("Icons/Icons/bird_-2");
            }
            else if (PlayerPrefs.GetInt("userID", 0) == 4)
            {
                previewBird.sprite = Resources.Load<Sprite>("Icons/Icons/bird_-3");
            }
        }
        if (iconID == 7)
        {
            SelectOverlay(0, false);
            placeholderButton.interactable = false;
        }
        else
        {
            SelectOverlay(PlayerPrefs.GetInt("pastOverlay", 0), false);
            placeholderButton.interactable = true;
        }
    }

    private void SelectOverlay(int overlayID, bool savePast = true)
    {
        if (savePast)
        {
            PlayerPrefs.SetInt("pastOverlay", PlayerPrefs.GetInt("overlay", 0));
        }
        PlayerPrefs.SetInt("overlay", overlayID);
        PlayerPrefs.Save();
        overlay0.interactable = (overlayID != 0);
        overlay1.interactable = (overlayID != 1);
        overlay2.interactable = (overlayID != 2);
        overlay3.interactable = (overlayID != 3);
        overlay4.interactable = (overlayID != 4);
        overlay5.interactable = (overlayID != 5);
        overlay6.interactable = (overlayID != 6);
        overlay7.interactable = (overlayID != 7);
        overlay8.interactable = (overlayID != 8);
        overlay9.interactable = (overlayID != 9);
        previewOverlay.rectTransform.localPosition = new Vector3(-32f, 44.66f, 0f);
        previewOverlay.gameObject.SetActive(true);
        if (overlayID == 8)
        {
            previewOverlay.rectTransform.localPosition = new Vector3(-35.36f, 31.6f, 0f);
        }
        if (overlayID == 0)
        {
            previewOverlay.gameObject.SetActive(false);
            previewOverlay.sprite = null;
        }
        else
        {
            previewOverlay.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + overlayID);
        }
    }
}
