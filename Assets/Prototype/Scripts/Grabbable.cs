using System;
using UnityEngine;

namespace ImmortalSuffering
{
    public class Grabbable : MonoBehaviour
    {
        public GrabAgent CurrentHolder { get; private set; }
        public bool IsGrabbed => CurrentHolder != null;

        private event Action onReleased;

        public bool TryGrab(GrabAgent agent, Action onReleased)
        {
            // 비활성화시 잡기 차단. 공중에서 잡지 못하게 하는 등의 로직에 사용 가능.
            if (!enabled || IsGrabbed) return false;
            if (CurrentHolder == agent) return true;
            CurrentHolder = agent;
            this.onReleased += onReleased;
            return true;
        }

        public void EvictAndGrab(GrabAgent agent, Action onReleased)
        {
            if (!enabled || CurrentHolder == agent) return;
            if (CurrentHolder != null)
                Release();

            CurrentHolder = agent;
            this.onReleased += onReleased;
        }

        public void Release()
        {
            onReleased?.Invoke();
            CurrentHolder = null;
            onReleased = delegate { };
        }
    }
}