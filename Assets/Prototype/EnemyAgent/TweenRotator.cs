using System.Collections;
using UnityEngine;

namespace ImmortalSuffering
{
    /// <summary>
    /// 화살 발사대의 회전에 사용하는 스크립트.
    /// 회전 각도를 지정하면 해당 각도가 되도록 부드럽게 회전을 수행함.
    /// 대상을 감지해 회전 각도를 설정하는 기능은 별도 컴포넌트로 분리.
    /// </summary>
    public class TweenRotator : MonoBehaviour
    {
        [Range(-90f, 270f)]
        [SerializeField] private float minAngle = -15f;
        [Range(-90f, 270f)]
        [SerializeField] private float maxAngle = 195f;
        // [SerializeField] private AnimationCurve tweenCurve;
        // [SerializeField] private float duration = .2f;
        [SerializeField] private float maxAngularSpeed = 180f;
        [SerializeField] private float slowDownAngle = 20f;
        [SerializeField] private float deadZoneAngle = 5f;
        private Coroutine rotateCoroutine = null;

        public void Rotate(float targetAngle)
        {
            Debug.Assert(minAngle <= maxAngle);
            if (rotateCoroutine != null)
                StopCoroutine(rotateCoroutine);

            rotateCoroutine = StartCoroutine(RotateCoroutine(targetAngle));
        }

        // 현재는 transform.right를 기준으로 삼으므로, -90 ~ 270도 사이의 범위로 각도를 조정.
        private float NormalizeAngle(float angle) => ((((angle + 90f) % 360f) + 360f) % 360f) - 90f;

        private IEnumerator RotateCoroutine(float targetAngle)
        {
            // targetAngle = Mathf.Clamp(((targetAngle + 90f) % 360f) - 90f, minAngle, maxAngle);
            // float startAngle = transform.rotation.eulerAngles.z;
            // float startTime = Time.fixedTime;

            // float t = (Time.fixedTime - startTime) / duration;
            // float currentAngle;
            // while (t < 1f)
            // {
            //     currentAngle = Mathf.LerpAngle(startAngle, targetAngle, tweenCurve.Evaluate(t));
            //     if (Mathf.Abs(currentAngle - targetAngle) < deadZoneAngle) break;
            //     transform.rotation = Quaternion.AngleAxis(currentAngle, Vector3.forward);

            //     yield return new WaitForFixedUpdate();
            //     t = (Time.fixedTime - startTime) / duration;
            // }

            // transform.rotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
            targetAngle = Mathf.Clamp(NormalizeAngle(targetAngle), minAngle, maxAngle);

            while (true)
            {
                float currentAngle = NormalizeAngle(transform.eulerAngles.z);
                float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
                float absDiff = Mathf.Abs(angleDiff);

                if (absDiff < deadZoneAngle)
                    break;

                float speedFactor = Mathf.Clamp01(absDiff / slowDownAngle); // 0~1 감속 비율
                float rotationSpeed = maxAngularSpeed * speedFactor;
                float deltaAngle = Mathf.Sign(angleDiff) * rotationSpeed * Time.fixedDeltaTime;

                // Overshoot 방지
                if (Mathf.Abs(deltaAngle) > absDiff)
                    deltaAngle = angleDiff;

                transform.rotation = Quaternion.Euler(0f, 0f, currentAngle + deltaAngle);

                yield return new WaitForFixedUpdate();
            }

            transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
        }
    }
}