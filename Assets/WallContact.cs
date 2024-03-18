using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class WallContact : MonoBehaviour
{
    public ZombieAgent agent;
    public Material hitWallMat;
    public MeshRenderer wallMeshRend;

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.transform.IsChildOf(agent.transform))
        {
            //Debug.Log("Agent hit wall, Contact made with: " + col.gameObject.transform.name + " Tot R: " + agent.GetCumulativeReward() + " Pos of part that collided with wall: " + col.gameObject.transform.localPosition + " vs wall pos: " + this.gameObject.transform.localPosition);
            //Debug.Log("Bool state: " + agent.hasCollided);
            wallMeshRend.material = hitWallMat;
            agent.HandleCollision(true);            
        }
    }
}
