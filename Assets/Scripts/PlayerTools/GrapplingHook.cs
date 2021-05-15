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
    [Tooltip("Layers where the hook connecting, not the layers for the ray to hit")]
    [SerializeField] LayerMask hookableLayers;
    [Tooltip("Hook prefab")]
    [SerializeField] GameObject hook = null;
    [Tooltip("Width of the line")]
    [SerializeField] float ropeWidth = 0.1f;

    LineRenderer lineRenderer = null; //attached line renderer
    SpringJoint joint = null; // current joint
    GameObject currentHook = null; // current hook
    float loadMass = 10;

    //A list with all rope sections
    public List<Vector3> allRopeSections = new List<Vector3>();

    public bool Hooked
    {
        get
        {
            return joint != null;
        }
    }

    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    private void LateUpdate()
    {
        //UpdateConnection();
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
        if (joint)
        {
            UpdateRope();
        }
    }

    private void StartGrapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance))
        {
            if (hookableLayers != (hookableLayers | (1 << hit.collider.gameObject.layer))) return;
            lineRenderer.enabled = true;
            currentHook = Instantiate(hook, hit.point, Quaternion.identity, hit.collider.transform);
            Debug.Log(currentHook.transform.position + " || " + firePoint.position + " Distance: " +Vector3.Distance(currentHook.transform.position, firePoint.position));

            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;

            joint.connectedAnchor = currentHook.transform.position;
           // joint.anchor = firePoint.position;
            loadMass = player.GetComponent<Rigidbody>().mass;
            float distance = Mathf.Abs(Vector3.Distance(currentHook.transform.position, firePoint.position));
            UpdateSpring(distance);
            UpdateRope();
        }
    }

    //Update the spring constant and the length of the spring
    private void UpdateSpring(float ropeLength)
    {
        //Someone said you could set this to infinity to avoid bounce, but it doesnt work
        //kRope = float.inf

        //
        //The mass of the rope
        //
        //Density of the wire (stainless steel) kg/m3
        float density = 7750f;
        //The radius of the wire
        float radius = 0.01f;

        float volume = Mathf.PI * radius * radius * ropeLength;

        float ropeMass = volume * density;

        //Add what the rope is carrying
        ropeMass += loadMass;


        //
        //The spring constant (has to recalculate if the rope length is changing)
        //
        //The force from the rope F = rope_mass * g, which is how much the top rope segment will carry
        float ropeForce = ropeMass * 9.81f;

        //Use the spring equation to calculate F = k * x should balance this force, 
        //where x is how much the top rope segment should stretch, such as 0.01m

        //Is about 146000
        float kRope = ropeForce / 0.01f;

        //print(ropeMass);

        //Add the value to the spring
        joint.spring = kRope * 1.0f;
        joint.damper = kRope * 0.7f;

        //Update length of the rope
        joint.maxDistance = ropeLength;
    }

    //Display the rope with a line renderer
    private void UpdateRope()
    {
        //This is not the actual width, but the width use so we can see the rope
        if (!joint) return;

        joint.anchor = player.InverseTransformPoint(firePoint.position);
        joint.connectedAnchor = currentHook.transform.position;

        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;

        Vector3[] positions = new Vector3[2];
        positions[1] = firePoint.position;
        positions[0] = currentHook.transform.position;

        //Add the positions to the line renderer
        lineRenderer.positionCount = positions.Length;

        lineRenderer.SetPositions(positions);
    }

    public void StopGrapple()
    {
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
        Destroy(currentHook);
        Destroy(joint);
    }
}
