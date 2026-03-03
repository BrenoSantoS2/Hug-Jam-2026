using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Canais de �udio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Biblioteca de Sons")]
    public AudioClip somLixo;
    public AudioClip somComidaEncontrada;
    public AudioClip somPortaAbrindo;
    public AudioClip somPortaFechando;
    public AudioClip somPassosJogador;
    public AudioClip somMonstroRastejando;
    public AudioClip somGhostCorrendo;
    public AudioClip somChuva;
    public AudioClip somTrovao;
    public AudioClip somEsconder;
    public AudioClip somEstáEscondido;
    public AudioClip somEntrandoBeco;
    public AudioClip somGameOver;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAudioSources();
    }

    private void InitializeAudioSources()
    {
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = false;
        musicSource.playOnAwake = false;
        sfxSource.playOnAwake = false;
    }

    private void ValidateAudioSources()
    {
        if (musicSource == null || sfxSource == null)
            InitializeAudioSources();
    }
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        ValidateAudioSources();
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayMusic(AudioClip musicClip)
    {
        ValidateAudioSources();
        if (musicSource == null || musicClip == null)
            return;

        if (musicSource.clip == musicClip)
            return;

        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayLoopSFX(AudioClip clip, float volume = 1f)
    {
        ValidateAudioSources();
        if (clip != null && sfxSource != null)
        {
            sfxSource.clip = clip;
            sfxSource.loop = true;
            sfxSource.volume = volume;
            sfxSource.Play();
        }
    }

    public void StopLoopSFX()
    {
        ValidateAudioSources();
        if (sfxSource != null)
        {
            sfxSource.Stop();
            sfxSource.loop = false;
        }
    }
}
