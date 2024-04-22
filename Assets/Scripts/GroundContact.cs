using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

[DisallowMultipleComponent]
public class GroundContact : MonoBehaviour
{
    public ZombieAgent agent;
    Vector3 initPos;
    Vector3 targetPos;

    [Header("Ground Check")] public bool agentDoneOnGroundContact; // Whether to reset agent on ground contact.
    public bool penalizeGroundContact; // Whether to penalize on contact.
    public float groundContactPenalty; // Penalty amount (ex: -1).
    public bool touchingGround;
    const string k_Ground = "ground"; // Tag of ground object.
    public float rewardDist;
    private bool rewardCollected = false;

    void OnCollisionEnter(Collision col)
    {

        if (col.transform.CompareTag(k_Ground))
        {
            if (penalizeGroundContact)
            {
                touchingGround = true;
                agent.HandleCollision(null, 3);
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
