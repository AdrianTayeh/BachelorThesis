using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class AgentCollision : MonoBehaviour
{
    public Agent agent;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("agent"))
        {
            //call reward
            
        }
    }
}
