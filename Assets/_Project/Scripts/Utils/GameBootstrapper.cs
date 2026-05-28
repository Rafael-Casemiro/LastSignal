using UnityEngine;
using LastSignal.Data;
using LastSignal.Narrative;

namespace LastSignal.Utils
{
    /// <summary>
    /// Dispara as primeiras mensagens do jogo para testar o sistema narrativo.
    /// Remova ou substitua por EventManager quando o sistema de eventos estiver pronto.
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("Test Messages")]
        [SerializeField] private MessageData[] openingMessages;

        private void Start()
        {
            // Pequeno delay para garantir que os sistemas inicializaram
            Invoke(nameof(SendOpeningMessages), 1.5f);
        }

        private void SendOpeningMessages()
        {
            if (openingMessages == null) return;
            foreach (var msg in openingMessages)
                NarrativeManager.Instance?.EnqueueMessage(msg);
        }
    }
}
