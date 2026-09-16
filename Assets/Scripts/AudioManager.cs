using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("SFX Contagem")]
    public AudioClip audioBeep;
    public AudioClip audioGo;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    public void PlayCountdownBeep() => PlaySFX(audioBeep);
    public void PlayCountdownGo() => PlaySFX(audioGo);
}