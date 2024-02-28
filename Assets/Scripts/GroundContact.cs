using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

[DisallowMultipleComponent]
public class GroundContact : MonoBehaviour
{
     public Agent agent;

    [Header("Ground Check")] public bool agentDoneOnGroundContact; // Whether to reset agent on ground contact.
    public bool penalizeGroundContact; // Whether to penalize on contact.
    public float groundContactPenalty; // Penalty amount (ex: -1).
    public bool touchingGround;
    const string k_Ground = "ground"; // Tag of ground object.


    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag(k_Ground))
        {
            touchingGround = true;
            if (penalizeGroundContact)
            {
                agent.SetReward(groundContactPenalty);
            }

            if (agentDoneOnGroundContact)
            {
                agent.EndEpisode();
            }
        }
    }


    void OnCollisionExit(Collision other)
    {
        if (other.transform.CompareTag(k_Ground))
        {
            touchingGround = false;
        }
    }
}
