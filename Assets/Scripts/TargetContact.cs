using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.VisualScripting;

[DisallowMultipleComponent]
public class TargetContact : MonoBehaviour
{
    [Header("Detect Targets")] public bool touchingTarget;

    public ZombieAgent agent;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.transform.IsChildOf(agent.transform))
        {
            touchingTarget = true;
            //Debug.Log("Agent hit target, Contact made with: " + col.gameObject.transform.name + "Tot R: " + agent.GetCumulativeReward() + "Agent pos at col: " + col.gameObject.transform.localPosition + " vs target pos: " + this.gameObject.transform.localPosition);
            Debug.Log("Touching target");
            agent.HandleCollision(false);
            
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
