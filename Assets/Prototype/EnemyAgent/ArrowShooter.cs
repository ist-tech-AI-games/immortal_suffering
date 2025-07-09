using UnityEngine;

namespace ImmortalSuffering
{
    public class ArrowShooter : MonoBehaviour
    {
        [SerializeField] private Arrow prefab;
        [SerializeField] private Transform shootPosition;
        [SerializeField] private Transform parentAfterShoot = null;
        [SerializeField] private float loadDelay = .5f, shootDelay = 2.5f;
        enum ShooterState { None, Loaded }
        private ShooterState state = ShooterState.None;
        private float lastActionTime = 0f;
        private Arrow arrow = null;

        void OnEnable()
        {
            lastActionTime = Time.time;
        }

        void FixedUpdate()
        {
            switch (state)
            {
                case ShooterState.None:
                    if (Time.fixedTime - lastActionTime > loadDelay)
                    {
                        lastActionTime = Time.fixedTime;
                        Load();
                    }
                    break;
                case ShooterState.Loaded:
                    if (Time.fixedTime - lastActionTime > shootDelay)
                    {
                        lastActionTime = Time.fixedTime;
                        Shoot();
                    }
                    break;
            }
        }

        private void Load()
        {
            Debug.Assert(arrow == null);
            arrow = Instantiate(prefab, shootPosition);
            state = ShooterState.Loaded;
        }

        private void Shoot()
        {
            Debug.Assert(arrow != null);
            arrow.transform.SetParent(parentAfterShoot);
            arrow.Shoot();
            arrow = null;
            state = ShooterState.None;
        }
    }
}