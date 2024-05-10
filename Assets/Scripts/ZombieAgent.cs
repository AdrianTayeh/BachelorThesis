using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Random = UnityEngine.Random;
using TreeEditor;
using Unity.Barracuda;
using Unity.MLAgents.Policies;
using Unity.Burst.CompilerServices;

public class ZombieAgent : Agent
{
    public float maxSpineAlignment = 10f;
    public float maxChestAlignment = 10f;
    public float maxHeadAlignment = 10f;

    [Header("Body Parts")]
    public Transform hips;
    public Transform chest;
    public Transform spine;
    public Transform head;
    public Transform thighL;
    public Transform shinL;
    public Transform footL;
    public Transform thighR;
    public Transform shinR;
    public Transform footR;
    public Transform armL;
    public Transform forearmL;
    public Transform handL;
    public Transform armR;
    public Transform forearmR;
    public Transform handR;

    BodyPart headBP;
    BodyPart thighRBP;
    BodyPart upperArmRBP;
    BodyPart thighLBP;


    [Header("Stabilizer")]
    [Range(1000, 4000)][SerializeField] float stabilizerTorque = 4000f;
    float minStabilizerTorque = 1000;
    float maxStabilizerTorque = 4000;
    //[SerializeField] Stabilizer hipsStabilizer;
    //[SerializeField] Stabilizer spineStabilizer;

    [Header("Walk Speed")]
    [Range(0.1f, 10f)]
    [SerializeField]
    float targetWalkingSpeed = 10;
    const float minWalkingSpeed = 0.1f;
    const float maxWalkingSpeed = 10;


    public float TargetWalkingSpeed
    {
        get { return targetWalkingSpeed; }
        set { targetWalkingSpeed = Mathf.Clamp(value, 0.1f, maxWalkingSpeed); }
    }

    public float StabilizerTorque
    {
        get { return stabilizerTorque; }
        set { stabilizerTorque = Mathf.Clamp(value,minStabilizerTorque, maxStabilizerTorque); }
    }

    public bool randomizeWalkSpeedEachEpisode;

    private Vector3 worldDirToWalk = Vector3.forward;

    [Header("Target")]
    public Transform target;
    public GameObject goalTarget;
    public Transform targetStart;

    public float spawnRadius = 12;

    [Header("Reward Weights")]
    public float wv = 0.02f;
    public float wo = 0.01f;
    public float wh = 0.01f;


    float resetTimer = 0.2f;
    float timer = 0;
    bool hasCollided = false;

    float distanceRewardTimer;

    float moveTimer;

    [SerializeField] Material redMaterial;

    OrientationCubeController orientationCube;

    JointDriveController jdController;

    private float  initDistance;

    public override void Initialize()
    {
        orientationCube = GetComponentInChildren<OrientationCubeController>();
        jdController = GetComponent<JointDriveController>();
        jdController.SetupBodyPart(hips);
        jdController.SetupBodyPart(chest);
        jdController.SetupBodyPart(spine);
        jdController.SetupBodyPart(head);
        jdController.SetupBodyPart(thighL);
        jdController.SetupBodyPart(shinL);
        jdController.SetupBodyPart(footL);
        jdController.SetupBodyPart(thighR);
        jdController.SetupBodyPart(shinR);
        jdController.SetupBodyPart(footR);
        jdController.SetupBodyPart(armL);
        jdController.SetupBodyPart(forearmL);
        jdController.SetupBodyPart(handL);
        jdController.SetupBodyPart(armR);
        jdController.SetupBodyPart(forearmR);
        jdController.SetupBodyPart(handR);

        //hipsStabilizer.uprightTorque = stabilizerTorque;
        //spineStabilizer.uprightTorque = stabilizerTorque;

        foreach (BodyPart bodyPart in jdController.bodyPartsList)
        {
            if (bodyPart.rb.transform == thighR)
            {
                 thighRBP = bodyPart;
            }

            if (bodyPart.rb.transform == head)
            {
                headBP = bodyPart;
            }
            if (bodyPart.rb.transform == armR)
            {
                upperArmRBP = bodyPart;
            }
            if (bodyPart.rb.transform == thighL)
            {
                thighLBP = bodyPart;
            }
        }

        RemoveLimb(thighRBP);
    }

    public override void OnEpisodeBegin()
    {
        foreach (var bodyPart in jdController.bodyPartsDict.Values)
        {
            bodyPart.Reset(bodyPart);
        }
        foreach (var bodyPart in jdController.bodyPartsDict.Values)
        {
            bodyPart.Reset(bodyPart);
        }
        goalTarget.transform.position = targetStart.transform.position;

        //MoveTargetToRandomPosition();

        /*
        float x, z;
        while (true)
        {
            x = Random.Range(targetStart.position.x - 12f, targetStart.position.x + 12f);
            z = Random.Range(targetStart.position.z - 12f, targetStart.position.z + 12f);
            if ((x < targetStart.position.x - 6f || x > targetStart.position.x + 6f)
                && (z < targetStart.position.z - 6f || z > targetStart.position.z + 6f))
                break;
        }
        goalTarget.transform.position = new Vector3( x, targetStart.position.y, z);
        */
        //hips.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0f);

        timer = 0;
        //distanceRewardTimer = 0;

        UpdateOrientationObjects();
        TargetWalkingSpeed = randomizeWalkSpeedEachEpisode ? Random.Range(1f, maxWalkingSpeed) : TargetWalkingSpeed;
        initDistance = Vector3.Distance(GetAvgPosition(), target.position);

    }

    void UpdateOrientationObjects()
    {
        worldDirToWalk = target.position - hips.position;
        orientationCube.UpdateOrientation(hips, target);      
    }

    public void MoveTargetToRandomPosition()
    {
        Vector3 newTargetPos = targetStart.position + (Random.insideUnitSphere * spawnRadius);
        newTargetPos.y = targetStart.position.y;
        target.position = newTargetPos;
    }

    public void RemoveLimb(BodyPart limb) // add string as parameter and NNmodel
    {
        limb.rb.mass = 0f;
        limb.rb.gameObject.GetComponent<Collider>().enabled = false;
        Collider[] cs = limb.rb.gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider c in cs)
            c.enabled = false;
        limb.rb.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
        Renderer[] rs = limb.rb.gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rs)
            r.enabled = false;

        //SetModel("Zombie Walker", brain, InferenceDevice.Burst);

    }


    Vector3 GetAvgPosition()
    {
        Vector3 posSum = Vector3.zero;
        int numOfRb = 0;
        foreach(var item in jdController.bodyPartsList)
        {
            numOfRb++;
            posSum += item.rb.position;
        }

        var avgPos = posSum / numOfRb;
        return avgPos;
    }


    public void CollectObservationBodyPart(BodyPart bp, VectorSensor sensor)
    {
        sensor.AddObservation(bp.groundContact.touchingGround); //Checks if bodypart is touching ground

        sensor.AddObservation(orientationCube.transform.InverseTransformDirection(bp.rb.velocity));
        sensor.AddObservation(orientationCube.transform.InverseTransformDirection(bp.rb.angularVelocity));

        sensor.AddObservation(orientationCube.transform.InverseTransformDirection(bp.rb.position - hips.position));

        if(bp.rb.transform != hips && bp.rb.transform != handL && bp.rb.transform != handR)
        {
            sensor.AddObservation(bp.rb.transform.localRotation);
            sensor.AddObservation(bp.currentStrength / jdController.maxJointForceLimit);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var cubeForward = orientationCube.transform.forward;

        var velGoal = cubeForward * TargetWalkingSpeed;
        var avgVel = GetAvgVelocity();
        sensor.AddObservation(Vector3.Distance(velGoal, avgVel));
        sensor.AddObservation(orientationCube.transform.InverseTransformDirection(avgVel));
        sensor.AddObservation(orientationCube.transform.InverseTransformDirection(velGoal));
        sensor.AddObservation(orientationCube.transform.InverseTransformDirection(target.position - hips.position));
        sensor.AddObservation(Quaternion.FromToRotation(hips.forward, cubeForward));
        sensor.AddObservation(Quaternion.FromToRotation(head.forward, cubeForward));
        sensor.AddObservation(orientationCube.transform.InverseTransformPoint(target.transform.position));

        foreach (var bodyPart in jdController.bodyPartsList)
        {
            CollectObservationBodyPart(bodyPart, sensor);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var bpDict = jdController.bodyPartsDict;
        var i = -1;

        var continuousActions = actions.ContinuousActions;
        bpDict[chest].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], continuousActions[++i]);
        bpDict[spine].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], continuousActions[++i]);

        bpDict[thighL].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[thighR].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[shinL].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[shinR].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[footR].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], continuousActions[++i]);
        bpDict[footL].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], continuousActions[++i]);

        bpDict[armL].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[armR].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[forearmL].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[forearmR].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[head].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);

        bpDict[chest].SetJointStrength(continuousActions[++i]);
        bpDict[spine].SetJointStrength(continuousActions[++i]);
        bpDict[head].SetJointStrength(continuousActions[++i]);
        bpDict[thighL].SetJointStrength(continuousActions[++i]);
        bpDict[shinL].SetJointStrength(continuousActions[++i]);
        bpDict[footL].SetJointStrength(continuousActions[++i]);
        bpDict[thighR].SetJointStrength(continuousActions[++i]);
        bpDict[shinR].SetJointStrength(continuousActions[++i]);
        bpDict[footR].SetJointStrength(continuousActions[++i]);
        bpDict[armL].SetJointStrength(continuousActions[++i]);
        bpDict[forearmL].SetJointStrength(continuousActions[++i]);
        bpDict[armR].SetJointStrength(continuousActions[++i]);
        bpDict[forearmR].SetJointStrength(continuousActions[++i]);

    }

    private void FixedUpdate()
    {
        UpdateOrientationObjects();

        timer += Time.deltaTime;
        if(timer > resetTimer)
        {
            hasCollided = false;
        }

        float currentDistance = Vector3.Distance(GetAvgPosition(), target.position);
        float rewardDist = 1 - (currentDistance / initDistance);
        AddReward(rewardDist);

        if (hips.position.y < -1)
        {
            OnEpisodeBegin();
        }


        var cubeForward = orientationCube.transform.forward;
        var matchSpeedReward = GetMatchingVelocityReward(cubeForward * TargetWalkingSpeed, GetAvgVelocity());

        if (float.IsNaN(matchSpeedReward))
        {
            throw new ArgumentException(
                "NaN in moveTowardsTargetReward.\n" +
                $" cubeForward: {cubeForward}\n" +
                $" hips.velocity: {jdController.bodyPartsDict[hips].rb.velocity}\n" +
                $" maximumWalkingSpeed: {maxWalkingSpeed}"
            );
        }

        var headForward = head.forward;
        headForward.y = 0;
        var lookAtTargetReward = (Vector3.Dot(cubeForward, headForward) + 1) * 0.5f;

        if (float.IsNaN(lookAtTargetReward))
        {
            throw new ArgumentException(
                "NaN in lookAtTargetReward.\n" +
                $" cubeForward: {cubeForward}\n" +
                $" head.forward: {head.forward}"
            );
        }
        

        AddReward(matchSpeedReward * lookAtTargetReward);

        /*
        float headSpeed = headBP.rb.velocity.magnitude;
        float bodySpeed = GetAvgVelocity().magnitude;

        if (headSpeed > bodySpeed)
        {
            float excessSpeed = headSpeed - bodySpeed;
            AddReward(Mathf.Clamp(-excessSpeed * 0.1f, -1f, 1f));
        }
        */



    }
    Vector3 GetAvgVelocity()
    {
        Vector3 velSum = Vector3.zero;

        int numOfRb = 0;
        foreach (BodyPart bodyPart in jdController.bodyPartsList)
        {
            if (bodyPart.rb.transform == head)
            {
                headBP = bodyPart;
            }
            numOfRb++;
            velSum += bodyPart.rb.velocity;
        }
        var avgVel = velSum / numOfRb;
        return avgVel;
    }

    public float GetMatchingVelocityReward(Vector3 velocityGoal, Vector3 actualVelocity)
    {
        //distance between our actual velocity and goal velocity
        var velDeltaMagnitude = Mathf.Clamp(Vector3.Distance(actualVelocity, velocityGoal), 0, TargetWalkingSpeed);

        //return the value on a declining sigmoid shaped curve that decays from 1 to 0
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        return Mathf.Pow(1 - Mathf.Pow(velDeltaMagnitude / TargetWalkingSpeed, 2), 2);
    }

    private void SpawnTarget()
    {
        goalTarget.transform.position = new Vector3(goalTarget.transform.position.x + 0.5f,goalTarget.transform.position.y, hips.position.z);
        if (goalTarget.transform.position.x >= targetStart.transform.position.x + 14 || goalTarget.transform.position.x <= targetStart.transform.position.x - 14)
        {
            EndEpisode();
        }
    }

    public void HandleCollision(GameObject obj, int type)
    {
        if (hasCollided == false && timer > resetTimer)
        {
            
            hasCollided = true;
            if (type == 1)
            {
                Debug.Log("Target Reached!");
                AddReward(1f);
                SpawnTarget();
                //MoveTargetToRandomPosition();
            }
            else if(type == 2)
            {
                //Debug.Log("Wall Contact");
                obj.GetComponent<MeshRenderer>().material = redMaterial;
                SetReward(-1f);
                EndEpisode();

            }
            else if(type  == 3)
            {
                //Debug.Log("Back touched ground!");
                SetReward(-1f);
                EndEpisode();
            }
        }
    }



}
