using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LastSignal.Data;
using LastSignal.UI;

namespace LastSignal.Narrative
{
    /// <summary>
    /// Orquestra o fluxo de mensagens e escolhas da narrativa.
    /// </summary>
    public class NarrativeManager : MonoBehaviour
    {
        public static NarrativeManager Instance { get; private set; }

        [Header("Dependencies")]
        [SerializeField] private TerminalUI terminalUI;
        [SerializeField] private ChoicePanel choicePanel;

        [Header("Messages Database")]
        [SerializeField] private MessageData[] messageDatabase;

        [Header("Timing")]
        [SerializeField] private float interMessageDelay = 0.8f;
        [SerializeField] private float choiceRevealDelay = 0.5f;

        private Queue<MessageData> _messageQueue = new();
        private HashSet<MessageData> _pendingMessages = new();
        private HashSet<MessageData> _displayedMessages = new();
        private MessageData _currentMessage;
        private bool _waitingForChoice = false;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            DisplaySystemMessage("LAST SIGNAL v0.4\nBUNKER ALFA — ONLINE\n────────────────────────");
            DisplaySystemMessage("Aguardando transmissões...");
        }

        public void EnqueueMessage(MessageData message)
        {
            if (message == null) return;
            if (IsGameOver())
            {
                _messageQueue.Clear();
                return;
            }
            if (_pendingMessages.Contains(message)) return;
            if (_displayedMessages.Contains(message)) return;

            _pendingMessages.Add(message);
            _messageQueue.Enqueue(message);
            TryProcessNext();
        }

        public void DisplaySystemMessage(string text)
        {
            terminalUI?.PrintLine(text, TerminalUI.LineType.System);
        }

        private bool IsGameOver()
        {
            var state = Core.GameManager.Instance?.CurrentState;
            return state == Core.GameState.GameOver || state == Core.GameState.Ending;
        }

        private void TryProcessNext()
        {
            if (_waitingForChoice || _messageQueue.Count == 0) return;
            if (IsGameOver()) return;

            _currentMessage = _messageQueue.Dequeue();
            _pendingMessages.Remove(_currentMessage);
            ShowMessage(_currentMessage);
        }

        private void ShowMessage(MessageData msg)
        {
            _displayedMessages.Add(msg);
            terminalUI?.PrintLine($"[{msg.senderDisplayName}] >> {msg.messageText}", TerminalUI.LineType.Incoming);
            Core.AudioManager.Instance?.PlayMessageReceived();

            if (msg.choices != null && msg.choices.Length > 0)
            {
                _waitingForChoice = true;
                StartCoroutine(ShowChoicesWhenReady(msg));
            }
            else
            {
                StartCoroutine(DelayedProcessNext());
            }
        }

        private IEnumerator ShowChoicesWhenReady(MessageData msg)
        {
            if (terminalUI != null)
                yield return new WaitUntil(() => !terminalUI.IsPrinting);
            if (IsGameOver()) yield break;
            yield return new WaitForSeconds(choiceRevealDelay);
            choicePanel?.ShowChoices(msg.choices, OnChoiceSelected);
        }

        private IEnumerator DelayedProcessNext()
        {
            if (terminalUI != null)
                yield return new WaitUntil(() => !terminalUI.IsPrinting);
            
            if (IsGameOver()) yield break;
            
            yield return new WaitForSeconds(interMessageDelay);
            TryProcessNext();
        }

        private void OnChoiceSelected(ChoiceData choice)
        {
            _waitingForChoice = false;
            terminalUI?.PrintLine($"> {choice.choiceText}", TerminalUI.LineType.Outgoing);
            ApplyConsequences(choice.consequences);
            Core.EndingManager.Instance?.RegisterContact();
            
            if (IsGameOver()) return; // Para imediatamente se a escolha causou Game Over
            
            StartCoroutine(DelayedProcessNext());
        }

        private void ApplyConsequences(ChoiceConsequence[] consequences)
        {
            if (consequences == null) return;

            var resources = Core.GameManager.Instance?.Resources;

            foreach (var c in consequences)
            {
                switch (c.type)
                {
                    case ConsequenceType.GainResource:
                        resources?.Receive((Core.CommodityType)System.Enum.Parse(typeof(Core.CommodityType), c.targetId), c.value);
                        break;
                    case ConsequenceType.LoseResource:
                        resources?.TrySpend((Core.CommodityType)System.Enum.Parse(typeof(Core.CommodityType), c.targetId), c.value);
                        break;
                    case ConsequenceType.ChangeReputation:
                        if (System.Enum.TryParse<Core.FactionId>(c.targetId, out var factionId))
                            Core.FactionManager.Instance?.ModifyReputation(factionId, c.value);
                        break;
                    case ConsequenceType.TriggerEnding:
                        Core.GameManager.Instance?.TriggerEnding((Core.EndingType)c.value);
                        break;
                }
            }
        }
    }
}
