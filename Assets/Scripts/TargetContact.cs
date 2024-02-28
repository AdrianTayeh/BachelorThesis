using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TargetContact : MonoBehaviour
{
    [Header("Detect Targets")] public bool touchingTarget;
    const string k_Target = "target"; // Tag on target object.

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag(k_Target))
        {
            touchingTarget = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.transform.CompareTag(k_Target))
        {
            touchingTarget = false;
        }
    }
}
