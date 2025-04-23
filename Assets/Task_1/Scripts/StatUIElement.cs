using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatUIElement : MonoBehaviour
{
    public Image backgroundImage;
    public Image iconImage;
    public TextMeshProUGUI statValueText;
    public TextMeshProUGUI statChangeText;

    private static readonly Color positiveBG = new Color32(0x2F, 0x9A, 0x5A, 180);
    private static readonly Color positiveIcon = new Color32(0x2F, 0x9A, 0x5A, 255);

    private static readonly Color negativeBG = new Color32(0xA9, 0x32, 0x32, 180);
    private static readonly Color negativeIcon = new Color32(0xA9, 0x32, 0x32, 255);

    private static readonly Color neutralBG = new Color32(0x3C, 0x82, 0xD9, 180);
    private static readonly Color neutralIcon = new Color32(0x49, 0x8A, 0xDB, 255);

    public void SetValue(float baseValue, float change)
    {
        statValueText.text = baseValue.ToString("F1");

        if (change == 0)
        {
            statChangeText.text = "";
            backgroundImage.color = neutralBG;
            iconImage.color = neutralIcon;
        }
        else
        {
            string sign = change > 0 ? "+" : "";
            statChangeText.text = $"{sign}{change.ToString("F1")}";
            statChangeText.color = change > 0 ? Color.green : Color.red;

            backgroundImage.color = change > 0 ? positiveBG : negativeBG;
            iconImage.color = change > 0 ? positiveIcon : negativeIcon;
        }
    }
}