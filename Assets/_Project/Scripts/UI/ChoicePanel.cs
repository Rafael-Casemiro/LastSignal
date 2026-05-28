using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LastSignal.Data;

namespace LastSignal.UI
{
    /// <summary>
    /// Painel de escolhas narrativas. Exibe botões e retorna a escolha via callback.
    /// </summary>
    public class ChoicePanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject choiceButtonPrefab;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private CanvasGroup canvasGroup;

        private List<GameObject> _activeButtons = new();
        private Action<ChoiceData> _onChoiceSelected;

        private void Awake()
        {
            Hide();
        }

        public void ShowChoices(ChoiceData[] choices, Action<ChoiceData> callback)
        {
            _onChoiceSelected = callback;
            ClearButtons();

            foreach (var choice in choices)
            {
                var btn = Instantiate(choiceButtonPrefab, buttonContainer);
                var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
                var button = btn.GetComponent<Button>();

                if (tmp != null)
                {
                    tmp.text = $"> {choice.choiceText}";
                    tmp.fontSize = 22; // Força o tamanho 18 via código
                    
                    // Aplica a cor verde do terminal para os botões de escolha
                    if (ColorUtility.TryParseHtmlString("#4DFF4D", out var color))
                        tmp.color = color;
                }

                var capturedChoice = choice;
                button?.onClick.AddListener(() => OnButtonClicked(capturedChoice));

                _activeButtons.Add(btn);
            }

            Show();
        }

        private void OnButtonClicked(ChoiceData choice)
        {
            Hide();
            ClearButtons();
            _onChoiceSelected?.Invoke(choice);
        }

        private void ClearButtons()
        {
            foreach (var btn in _activeButtons)
                Destroy(btn);
            _activeButtons.Clear();
        }

        private void Show()
        {
            // SetActive first so Awake runs (and calls Hide) before we override
            gameObject.SetActive(true);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }

        private void Hide()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }
}
