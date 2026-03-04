using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using TMPro;
using UnityEngine.Serialization;

[RequireComponent(typeof(InteractableItem))]
public class Porta : MonoBehaviour
{
    [Header("Comportamento")]
    public bool hasResponse = true;

    [Header("Referências")]
    public Animator doorAnimator;
    public VideoPlayer videoPlayer;
    public CanvasGroup videoCanvasGroup;
    public CanvasGroup middleTextCanvasGroup;
    public TextMeshProUGUI middleText;

    [Header("Vídeo")]
    [FormerlySerializedAs("videoToPlay")]
    public VideoClip firstVideoClip;
    public VideoClip secondLoopVideoClip;

    [Header("Timing")]
    public float secondVideoLoopDuration = 3f;
    public float videoFadeInDuration = 0.3f;
    public float videoFadeOutDuration = 0.3f;
    public float textFadeInDuration = 0.3f;
    public float textDisplayDuration = 1.2f;
    public float textFadeOutDuration = 0.3f;

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

        ResetAndClearVideoPlayer();

        if (middleTextCanvasGroup != null)
            middleTextCanvasGroup.alpha = 0f;
    }

    void OnEnable()
    {

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
        Time.timeScale = 0f;

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("open");
            yield return new WaitForSecondsRealtime(GetAnimationDuration("open"));
        }

        if (videoCanvasGroup != null)
            yield return FadeInVideo();

        if (videoPlayer != null)
        {
            if (firstVideoClip != null)
            {
                videoPlayer.isLooping = false;
                videoPlayer.clip = firstVideoClip;
                videoPlayer.Play();
                yield return new WaitForSecondsRealtime((float)firstVideoClip.length);
            }

            if (secondLoopVideoClip != null)
            {
                videoPlayer.clip = secondLoopVideoClip;
                videoPlayer.isLooping = true;
                videoPlayer.Play();

                Coroutine textCoroutine = null;
                if (middleTextCanvasGroup != null)
                    textCoroutine = StartCoroutine(PlayMiddleTextSequence());

                yield return new WaitForSecondsRealtime(secondVideoLoopDuration);
                ResetAndClearVideoPlayer();

                if (textCoroutine != null)
                    yield return textCoroutine;
            }
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

            SoundManager.Instance.PlaySFX(SoundManager.Instance.somPortaFechando);
        }

        if (GameManager.Instance != null)
            GameManager.Instance.AddFood();

        if (GameManager.Instance != null)
            GameManager.Instance.IncrementExploredCount();

        if (SoundManager.Instance != null && SoundManager.Instance.somComidaEncontrada != null)
            SoundManager.Instance.PlaySFX(SoundManager.Instance.somComidaEncontrada);

        Time.timeScale = 1f;
        sequenceCoroutine = null;
    }

    private IEnumerator FadeInVideo()
    {
        yield return FadeCanvasRealtime(videoCanvasGroup, 0f, 1f, videoFadeInDuration);
    }

    private IEnumerator FadeOutVideo()
    {
        yield return FadeCanvasRealtime(videoCanvasGroup, 1f, 0f, videoFadeOutDuration);
        ResetAndClearVideoPlayer();
    }

    private IEnumerator PlayMiddleTextSequence()
    {
        if (middleTextCanvasGroup == null)
            yield break;

        middleTextCanvasGroup.alpha = 0f;

        if (middleText != null)
            middleText.enabled = true;

        yield return FadeCanvasRealtime(middleTextCanvasGroup, 0f, 1f, textFadeInDuration);
        yield return new WaitForSecondsRealtime(textDisplayDuration);
        yield return FadeCanvasRealtime(middleTextCanvasGroup, 1f, 0f, textFadeOutDuration);

        if (middleText != null)
            middleText.enabled = false;
    }

    private IEnumerator FadeCanvasRealtime(CanvasGroup canvasGroup, float start, float end, float duration)
    {
        if (canvasGroup == null)
            yield break;

        if (duration <= 0f)
        {
            canvasGroup.alpha = end;
            yield break;
        }

        float elapsed = 0f;
        float realStartTime = Time.realtimeSinceStartup;
        while (elapsed < duration)
        {
            elapsed = Time.realtimeSinceStartup - realStartTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = end;
    }

    private float GetAnimationDuration(string triggerName)
    {
        if (doorAnimator == null)
            return 0f;

        AnimatorStateInfo stateInfo = doorAnimator.GetCurrentAnimatorStateInfo(0);
        return 1f;
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

