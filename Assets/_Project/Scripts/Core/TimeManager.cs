using UnityEngine;
using UnityEngine.Events;

namespace LastSignal.Core
{
    public class TimeManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private int totalDays = 14;
        [SerializeField] private float realSecondsPerDay = 60f;
        [SerializeField, Range(0.1f, 5f)] private float daySpeedMultiplier = 1f;

        public int CurrentDay { get; private set; } = 1;
        public float DayProgress { get; private set; } = 0f;
        public bool IsRunning { get; private set; } = false;

        public UnityEvent<int> OnDayChanged = new();
        public UnityEvent OnCollapseDay = new();

        private float _dayTimer;

        public void Initialize(int startDay = 1)
        {
            CurrentDay = startDay;
            _dayTimer = 0f;
            IsRunning = true;
        }

        private void Update()
        {
            if (!IsRunning) return;
            if (GameManager.Instance == null) return;
            if (GameManager.Instance.CurrentState != GameState.Playing) return;

            _dayTimer += UnityEngine.Time.deltaTime * daySpeedMultiplier;
            DayProgress = _dayTimer / realSecondsPerDay;

            if (_dayTimer >= realSecondsPerDay)
            {
                _dayTimer = 0f;
                AdvanceDay();
            }
        }

        private void AdvanceDay()
        {
            CurrentDay++;
            OnDayChanged.Invoke(CurrentDay);
            AudioManager.Instance?.PlayDayChanged();

            if (CurrentDay >= totalDays)
            {
                IsRunning = false;
                OnCollapseDay.Invoke();
            }
        }

        public int DaysRemaining => Mathf.Max(0, totalDays - CurrentDay);

        public void Pause() => IsRunning = false;
        public void Resume() => IsRunning = true;
    }
}
