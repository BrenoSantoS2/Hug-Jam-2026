using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Canais de Áudio")]
    [SerializeField] private AudioSource musicSource; // Para trilha sonora (Loop)
    [SerializeField] private AudioSource sfxSource; // Para efeitos sonoros (one shot)

    [Header("Biblioteca de Sons")]
    // Adicionar os clipes de som aqui para facil acesso
    public AudioClip somLixo;
    public AudioClip somPortaAbrindo;
    public AudioClip somPassosMonstro;
    public AudioClip somEsconder;
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
    }

    // Toca efeito sonoro uma única vez
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if(clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    // Toca ou troca a música de fundo (em loop)
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource.clip == musicClip) return;

        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.Play();
    }
}
