using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttachmentItemUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public GameObject outlineHighlight;
    public Button button;

    public AttachmentData Data { get; private set; }

    private System.Action<AttachmentData> onClickCallback;

    public void Setup(AttachmentData attachmentData, System.Action<AttachmentData> onClick)
    {
        Data = attachmentData;
        iconImage.sprite = Data.icon;
        nameText.text = Data.attachmentName;
        onClickCallback = onClick;

        outlineHighlight.SetActive(false);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickCallback?.Invoke(Data));
    }

    public void SetSelected(bool selected)
    {
        outlineHighlight.SetActive(selected);
    }
}