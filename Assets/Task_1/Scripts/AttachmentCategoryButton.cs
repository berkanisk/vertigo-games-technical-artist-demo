using UnityEngine;
using UnityEngine.UI;

public class AttachmentCategoryButton : MonoBehaviour
{
    public GameObject selectedImage;
    public Image iconImage;

    public void SetSelected(bool isActive)
    {
        if (selectedImage != null)
            selectedImage.SetActive(isActive);
    }
    public void SetIcon(Sprite newIcon)
    {
        if (iconImage != null)
            iconImage.sprite = newIcon;
    }

}