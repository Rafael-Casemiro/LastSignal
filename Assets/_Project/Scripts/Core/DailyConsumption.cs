using UnityEngine;

namespace LastSignal.Core
{
    // Drena recursos automaticamente a cada dia. Simula o custo de manter o bunker vivo.
    public class DailyConsumption : MonoBehaviour
    {
        [Header("Consumo diário por sobrevivente")]
        [SerializeField] private int foodPerDay     = 5;
        [SerializeField] private int waterPerDay    = 4;
        [SerializeField] private int fuelPerDay     = 2;
        [SerializeField] private int energyPerDay   = 3;
        [SerializeField] private int medicinePerDay = 1;

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
            var rm = GameManager.Instance?.Resources;
            if (rm == null) return;

            rm.TrySpend(CommodityType.Food,     foodPerDay);
            rm.TrySpend(CommodityType.Water,    waterPerDay);
            rm.TrySpend(CommodityType.Fuel,     fuelPerDay);
            rm.TrySpend(CommodityType.Energy,   energyPerDay);
            rm.TrySpend(CommodityType.Medicine, medicinePerDay);

            CheckCritical(rm, day);
        }

        void CheckCritical(ResourceManager rm, int day)
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            if (rm.Get(CommodityType.Food) <= 0 && rm.Get(CommodityType.Water) <= 0)
                gm.TriggerGameOver(GameOverReason.ResourcesDepleted);
        }
    }
}
