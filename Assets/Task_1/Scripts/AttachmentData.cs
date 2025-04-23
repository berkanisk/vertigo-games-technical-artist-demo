using UnityEngine;

public enum AttachmentCategory
{
    Sight,
    Mag,
    Barrel,
    Stock,
    Tactical
}

[CreateAssetMenu(menuName = "Attachment/Attachment Data", fileName = "NewAttachmentData")]
public class AttachmentData : ScriptableObject
{
    public string attachmentName;
    public AttachmentCategory category;
    public Sprite icon;

    [Header("Stat Modifiers")]
    public float powerMod;
    public float damageMod;
    public float accuracyMod;
    public float rangeMod;
    public float fireRateMod;
    public float speedMod;

}