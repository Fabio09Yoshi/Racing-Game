using System.Collections.Generic;
using UnityEngine;

public class RacePositionManager : MonoBehaviour
{
    public static RacePositionManager instance;

    public CarController player;
    public CarController[] allCars;

    private void Awake()
    {
        instance = this;
    }

    public int GetPlayerPosition()
    {
        List<CarController> ranking = new List<CarController>(allCars);

        ranking.Sort(CompareCars);

        return ranking.IndexOf(player) + 1;
    }

    public string GetPlayerOrdinalPosition()
    {
        int position = GetPlayerPosition();

        return GetOrdinal(position);
    }

    private int CompareCars(CarController a, CarController b)
    {
        // 1. Compara o progresso geral da corrida
        if (a.RaceProgress != b.RaceProgress)
        {
            return b.RaceProgress.CompareTo(a.RaceProgress);
        }

        // 2. Se estão no mesmo checkpoint,
        // quem está mais perto do próximo checkpoint está na frente
        float distanceA = GetDistanceToCheckpoint(a);
        float distanceB = GetDistanceToCheckpoint(b);

        return distanceA.CompareTo(distanceB);
    }

    private float GetDistanceToCheckpoint(CarController car)
    {
        int checkpoint = car.nextCheckpoint;

        if (checkpoint >= RaceManager.instance.allCheckpoint.Length)
        {
            checkpoint = 0;
        }

        Vector3 checkpointPosition =
            RaceManager.instance.allCheckpoint[checkpoint].transform.position;

        return Vector3.Distance(
            car.transform.position,
            checkpointPosition
        );
    }

    private string GetOrdinal(int position)
    {
        if (position % 100 >= 11 && position % 100 <= 13)
        {
            return position + "th";
        }

        switch (position % 10)
        {
            case 1:
                return position + "st";

            case 2:
                return position + "nd";

            case 3:
                return position + "rd";

            default:
                return position + "th";
        }
    }
}