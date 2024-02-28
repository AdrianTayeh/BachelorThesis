using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{


    private void Start()
    {
        NewRandomSpawn();
    }

    public void NewRandomSpawn()
    {
        float x = Random.Range(-5f, 5f);
        float z = Random.Range(-5, 0);
        transform.position = new Vector3(x, 0.75f, z);
    }
}
