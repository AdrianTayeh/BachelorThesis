using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.VisualScripting;

[DisallowMultipleComponent]
public class TargetContact : MonoBehaviour
{

    public ZombieAgent agent;
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.transform.IsChildOf(agent.transform))
        {
            //agent.SetReward(1);
            //Debug.Log("Target Reached!");
            //Debug.Log(col.gameObject.name);
            agent.HandleCollision(null, 1);
            agent.EndEpisode();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.transform.IsChildOf(agent.transform))
        {
            //Debug.Log("exiting target collision");
        }
    }
}
