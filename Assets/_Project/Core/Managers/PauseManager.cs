using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public Button continueButton;
    public bool isPaused;
    public static bool IsGamePaused { get; private set; }

    private InputAction pauseAction;
    private CanvasGroup pauseCanvasGroup;

    void Awake()
    {
        pauseAction = new InputAction("Pause", binding: "<Keyboard>/escape");
        pauseAction.AddBinding("<Gamepad>/start");
        pauseAction.Enable();
    }

    void OnEnable()
    {
        Time.timeScale = 1f;
        isPaused = false;
        IsGamePaused = false;
        
        FindPausePanel();
        TryWireContinueButton();

        HidePausePanel();
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        Time.timeScale = 1f;
        IsGamePaused = false;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnDestroy()
    {
        pauseAction?.Disable();
        pauseAction?.Dispose();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Quando a cena carregar, tenta encontrar o painel de pause novamente
        isPaused = false;
        IsGamePaused = false;
        Time.timeScale = 1f;
        FindPausePanel();
        TryWireContinueButton();
        
        // Esconde o painel usando CanvasGroup
        HidePausePanel();
    }

    private void FindPausePanel()
    {
        // Se já tem uma referência válida, não precisa buscar
        if (pausePanel != null && pauseCanvasGroup != null)
            return;

        // Tenta encontrar por tag
        GameObject foundPanel = GameObject.FindGameObjectWithTag("PausePanel");
        if (foundPanel != null)
        {
            pausePanel = foundPanel;
            pauseCanvasGroup = foundPanel.GetComponent<CanvasGroup>();
            return;
        }

        // Se não encontrou por tag, tenta encontrar por nome
        foundPanel = GameObject.Find("PausePanel");
        if (foundPanel == null)
        {
            foundPanel = GameObject.Find("Pause Panel");
        }
        if (foundPanel == null)
        {
            foundPanel = GameObject.Find("Panel Pause");
        }

        if (foundPanel != null)
        {
            pausePanel = foundPanel;
            pauseCanvasGroup = foundPanel.GetComponent<CanvasGroup>();
        }
    }

    void Update()
    {
        if (pauseAction.WasPressedThisFrame())
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        // Tenta encontrar o painel se não tiver referência
        if (pausePanel == null || pauseCanvasGroup == null)
        {
            FindPausePanel();
        }

        TryWireContinueButton();

        // Valida se o painel ainda existe antes de pausar
        if (pausePanel == null || pauseCanvasGroup == null)
        {
            Debug.LogWarning("PausePanel não foi encontrado na cena ou não possui CanvasGroup! Certifique-se de que existe um GameObject com a tag 'PausePanel' e componente CanvasGroup.");
            return;
        }

        ShowPausePanel();
        Time.timeScale = 0f;
        isPaused = true;
        IsGamePaused = true;
    }

    public void ResumeGame()
    {
        HidePausePanel();
        Time.timeScale = 1f;
        isPaused = false;
        IsGamePaused = false;
    }

    public void GoToMainMenu(string menuSceneName)
    {
        Debug.Log("GoToMainMenu chamado - Parando áudios");
        StopAllAudio();
        
        // Reseta os flashbacks para que voltem a tocar quando entrar nas fases novamente
        LevelStarter.ResetFlashbacksFromMenu();
        
        Time.timeScale = 1f;
        isPaused = false;
        IsGamePaused = false;
        
        SceneManager.LoadScene(menuSceneName);
    }

    private void TryWireContinueButton()
    {
        if (pausePanel == null)
            return;

        if (continueButton == null)
        {
            Button[] buttons = pausePanel.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                string buttonName = button.gameObject.name.ToLowerInvariant();
                if (buttonName.Contains("continu") || buttonName.Contains("resume") || buttonName.Contains("continue"))
                {
                    continueButton = button;
                    break;
                }
            }
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(ResumeGame);
            continueButton.onClick.AddListener(ResumeGame);
        }
    }

    private void StopAllAudio()
    {
        if (SoundManager.Instance != null)
        {
            Debug.Log("Chamando StopAllSounds no SoundManager");
            SoundManager.Instance.StopAllSounds();
        }
        else
        {
            Debug.LogWarning("SoundManager.Instance é NULL!");
        }
        
        // Para TODOS os AudioSources da cena (incluindo os que não são do SoundManager)
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        Debug.Log($"Parando {allAudioSources.Length} AudioSources na cena");
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
                Debug.Log($"AudioSource parado: {audioSource.gameObject.name}");
            }
        }
    }

    private void ShowPausePanel()
    {
        if (pauseCanvasGroup != null)
        {
            pauseCanvasGroup.alpha = 1f;
            pauseCanvasGroup.interactable = true;
            pauseCanvasGroup.blocksRaycasts = true;
        }
    }

    private void HidePausePanel()
    {
        if (pauseCanvasGroup != null)
        {
            pauseCanvasGroup.alpha = 0f;
            pauseCanvasGroup.interactable = false;
            pauseCanvasGroup.blocksRaycasts = false;
        }
    }
}
