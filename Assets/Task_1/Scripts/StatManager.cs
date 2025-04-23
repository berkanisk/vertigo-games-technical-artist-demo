using UnityEngine;
using System.Collections.Generic;

public class StatManager : MonoBehaviour
{
    [System.Serializable]
    public class StatUISet
    {
        public StatUIElement power;
        public StatUIElement damage;
        public StatUIElement accuracy;
        public StatUIElement range;
        public StatUIElement fireRate;
        public StatUIElement speed;
    }

    [Header("UI Stat Sets")]
    public List<StatUISet> UISets = new();

    [Header("Base Stats")]
    public float basePower = 56;
    public float baseDamage = 82;
    public float baseAccuracy = 44;
    public float baseRange = 22;
    public float baseFireRate = 800;
    public float baseSpeed = 96;

    public void PreviewStatsWith(Dictionary<AttachmentCategory, AttachmentData> equipped, AttachmentData preview)
    {
        float totalPower = basePower;
        float totalDamage = baseDamage;
        float totalAccuracy = baseAccuracy;
        float totalRange = baseRange;
        float totalFireRate = baseFireRate;
        float totalSpeed = baseSpeed;
        
        foreach (var kvp in equipped)
        {
            var data = kvp.Value;
            totalPower += data.powerMod;
            totalDamage += data.damageMod;
            totalAccuracy += data.accuracyMod;
            totalRange += data.rangeMod;
            totalFireRate += data.fireRateMod;
            totalSpeed += data.speedMod;
        }
        
        float previewDeltaPower = 0;
        float previewDeltaDamage = 0;
        float previewDeltaAccuracy = 0;
        float previewDeltaRange = 0;
        float previewDeltaFireRate = 0;
        float previewDeltaSpeed = 0;

        if (preview != null)
        {
            if (equipped.TryGetValue(preview.category, out var current))
            {
                previewDeltaPower = preview.powerMod - current.powerMod;
                previewDeltaDamage = preview.damageMod - current.damageMod;
                previewDeltaAccuracy = preview.accuracyMod - current.accuracyMod;
                previewDeltaRange = preview.rangeMod - current.rangeMod;
                previewDeltaFireRate = preview.fireRateMod - current.fireRateMod;
                previewDeltaSpeed = preview.speedMod - current.speedMod;
            }
            else
            {
                previewDeltaPower = preview.powerMod;
                previewDeltaDamage = preview.damageMod;
                previewDeltaAccuracy = preview.accuracyMod;
                previewDeltaRange = preview.rangeMod;
                previewDeltaFireRate = preview.fireRateMod;
                previewDeltaSpeed = preview.speedMod;
            }
        }
        
        foreach (var ui in UISets)
        {
            ui.power.SetValue(totalPower, previewDeltaPower);
            ui.damage.SetValue(totalDamage, previewDeltaDamage);
            ui.accuracy.SetValue(totalAccuracy, previewDeltaAccuracy);
            ui.range.SetValue(totalRange, previewDeltaRange);
            ui.fireRate.SetValue(totalFireRate, previewDeltaFireRate);
            ui.speed.SetValue(totalSpeed, previewDeltaSpeed);
        }
    }

    public void ApplyEquippedStats(Dictionary<AttachmentCategory, AttachmentData> equipped)
    {
        PreviewStatsWith(equipped, null);
    }

    public void ResetStats()
    {
        PreviewStatsWith(new Dictionary<AttachmentCategory, AttachmentData>(), null);
    }
}
