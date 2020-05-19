using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    LineRenderer lineRenderer;
    Vector3 grapplePoint;
    [SerializeField] LayerMask grappleableLayer;
    [SerializeField] Transform firePoint;
    [SerializeField] Transform playerCamera;
    [SerializeField] Transform player;
    [SerializeField] int maxDistance;
    [Header("JointData")]
    [Tooltip("Max grapplePoint- Player distance multi to distance on contact")]
    [SerializeField] float jointDistanceMaxMulti = 0.8f;
    [Tooltip("Min grapplePoint- Player distance multi to distance on contact")]
    [SerializeField] float jointDistanceMinMulti = 0.25f;
    [SerializeField] float jointSpringStrength = 4.5f;
    [SerializeField] float jointDamperStrength = 7f;
    [SerializeField] float jointMassScale = 4.5f;
    SpringJoint joint;

    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    private void LateUpdate()
    {
        DrawRope();
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
        if(Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance, grappleableLayer))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

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
        Destroy(joint);
    }

    void DrawRope()
    {
        if (!joint) return; //if not grappling, don´t draw
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
}
