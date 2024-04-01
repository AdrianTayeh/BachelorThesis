using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class EnvironmentCheckpoints : MonoBehaviour
{
    public Transform checkpointsTransform;
    public int environmentID;
    public List<CheckpointSingle> checkpointSingleList;
    public Dictionary<int, int> environmentProgress = new Dictionary<int, int>();
    public ZombieAgent agent;

    private void Awake()
    {
        checkpointSingleList = new List<CheckpointSingle>();
        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            checkpointSingle.SetEnvironmentCheckpoints(this);
            checkpointSingle.checkpointIndex = checkpointSingleList.Count;
            checkpointSingleList.Add(checkpointSingle);
        }

        environmentProgress[environmentID] = 0;
    }

    public void AgentThroughCheckpoint(CheckpointSingle checkpointSingle)
    {
        if (checkpointSingleList.Contains(checkpointSingle))
        {
            int checkpointIndex = checkpointSingleList.IndexOf(checkpointSingle);

            if (environmentProgress.TryGetValue(environmentID, out int currentProgress))
            {
                if (checkpointIndex == currentProgress)
                {
                    //Debug.Log("Correct checkpoint");
                    environmentProgress[environmentID] = (currentProgress + 1);
                    agent.AddReward(1f);
                    Debug.Log("Agent #" + environmentID + " passed through gate #" + environmentProgress[environmentID] + " Current total reward: " + agent.GetCumulativeReward());
                }
                else
                {
                    //Debug.Log(Wrong checkpoint);
                    //agent.AddReward(-1f);
                }
            }
        }
    }

    public void AgentExitedCheckpoint(CheckpointSingle checkpointSingle)
    {
    }
}
