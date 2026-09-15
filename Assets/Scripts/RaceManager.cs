using UnityEngine;

public class RaceManager : MonoBehaviour
{

    public static RaceManager instance;

    public Checkpoints[] allCheckpoint;


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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
