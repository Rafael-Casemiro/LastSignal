using UnityEngine;
using LastSignal.UI;

namespace LastSignal.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Managers")]
        [SerializeField] private TimeManager timeManager;
        [SerializeField] private ResourceManager resourceManager;

        public TimeManager Time => timeManager;
        public ResourceManager Resources => resourceManager;

        public GameState CurrentState { get; private set; } = GameState.Playing;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            InitializeGame();
        }

        private void InitializeGame()
        {
            CurrentState = GameState.Playing;
            timeManager?.Initialize();
            resourceManager?.Initialize();
        }

        public void TriggerGameOver(GameOverReason reason)
        {
            if (CurrentState == GameState.GameOver || CurrentState == GameState.Ending) return;
            CurrentState = GameState.GameOver;
            timeManager?.Pause();

            string title   = "[ FIM DE JOGO ]";
            string message = reason switch
            {
                GameOverReason.ResourcesDepleted  => "Os recursos do bunker se esgotaram.\nSeus sobreviventes não resistiram.\n\n[ COLAPSO TOTAL ]",
                GameOverReason.ReputationCollapsed => "Todas as facções se voltaram contra você.\nO bunker está isolado e sem aliados.\n\n[ COLAPSO DIPLOMÁTICO ]",
                _ => "O tempo se esgotou.\n\n[ FIM ]"
            };
            GameOverScreen.Instance?.Show(title, message);
        }

        public void TriggerEnding(EndingType ending)
        {
            TriggerEndingWithMessage(ending, "");
        }

        public void TriggerEndingWithMessage(EndingType ending, string message)
        {
            if (CurrentState == GameState.GameOver || CurrentState == GameState.Ending) return;
            CurrentState = GameState.Ending;
            timeManager?.Pause();

            string title = ending switch
            {
                EndingType.Survived   => "[ FIM: SOBREVIVÊNCIA ]",
                EndingType.Sacrificed => "[ FIM: SACRIFÍCIO ]",
                EndingType.Betrayed   => "[ FIM: DITADURA ]",
                EndingType.Isolated   => "[ FIM: ISOLAMENTO ]",
                _ => "[ FIM ]"
            };
            GameOverScreen.Instance?.Show(title, message);
        }
    }

    public enum GameState { Playing, Paused, GameOver, Ending }
    public enum GameOverReason { ResourcesDepleted, ReputationCollapsed, TimedOut }
    public enum EndingType { Survived, Sacrificed, Betrayed, Isolated }
}
