using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LastSignal.Core
{
    public class FactionManager : MonoBehaviour
    {
        public static FactionManager Instance { get; private set; }

        [SerializeField] private FactionData[] factions;

        private Dictionary<FactionId, FactionData> _factions = new();

        public UnityEvent<FactionId, int, RelationshipState> OnReputationChanged = new();

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            foreach (var f in factions)
                _factions[f.id] = f;
        }

        public void ModifyReputation(FactionId faction, int delta)
        {
            if (!_factions.TryGetValue(faction, out var data)) return;
            data.reputation = Mathf.Clamp(data.reputation + delta, -100, 100);
            var state = GetRelationship(faction);
            OnReputationChanged.Invoke(faction, data.reputation, state);
        }

        public int GetReputation(FactionId faction)
            => _factions.TryGetValue(faction, out var d) ? d.reputation : 0;

        public RelationshipState GetRelationship(FactionId faction)
        {
            int rep = GetReputation(faction);
            if (rep >= 60)  return RelationshipState.Allied;
            if (rep >= 20)  return RelationshipState.Friendly;
            if (rep >= -20) return RelationshipState.Neutral;
            if (rep >= -60) return RelationshipState.Hostile;
            return RelationshipState.Enemy;
        }

        // Modificador de preço: aliados dão desconto, inimigos se recusam a negociar
        public float GetPriceMultiplier(FactionId faction)
        {
            return GetRelationship(faction) switch
            {
                RelationshipState.Allied   => 0.75f,
                RelationshipState.Friendly => 0.90f,
                RelationshipState.Neutral  => 1.00f,
                RelationshipState.Hostile  => 1.35f,
                RelationshipState.Enemy    => 2.00f,
                _ => 1f
            };
        }

        public bool HasMinRelationship(FactionId faction, RelationshipState min)
            => (int)GetRelationship(faction) >= (int)min;

        public FactionData GetData(FactionId id)
            => _factions.TryGetValue(id, out var d) ? d : null;
    }

    public enum FactionId
    {
        Military,
        Refugees,
        Traders,
        NorthCoalition,
        UndergroundNetwork
    }

    public enum RelationshipState { Enemy = 0, Hostile = 1, Neutral = 2, Friendly = 3, Allied = 4 }

    [Serializable]
    public class FactionData
    {
        public FactionId id;
        public string displayName;
        [Range(-100, 100)] public int reputation = 0;
    }
}
