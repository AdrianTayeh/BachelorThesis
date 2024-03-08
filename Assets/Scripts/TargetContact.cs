using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.VisualScripting;

[DisallowMultipleComponent]
public class TargetContact : MonoBehaviour
{
    [Header("Detect Targets")] public bool touchingTarget;

    public Agent agent;
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.transform.IsChildOf(agent.transform))
        {
            touchingTarget = true;
            agent.AddReward(10);
            //Debug.Log("Tot reward:" + agent.GetCumulativeReward());
            agent.EndEpisode();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.transform.IsChildOf(agent.transform))
        {
            touchingTarget = false;
        }
    }
}
