using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LastSignal.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        private CanvasGroup _creditsGroup;

        void Awake()
        {
            var creditsPanel = transform.Find("CreditsPanel");
            if (creditsPanel != null)
            {
                _creditsGroup = creditsPanel.GetComponent<CanvasGroup>();
                SetCreditsVisible(false);
            }

            var playBtn    = transform.Find("Buttons/PlayButton")?.GetComponent<Button>();
            var creditsBtn = transform.Find("Buttons/CreditsButton")?.GetComponent<Button>();
            var closeBtn   = transform.Find("CreditsPanel/CreditsClose")?.GetComponent<Button>();

            if (playBtn != null)    playBtn.onClick.AddListener(StartGame);
            if (creditsBtn != null) creditsBtn.onClick.AddListener(ShowCredits);
            if (closeBtn != null)   closeBtn.onClick.AddListener(HideCredits);
        }

        public void StartGame()
        {
            SceneManager.LoadScene("SampleScene");
        }

        public void ShowCredits()   => SetCreditsVisible(true);
        public void HideCredits()   => SetCreditsVisible(false);

        private void SetCreditsVisible(bool visible)
        {
            if (_creditsGroup == null) return;
            _creditsGroup.alpha          = visible ? 1f : 0f;
            _creditsGroup.interactable   = visible;
            _creditsGroup.blocksRaycasts = visible;
        }
    }
}
