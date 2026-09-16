using JetBrains.Annotations;
using System.Collections;
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

    public int nextCheckpoint;
    public int currentLap;

    public bool isAI;

    public int currentTarget;
    private Vector3 targetPoint;
    public float ai_AccelerateSpeed = 1f, ai_TurnSpeed = .8f, ai_ReachPointRange = 5f, ai_PointVariance = 3f, ai_maxTurn = 30;
    private float ai_speedInput, ai_speedMod;

    public float raceTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.transform.parent = null;

        dragOnGround = rb.linearDamping;

        UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;


        if (isAI)
        {
            targetPoint = RaceManager.instance.allCheckpoint[currentTarget].transform.position;
            RandomiseAITarget();

            ai_speedMod = Random.Range(1, 1.5f);
        }


    }

    void Update()
    {
        bool isGameActive = RaceManager.instance.currentState == RaceManager.RaceState.Racing ||
                           RaceManager.instance.currentState == RaceManager.RaceState.Finished;

        if (RaceManager.instance != null && !isGameActive)
        {
            speedInput = 0f;
            turnInput = 0f;
            return;
        }

        raceTime += Time.deltaTime;

        if (!isAI)
        {
            var ts = System.TimeSpan.FromSeconds(raceTime);
            UIManager.instance.raceTimerText.text = string.Format("{0:00}:{1:00}:{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);

            int position = RacePositionManager.instance.GetPlayerPosition();
            UIManager.instance.positionText.text = RacePositionManager.instance.GetPlayerOrdinalPosition();

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
        }
        else
        {
            // Lógica de pilotagem da IA (roda tanto para os bots quanto para o jogador ao terminar)
            targetPoint.y = transform.position.y;

            if (Vector3.Distance(transform.position, targetPoint) < ai_ReachPointRange)
            {
                SetNextAITarget();
            }

            Vector3 targetDirection = targetPoint - transform.position;

            float angle = Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up);

            turnInput = Mathf.Clamp(angle / ai_maxTurn, -1f, 1f);

            if (Mathf.Abs(angle) < ai_maxTurn)
            {
                ai_speedInput = Mathf.MoveTowards(ai_speedInput, 1f, ai_AccelerateSpeed * Time.deltaTime);
            }
            else
            {
                ai_speedInput = Mathf.MoveTowards(ai_speedInput, ai_TurnSpeed, ai_AccelerateSpeed * Time.deltaTime);
            }
            speedInput = ai_speedInput * forwardAcceleration * ai_speedMod;
        }

        // Rotacionar rodas
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);
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
       //Debug.Log(gameObject.name + " | Checkpoint: " + checkpointNumber + " | Esperando: " + nextCheckpoint);
        if (checkpointNumber == nextCheckpoint)
        {
            nextCheckpoint++;

            if (nextCheckpoint == RaceManager.instance.allCheckpoint.Length)
            { 
                nextCheckpoint = 0;
                LapCompleted();
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


    public void LapCompleted()
    {
        if (currentLap >= RaceManager.instance.totalLaps)
        {
            currentLap = RaceManager.instance.totalLaps; 

            if (!isAI)
            {
                UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;
                FinishRaceForPlayer();
            }
            return;
        }

        currentLap++;

        if (!isAI)
        {
            UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;

            // Entrou na última volta
            if (currentLap == RaceManager.instance.totalLaps)
            {
                RaceManager.instance.StartShowUIFinalLap();
            }
        }
    }
    public int RaceProgress
    {
        get
        {
            return currentLap * RaceManager.instance.allCheckpoint.Length + nextCheckpoint;
        }
    }

    private void FinishRaceForPlayer()
    {
        isAI = true; 


        currentTarget = nextCheckpoint;
        targetPoint = RaceManager.instance.allCheckpoint[currentTarget].transform.position;
        RandomiseAITarget();

        ai_speedMod = 1f;

        RaceManager.instance.FinishedTheRace();
    }

}
