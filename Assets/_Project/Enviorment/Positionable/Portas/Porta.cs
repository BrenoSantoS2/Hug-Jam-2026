using UnityEngine;
using UnityEngine.Video;
using System.Collections;

[RequireComponent(typeof(InteractableItem))]
public class Porta : MonoBehaviour
{
    [Header("Comportamento")]
    public bool hasResponse = true;

    [Header("Referências")]
    public Animator doorAnimator;
    public VideoPlayer videoPlayer;
    public CanvasGroup videoCanvasGroup;

    [Header("Vídeo")]
    public VideoClip videoToPlay;

    [Header("Timing")]
    public float videoFadeInDuration = 0.3f;
    public float videoFadeOutDuration = 0.3f;

    private InteractableItem interactable;
    private bool hasBeenAnswered = false;
    private Coroutine sequenceCoroutine;

    void Awake()
    {
        interactable = GetComponent<InteractableItem>();
        if (interactable != null)
        {
            interactable.reusable = false;
            interactable.onInteractionComplete.AddListener(OnInteraction);
            interactable.onSubsequentInteraction.AddListener(OnNoResponse);
        }

        if (videoCanvasGroup == null)
            videoCanvasGroup = FindFirstObjectByType<CanvasGroup>();

        if (videoCanvasGroup != null)
            videoCanvasGroup.alpha = 0f;
    }

    public void OnInteraction()
    {
        if (hasResponse && !hasBeenAnswered)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.somPortaAbrindo);
            if (sequenceCoroutine != null)
                StopCoroutine(sequenceCoroutine);

            sequenceCoroutine = StartCoroutine(AnswerSequence());
            hasBeenAnswered = true;

            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowDialogue(DialogueType.DoorAnswered);
        }
        else if (!hasResponse && !hasBeenAnswered)
        {
            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowDialogue(DialogueType.DoorNoResponse);
            hasBeenAnswered = true;
        }
    }

    public void OnNoResponse()
    {
        if (DialogueSystem.Instance != null)
            DialogueSystem.Instance.ShowDialogue(DialogueType.DoorNoResponse);
    }

    private IEnumerator AnswerSequence()
    {
        // Congelar tempo
        Time.timeScale = 0f;

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("open");
            yield return new WaitForSecondsRealtime(GetAnimationDuration("open"));
        }

        if (videoCanvasGroup != null)
            yield return FadeInVideo();

        if (videoPlayer != null && videoToPlay != null)
        {
            videoPlayer.clip = videoToPlay;
            videoPlayer.Play();
            yield return new WaitForSecondsRealtime((float)videoToPlay.length);
        }

        if (videoCanvasGroup != null)
            yield return FadeOutVideo();

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("giving");
            yield return new WaitForSecondsRealtime(GetAnimationDuration("giving"));
        }

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("close");
            yield return new WaitForSecondsRealtime(GetAnimationDuration("close"));
        }

        // Descongelar tempo
        Time.timeScale = 1f;
        sequenceCoroutine = null;
    }

    private IEnumerator FadeInVideo()
    {
        float elapsed = 0f;
        float realStartTime = Time.realtimeSinceStartup;
        while (elapsed < videoFadeInDuration)
        {
            elapsed = Time.realtimeSinceStartup - realStartTime;
            videoCanvasGroup.alpha = Mathf.Clamp01(elapsed / videoFadeInDuration);
            yield return null;
        }
        videoCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutVideo()
    {
        float elapsed = 0f;
        float realStartTime = Time.realtimeSinceStartup;
        while (elapsed < videoFadeOutDuration)
        {
            elapsed = Time.realtimeSinceStartup - realStartTime;
            videoCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / videoFadeOutDuration);
            yield return null;
        }
        videoCanvasGroup.alpha = 0f;

        if (videoPlayer != null)
            videoPlayer.Stop();
    }

    private float GetAnimationDuration(string triggerName)
    {
        if (doorAnimator == null)
            return 0f;

        AnimatorStateInfo stateInfo = doorAnimator.GetCurrentAnimatorStateInfo(0);
        return 1f;
    }
}

