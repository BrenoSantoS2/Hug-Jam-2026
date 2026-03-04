using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Canais de Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopSfxSource;
    [SerializeField] private AudioSource ambienceLoopSource;
    [SerializeField] private AudioSource interactionLoopSource;
    
    [Header("Controle de Volume")]
    [Range(0f, 1f)] public float volumeMaster = 1f;
    [Range(0f, 1f)] public float volumeMusica = 0.7f;
    [Range(0f, 1f)] public float volumeSFX = 1f;
    [Range(0f, 1f)] public float volumeLoops = 0.8f;
    [Range(0f, 1f)] public float volumeAmbiente = 0.5f;
    [Range(0f, 1f)] public float volumeInteracao = 0.7f;
    
    [Header("Biblioteca de Sons")]
    public AudioClip musicaPrincipal;
    public AudioClip somLixo;
    public AudioClip somRevirandoLixo;
    public AudioClip somComidaEncontrada;
    public AudioClip somPortaAbrindo;
    public AudioClip somPortaFechando;
    public AudioClip somPassosJogador;
    public AudioClip somMonstroRastejando;
    public AudioClip somMonstroViuJogador;
    public AudioClip somGhostCorrendo;
    public AudioClip somChuva;
    public AudioClip somTrovao;
    public AudioClip somEsconder;
    public AudioClip somEstáEscondido;
    public AudioClip somEntrandoBeco;
    public AudioClip somGameOver;
    public AudioClip somBotao;


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

        if (musicaPrincipal != null)
            PlayMusic(musicaPrincipal);
    }

    private void InitializeAudioSources()
    {
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();
        if (loopSfxSource == null)
            loopSfxSource = gameObject.AddComponent<AudioSource>();
        if (ambienceLoopSource == null)
            ambienceLoopSource = gameObject.AddComponent<AudioSource>();
        if (interactionLoopSource == null)
            interactionLoopSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = false;
        musicSource.playOnAwake = false;
        sfxSource.playOnAwake = false;
        loopSfxSource.loop = true;
        loopSfxSource.playOnAwake = false;
        ambienceLoopSource.loop = true;
        ambienceLoopSource.playOnAwake = false;
        interactionLoopSource.loop = true;
        interactionLoopSource.playOnAwake = false;
    }

    private void ValidateAudioSources()
    {
        if (musicSource == null || sfxSource == null || loopSfxSource == null || ambienceLoopSource == null || interactionLoopSource == null)
            InitializeAudioSources();
    }
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        ValidateAudioSources();
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume * volumeSFX * volumeMaster);
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
        musicSource.volume = volumeMusica * volumeMaster;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayLoopSFX(AudioClip clip, float volume = 1f)
    {
        ValidateAudioSources();
        if (clip != null && loopSfxSource != null)
        {
            loopSfxSource.clip = clip;
            loopSfxSource.loop = true;
            loopSfxSource.volume = volume * volumeLoops * volumeMaster;
            loopSfxSource.Play();
        }
    }

    public void StopLoopSFX()
    {
        ValidateAudioSources();
        if (loopSfxSource != null)
        {
            loopSfxSource.Stop();
            loopSfxSource.loop = false;
        }
    }

    public void PlayAmbienceLoopSFX(AudioClip clip, float volume = 1f)
    {
        ValidateAudioSources();
        if (clip != null && ambienceLoopSource != null)
        {
            ambienceLoopSource.clip = clip;
            ambienceLoopSource.loop = true;
            ambienceLoopSource.volume = volume * volumeAmbiente * volumeMaster;
            ambienceLoopSource.Play();
        }
    }

    public void StopAmbienceLoopSFX()
    {
        ValidateAudioSources();
        if (ambienceLoopSource != null)
        {
            ambienceLoopSource.Stop();
            ambienceLoopSource.loop = false;
        }
    }

    public void PlayInteractionLoopSFX(AudioClip clip, float volume = 1f)
    {
        ValidateAudioSources();
        if (clip != null && interactionLoopSource != null)
        {
            interactionLoopSource.clip = clip;
            interactionLoopSource.loop = true;
            interactionLoopSource.volume = volume * volumeInteracao * volumeMaster;
            interactionLoopSource.Play();
        }
    }

    public void StopInteractionLoopSFX()
    {
        ValidateAudioSources();
        if (interactionLoopSource != null)
        {
            interactionLoopSource.Stop();
            interactionLoopSource.loop = false;
        }
    }

    public void MuteMusic()
    {
        ValidateAudioSources();
        if (musicSource != null)
            musicSource.volume = 0f;
    }

    public void UnmuteMusic()
    {
        ValidateAudioSources();
        if (musicSource != null)
            musicSource.volume = volumeMusica * volumeMaster;
    }

    public void MuteAmbience()
    {
        ValidateAudioSources();
        if (ambienceLoopSource != null)
            ambienceLoopSource.volume = 0f;
    }

    public void UnmuteAmbience()
    {
        ValidateAudioSources();
        if (ambienceLoopSource != null)
            ambienceLoopSource.volume = volumeAmbiente * volumeMaster;
    }

    public void MuteAllExceptSFX()
    {
        MuteMusic();
        MuteAmbience();
        if (loopSfxSource != null)
            loopSfxSource.volume = 0f;
        if (interactionLoopSource != null)
            interactionLoopSource.volume = 0f;
    }}