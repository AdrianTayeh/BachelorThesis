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
            Debug.Log("Agent hit target");
            agent.SetReward(1);
            agent.EndEpisode();
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.transform.IsChildOf(agent.transform))
        {
            touchingTarget = false;
        }

    }
}
