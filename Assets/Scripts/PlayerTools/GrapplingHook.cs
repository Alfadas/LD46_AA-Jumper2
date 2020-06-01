using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [Tooltip("Attatchment point to player")]
    [SerializeField] Transform firePoint = null;
    [Tooltip("Main camera")]
    [SerializeField] Transform playerCamera = null;
    [Tooltip("Transform of the player")]
    [SerializeField] Transform player = null;
    [Tooltip("Max distance for the hook to hit")]
    [SerializeField] int maxDistance = 100;
    [Tooltip("Hook prefab")]
    [SerializeField] GameObject hook = null;
    [Header("JointData")]
    [Tooltip("Max grapplePoint- Player distance multi to distance on contact")]
    [SerializeField] float jointDistanceMaxMulti = 0.8f;
    [Tooltip("Min grapplePoint- Player distance multi to distance on contact")]
    [SerializeField] float jointDistanceMinMulti = 0.25f;
    [SerializeField] float jointSpringStrength = 4.5f;
    [SerializeField] float jointDamperStrength = 7f;
    [SerializeField] float jointMassScale = 4.5f;
    LineRenderer lineRenderer = null; //attached line renderer
    SpringJoint joint = null; // current joint
    GameObject currentHook = null; // current hook

    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    private void LateUpdate()
    {
        UpdateConnection();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Grapple"))
        {
            if (joint)
            {
                StopGrapple();
            }
            else
            {
                StartGrapple();
            }
            
        }
    }

    private void StartGrapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance))
        {
            currentHook = Instantiate(hook, hit.point, Quaternion.identity, hit.collider.transform);

            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = currentHook.transform.position;

            float distanceFromPoint = Vector3.Distance(player.position, currentHook.transform.position);

            joint.maxDistance = distanceFromPoint * jointDistanceMaxMulti;
            joint.minDistance = distanceFromPoint * jointDistanceMinMulti;

            joint.spring = jointSpringStrength;
            joint.damper = jointDamperStrength;
            joint.massScale = jointMassScale;

            lineRenderer.positionCount = 2;
        }
    }

    void StopGrapple()
    {
        lineRenderer.positionCount = 0;
        Destroy(currentHook);
        Destroy(joint);
    }

    void UpdateConnection()
    {
        if (!joint) return; //if not grappling, don´t draw
        joint.connectedAnchor = currentHook.transform.position;
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, joint.connectedAnchor);
    }
}
