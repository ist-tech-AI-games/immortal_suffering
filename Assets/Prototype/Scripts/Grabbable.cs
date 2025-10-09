using System;
using UnityEngine;
using UnityEngine.Events;

namespace ImmortalSuffering
{
    public class Grabbable : MonoBehaviour
    {
        public GrabAgent CurrentHolder { get; private set; }
        public bool IsGrabbed => CurrentHolder != null;

        // [임시 방편] 플레이어의 경우, 여기에서 상태를 참조해 공중에서 잡지 못하게 함.
        // 이 기능을 사용하지 않으려면 null로 남겨 둘 것.
        [SerializeField] private CharacterMovement characterMovement;
        [SerializeField] private UnityEvent onGrabStarted, onGrabReleased;

        private event Action onReleased;

        private bool IsCharacterGrabbableState()
        {
            if (characterMovement == null) return true;
            switch (characterMovement.currentState)
            {
                case PlayerState.Idle:
                case PlayerState.Moving:
                    return true;
                default:
                    return false;
            }
        }

        public bool Grab(GrabAgent agent, Action onReleased, bool evict = false)
        {
            if (!enabled || CurrentHolder == agent || !IsCharacterGrabbableState()) return false;
            if (IsGrabbed)
            {
                if (!evict) return false;
                Release();
            }

            CurrentHolder = agent;
            this.onReleased += onReleased;
            onGrabStarted?.Invoke();
            return true;
        }

        public void Release()
        {
            onReleased?.Invoke();
            CurrentHolder = null;
            onReleased = delegate { };
            onGrabReleased?.Invoke();
        }
    }
}