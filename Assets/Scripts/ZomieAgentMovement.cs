using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ZomieAgentMovement : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject[] walls;


    public override void OnEpisodeBegin()
    {
        //base.OnEpisodeBegin();
        transform.position = new Vector3(4.5f, 0, 4.5f);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //base.CollectObservations(sensor);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 1f;
        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed; 
        //base.OnActionReceived(actions);
    }


    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            Debug.Log("Collided with wall");
            SetReward(-3);
            EndEpisode();
        }
        else if (other.CompareTag("Target"))
        {
            Debug.Log("Collided with target");
            SetReward(+5);
            EndEpisode();
        }
    }
    */
   
}
