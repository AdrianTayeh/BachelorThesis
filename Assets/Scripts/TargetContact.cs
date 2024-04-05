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
            //Debug.Log("Agent hit target, Contact made with: " + col.gameObject.transform.name + "Tot R: " + agent.GetCumulativeReward() + "Agent pos at col: " + col.gameObject.transform.localPosition + " vs target pos: " + this.gameObject.transform.localPosition);
            agent.HandleCollision(true);
            
        }
    }
}
