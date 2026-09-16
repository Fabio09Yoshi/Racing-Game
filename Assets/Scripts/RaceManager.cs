using UnityEngine;

public class RaceManager : MonoBehaviour
{

    public static RaceManager instance;

    public Checkpoints[] allCheckpoint;

    public int totalLaps;

    public bool isStarting;
    public float timeBetweenStartCount = 1f;
    private float startCounter;
    public int countdownCurrent = 3;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i=0; i < allCheckpoint.Length; i++) 
        {
            allCheckpoint[i].checkpointNumber = i;
        }

        isStarting = true;
        startCounter = timeBetweenStartCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarting)
        {
            startCounter -= Time.deltaTime;
            if (startCounter < 0)
            {
                countdownCurrent--;
                startCounter = timeBetweenStartCount;

                if (countdownCurrent == 0)
                {
                    isStarting = false;
                }
            }
        }
        
    }
}
