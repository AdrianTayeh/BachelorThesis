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
    private bool rewardCollected = false;

    private void Start()
    {
        initPos = GameObject.Find("hips").transform.position;
        targetPos = GameObject.FindGameObjectWithTag("target").transform.position;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag(k_Ground))
        {
            agent.SetReward(0f);

            touchingGround = true;
            if (penalizeGroundContact)
            {
                agent.AddReward(groundContactPenalty);
            }

            if (agentDoneOnGroundContact)
            {
                rewardDist = 1 - (Vector3.Distance(GameObject.Find("hips").transform.position, targetPos)/Vector3.Distance(initPos, targetPos));
                agent.AddReward(10*rewardDist);
                //Debug.Log("Dist Rewards: " + (rewardDist * 10) + " Tot rewards: " + agent.GetCumulativeReward());
                rewardCollected = false;
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
