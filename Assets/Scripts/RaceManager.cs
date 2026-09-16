using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;

    public enum RaceState { Waiting, Countdown, Racing, Finished}

    public RaceState currentState = RaceState.Waiting;

    [Header("Countdown Configuration")]
    public float delayBeforeCountdown = 1f; // Tempo de espera antes do número 3 aparecer
    public float timeBetweenCount = 1f;    
    public float uiElementsDisplayDuration = 1f; //Tempo do Go e Final Lap na tela

    [Header("Gameplay Events")]
    public UnityEvent onRaceStart; 

    public Checkpoints[] allCheckpoint;
    public int totalLaps;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        for (int i = 0; i < allCheckpoint.Length; i++)
        {
            allCheckpoint[i].checkpointNumber = i;
        }

        HideAllCountdownUI();

        StartRaceCountdown();
    }

    /// Método público para no futuro podr ser chamado por um gerenciador de cutscene.
    public void StartRaceCountdown()
    {
        if (currentState != RaceState.Waiting) return;

        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        currentState = RaceState.Countdown;

        yield return new WaitForSeconds(delayBeforeCountdown);

        UIManager.instance.countdownLight.gameObject.SetActive(true);

        ShowUIForNumber(3);
        AudioManager.instance.PlayCountdownBeep();
        yield return new WaitForSeconds(timeBetweenCount);

        ShowUIForNumber(2);
        AudioManager.instance.PlayCountdownBeep();
        yield return new WaitForSeconds(timeBetweenCount);

        ShowUIForNumber(1);
        AudioManager.instance.PlayCountdownBeep();
        yield return new WaitForSeconds(timeBetweenCount);

        currentState = RaceState.Racing;
        ShowUIForNumber(0);
        AudioManager.instance.PlayCountdownGo();
        UIManager.instance.countdownLight.gameObject.SetActive(false);

        onRaceStart?.Invoke();

        yield return new WaitForSeconds(uiElementsDisplayDuration);
        HideAllCountdownUI();
    }

    private void ShowUIForNumber(int number)
    {
        UIManager.instance.countdownNumber3.gameObject.SetActive(number == 3);
        UIManager.instance.countdownRed1.gameObject.SetActive(number == 3);
        UIManager.instance.countdownNumber2.gameObject.SetActive(number == 2);
        UIManager.instance.countdownRed2.gameObject.SetActive(number == 2);
        UIManager.instance.countdownNumber1.gameObject.SetActive(number == 1);
        UIManager.instance.countdownRed3.gameObject.SetActive(number == 1);
        UIManager.instance.countdownGo.gameObject.SetActive(number == 0);
        UIManager.instance.countdownGreen.gameObject.SetActive(number == 0);
    }

    private void HideAllCountdownUI()
    {
        UIManager.instance.countdownNumber3.gameObject.SetActive(false);
        UIManager.instance.countdownRed1.gameObject.SetActive(false);
        UIManager.instance.countdownNumber2.gameObject.SetActive(false);
        UIManager.instance.countdownRed2.gameObject.SetActive(false);
        UIManager.instance.countdownNumber1.gameObject.SetActive(false);
        UIManager.instance.countdownRed3.gameObject.SetActive(false);
        UIManager.instance.countdownGo.gameObject.SetActive(false);
        UIManager.instance.countdownGreen.gameObject.SetActive(false);

        //if (UIManager.instance.countdownLight != null)
        //{
        //    UIManager.instance.countdownLight.gameObject.SetActive(false);
        //}

        if (UIManager.instance.finalLapText != null)
        {
            UIManager.instance.finalLapText.gameObject.SetActive(false);
        }

        if (UIManager.instance.finishedText != null)
        {
            UIManager.instance.finishedText.gameObject.SetActive(false);
        }

    }

    public void StartShowUIFinalLap()
    {
        if (currentState == RaceState.Finished) return;

        StartCoroutine(ShowUIFinalLap());
    }

    public void FinishedTheRace()
    {
        if (currentState == RaceState.Finished) return;

        currentState = RaceState.Finished;
        UIManager.instance.finishedText.gameObject.SetActive(true);          
    }

    public IEnumerator ShowUIFinalLap()
    {

        if (UIManager.instance.finalLapText != null)
        {
            UIManager.instance.finalLapText.gameObject.SetActive(true);
        }

        //Adicionar Som de Final Lap

        UIManager.instance.finalLapText.gameObject.SetActive(true);

        yield return new WaitForSeconds(uiElementsDisplayDuration);

        if (UIManager.instance.finalLapText != null)
        {
            UIManager.instance.finalLapText.gameObject.SetActive(false);
        }
    }


}