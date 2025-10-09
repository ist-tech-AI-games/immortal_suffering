using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.Events;

namespace ImmortalSuffering
{
    /// <summary>
    /// 게임 시작 시 Seed를 기반으로 적을 생성하는 데 사용.
    /// </summary>
    public class EnemyGenerator : MonoBehaviour
    {
        [field: SerializeField] public int Seed;
        [SerializeField] private Transform enemyParent;
        [SerializeField] private Transform[] spawnPoints;

        // 적의 능력치나 대략적인 생성 비중 등에도 변화를 주고 싶은 경우, 나중에 이를 감싸는 구조체로 리펙토링 가능
        [System.Serializable]
        struct PrefabEntry
        {
            public GameObject Prefab;
            public uint Weight;
        }
        [SerializeField] private PrefabEntry[] prefabs;

        [SerializeField] private int minCount;
        [SerializeField] private int maxCount;

        [SerializeField] private UnityEvent<int> onEnemiesGenerated;

        private void Awake()
        {
            Academy.Instance.OnEnvironmentReset += ResetEnemies;

            // ResetEnemies();
        }

        private void ResetEnemies()
        {
            ClearEnemies();
            int enemyCount = Generate();
            onEnemiesGenerated?.Invoke(enemyCount);
        }

        [ContextMenu("Generate Enemies")]
        public int Generate()
        {
            if (minCount < 0 || maxCount < 0 || minCount > maxCount || maxCount > spawnPoints.Length)
            {
                Debug.LogError($"Assertion 0 <= minCount({minCount}) <= maxCount({maxCount}) <= spawnPoints.Length({spawnPoints.Length}) failed.");
                return 0;
            }

            // Python API is attached
            if (Academy.Instance.IsCommunicatorOn)
                Random.InitState(Academy.Instance.EnvironmentParameters.GetWithDefault("seed", 1234f).GetHashCode());
            else // On Editor or Standalone
                Random.InitState(Seed.GetHashCode());

            int enemyCount = Random.Range(minCount, maxCount + 1);


            // reservoir algorithm
            Transform[] samplePoints = new Transform[enemyCount];
            for (int i = 0; i < enemyCount; i++)
                samplePoints[i] = spawnPoints[i];
            for (int i = enemyCount + 1; i < spawnPoints.Length; i++)
            {
                int j = Random.Range(0, i + 1);
                if (j < enemyCount)
                    samplePoints[j] = spawnPoints[i];
            }

            foreach (var point in samplePoints)
            {
                Instantiate(SelectPrefab(), point.position, Quaternion.identity, enemyParent);
            }

            return enemyCount;
        }

        // For Debug Purposes
        [ContextMenu("ClearEnemiesUnderParent")]
        private void ClearEnemies()
        {
            if (enemyParent == null) return;
            for (int i = enemyParent.childCount - 1; i >= 0; i--)
                Destroy(enemyParent.GetChild(i).gameObject);
        }

        private GameObject SelectPrefab()
        {
            uint totalEntryWeight = 0;
            foreach (var entry in prefabs)
                totalEntryWeight += entry.Weight;

            int r = Random.Range(0, (int)totalEntryWeight);

            uint cumulative = 0;
            foreach (var entry in prefabs)
            {
                cumulative += entry.Weight;
                if (r < cumulative)
                    return entry.Prefab;
            }

            // Not Reached
            return null;
        }
    }
}