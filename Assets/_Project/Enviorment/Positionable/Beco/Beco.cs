using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(InteractableItem))]
public class Beco : MonoBehaviour
{
    [Header("Requisitos")]
    public int foodRequired = 3;

    [Header("Cena")]
    public string nextSceneName = "";

    [Header("Visual")]
    public CanvasGroup fadeCanvasGroup;
    public Animator flashbackAnimator;
    public GameObject flashbackPrefab;

    [Header("Timing")]
    public float fadeOutDuration = 1f;
    public float flashbackDuration = 3f;

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
    }

    public void OnInteraction()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("GameManager não encontrado!");
            return;
        }

        if (gameManager.foodCollected < foodRequired)
        {
            DialogueSystem.Instance.ShowDialogue(DialogueType.NotEnoughFood);
            Debug.Log($"Comida insuficiente! Tem {gameManager.foodCollected}, precisa de {foodRequired}.");
        }
        else
        {
            if (conclusionSequence != null)
                StopCoroutine(conclusionSequence);

            conclusionSequence = StartCoroutine(ConclusionSequence());
        }
    }

    private IEnumerator ConclusionSequence()
    {
        yield return FadeOutScreen();

        if (flashbackPrefab != null)
        {
            GameObject inst = Instantiate(flashbackPrefab);

            Canvas instCanvas = inst.GetComponentInChildren<Canvas>();
            if (instCanvas != null)
                instCanvas.sortingOrder = 10000;

            yield return FadeRevealScreen();
            yield return new WaitUntil(() => inst == null || !inst.activeInHierarchy);
            yield return FadeOutScreen();
        }
        else if (flashbackAnimator != null)
        {
            yield return FadeRevealScreen();

            flashbackAnimator.SetTrigger("play");
            yield return new WaitForSeconds(flashbackDuration);

            yield return FadeOutScreen();
        }

        LoadNextScene();
    }

    private IEnumerator FadeRevealScreen()
    {
        if (fadeCanvasGroup == null)
            yield break;

        float elapsed = 0f;
        float startAlpha = fadeCanvasGroup.alpha;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / fadeOutDuration));
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
    }

    private IEnumerator FadeOutScreen()
    {
        if (fadeCanvasGroup == null)
            yield break;

        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeOutDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;
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
}

