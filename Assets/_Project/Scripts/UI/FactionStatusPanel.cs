using System;
using UnityEngine;
using TMPro;
using LastSignal.Core;

namespace LastSignal.UI
{
    /// <summary>
    /// Painel de status das facções. Arraste labels para os entries no Inspector.
    /// </summary>
    public class FactionStatusPanel : MonoBehaviour
    {
        [Serializable]
        public class FactionEntry
        {
            public FactionId faction;
            public TextMeshProUGUI label;
        }

        [SerializeField] private FactionEntry[] entries;

        private void Start()
        {
            var fm = FactionManager.Instance;
            if (fm == null) return;
            fm.OnReputationChanged.AddListener(OnReputationChanged);
            RefreshAll();
        }

        private void OnDestroy()
        {
            if (FactionManager.Instance != null)
                FactionManager.Instance.OnReputationChanged.RemoveListener(OnReputationChanged);
        }

        private void OnReputationChanged(FactionId faction, int rep, RelationshipState state)
        {
            foreach (var entry in entries)
                if (entry.faction == faction)
                    UpdateEntry(entry, rep, state);
        }

        private void RefreshAll()
        {
            var fm = FactionManager.Instance;
            if (fm == null) return;
            foreach (var entry in entries)
                UpdateEntry(entry, fm.GetReputation(entry.faction), fm.GetRelationship(entry.faction));
        }

        private void UpdateEntry(FactionEntry entry, int rep, RelationshipState state)
        {
            if (entry.label == null) return;

            string stateName = state switch
            {
                RelationshipState.Allied   => "ALIADO",
                RelationshipState.Friendly => "AMIGÁVEL",
                RelationshipState.Neutral  => "NEUTRO",
                RelationshipState.Hostile  => "HOSTIL",
                RelationshipState.Enemy    => "INIMIGO",
                _ => "?"
            };

            entry.label.text = $"{entry.faction}: {stateName} ({rep:+0;-0;0})";
            entry.label.color = state switch
            {
                RelationshipState.Allied   => new Color(0.2f, 0.9f, 0.4f),
                RelationshipState.Friendly => new Color(0.4f, 0.8f, 1f),
                RelationshipState.Neutral  => new Color(0.8f, 0.8f, 0.8f),
                RelationshipState.Hostile  => new Color(1f, 0.5f, 0.2f),
                RelationshipState.Enemy    => new Color(1f, 0.2f, 0.2f),
                _ => Color.white
            };
        }
    }
}
