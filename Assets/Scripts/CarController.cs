using JetBrains.Annotations;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody rb;

    public float maxSpeed;

    public float forwardAcceleration = 8f;
    public float reverseAcceleration = 4f;
    private float speedInput;

    public float turnStrenght = 180f;
    private float turnInput;

    private bool grounded;

    public Transform groundRayPoint, groundRayPoint2;
    public LayerMask groundLayerMask;
    public float groundRayLenghth = 0.75f;

    private float dragOnGround;
    public float gravityModifier = 10f;

    public Transform leftFrontWheel, rightFrontWheel;
    public float maxWheelTurn = 25f;

    private int nextCheckpoint;
    public int currentLap;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.transform.parent = null;

        dragOnGround = rb.linearDamping;
    }

    // Update is called once per frame
    void Update()
    {
        speedInput = 0f;
        if (Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * forwardAcceleration;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverseAcceleration;

        }

        turnInput = Input.GetAxis("Horizontal");

        //if (grounded && Input.GetAxis("Vertical") != 0)
        //{
        //    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrenght * Time.deltaTime * Mathf.Sign(speedInput) * (rb.linearVelocity.magnitude / maxSpeed), 0f)); 
        //}

        //Turning Wheels

        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);

        // transform.position = rb.position;
    }

    void FixedUpdate()
    {

        grounded = false;

        RaycastHit hit;
        Vector3 normalTarget = Vector3.zero;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLenghth, groundLayerMask))
        {
            grounded = true;

            normalTarget = hit.normal;
        }
        if (Physics.Raycast(groundRayPoint2.position, -transform.up, out hit, groundRayLenghth, groundLayerMask))
        {
            grounded = true;

            normalTarget = (normalTarget + hit.normal) / 2f;
        }


        //Quando on ground rotaciona para encontrar o estado normal
        if (grounded)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation;
        }

        //Accelerates the Car
        if (grounded)
        {
            rb.linearDamping = dragOnGround;

            rb.AddForce(transform.forward * speedInput * 1000f);
        }
        else
        {
            rb.linearDamping = 0.1f;

            rb.AddForce(-Vector3.up * gravityModifier * 100f);

        }


        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        //Debug.Log(rb.linearVelocity.magnitude);

        transform.position = rb.position;

        if (grounded && Input.GetAxis("Vertical") != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrenght * Time.deltaTime * Mathf.Sign(speedInput) * (rb.linearVelocity.magnitude / maxSpeed), 0f));
        }


    }

    public void CheckpointHit(int checkpointNumber)
    {
        if (checkpointNumber == nextCheckpoint)
        {
            nextCheckpoint++;

            if (nextCheckpoint == RaceManager.instance.allCheckpoint.Length)
            { 
                nextCheckpoint = 0;
                currentLap++;
            }
        }

    }
}
