using UnityEngine;
using LastSignal.Narrative;

namespace LastSignal.Core
{
    public class EndingManager : MonoBehaviour
    {
        public static EndingManager Instance { get; private set; }

        [Header("Thresholds")]
        [SerializeField] private int sacrificeReputationThreshold  = 70;
        [SerializeField] private int dictatorshipMilitaryThreshold = 80;
        [SerializeField] private int isolationDaysWithoutContact   = 5;

        private int _daysSinceLastContact = 0;
        private bool _endingTriggered = false;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            var tm = GameManager.Instance?.Time;
            if (tm != null)
            {
                tm.OnDayChanged.AddListener(EvaluateEndings);
                tm.OnCollapseDay.AddListener(OnTimeExpired);
            }
        }

        void OnDestroy()
        {
            var tm = GameManager.Instance?.Time;
            if (tm != null)
            {
                tm.OnDayChanged.RemoveListener(EvaluateEndings);
                tm.OnCollapseDay.RemoveListener(OnTimeExpired);
            }
        }

        void EvaluateEndings(int day)
        {
            if (_endingTriggered) return;

            var fm = FactionManager.Instance;
            var rm = GameManager.Instance?.Resources;
            if (fm == null || rm == null) return;

            // Ending: Dictatorship — Aliança total com Militares
            if (fm.GetReputation(FactionId.Military) >= dictatorshipMilitaryThreshold)
            {
                TriggerEnding(EndingType.Betrayed,
                    "PROTOCOLO ALFA ATIVADO.\nO controle militar assumiu o bunker.\nVocê tem poder total — e perdeu tudo que o definia.\n\n[ FIM: DITADURA ]");
                return;
            }

            // Ending: Sacrifice — Reputação máxima com Refugiados
            if (fm.GetReputation(FactionId.Refugees) >= sacrificeReputationThreshold
                && rm.Get(CommodityType.Food) < 15)
            {
                TriggerEnding(EndingType.Sacrificed,
                    "Você distribuiu os últimos recursos.\nOs refugiados sobreviverão.\nSeu bunker não.\n\n[ FIM: SACRIFÍCIO ]");
                return;
            }

            // Ending: Isolation
            if (_daysSinceLastContact >= isolationDaysWithoutContact)
            {
                TriggerEnding(EndingType.Isolated,
                    "Silêncio total. Nenhuma transmissão recebida ou enviada.\nO mundo parou de existir para vocês.\n\n[ FIM: ISOLAMENTO ]");
                return;
            }
        }

        void OnTimeExpired()
        {
            if (_endingTriggered) return;

            var rm = GameManager.Instance?.Resources;
            bool survived = rm != null
                && rm.Get(CommodityType.Food) > 20
                && rm.Get(CommodityType.Water) > 15;

            if (survived)
                TriggerEnding(EndingType.Survived,
                    "O prazo chegou ao fim.\nSeus sobreviventes resistiram.\nAlguns pagaram o preço. Outros verão o amanhã.\n\n[ FIM: SOBREVIVÊNCIA ]");
            else
                TriggerEnding(EndingType.Isolated,
                    "Os recursos acabaram antes do tempo.\nO último sinal foi enviado às 03:14.\nNinguém respondeu.\n\n[ FIM: COLAPSO ]");
        }

        void TriggerEnding(EndingType type, string message)
        {
            _endingTriggered = true;
            Narrative.NarrativeManager.Instance?.DisplaySystemMessage("\n" + message);
            GameManager.Instance?.TriggerEndingWithMessage(type, message);
        }

        // Chamado pelo NarrativeManager quando o jogador responde a uma mensagem
        public void RegisterContact() => _daysSinceLastContact = 0;

        public void IncrementIsolation() => _daysSinceLastContact++;
    }
}
