using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ImmortalSuffering
{
    public class DistanceCalculator : MonoBehaviour
    {
        [Header("Calculation")]
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Transform src, dst;
        [SerializeField] private RectInt bound;

        [Header("Display")]
        [SerializeField] private TextMeshProUGUI textComp;
        [SerializeField] private string placeholder = "{0:0.0} left";

        public float Distance { get; private set; }

        private HashSet<Vector2Int> visitedSet = new();

        int frame = 0;
        void FixedUpdate()
        {
            if (frame++ % 3 != 0) return;

            UpdateDistance();
        }

        public void UpdateDistance()
        {
            visitedSet.Clear();
            // 현재는 이산 좌표 기반 근사 사용
            Distance = DiscreteBFS(
                (Vector2Int)tilemap.WorldToCell(src.position),
                (Vector2Int)tilemap.WorldToCell(dst.position),
                (v) => visitedSet.Add(v),
                (v) => visitedSet.Contains(v)
            );

            textComp?.SetText(placeholder, Distance);
        }

        private int DiscreteBFS(Vector2Int src, Vector2Int dst, Action<Vector2Int> visit, Predicate<Vector2Int> visited)
        {
            Queue<(Vector2Int pos, int dist)> queue = new Queue<(Vector2Int, int)>();

            queue.Enqueue((src, 0));
            visit(src);

            Vector2Int[] directions = new Vector2Int[]
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            while (queue.Count > 0)
            {
                var (current, dist) = queue.Dequeue();

                if (current == dst)
                    return dist;

                foreach (var dir in directions)
                {
                    Vector2Int next = current + dir;

                    if (bound.Contains(next) &&
                        !visited(next) &&
                        tilemap.GetTile((Vector3Int)next) == null)
                    {
                        visit(next);
                        queue.Enqueue((next, dist + 1));
                    }
                }
            }

            return -1;
        }
    }
}