using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class ObjectContact : MonoBehaviour
{
    [HideInInspector] public Agent agent;

    public float targetReward = 1f;

    public float groundContactPenalty = -1f;
    public float wallContactPenalty = 0f;

    public bool touchingGround;
    public bool touchingWall;
    public bool touchingTarget;

    const string groundTag = "ground";
    const string wallTag = "Wall";
    const string targetTag = "target";

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag(groundTag))
        {
            touchingGround = true;
        }
        if(col.transform.CompareTag(wallTag))
        {
            touchingWall = true;
        }
        if(col.transform.CompareTag(targetTag))
        { 
            touchingTarget = true; 
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.transform.CompareTag(groundTag))
        {
            agent.AddReward(groundContactPenalty);
        }

        if (col.transform.CompareTag(wallTag))
        {
            agent.AddReward(wallContactPenalty);
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.transform.CompareTag(groundTag))
        {
            touchingGround = false;
        }

        if (col.transform.CompareTag(wallTag))
        {
            touchingWall = false;
        }

        if (col.transform.CompareTag(targetTag))
        {
            touchingTarget = false;
        }
    }
}
