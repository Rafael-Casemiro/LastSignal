using UnityEngine;

namespace LastSignal.Core
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Clips")]
        [SerializeField] private AudioClip typingClick;
        [SerializeField] private AudioClip alertSound;
        [SerializeField] private AudioClip messageReceived;
        [SerializeField] private AudioClip dayChangedSound;
        [SerializeField] private AudioClip gameOverSound;

        [Header("Volume")]
        [SerializeField, Range(0f, 1f)] private float typingVolume  = 0.15f;
        [SerializeField, Range(0f, 1f)] private float sfxVolume     = 0.6f;

        private AudioSource _source;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            _source = GetComponent<AudioSource>();
        }

        public void PlayTypingClick()
        {
            if (typingClick == null || _source == null) return;
            _source.PlayOneShot(typingClick, typingVolume);
        }

        public void PlayAlert()
        {
            if (alertSound == null || _source == null) return;
            _source.PlayOneShot(alertSound, sfxVolume);
        }

        public void PlayMessageReceived()
        {
            if (messageReceived == null || _source == null) return;
            _source.PlayOneShot(messageReceived, sfxVolume);
        }

        public void PlayDayChanged()
        {
            if (dayChangedSound == null || _source == null) return;
            _source.PlayOneShot(dayChangedSound, sfxVolume);
        }

        public void PlayGameOver()
        {
            if (gameOverSound == null || _source == null) return;
            _source.PlayOneShot(gameOverSound, sfxVolume);
        }
    }
}
