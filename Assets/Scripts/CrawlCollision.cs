using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class CrawlCollision : MonoBehaviour
{
    public Agent agent;

    private void OnCollisionEnter(Collision col)
    {
        Debug.Log("Collided with: " +  col.gameObject.name);
        if (col.transform.CompareTag("ground"))
        {
            agent.SetReward(-1f);
            Debug.Log("Fell on back");
            agent.EndEpisode();
        }
    }
}
