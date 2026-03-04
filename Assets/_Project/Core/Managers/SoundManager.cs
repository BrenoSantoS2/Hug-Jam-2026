using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    [Header("Configurações de Cena")]
    public string mainMenuSceneName = "MainMenu";
    public string level1SceneName = "Level1";
    
    private bool isInMainMenu = false;
    
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

        // Escuta mudanças de cena
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Verifica se está iniciando no MainMenu
        CheckIfMainMenuScene();

        if (musicaPrincipal != null && !isInMainMenu)
            PlayMusic(musicaPrincipal);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckIfMainMenuScene();

        if (!isInMainMenu)
        {
            RestartLevelMusic();
        }
    }

    private void RestartLevelMusic()
    {
        ValidateAudioSources();
        if (musicSource == null || musicaPrincipal == null)
            return;

        musicSource.Stop();
        musicSource.clip = musicaPrincipal;
        musicSource.volume = volumeMusica * volumeMaster;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void CheckIfMainMenuScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        bool wasInMainMenu = isInMainMenu;
        isInMainMenu = currentScene.Equals(mainMenuSceneName, System.StringComparison.OrdinalIgnoreCase);

        Debug.Log($"CheckIfMainMenuScene: Cena atual = {currentScene}, isInMainMenu = {isInMainMenu}");

        if (isInMainMenu && !wasInMainMenu)
        {
            // Entrou no MainMenu - muta tudo
            MuteAllSoundManager();
            Debug.Log("SoundManager: Entrou no MainMenu - todos os volumes = 0");
        }
        else if (!isInMainMenu && wasInMainMenu)
        {
            // Saiu do MainMenu - restaura volumes e reinicia música se necessário
            UnmuteAllSoundManager();
            Debug.Log("SoundManager: Saiu do MainMenu - volumes restaurados");
        }
        else if (!isInMainMenu)
        {
            // Está em um nível - garante que a música continue tocando
            if (musicSource != null && musicaPrincipal != null)
            {
                if (!musicSource.isPlaying)
                {
                    Debug.Log("SoundManager: Música não está tocando, reiniciando...");
                    PlayMusic(musicaPrincipal);
                }
                else
                {
                    Debug.Log($"SoundManager: Música {musicSource.clip?.name} está tocando corretamente");
                }
            }
        }

        // Verifica se está no Level 1 e para a chuva se estiver tocando
        if (currentScene.Equals(level1SceneName, System.StringComparison.OrdinalIgnoreCase))
        {
            StopAmbienceLoopSFX();
            Debug.Log("SoundManager: Level 1 detectado - som de ambiente (chuva) parado");
        }
    }

    private void MuteAllSoundManager()
    {
        ValidateAudioSources();
        if (musicSource != null) musicSource.volume = 0f;
        if (sfxSource != null) sfxSource.volume = 0f;
        if (loopSfxSource != null) loopSfxSource.volume = 0f;
        if (ambienceLoopSource != null) ambienceLoopSource.volume = 0f;
        if (interactionLoopSource != null) interactionLoopSource.volume = 0f;
    }

    private void UnmuteAllSoundManager()
    {
        ValidateAudioSources();
        if (musicSource != null) 
        {
            musicSource.volume = volumeMusica * volumeMaster;
            // Reinicia a música principal se ela não estiver tocando
            if (musicaPrincipal != null && !musicSource.isPlaying)
            {
                PlayMusic(musicaPrincipal);
            }
        }
        if (sfxSource != null) sfxSource.volume = volumeSFX * volumeMaster;
        if (loopSfxSource != null) loopSfxSource.volume = volumeLoops * volumeMaster;
        if (ambienceLoopSource != null) ambienceLoopSource.volume = volumeAmbiente * volumeMaster;
        if (interactionLoopSource != null) interactionLoopSource.volume = volumeInteracao * volumeMaster;
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

        musicSource.loop = true;
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
        {
            Debug.LogWarning("PlayMusic: musicSource ou musicClip é null");
            return;
        }

        // Se já está tocando a mesma música, não precisa reiniciar
        if (musicSource.clip == musicClip && musicSource.isPlaying)
        {
            Debug.Log($"PlayMusic: {musicClip.name} já está tocando");
            return;
        }

        Debug.Log($"PlayMusic: Iniciando {musicClip.name}");
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
    }

    public void StopAllSounds()
    {
        ValidateAudioSources();
        
        Debug.Log("StopAllSounds chamado - Parando TODOS os áudios");
        
        // Para a música
        if (musicSource != null)
        {
            musicSource.Stop();
            musicSource.clip = null;
        }

        // Para todos os SFX
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }

        // Para todos os loops e limpa os clips
        if (loopSfxSource != null)
        {
            loopSfxSource.Stop();
            loopSfxSource.loop = false;
            loopSfxSource.clip = null;
        }
        
        if (ambienceLoopSource != null)
        {
            ambienceLoopSource.Stop();
            ambienceLoopSource.loop = false;
            ambienceLoopSource.clip = null;
        }
        
        if (interactionLoopSource != null)
        {
            interactionLoopSource.Stop();
            interactionLoopSource.loop = false;
            interactionLoopSource.clip = null;
        }
    }
}