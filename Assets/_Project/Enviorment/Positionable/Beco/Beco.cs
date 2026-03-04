using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(InteractableItem))]
public class Beco : MonoBehaviour
{
    [Header("Requisitos")]
    public int foodRequired = 3;
    public bool requireAllItemsExplored = false;

    [Header("Cena")]
    public string nextSceneName = "";

    [Header("Visual")]
    public CanvasGroup fadeCanvasGroup;
    public GameObject flashbackPrefab;
    public bool isVideo = false; 
    public VideoPlayer videoPlayer; 
    public CanvasGroup videoCanvasGroup; 
    public VideoClip finalVideoClip;

    [Header("Timing")]
    public float fadeOutDuration = 1f;

    private InteractableItem interactable;
    private Coroutine conclusionSequence;


    void Awake()
    {
        interactable = GetComponent<InteractableItem>();
        if (interactable != null)
        {
            interactable.onInteractionComplete.AddListener(OnInteraction);
        }

        if (fadeCanvasGroup == null)
            fadeCanvasGroup = FindFirstObjectByType<CanvasGroup>();

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;

        ResetAndClearVideoPlayer();
    }

    public void OnInteraction()
    {
        if (requireAllItemsExplored)
        {
            int total = GameManager.Instance.GetTotalItemsCount();
            int explored = GameManager.Instance.GetExploredItemsCount();
            
            // Verificar se há itens registrados E se todos foram explorados
            if (total == 0 || explored < total)
            {
                DialogueSystem.Instance.ShowDialogue(DialogueType.ExplorationIncomplete);
                Debug.Log($"Exploração incompleta! Visitou {explored}/{total} locais.");
                return;
            }
        }
        else if (GameManager.Instance == null || GameManager.Instance.foodCollected < foodRequired)
        {
            DialogueSystem.Instance.ShowDialogue(DialogueType.NotEnoughFood);
            Debug.Log($"Comida insuficiente! Tem {(GameManager.Instance != null ? GameManager.Instance.foodCollected : 0)}, precisa de {foodRequired}.");
            return;
        }

        if (SoundManager.Instance != null && SoundManager.Instance.somEntrandoBeco != null)
            SoundManager.Instance.PlaySFX(SoundManager.Instance.somEntrandoBeco);

        if (conclusionSequence != null)
            StopCoroutine(conclusionSequence);

        conclusionSequence = StartCoroutine(ConclusionSequence());
    }

    private IEnumerator ConclusionSequence()
    {
        Time.timeScale = 0f;

        yield return FadeOutScreen();

        if (isVideo)
        {
            if (videoPlayer != null && finalVideoClip != null)
            {
                videoPlayer.clip = finalVideoClip;

                if (videoPlayer.gameObject != null && !videoPlayer.gameObject.activeInHierarchy)
                    videoPlayer.gameObject.SetActive(true);

                if (videoCanvasGroup != null)
                {
                    videoCanvasGroup.alpha = 0f;
                    yield return StartCoroutine(FadeCanvas(videoCanvasGroup, 0f, 1f, fadeOutDuration));
                }
                videoPlayer.Play();

                float startTime = Time.realtimeSinceStartup;
                while (videoPlayer.isPlaying && (Time.realtimeSinceStartup - startTime) < (float)finalVideoClip.length)
                    yield return null;

                if (videoCanvasGroup != null)
                    yield return StartCoroutine(FadeCanvas(videoCanvasGroup, 1f, 0f, fadeOutDuration));

                ResetAndClearVideoPlayer();
            }
        }
        else
        {
            if (flashbackPrefab != null)
            {
                GameObject inst = Instantiate(flashbackPrefab);
                Canvas instCanvas = inst.GetComponentInChildren<Canvas>();
                if (instCanvas != null)
                    instCanvas.sortingOrder = 10000;
                yield return new WaitUntil(() => inst == null || !inst.activeInHierarchy);
            }
        }

        Time.timeScale = 1f;
        LoadNextScene();
    }


    private IEnumerator FadeOutScreen()
    {
        if (fadeCanvasGroup == null)
            yield break;

        float elapsed = 0f;
        float realStartTime = Time.realtimeSinceStartup;
        while (elapsed < fadeOutDuration)
        {
            elapsed = Time.realtimeSinceStartup - realStartTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeOutDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeCanvas(CanvasGroup cg, float start, float end, float duration)
    {
        if (cg == null) yield break;
        float elapsed = 0f;
        float realStartTime = Time.realtimeSinceStartup;
        while (elapsed < duration)
        {
            elapsed = Time.realtimeSinceStartup - realStartTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        cg.alpha = end;
    }

    private void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("Não há próxima cena após esta!");
            }
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void ResetAndClearVideoPlayer()
    {
        if (videoPlayer == null)
            return;

        videoPlayer.Stop();
        videoPlayer.isLooping = false;
        videoPlayer.clip = null;

        if (videoPlayer.targetTexture != null)
        {
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = videoPlayer.targetTexture;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = previous;
        }
    }
}

