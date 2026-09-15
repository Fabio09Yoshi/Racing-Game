using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public CarController carCtrl;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            Debug.Log("Hit Checkpoint Number: " + other.GetComponent<Checkpoints>().checkpointNumber);

            carCtrl.CheckpointHit(other.GetComponent<Checkpoints>().checkpointNumber);
        }
    }
}
