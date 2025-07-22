using TMPro;
using UnityEngine;

namespace ImmortalSuffering
{
    public class StageTimer : MonoBehaviour
    {
        public float CurrentTime => Time.fixedTime - started;
        public bool Initialized { get; private set; } = false;
        [SerializeField] private TextMeshProUGUI textUI;
        [SerializeField] private string placeholder = "{0:0.00}s";
        private float started = 0f;

        void Start()
        {
            StartTimer();
        }

        void FixedUpdate()
        {
            if (Initialized) UpdateText();
        }

        public void StartTimer()
        {
            started = Time.fixedTime;
            UpdateText();
            Initialized = true;
        }

        private void UpdateText() => textUI?.SetText(placeholder, CurrentTime);
    }
}