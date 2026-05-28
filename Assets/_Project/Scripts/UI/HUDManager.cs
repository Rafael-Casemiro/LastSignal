using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LastSignal.Core;

namespace LastSignal.UI
{
    public class HUDManager : MonoBehaviour
    {
        [Header("Day Info")]
        [SerializeField] private TextMeshProUGUI dayLabel;
        [SerializeField] private TextMeshProUGUI daysRemainingLabel;

        [Header("Day Progress")]
        [SerializeField] private Slider dayProgressBar;

        [Header("Resources")]
        [SerializeField] private TextMeshProUGUI foodLabel;
        [SerializeField] private TextMeshProUGUI waterLabel;
        [SerializeField] private TextMeshProUGUI fuelLabel;
        [SerializeField] private TextMeshProUGUI medicineLabel;
        [SerializeField] private TextMeshProUGUI energyLabel;
        [SerializeField] private TextMeshProUGUI creditsLabel;

        [Header("Colors")]
        [SerializeField] private Color normalColor   = new Color(0.2f, 0.9f, 0.4f);
        [SerializeField] private Color criticalColor = new Color(1f, 0.2f, 0.2f);
        [SerializeField] private Color flashColor    = Color.white;
        [SerializeField] private int criticalThreshold = 20;
        [SerializeField] private float flashDuration   = 0.15f;

        private Dictionary<TextMeshProUGUI, Coroutine> _flashCoroutines = new();

        private void Start()
        {
            var tm = GameManager.Instance?.Time;
            if (tm != null)
            {
                tm.OnDayChanged.AddListener(OnDayChanged);
                OnDayChanged(tm.CurrentDay);
            }

            var rm = GameManager.Instance?.Resources;
            if (rm != null)
                rm.OnResourceChanged.AddListener(OnResourceChanged);

            var mm = MarketManager.Instance;
            if (mm != null)
                mm.OnPriceChanged.AddListener(OnPriceChanged);

            RefreshAll();
        }

        private void Update()
        {
            if (dayProgressBar == null) return;
            var tm = GameManager.Instance?.Time;
            if (tm != null)
                dayProgressBar.value = tm.DayProgress;
        }

        private void OnDestroy()
        {
            GameManager.Instance?.Time?.OnDayChanged.RemoveListener(OnDayChanged);
            GameManager.Instance?.Resources?.OnResourceChanged.RemoveListener(OnResourceChanged);
            if (MarketManager.Instance != null)
                MarketManager.Instance.OnPriceChanged.RemoveListener(OnPriceChanged);
        }

        private void OnDayChanged(int day)
        {
            if (dayLabel != null) dayLabel.text = $"DIA {day}";
            var remaining = GameManager.Instance?.Time?.DaysRemaining ?? 0;
            if (daysRemainingLabel != null)
                daysRemainingLabel.text = $"COLAPSO EM {remaining}d";
        }

        private void OnResourceChanged(CommodityType type, int value)
        {
            switch (type)
            {
                case CommodityType.Food:     RefreshResource(foodLabel,     "COMIDA", value); break;
                case CommodityType.Water:    RefreshResource(waterLabel,    "ÁGUA",   value); break;
                case CommodityType.Fuel:     RefreshResource(fuelLabel,     "COMB",   value); break;
                case CommodityType.Medicine: RefreshResource(medicineLabel, "MED",    value); break;
                case CommodityType.Energy:   RefreshResource(energyLabel,   "ENERG",  value); break;
                case CommodityType.Credits:  RefreshResource(creditsLabel,  "CRÉD",   value); break;
            }
        }

        private void OnPriceChanged(CommodityType type, float previous, float current)
        {
            var rm = GameManager.Instance?.Resources;
            if (rm == null) return;
            var mm = MarketManager.Instance;

            switch (type)
            {
                case CommodityType.Food:
                    RefreshResourceWithPrice(foodLabel,  "COMIDA", rm.Get(CommodityType.Food),  mm?.GetPriceTrend(type));
                    break;
                case CommodityType.Water:
                    RefreshResourceWithPrice(waterLabel, "ÁGUA",   rm.Get(CommodityType.Water), mm?.GetPriceTrend(type));
                    break;
                case CommodityType.Fuel:
                    RefreshResourceWithPrice(fuelLabel,  "COMB",   rm.Get(CommodityType.Fuel),  mm?.GetPriceTrend(type));
                    break;
            }
        }

        private void RefreshAll()
        {
            var rm = GameManager.Instance?.Resources;
            if (rm == null) return;
            RefreshResource(foodLabel,     "COMIDA", rm.Get(CommodityType.Food));
            RefreshResource(waterLabel,    "ÁGUA",   rm.Get(CommodityType.Water));
            RefreshResource(fuelLabel,     "COMB",   rm.Get(CommodityType.Fuel));
            RefreshResource(medicineLabel, "MED",    rm.Get(CommodityType.Medicine));
            RefreshResource(energyLabel,   "ENERG",  rm.Get(CommodityType.Energy));
            RefreshResource(creditsLabel,  "CRÉD",   rm.Get(CommodityType.Credits));
        }

        private void RefreshResource(TextMeshProUGUI label, string name, int value)
        {
            if (label == null) return;
            label.text = $"{name}: {value}";
            Color targetColor = value <= criticalThreshold ? criticalColor : normalColor;

            if (_flashCoroutines.TryGetValue(label, out var existing) && existing != null)
                StopCoroutine(existing);
            _flashCoroutines[label] = StartCoroutine(FlashLabel(label, targetColor));
        }

        private void RefreshResourceWithPrice(TextMeshProUGUI label, string name, int value, string trend)
        {
            if (label == null) return;
            string trendStr = string.IsNullOrEmpty(trend) ? "" : $" ({trend})";
            label.text = $"{name}: {value}{trendStr}";
            label.color = value <= criticalThreshold ? criticalColor : normalColor;
        }

        private IEnumerator FlashLabel(TextMeshProUGUI label, Color targetColor)
        {
            label.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            if (label != null)
                label.color = targetColor;
        }
    }
}
