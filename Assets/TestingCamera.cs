using UnityEngine;
using Cinemachine;

public class RunwayCamera : MonoBehaviour
{
    public GameObject objectToFollow;
    public float distanceFromObject = 7; // Distance of the camera from the object

    void Start()
    {
        CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();

        // Assign the follow target
        if (vcam != null && objectToFollow != null)
        {
            vcam.Follow = objectToFollow.transform;
            vcam.LookAt = objectToFollow.transform;

            vcam.transform.localPosition = new Vector3(0, 3, -distanceFromObject);

            vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z *= -1;
        }
        else
        {
            Debug.LogWarning("Virtual Camera or Object to follow is not assigned.");
        }
    }
}
