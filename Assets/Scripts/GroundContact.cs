using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

[DisallowMultipleComponent]
public class GroundContact : MonoBehaviour
{
    public Agent agent;
    Vector3 initPos;
    Vector3 targetPos;

    [Header("Ground Check")] public bool agentDoneOnGroundContact; // Whether to reset agent on ground contact.
    public bool penalizeGroundContact; // Whether to penalize on contact.
    public float groundContactPenalty; // Penalty amount (ex: -1).
    public bool touchingGround;
    const string k_Ground = "ground"; // Tag of ground object.
    public float rewardDist;
    private void Start()
    {
        initPos = GameObject.Find("hips").transform.position;
        targetPos = GameObject.FindGameObjectWithTag("target").transform.position;
    }

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
                rewardDist = 1 - (Vector3.Distance(GameObject.Find("hips").transform.position, targetPos)/Vector3.Distance(initPos, targetPos));
                //agent.SetReward(5*rewardDist);
                Debug.Log("Col Rewards: " + (groundContactPenalty) + "Tot rewards: " + agent.GetCumulativeReward());
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
