using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    [Header("Referências")]
    public TextMeshProUGUI dialogueText;
    public CanvasGroup canvasGroup;

    [Header("Timing")]
    public float displayDuration = 3f;
    public float fadeInDuration = 0.3f;
    public float fadeOutDuration = 0.3f;

    [Header("Banco de Diálogos")]
    [SerializeField] private DialogueBank dialogueBank;

    private Coroutine currentDialogueCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (dialogueText == null)
            dialogueText = GetComponentInChildren<TextMeshProUGUI>();

        if (canvasGroup == null)
            canvasGroup = GetComponentInParent<CanvasGroup>();

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    public void ShowDialogue(DialogueType type)
    {
        if (dialogueBank == null)
        {
            Debug.LogWarning("DialogueBank não configurado!");
            return;
        }

        string dialogueLine = dialogueBank.GetRandomDialogue(type);
        if (string.IsNullOrEmpty(dialogueLine))
        {
            Debug.LogWarning($"Nenhum diálogo encontrado para o tipo: {type}");
            return;
        }

        if (currentDialogueCoroutine != null)
            StopCoroutine(currentDialogueCoroutine);

        currentDialogueCoroutine = StartCoroutine(DisplayDialogueRoutine(dialogueLine));
    }

    private IEnumerator DisplayDialogueRoutine(string text)
    {
        canvasGroup.alpha = 0f;
        dialogueText.text = text;

        yield return FadeIn();
        yield return new WaitForSeconds(displayDuration);
        yield return FadeOut();

        currentDialogueCoroutine = null;
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeOutDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}

public enum DialogueType
{
    TrashFoundFood,
    TrashEmpty,
    HidingEnter,
    HidingExit
}

