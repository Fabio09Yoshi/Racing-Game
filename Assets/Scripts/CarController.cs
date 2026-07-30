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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.transform.parent = null;
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

        if (Input.GetAxis("Vertical") != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrenght * Time.deltaTime, 0f)); 
        }



        transform.position = rb.position;
    }

    void FixedUpdate()
    {
       rb.AddForce(transform.forward * speedInput * 1000f);
        

    }
}
