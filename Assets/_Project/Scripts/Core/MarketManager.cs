using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LastSignal.Core
{
    public class MarketManager : MonoBehaviour
    {
        public static MarketManager Instance { get; private set; }

        [SerializeField] private CommodityMarketData[] commodities;

        private Dictionary<CommodityType, CommodityMarketData> _market = new();

        public UnityEvent<CommodityType, float, float> OnPriceChanged = new();

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            foreach (var c in commodities)
                _market[c.type] = c;
        }

        void Start()
        {
            var tm = GameManager.Instance?.Time;
            if (tm != null) tm.OnDayChanged.AddListener(OnDayAdvanced);
        }

        void OnDestroy()
        {
            var tm = GameManager.Instance?.Time;
            if (tm != null) tm.OnDayChanged.RemoveListener(OnDayAdvanced);
        }

        void OnDayAdvanced(int day)
        {
            foreach (var entry in _market.Values)
                FluctuatePrice(entry);
        }

        void FluctuatePrice(CommodityMarketData data)
        {
            float previous = data.currentPrice;
            float delta = UnityEngine.Random.Range(-data.volatility, data.volatility);
            data.currentPrice = Mathf.Clamp(data.currentPrice + delta, data.minPrice, data.maxPrice);
            OnPriceChanged.Invoke(data.type, previous, data.currentPrice);
        }

        // Aplica choque de preço (evento externo, ex: tempestade destruiu plantações)
        public void ApplyPriceShock(CommodityType type, float percentageChange)
        {
            if (!_market.TryGetValue(type, out var data)) return;
            float previous = data.currentPrice;
            data.currentPrice = Mathf.Clamp(
                data.currentPrice * (1f + percentageChange / 100f),
                data.minPrice, data.maxPrice);
            OnPriceChanged.Invoke(type, previous, data.currentPrice);
        }

        public float GetPrice(CommodityType type)
            => _market.TryGetValue(type, out var d) ? d.currentPrice : 0f;

        public float GetPriceChange(CommodityType type)
        {
            if (!_market.TryGetValue(type, out var d)) return 0f;
            return d.previousPrice > 0 ? (d.currentPrice - d.previousPrice) / d.previousPrice * 100f : 0f;
        }

        public string GetPriceTrend(CommodityType type)
        {
            float change = GetPriceChange(type);
            if (Mathf.Abs(change) < 1f) return "";
            return change > 0 ? $"↑ +{change:F0}%" : $"↓ {change:F0}%";
        }
    }

    [Serializable]
    public class CommodityMarketData
    {
        public CommodityType type;
        public float currentPrice = 10f;
        public float minPrice = 2f;
        public float maxPrice = 100f;
        [Tooltip("Variação máxima por dia (unidades absolutas)")]
        public float volatility = 3f;
        [HideInInspector] public float previousPrice;

        public void SnapshotPrice() => previousPrice = currentPrice;
    }
}
