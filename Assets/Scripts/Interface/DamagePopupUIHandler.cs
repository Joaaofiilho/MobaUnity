using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Interface
{
    public class DamagePopupUIHandler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI damageText;

        /// <summary>
        /// The text scales up depending on the damage dealt. maxDamageToScale tells the upper value to it to scale, so it
        /// doesn't scale towards infinity.
        /// </summary>
        [SerializeField] private float maxDamageToScale = 600f;

        [SerializeField] private float minTextSize = 14f;
        [SerializeField] private float maxTextSize = 24f;

        public void StyleText(AttackInformation attackInformation)
        {
            var damage = Mathf.RoundToInt(attackInformation.DamageAmount);

            damageText.text = damage.ToString(CultureInfo.CurrentCulture);
            damageText.fontSize = Mathf.Round(Mathf.Lerp(minTextSize, maxTextSize,
                Mathf.Min(damage, maxDamageToScale) / maxDamageToScale));
            damageText.outlineWidth = 0.3f;
            damageText.outlineColor = new Color32(0, 0, 0, 255);

            Color32 color = default;

            switch (attackInformation.DamageType)
            {
                case DamageType.AttackDamage:
                    color = ColorUtils.AttackDamageColor;
                    break;
                case DamageType.SpellDamage:
                    color = ColorUtils.SpellDamageColor;
                    break;
                case DamageType.TrueDamage:
                    color = ColorUtils.TrueDamageColor;
                    break;
            }

            damageText.color = color;
        }
    }
}