using System;
using System.Collections.Generic;
using UnityEngine;
using LastSignal.Data;
using LastSignal.Narrative;

namespace LastSignal.Core
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        [SerializeField] private EventEntry[] events;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            var tm = GameManager.Instance?.Time;
            if (tm != null)
                tm.OnDayChanged.AddListener(OnDayChanged);

            var rm = GameManager.Instance?.Resources;
            if (rm != null)
                rm.OnResourceChanged.AddListener(OnResourceChanged);

            // Dispara eventos do Dia 1 após todos os sistemas inicializarem
            Invoke(nameof(EvaluateOnStart), 2.0f);
        }

        private void EvaluateOnStart() => EvaluateEvents(EvaluationTrigger.DayChange);

        void OnDestroy()
        {
            var tm = GameManager.Instance?.Time;
            if (tm != null) tm.OnDayChanged.RemoveListener(OnDayChanged);

            var rm = GameManager.Instance?.Resources;
            if (rm != null) rm.OnResourceChanged.RemoveListener(OnResourceChanged);
        }

        void OnDayChanged(int day) => EvaluateEvents(EvaluationTrigger.DayChange);

        void OnResourceChanged(CommodityType type, int value) => EvaluateEvents(EvaluationTrigger.ResourceChange);

        void EvaluateEvents(EvaluationTrigger trigger)
        {
            if (events == null) return;

            int currentDay = GameManager.Instance?.Time?.CurrentDay ?? 1;
            var rm = GameManager.Instance?.Resources;

            foreach (var entry in events)
            {
                if (entry == null || entry.message == null) continue;
                if (entry.triggerOnce && entry.hasTriggered) continue;
                if (!entry.condition.MatchesTrigger(trigger)) continue;
                if (!entry.condition.IsDayInRange(currentDay)) continue;
                if (rm != null && !entry.condition.IsResourceMet(rm)) continue;

                entry.hasTriggered = true;
                NarrativeManager.Instance?.EnqueueMessage(entry.message);
            }
        }

        // Reseta todos os eventos (útil para reiniciar o jogo)
        public void ResetAll()
        {
            if (events == null) return;
            foreach (var e in events) e.hasTriggered = false;
        }
    }

    public enum EvaluationTrigger { DayChange, ResourceChange, Both }
    public enum ResourceComparison { LessThan, LessOrEqual, GreaterThan, GreaterOrEqual }

    [Serializable]
    public class EventEntry
    {
        public string id = "event_id";
        public MessageData message;
        public bool triggerOnce = true;
        public EventCondition condition;
        [HideInInspector] public bool hasTriggered;
    }

    [Serializable]
    public class EventCondition
    {
        [Header("Quando avaliar")]
        public EvaluationTrigger evaluateOn = EvaluationTrigger.DayChange;

        [Header("Condição de Dia")]
        public int minDay = 1;
        public int maxDay = 999;

        [Header("Condição de Recurso (opcional)")]
        public bool checkResource = false;
        public CommodityType resourceType = CommodityType.Food;
        public ResourceComparison comparison = ResourceComparison.LessThan;
        public int threshold = 30;

        public bool MatchesTrigger(EvaluationTrigger trigger)
        {
            if (evaluateOn == EvaluationTrigger.Both) return true;
            return evaluateOn == trigger;
        }

        public bool IsDayInRange(int day) => day >= minDay && day <= maxDay;

        public bool IsResourceMet(ResourceManager rm)
        {
            if (!checkResource) return true;
            int value = rm.Get(resourceType);
            return comparison switch
            {
                ResourceComparison.LessThan       => value < threshold,
                ResourceComparison.LessOrEqual    => value <= threshold,
                ResourceComparison.GreaterThan    => value > threshold,
                ResourceComparison.GreaterOrEqual => value >= threshold,
                _ => true
            };
        }
    }
}
