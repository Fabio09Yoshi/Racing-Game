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

    public Transform groundRayPoint;
    public LayerMask groundLayerMask;
    public float groundRayLenghth = 0.75f;

    private float dragOnGround;
    public float gravityModifier = 10f;


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

        if (grounded && Input.GetAxis("Vertical") != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrenght * Time.deltaTime * Mathf.Sign(speedInput) * (rb.linearVelocity.magnitude / maxSpeed), 0f)); 
        }



        transform.position = rb.position;
    }

    void FixedUpdate()
    {

        grounded = false;

        RaycastHit hit;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLenghth, groundLayerMask))
        {
            grounded = true;
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

        Debug.Log(rb.linearVelocity.magnitude);
        

    }
}
