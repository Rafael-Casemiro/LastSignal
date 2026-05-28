using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LastSignal.Core
{
    public class ResourceManager : MonoBehaviour
    {
        [Header("Starting Resources")]
        [SerializeField] private int startingFood = 100;
        [SerializeField] private int startingWater = 100;
        [SerializeField] private int startingFuel = 50;
        [SerializeField] private int startingMedicine = 30;
        [SerializeField] private int startingEnergy = 80;
        [SerializeField] private int startingCredits = 500;

        private Dictionary<CommodityType, int> _stockpile = new();

        public UnityEvent<CommodityType, int> OnResourceChanged = new();
        public UnityEvent OnResourcesDepleted = new();

        public void Initialize()
        {
            _stockpile = new Dictionary<CommodityType, int>
            {
                { CommodityType.Food,     startingFood },
                { CommodityType.Water,    startingWater },
                { CommodityType.Fuel,     startingFuel },
                { CommodityType.Medicine, startingMedicine },
                { CommodityType.Energy,   startingEnergy },
                { CommodityType.Credits,  startingCredits },
            };
            foreach (var kvp in _stockpile)
                OnResourceChanged.Invoke(kvp.Key, kvp.Value);
        }

        public int Get(CommodityType type)
        {
            return _stockpile.TryGetValue(type, out int val) ? val : 0;
        }

        public bool TrySpend(CommodityType type, int amount)
        {
            if (Get(type) < amount) return false;
            Modify(type, -amount);
            return true;
        }

        public void Receive(CommodityType type, int amount)
        {
            Modify(type, amount);
        }

        private void Modify(CommodityType type, int delta)
        {
            if (!_stockpile.ContainsKey(type)) _stockpile[type] = 0;
            _stockpile[type] = Mathf.Max(0, _stockpile[type] + delta);
            OnResourceChanged.Invoke(type, _stockpile[type]);

            CheckDepletion();
        }

        public void Set(CommodityType type, int value)
        {
            if (!_stockpile.ContainsKey(type)) return;
            _stockpile[type] = Mathf.Max(0, value);
            OnResourceChanged.Invoke(type, _stockpile[type]);
        }

        private void CheckDepletion()
        {
            if (Get(CommodityType.Food) <= 0 || Get(CommodityType.Water) <= 0)
                OnResourcesDepleted.Invoke();
        }
    }

    public enum CommodityType
    {
        Food,
        Water,
        Fuel,
        Medicine,
        Energy,
        Credits
    }
}
