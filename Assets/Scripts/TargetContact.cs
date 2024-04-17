using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.VisualScripting;

[DisallowMultipleComponent]
public class TargetContact : MonoBehaviour
{
    [Header("Detect Targets")] public bool touchingTarget;
    const string k_Target = "target";

    public ZombieAgent agent;
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.transform.IsChildOf(agent.transform))
        {
            touchingTarget = true;
            //agent.SetReward(1);
            //Debug.Log("Target Reached!");
            //Debug.Log(col.gameObject.name);
            agent.HandleCollision(null, 1);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.transform.IsChildOf(agent.transform))
        {
            touchingTarget = false;
            //Debug.Log("exiting target collision");
        }
    }
}
