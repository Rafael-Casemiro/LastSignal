using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LastSignal.UI
{
    [ExecuteAlways]
    public class UISetup : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] private RectTransform terminalRect;
        [SerializeField] private ScrollRect terminalScrollRect;
        [SerializeField] private RectTransform choicePanelRect;

        private void Awake() => Apply();
        private void OnValidate() => Apply();

        [ContextMenu("Apply Setup")]
        public void Apply()
        {
            FixTerminalLayout();
            FixScrollViewVisuals();
            FixContentRect();
            FixChoicePanelLayout();
            FixHUDLayout();
            FixFactionPanelLayout();
        }

        private void FixTerminalLayout()
        {
            if (terminalRect == null) return;
            // Reduzido ~5% width, ~15% height, centralizado
            terminalRect.anchorMin = new Vector2(0.18f, 0.18f);
            terminalRect.anchorMax = new Vector2(0.82f, 0.85f);
            terminalRect.offsetMin = Vector2.zero;
            terminalRect.offsetMax = Vector2.zero;
        }

        private void FixScrollViewVisuals()
        {
            if (terminalScrollRect == null) return;

            var img = terminalScrollRect.GetComponent<Image>();
            if (img != null)
            {
                ColorUtility.TryParseHtmlString("#020502", out var bgColor);
                img.color = bgColor;
            }

            var viewport = terminalScrollRect.viewport;
            if (viewport == null) return;

            var vpImg = viewport.GetComponent<Image>();
            if (vpImg == null) vpImg = viewport.gameObject.AddComponent<Image>();
            vpImg.color = Color.white;

            if (viewport.GetComponent<Mask>() == null)
                viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;
        }

        private void FixContentRect()
        {
            if (terminalScrollRect == null) return;
            var content = terminalScrollRect.content;
            if (content == null) return;

            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.pivot     = new Vector2(0.5f, 1f);
            content.offsetMin = new Vector2(0, content.offsetMin.y);
            content.offsetMax = new Vector2(0, 0);

            var vlg = content.GetComponent<VerticalLayoutGroup>();
            if (vlg == null) vlg = content.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.UpperLeft;
            vlg.spacing = 14f; 
            vlg.padding = new RectOffset(32, 24, 24, 20); 
            vlg.childForceExpandWidth  = true;
            vlg.childForceExpandHeight = false;

            var csf = content.GetComponent<ContentSizeFitter>();
            if (csf == null) csf = content.gameObject.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        private void FixChoicePanelLayout()
        {
            if (choicePanelRect == null) return;
            // Aproximar o choice panel do terminal
            choicePanelRect.anchorMin = new Vector2(0.18f, 0.05f);
            choicePanelRect.anchorMax = new Vector2(0.82f, 0.17f);
            choicePanelRect.offsetMin = Vector2.zero;
            choicePanelRect.offsetMax = Vector2.zero;
            
            var vlg = choicePanelRect.GetComponent<VerticalLayoutGroup>();
            if (vlg != null)
            {
                vlg.spacing = 8f;
                vlg.childAlignment = TextAnchor.UpperLeft;
            }
        }

        private void FixHUDLayout()
        {
            var hud = FindObjectOfType<HUDManager>();
            if (hud != null)
            {
                var rt = hud.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.localScale = Vector3.one;
                    // Ampliar a âncora para garantir que todos os textos caibam na tela
                    rt.anchorMin = new Vector2(0.02f, 0.88f);
                    rt.anchorMax = new Vector2(0.98f, 0.98f);
                    rt.offsetMin = Vector2.zero;
                    rt.offsetMax = Vector2.zero;
                }
                
                // Remover o fundo escuro do HUD que ficava cortado
                var img = hud.GetComponent<Image>();
                if (img != null) img.color = Color.clear;
                
                // Ocultar a barra azul (Slider) via Reflection
                var field = typeof(HUDManager).GetField("dayProgressBar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    var slider = field.GetValue(hud) as UnityEngine.UI.Slider;
                    if (slider != null) slider.gameObject.SetActive(false);
                }

                // Ajustar alinhamento interno para não espalhar e virar um painel compacto
                var hlg = hud.GetComponent<HorizontalLayoutGroup>();
                if (hlg != null)
                {
                    hlg.spacing = 8f; // Reduzido para caber tudo (antes 15f)
                    hlg.childAlignment = TextAnchor.MiddleCenter;
                    hlg.childForceExpandWidth = false;
                }

                foreach (var txt in hud.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    txt.fontSize = 18;
                    txt.alignment = TextAlignmentOptions.Center;
                }
            }
        }

        private void FixFactionPanelLayout()
        {
            var factionPanel = FindObjectOfType<FactionStatusPanel>();
            if (factionPanel != null)
            {
                var rt = factionPanel.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchorMin = new Vector2(1, 0.3f);
                    rt.anchorMax = new Vector2(1, 0.85f);
                    rt.offsetMin = new Vector2(-320, 0); // Aumentado para 320px para caber textos longos
                    rt.offsetMax = new Vector2(-20, 0);
                    
                    var img = factionPanel.GetComponent<Image>();
                    if (img == null) img = factionPanel.gameObject.AddComponent<Image>();
                    img.color = new Color(0, 0, 0, 0.5f); // Fundo mais escuro
                }
                
                // Arrumar o espaçamento gigante entre as facções
                var vlg = factionPanel.GetComponent<VerticalLayoutGroup>();
                if (vlg == null) vlg = factionPanel.gameObject.AddComponent<VerticalLayoutGroup>();
                vlg.padding = new RectOffset(20, 20, 20, 20);
                vlg.spacing = 18f;
                vlg.childAlignment = TextAnchor.UpperLeft;
                vlg.childForceExpandHeight = false;

                // Melhorar legibilidade: alinhamento à esquerda e evitar corte
                foreach (var txt in factionPanel.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    txt.alignment = TextAlignmentOptions.Left;
                    txt.fontSize = 16; // Aumentado o tamanho da fonte
                    txt.enableWordWrapping = true;
                    txt.overflowMode = TextOverflowModes.Overflow;
                }
            }
        }
    }
}
