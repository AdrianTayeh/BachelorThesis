using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{

    public Transform centerPos; 
    private void Start()
    {
        NewRandomSpawn();
    }

    public void NewRandomSpawn()
    {
        
        float x = Random.Range(centerPos.position.x - 2f, centerPos.position.x + 2f);
        float z = Random.Range(centerPos.position.z - 4f, centerPos.position.z + 4f);
        transform.position = new Vector3(x, 1f, z);
    }
}
