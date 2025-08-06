using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;

namespace ImmortalSuffering
{
    public class GrabAgent : MonoBehaviour
    {
        [SerializeField] private string[] enabledWhenStateIs = { "Chase" };
        [SerializeField] private Collider2D grabTrigger;
        [SerializeField] private ParentConstraint constraint;
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private float maxGrabbingTime = 4f;
        [SerializeField] private UnityEvent onGrabStarted;
        [SerializeField] private UnityEvent onGrabReleased;
        private Animator animator;
        private Grabbable currentGrabbing = null;
        private Coroutine coroutine;
        private ContactFilter2D contactFilter;

        void Awake()
        {
            animator = GetComponentInParent<Animator>();
            contactFilter = new ContactFilter2D().NoFilter();
            contactFilter.SetLayerMask(targetMask);
            constraint.constraintActive = false;
        }

        int frame = 0;
        Collider2D[] detectedColliders = new Collider2D[4];
        void FixedUpdate()
        {
            if (frame++ % 3 != 0) return; // skip frame for performance
            if (currentGrabbing != null || !IsGrabbableState()) return;

            int cnt = Physics2D.OverlapCollider(grabTrigger, contactFilter, detectedColliders);
            for (int i = 0; i < cnt; i++)
            {
                if (Grab(detectedColliders[i].transform)) break;
            }
        }


        private bool Grab(Transform target)
        {
            if (!target.TryGetComponent(out Grabbable grabbable) || !grabbable.Grab(this, OnRelease, evict: true))
                return false;

            currentGrabbing = grabbable;

            constraint.SetSources(new List<ConstraintSource>() {new ConstraintSource() {sourceTransform = target, weight = 1f}});
            Vector3 positionOffset = target.InverseTransformPoint( constraint.transform.position);
            Quaternion rotationOffset = Quaternion.Inverse(target.rotation) * constraint.transform.rotation;
            constraint.SetTranslationOffset(0, positionOffset);
            constraint.SetRotationOffset(0, rotationOffset.eulerAngles);
            constraint.constraintActive = true;

            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(Tick());

            onGrabStarted?.Invoke();
            return true;
        }

        public void TryRelease()
        {
            if (currentGrabbing == null || currentGrabbing.CurrentHolder != this) return;
            currentGrabbing.Release();
        }

        IEnumerator Tick()
        {
            yield return new WaitForSeconds(maxGrabbingTime);
            TryRelease();
        }

        private void OnRelease()
        {
            currentGrabbing = null;
            constraint.constraintActive = false;
            if (coroutine != null) StopCoroutine(coroutine);
            onGrabReleased?.Invoke();
        }

        // TODO: Refactor
        private bool IsGrabbableState() => enabledWhenStateIs.Any((x) => animator.GetCurrentAnimatorStateInfo(0).IsName(x));
    }
}