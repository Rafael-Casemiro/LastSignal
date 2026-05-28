using UnityEngine;
using UnityEngine.UI;

namespace LastSignal.UI
{
    /// <summary>
    /// Efeito CRT com scanlines procedurais e flicker sutil.
    /// Adicione em um RawImage fullscreen no topo do Canvas (o script configura o RectTransform).
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class CRTEffect : MonoBehaviour
    {
        [Header("Scanlines")]
        [SerializeField, Range(0f, 0.5f)] private float scanlineAlpha  = 0.12f;
        [SerializeField] private int scanlineHeight = 2;
        [SerializeField] private int scanlineGap    = 4;

        [Header("Flicker")]
        [SerializeField] private bool enableFlicker         = true;
        [SerializeField, Range(0f, 0.06f)] private float flickerIntensity = 0.025f;
        [SerializeField] private float flickerSpeed         = 10f;

        private RawImage _image;

        private void Awake()
        {
            _image = GetComponent<RawImage>();
            _image.raycastTarget = false;

            // Estica para cobrir toda a tela
            var rt = GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            GenerateScanlineTexture();
        }

        private void Update()
        {
            if (!enableFlicker || _image == null) return;
            float n = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
            float brightness = 1f - flickerIntensity + n * flickerIntensity;
            var c = _image.color;
            _image.color = new Color(brightness, brightness, brightness, c.a);
        }

        private void GenerateScanlineTexture()
        {
            int period = scanlineHeight + scanlineGap;
            // Altura = 2× período para que wrapMode.Repeat tile sem artefactos
            var tex = new Texture2D(1, period * 2, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode   = TextureWrapMode.Repeat
            };

            for (int y = 0; y < period * 2; y++)
            {
                int row = y % period;
                Color c = row < scanlineHeight
                    ? new Color(0f, 0f, 0f, scanlineAlpha)
                    : Color.clear;
                tex.SetPixel(0, y, c);
            }

            tex.Apply();
            _image.texture = tex;
        }
    }
}
