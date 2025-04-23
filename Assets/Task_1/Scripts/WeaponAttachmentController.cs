using System.Collections.Generic;
using UnityEngine;

public class WeaponAttachmentController : MonoBehaviour
{
    [Header("Weapon Name")]
    public string weaponName = "Dash";
    
    [Header("Sight Attachments")]
    public GameObject[] sights;

    [Header("Mag Attachments")]
    public GameObject[] mags;

    [Header("Barrel Attachments")]
    public GameObject[] barrels;

    [Header("Stock Attachments")]
    public GameObject[] stocks;

    [Header("Tactical Attachments")]
    public GameObject[] tacticals;

    private Dictionary<AttachmentCategory, GameObject[]> attachmentMap;

    private void Awake()
    {
        attachmentMap = new Dictionary<AttachmentCategory, GameObject[]>
        {
            { AttachmentCategory.Sight, sights },
            { AttachmentCategory.Mag, mags },
            { AttachmentCategory.Barrel, barrels },
            { AttachmentCategory.Stock, stocks },
            { AttachmentCategory.Tactical, tacticals }
        };
    }

    public void PreviewAttachment(AttachmentCategory category, string shortAttachmentName)
    {
        string cleanName = shortAttachmentName.Replace(" ", "").Trim();
        string fullName = $"{weaponName}_{cleanName}";

        foreach (var obj in attachmentMap[category])
            obj.SetActive(false);

        foreach (var obj in attachmentMap[category])
        {
            if (obj.name == fullName)
            {
                obj.SetActive(true);
                return;
            }
        }
    }

    public void ResetAttachment(AttachmentCategory category)
    {
        var list = attachmentMap[category];
        foreach (var obj in list)
            obj.SetActive(false);
        
        if (list.Length > 0)
            list[0].SetActive(true);
    }

}