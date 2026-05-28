using System.IO;
using UnityEngine;

namespace LastSignal.Core
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private const string SAVE_FILE = "lastsignal_save.json";
        private string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILE);

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void SaveGame()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            var rm = gm.Resources;
            var fm = FactionManager.Instance;

            var data = new SaveData
            {
                currentDay    = gm.Time?.CurrentDay ?? 1,
                food          = rm?.Get(CommodityType.Food)     ?? 0,
                water         = rm?.Get(CommodityType.Water)    ?? 0,
                fuel          = rm?.Get(CommodityType.Fuel)     ?? 0,
                medicine      = rm?.Get(CommodityType.Medicine) ?? 0,
                energy        = rm?.Get(CommodityType.Energy)   ?? 0,
                credits       = rm?.Get(CommodityType.Credits)  ?? 0,
                repMilitary   = fm?.GetReputation(FactionId.Military)            ?? 0,
                repRefugees   = fm?.GetReputation(FactionId.Refugees)            ?? 0,
                repTraders    = fm?.GetReputation(FactionId.Traders)             ?? 0,
                repCoalition  = fm?.GetReputation(FactionId.NorthCoalition)      ?? 0,
                repUnderground = fm?.GetReputation(FactionId.UndergroundNetwork) ?? 0,
            };

            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
            Debug.Log($"[SaveManager] Salvo em {SavePath}");
        }

        public bool LoadGame()
        {
            if (!File.Exists(SavePath)) return false;

            var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
            if (data == null) return false;

            var gm = GameManager.Instance;
            if (gm == null) return false;

            gm.Time?.Initialize(data.currentDay);

            var rm = gm.Resources;
            if (rm != null)
            {
                rm.Initialize();
                rm.Set(CommodityType.Food,     data.food);
                rm.Set(CommodityType.Water,    data.water);
                rm.Set(CommodityType.Fuel,     data.fuel);
                rm.Set(CommodityType.Medicine, data.medicine);
                rm.Set(CommodityType.Energy,   data.energy);
                rm.Set(CommodityType.Credits,  data.credits);
            }

            var fm = FactionManager.Instance;
            if (fm != null)
            {
                fm.ModifyReputation(FactionId.Military,          data.repMilitary    - fm.GetReputation(FactionId.Military));
                fm.ModifyReputation(FactionId.Refugees,          data.repRefugees    - fm.GetReputation(FactionId.Refugees));
                fm.ModifyReputation(FactionId.Traders,           data.repTraders     - fm.GetReputation(FactionId.Traders));
                fm.ModifyReputation(FactionId.NorthCoalition,    data.repCoalition   - fm.GetReputation(FactionId.NorthCoalition));
                fm.ModifyReputation(FactionId.UndergroundNetwork, data.repUnderground - fm.GetReputation(FactionId.UndergroundNetwork));
            }

            Debug.Log("[SaveManager] Jogo carregado.");
            return true;
        }

        public bool HasSave() => File.Exists(SavePath);

        public void DeleteSave()
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }

        [System.Serializable]
        private class SaveData
        {
            public int currentDay;
            public int food, water, fuel, medicine, energy, credits;
            public int repMilitary, repRefugees, repTraders, repCoalition, repUnderground;
        }
    }
}
