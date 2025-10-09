using System.Collections;
using UnityEngine;
using Unity.MLAgents;

public class Tester : MonoBehaviour
{
    public int episodesToCheck = 3;
    IEnumerator Start()
    {
        Academy.Instance.AutomaticSteppingEnabled = false;

        var agent = FindObjectOfType<Agent>();
        if (!agent) { Debug.LogError("씬에 Agent가 없습니다."); yield break; }

        int episodes = 0;
        int steps = 0;

        agent.EndEpisode(); // 에피소드 경계 맞추기
        Academy.Instance.EnvironmentStep();
        yield return null;

        while (episodes < episodesToCheck)
        {
            agent.RequestDecision();
            Academy.Instance.EnvironmentStep();
            steps++;
            if (agent.StepCount == 0 && steps > 1) // 새 에피소드로 넘어간 순간
            {
                Debug.Log($"Episode #{episodes + 1} finished. CumReward={agent.GetCumulativeReward()}");
                episodes++;
            }

            if (steps > 10000) { Debug.LogError("스텝 과다/루프 의심"); break; }
            yield return null;
        }

        Academy.Instance.AutomaticSteppingEnabled = true;
        Debug.Log("Smoke test done.");
    }
}
