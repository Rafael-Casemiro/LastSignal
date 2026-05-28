using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace LastSignal.UI
{
    public class GameOverScreen : MonoBehaviour
    {
        public static GameOverScreen Instance { get; private set; }

        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private TextMeshProUGUI messageLabel;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        [Header("Settings")]
        [SerializeField] private float fadeDuration   = 1.2f;
        [SerializeField] private float typewriterSpeed = 0.03f;
        [SerializeField] private string gameSceneName  = "SampleScene";
        [SerializeField] private string menuSceneName  = "MainMenu";

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            Hide();
        }

        void Start()
        {
            restartButton?.onClick.AddListener(OnRestart);
            menuButton?.onClick.AddListener(OnMainMenu);
        }

        public void Show(string title, string message)
        {
            gameObject.SetActive(true);
            if (canvasGroup != null)
            {
                canvasGroup.interactable  = false;
                canvasGroup.blocksRaycasts = false;
            }
            if (titleLabel != null) titleLabel.text = title;
            if (messageLabel != null) messageLabel.text = "";
            StartCoroutine(FadeAndType(message));
        }

        private IEnumerator FadeAndType(string message)
        {
            // Fade in
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                if (canvasGroup != null) canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }
            if (canvasGroup != null)
            {
                canvasGroup.alpha          = 1f;
                canvasGroup.interactable   = true;
                canvasGroup.blocksRaycasts = true;
            }

            // Typewrite message
            if (messageLabel != null)
            {
                foreach (char c in message)
                {
                    messageLabel.text += c;
                    yield return new WaitForSecondsRealtime(typewriterSpeed);
                }
            }
        }

        private void Hide()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha          = 0f;
                canvasGroup.interactable   = false;
                canvasGroup.blocksRaycasts = false;
            }
            gameObject.SetActive(false);
        }

        private void OnRestart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(gameSceneName);
        }

        private void OnMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(menuSceneName);
        }
    }
}
