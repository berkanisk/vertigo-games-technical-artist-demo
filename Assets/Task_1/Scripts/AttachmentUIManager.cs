using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttachmentUIManager : MonoBehaviour
{
    [Header("Equip Button")]
    public Button equipButton;
    public TextMeshProUGUI equipButtonText;
    public Color equippedColor = Color.gray;
    public Color defaultColor = Color.white;

    [Header("Attachment List UI")]
    public Transform attachmentListParent;
    public GameObject attachmentItemPrefab;

    [Header("Stats & Weapon Controller")]
    public WeaponAttachmentController weaponController;
    public StatManager statManager;

    [Header("Category Buttons")]
    public List<AttachmentCategoryButton> categoryButtons;
    public List<AttachmentCategory> categoryOrder;

    [Header("Attachments by Category")]
    public List<AttachmentData> sightAttachments;
    public List<AttachmentData> magAttachments;
    public List<AttachmentData> barrelAttachments;
    public List<AttachmentData> stockAttachments;
    public List<AttachmentData> tacticalAttachments;

    private Dictionary<AttachmentCategory, List<AttachmentData>> allAttachmentsByCategory;
    private Dictionary<AttachmentCategory, AttachmentData> equippedAttachments = new();

    private AttachmentData previewedData = null;
    private AttachmentCategory activeCategory;
    private List<AttachmentItemUI> spawnedItems = new();
    private AttachmentItemUI selectedItem;

    private void Start()
    {
        allAttachmentsByCategory = new()
        {
            { AttachmentCategory.Sight, sightAttachments },
            { AttachmentCategory.Mag, magAttachments },
            { AttachmentCategory.Barrel, barrelAttachments },
            { AttachmentCategory.Stock, stockAttachments },
            { AttachmentCategory.Tactical, tacticalAttachments }
        };
        
        foreach (var pair in equippedAttachments)
        {
            int index = categoryOrder.IndexOf(pair.Key);
            if (index >= 0 && index < categoryButtons.Count)
            {
                categoryButtons[index].SetIcon(pair.Value.icon);
            }
        }

        OnAttachmentCategoryClicked(AttachmentCategory.Sight);
    }


   public void OnAttachmentCategoryClicked(AttachmentCategory category)
   {
        if (previewedData != null && previewedData.category != category) 
        {
            bool isEquipped = equippedAttachments.TryGetValue(previewedData.category, out var equipped) &&
                              equipped == previewedData;

            if (!isEquipped)
            {
                weaponController.ResetAttachment(previewedData.category);
                
                if (equippedAttachments.TryGetValue(previewedData.category, out var equippedData))
                {
                    weaponController.PreviewAttachment(previewedData.category, equippedData.name);
                }
            }

            previewedData = null;
            statManager.ApplyEquippedStats(equippedAttachments);
        }

        activeCategory = category;
        
        for (int i = 0; i < categoryButtons.Count; i++)
            categoryButtons[i].SetSelected(categoryOrder[i] == category);
        
        foreach (var item in spawnedItems)
            Destroy(item.gameObject);
        spawnedItems.Clear();
        
        var list = GetAttachmentListByCategory(category);

        foreach (var data in list)
        {
            var itemGO = Instantiate(attachmentItemPrefab, attachmentListParent);
            var itemUI = itemGO.GetComponent<AttachmentItemUI>();
            itemUI.Setup(data, OnAttachmentSelected);
            spawnedItems.Add(itemUI);
        }

        selectedItem = null;
        
        if (equippedAttachments.TryGetValue(category, out var equippedAttachment))
        {
            weaponController.PreviewAttachment(category, equippedAttachment.name);

            foreach (var item in spawnedItems)
            {
                bool isEquipped = item.Data == equippedAttachment;
                item.SetSelected(isEquipped);

                if (isEquipped)
                    selectedItem = item;
            }
        }
        else
        {
            weaponController.ResetAttachment(category);
            
            foreach (var item in spawnedItems)
            {
                item.SetSelected(false); 
            } 
        }

        statManager.ApplyEquippedStats(equippedAttachments);
        UpdateEquipButtonUI();
    }



    private List<AttachmentData> GetAttachmentListByCategory(AttachmentCategory category)
    {
        return category switch
        {
            AttachmentCategory.Sight => sightAttachments,
            AttachmentCategory.Mag => magAttachments,
            AttachmentCategory.Barrel => barrelAttachments,
            AttachmentCategory.Stock => stockAttachments,
            AttachmentCategory.Tactical => tacticalAttachments,
            _ => new List<AttachmentData>()
        };
    }

    private void OnAttachmentSelected(AttachmentData selectedData)
    {
        previewedData = selectedData;

        weaponController.PreviewAttachment(selectedData.category, selectedData.name);
        statManager.PreviewStatsWith(equippedAttachments, previewedData);

        foreach (var item in spawnedItems)
            item.SetSelected(false);

        selectedItem = spawnedItems.Find(x => x.Data == selectedData);
        selectedItem?.SetSelected(true);

        UpdateEquipButtonUI();
    }

    public void OnEquipButtonClicked()
    {
        if (previewedData == null) return;

        equippedAttachments[previewedData.category] = previewedData;

        statManager.ApplyEquippedStats(equippedAttachments);
        
        int index = categoryOrder.IndexOf(previewedData.category);
        if (index >= 0 && index < categoryButtons.Count)
        {
            categoryButtons[index].SetIcon(previewedData.icon);
        }

        UpdateEquipButtonUI();
    }


    public void OnResetButtonClicked()
    {
        foreach (var category in categoryOrder)
        {
            weaponController.ResetAttachment(category);
        }
        
        equippedAttachments.Clear();
        
        statManager.ApplyEquippedStats(equippedAttachments);
        
        foreach (var item in spawnedItems)
            item.SetSelected(false);

        selectedItem = null;
        previewedData = null;
        
        for (int i = 0; i < categoryButtons.Count; i++)
        {
            var category = categoryOrder[i];
            var defaultIcon = GetAttachmentListByCategory(category)[0].icon;
            categoryButtons[i].SetIcon(defaultIcon);
        }

        UpdateEquipButtonUI();
    }



    private void UpdateEquipButtonUI()
    {
        if (equipButton == null || equipButtonText == null)
            return;

        if (previewedData == null)
        {
            equipButton.gameObject.SetActive(false);
            return;
        }

        equipButton.gameObject.SetActive(true);

        bool isEquipped = equippedAttachments.TryGetValue(previewedData.category, out var equipped) &&
                          equipped == previewedData;

        equipButtonText.text = isEquipped ? "EQUIPPED" : "EQUIP";
        equipButton.interactable = !isEquipped;
        equipButton.image.color = isEquipped ? equippedColor : defaultColor;
    }
    
    public void OnSightCategoryButtonClicked() => OnAttachmentCategoryClicked(AttachmentCategory.Sight);
    public void OnMagCategoryButtonClicked() => OnAttachmentCategoryClicked(AttachmentCategory.Mag);
    public void OnBarrelCategoryButtonClicked() => OnAttachmentCategoryClicked(AttachmentCategory.Barrel);
    public void OnStockCategoryButtonClicked() => OnAttachmentCategoryClicked(AttachmentCategory.Stock);
    public void OnTacticalCategoryButtonClicked() => OnAttachmentCategoryClicked(AttachmentCategory.Tactical);
}
