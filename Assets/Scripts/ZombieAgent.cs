using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Random = UnityEngine.Random;
public class ZombieAgent : Agent
{
    public bool earlyTraining = false;

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



    [Header("Walk Speed")]
    [Range(0.1f, 4f)]
    [SerializeField]
    float targetWalkingSpeed = 2;
    float minWalkingSpeed = 0.1f;
    float maxWalkingSpeed = 4f;

    public float TargetWalkingSpeed
    {
        get { return targetWalkingSpeed; }
        set { targetWalkingSpeed = Mathf.Clamp(value, minWalkingSpeed, maxWalkingSpeed); }
    }

    public bool randomizeWalkSpeedEachEpisode;

    [Header("Target")]
    public Transform target;
    public GameObject goalTarget;
    public Transform targetStart;

    OrientationCubeController orientationCube;

    JointDriveController jdController;

    private Vector3 worldDirToWalk = Vector3.right;
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

    }

    public override void OnEpisodeBegin()
    {
        foreach (var bodyPart in jdController.bodyPartsDict.Values)
        {
            bodyPart.Reset(bodyPart);
        }
        float x = Random.Range(targetStart.position.x - 2f, targetStart.position.x + 2f);
        float z = Random.Range(targetStart.position.z - 4f, targetStart.position.z + 4f);
        //goalTarget.transform.position = new Vector3(x, 1f, z);

        hips.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0f);

        UpdateOrientationObjects();
        TargetWalkingSpeed = randomizeWalkSpeedEachEpisode ? Random.Range(minWalkingSpeed, maxWalkingSpeed) : TargetWalkingSpeed;
        initDistance = Vector3.Distance(hips.position, target.position);

    }

    void UpdateOrientationObjects()
    {
        worldDirToWalk = target.position - hips.position;
        orientationCube.UpdateOrientation(hips, target);      
    }

    Vector3 GetAvgVelocity()
    {
        Vector3 velSum = Vector3.zero;

        int numOfRb = 0;
        foreach (var item in jdController.bodyPartsList)
        {
            numOfRb++;
            velSum += item.rb.velocity;
        }
        var avgVel = velSum / numOfRb;
        return avgVel;
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

    public float GetMatchingVelocityReward(Vector3 velocityGoal, Vector3 actualVelocity)
    {
        var velDeltaMagnitude = Mathf.Clamp(Vector3.Distance(actualVelocity, velocityGoal), 0, TargetWalkingSpeed);

        if (TargetWalkingSpeed == 0) TargetWalkingSpeed = 0.01f;

        return Mathf.Pow(1 - Mathf.Pow(velDeltaMagnitude / TargetWalkingSpeed, 2), 2);
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

        var cubeForward = orientationCube.transform.forward;
        var lookAtTargetReward = Vector3.Dot(head.forward, cubeForward) + 1;

        var matchSpeedReward = GetMatchingVelocityReward(cubeForward * TargetWalkingSpeed, GetAvgVelocity());

        if (earlyTraining)
        {
            matchSpeedReward = Vector3.Dot(GetAvgVelocity(), cubeForward);
            if (matchSpeedReward > 0) matchSpeedReward = GetMatchingVelocityReward(cubeForward * TargetWalkingSpeed, GetAvgVelocity());
        }

        //AddReward(matchSpeedReward + 0.1f * lookAtTargetReward);
    }




}
