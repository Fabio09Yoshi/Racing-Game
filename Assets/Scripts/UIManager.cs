using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TMP_Text raceTimerText, lapCounterText, positionText,countdownNumber3, countdownNumber2, countdownNumber1, countdownGo, finalLapText, finishedText;

    public Image countdownLight, countdownRed3, countdownRed2, countdownRed1, countdownGreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }

}
