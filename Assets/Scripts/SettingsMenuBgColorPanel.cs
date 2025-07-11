using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuBgColorPanel : MonoBehaviour
{
    public Slider rSlider;
    public Slider gSlider;
    public Slider bSlider;
    public TMP_InputField hexValue;
    public Image previewImage;
    public Button resetButton;

    void Awake()
    {
        var backgroundColor = PlayerPrefs.GetString("BackgroundColor", "58;58;58").Split(";");
        try
        {
            rSlider.value = int.Parse(backgroundColor[0]);
            gSlider.value = int.Parse(backgroundColor[1]);
            bSlider.value = int.Parse(backgroundColor[2]);
        }
        catch
        {
            Debug.LogError("Invalid BackgroundColor format");
            rSlider.value = 58; gSlider.value = 58; bSlider.value = 58;
        }
        SlidersChanged();

        rSlider.onValueChanged.AddListener(_ => SlidersChanged());
        gSlider.onValueChanged.AddListener(_ => SlidersChanged());
        bSlider.onValueChanged.AddListener(_ => SlidersChanged());

        hexValue.onValueChanged.AddListener(value =>
        {
            var v = value.StartsWith("#") ? value[1..] : value;
            if (v.Length == 6 && ColorUtility.TryParseHtmlString("#" + v, out var col))
            {
                rSlider.SetValueWithoutNotify(col.r * 255f);
                gSlider.SetValueWithoutNotify(col.g * 255f);
                bSlider.SetValueWithoutNotify(col.b * 255f);
                previewImage.color = col;
                PlayerPrefs.SetString("BackgroundColor", $"{(int)(col.r * 255)};{(int)(col.g * 255)};{(int)(col.b * 255)}");
                PlayerPrefs.Save();
            }
        });

        resetButton.onClick.AddListener(() =>
        {
            hexValue.text = "#3A3A3A";
        });
    }

    void SlidersChanged()
    {
        var col = new Color(rSlider.value / 255f, gSlider.value / 255f, bSlider.value / 255f);
        previewImage.color = col;
        hexValue.SetTextWithoutNotify($"#{ColorUtility.ToHtmlStringRGB(col)}");
        PlayerPrefs.SetString("BackgroundColor", $"{(int)rSlider.value};{(int)gSlider.value};{(int)bSlider.value}");
        PlayerPrefs.Save();
    }
}