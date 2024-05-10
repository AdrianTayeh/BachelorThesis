using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backreset : MonoBehaviour
{
    
    [SerializeField] ZombieAgent agent;
    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("ground"))
        {
            agent.HandleCollision(null, 3);
        }
    }
    
}
