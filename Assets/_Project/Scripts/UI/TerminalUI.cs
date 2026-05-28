using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

namespace LastSignal.UI
{
    public class TerminalUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform logContainer;
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private ScrollRect scrollRect;

        [Header("Settings")]
        [SerializeField] private float typewriterSpeed = 0.02f;
        [SerializeField] private int maxLines = 80;

        [Header("Colors")]
        [SerializeField] private Color colorSystem     = new Color(0.3f, 1f, 0.3f);
        [SerializeField] private Color colorIncoming   = new Color(0.9f, 0.9f, 0.9f);
        [SerializeField] private Color colorOutgoing   = new Color(0.4f, 0.7f, 1f);
        [SerializeField] private Color colorWarning    = new Color(1f, 0.6f, 0.2f);
        [SerializeField] private Color colorMilitary   = new Color(1f, 0.85f, 0.3f);
        [SerializeField] private Color colorTrader     = new Color(0.4f, 0.9f, 0.9f);
        [SerializeField] private Color colorEmergency  = new Color(1f, 0.2f, 0.2f);

        public enum LineType { System, Incoming, Outgoing, Warning, Military, Trader, Emergency }

        public bool IsPrinting => _printing;

        private List<GameObject> _lines = new();
        private Queue<(string text, LineType type)> _queue = new();
        private bool _printing = false;
        private bool _skipRequested = false;
        private string _currentFullText = "";
        private TextMeshProUGUI _currentTmp;

        public void ClearTerminal()
        {
            foreach (var line in _lines)
            {
                if (line != null) Destroy(line);
            }
            _lines.Clear();
        }

        void Awake()
        {
            maxLines = 14; // Força limite retro
            
            if (scrollRect == null)
                scrollRect = GetComponentInChildren<ScrollRect>();
            if (scrollRect != null && logContainer == null)
                logContainer = scrollRect.content;
        }

        void Update()
        {
            if (!_printing) return;
            bool skip = (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                     || (Mouse.current    != null && Mouse.current.leftButton.wasPressedThisFrame);
            if (skip) _skipRequested = true;
        }

        public void PrintLine(string text, LineType type = LineType.System)
        {
            if (linePrefab == null || logContainer == null) return;
            _queue.Enqueue((text, type));
            if (!_printing)
                StartCoroutine(ProcessQueue());
        }

        private IEnumerator ProcessQueue()
        {
            _printing = true;
            while (_queue.Count > 0)
            {
                var (text, type) = _queue.Dequeue();
                yield return StartCoroutine(Typewrite(text, type));
                yield return new WaitForSeconds(0.06f);
            }
            _printing = false;
        }

        private IEnumerator Typewrite(string text, LineType type)
        {
            var go = Instantiate(linePrefab, logContainer);
            var tmp = go.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp == null) { yield break; }

            tmp.color = GetColor(type);
            tmp.enableWordWrapping = true;
            tmp.text = "";
            _currentTmp = tmp;
            _currentFullText = text;
            _skipRequested = false;

            _lines.Add(go);
            TrimLines();

            int charIndex = 0;
            string currentPrinted = "";
            foreach (char c in text)
            {
                if (_skipRequested)
                {
                    tmp.text = text;
                    break;
                }
                currentPrinted += c;
                tmp.text = currentPrinted + "<color=#4DFF4D>_</color>";
                charIndex++;
                if (charIndex % 3 == 0)
                    Core.AudioManager.Instance?.PlayTypingClick();
                yield return new WaitForSecondsRealtime(typewriterSpeed);
            }

            if (!_skipRequested)
                tmp.text = text;

            _skipRequested = false;
            yield return null; // wait one frame for layout to settle
            ScrollToBottom();
        }

        private void TrimLines()
        {
            while (_lines.Count > maxLines)
            {
                if (_lines[0] != null) Destroy(_lines[0]);
                _lines.RemoveAt(0);
            }
        }

        private void ScrollToBottom()
        {
            if (scrollRect == null) return;
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
            scrollRect.verticalNormalizedPosition = 0f;
        }

        private Color GetColor(LineType t)
        {
            Color Parse(string hex, Color fallback) => ColorUtility.TryParseHtmlString(hex, out var c) ? c : fallback;

            return t switch
            {
                LineType.System    => Parse("#E5E5E5", colorSystem),
                LineType.Incoming  => Parse("#4DFF4D", colorIncoming),
                LineType.Outgoing  => Parse("#4DFF4D", colorOutgoing),
                LineType.Warning   => Parse("#FF9933", colorWarning),
                LineType.Military  => Parse("#FFD94D", colorMilitary),
                LineType.Trader    => Parse("#66E5E5", colorTrader),
                LineType.Emergency => Parse("#FF3333", colorEmergency),
                _ => Parse("#4DFF4D", colorSystem)
            };
        }
    }
}
