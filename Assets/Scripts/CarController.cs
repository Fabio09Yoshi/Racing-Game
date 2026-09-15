using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

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

    public bool isAI;

    public int currentTarget;
    private Vector3 targetPoint;
    public float ai_AccelerateSpeed = 1f, ai_TurnSpeed = .8f, ai_ReachPointRange = 5f, ai_PointVariance = 3f, ai_maxTurn = 30;
    private float ai_speedInput;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.transform.parent = null;

        dragOnGround = rb.linearDamping;

        if (isAI)
        {
            targetPoint = RaceManager.instance.allCheckpoint[currentTarget].transform.position;
            RandomiseAITarget();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!isAI)
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

        }
        else
        {
            targetPoint.y = transform.position.y;

            if (Vector3.Distance(transform.position, targetPoint) < ai_ReachPointRange)
            {
                SetNextAITarget();
            }

            Vector3 targetDirection = targetPoint - transform.position;

            float angle = Vector3.SignedAngle(transform.forward,targetDirection,Vector3.up);

            turnInput = Mathf.Clamp(angle / ai_maxTurn, -1f, 1f);

            if (Mathf.Abs(angle) < ai_maxTurn)
            {
                ai_speedInput = Mathf.MoveTowards(ai_speedInput, 1f, ai_AccelerateSpeed * Time.deltaTime);
            }
            else
            { 
                ai_speedInput = Mathf.MoveTowards(ai_speedInput, ai_TurnSpeed, ai_AccelerateSpeed * Time.deltaTime);
            }
            speedInput = ai_speedInput * forwardAcceleration;
        }

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

        if (grounded && Mathf.Abs(speedInput) > 0.01f)
        {
            float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f,turnInput * turnStrenght * Time.deltaTime * Mathf.Sign(speedInput) * speedFactor,0f));
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

        if (isAI)
        {
            if (checkpointNumber == currentTarget)
            {
                SetNextAITarget();
            }
        }
    }

    public void SetNextAITarget()
    {
        currentTarget++;
        if (currentTarget >= RaceManager.instance.allCheckpoint.Length)
        {
            currentTarget = 0;
        }

        targetPoint = RaceManager.instance.allCheckpoint[currentTarget].transform.position;
        RandomiseAITarget();
    }


    public void RandomiseAITarget()
    {
        targetPoint += new Vector3(Random.Range(-ai_PointVariance, ai_PointVariance), 0f, Random.Range(-ai_PointVariance, ai_PointVariance));
    }
}
