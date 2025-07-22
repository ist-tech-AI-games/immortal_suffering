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

        public bool TryGrab(GrabAgent agent, Action onReleased)
        {
            // 비활성화시 잡기 차단. 공중에서 잡지 못하게 하는 등의 로직에 사용 가능.
            if (!enabled || IsGrabbed || !IsCharacterGrabbableState()) return false;
            if (CurrentHolder == agent) return true;
            CurrentHolder = agent;
            this.onReleased += onReleased;
            onGrabStarted?.Invoke();
            return true;
        }

        public void EvictAndGrab(GrabAgent agent, Action onReleased)
        {
            if (!enabled || CurrentHolder == agent || !IsCharacterGrabbableState()) return;
            if (CurrentHolder != null)
                Release();

            CurrentHolder = agent;
            this.onReleased += onReleased;
            onGrabStarted?.Invoke();
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