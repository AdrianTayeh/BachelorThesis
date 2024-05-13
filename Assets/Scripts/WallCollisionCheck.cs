using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class WallCollisionCheck : MonoBehaviour
{
    [SerializeField] ZombieAgent agent;
    [SerializeField] private Material redMaterial;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.IsChildOf(agent.transform))
        {
            //agent.SetReward(-1f);
            //Debug.Log("Contacted Wall");
            //this.GetComponent<MeshRenderer>().material = redMaterial;
            
            //if(collision.)
            agent.HandleCollision(this.gameObject, 2);
        }
    }

}
