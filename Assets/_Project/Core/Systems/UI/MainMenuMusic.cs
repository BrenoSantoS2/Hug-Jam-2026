using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    [Header("Áudio do Menu")]
    public AudioClip musicaMenu;
    [Range(0f, 1f)] public float volume = 0.7f;

    private AudioSource audioSource;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (musicaMenu != null)
        {
            audioSource.clip = musicaMenu;
            audioSource.volume = volume;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.Play();
        }
    }

    private void OnDisable()
    {
        if (audioSource != null)
            audioSource.Stop();    }
}