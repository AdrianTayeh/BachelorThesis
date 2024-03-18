using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    private EnvironmentCheckpoints environmentCheckpoints;
    private bool agentInside = false;
    public int checkpointIndex;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("agent"))
        {
            environmentCheckpoints.AgentThroughCheckpoint(this);
            agentInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("agent"))
        {
            agentInside = false;
            environmentCheckpoints.AgentExitedCheckpoint(this);
        }
    }

    public void SetEnvironmentCheckpoints(EnvironmentCheckpoints environmentCheckpoints)
    {
        this.environmentCheckpoints = environmentCheckpoints;
    }
}
