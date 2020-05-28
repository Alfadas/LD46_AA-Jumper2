using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField] Transform firePoint;
    [SerializeField] Transform playerCamera;
    [SerializeField] Transform player;
    [SerializeField] int maxDistance;
    [SerializeField] GameObject hook;
    [Header("JointData")]
    [Tooltip("Max grapplePoint- Player distance multi to distance on contact")]
    [SerializeField] float jointDistanceMaxMulti = 0.8f;
    [Tooltip("Min grapplePoint- Player distance multi to distance on contact")]
    [SerializeField] float jointDistanceMinMulti = 0.25f;
    [SerializeField] float jointSpringStrength = 4.5f;
    [SerializeField] float jointDamperStrength = 7f;
    [SerializeField] float jointMassScale = 4.5f;
    SpringJoint joint;
    GameObject currentHook;

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
