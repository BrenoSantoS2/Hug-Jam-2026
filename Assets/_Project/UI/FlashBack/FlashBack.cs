using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class DialogueFader : MonoBehaviour
{
    [Header("Referências de UI")]
    public TextMeshProUGUI textElement;
    public CanvasGroup textCanvasGroup;
    public CanvasGroup panelCanvasGroup;

    [Header("Configurações de Tempo")]
    public float fadeDuration = 1.0f;
    public float displayDuration = 2.5f;

    [Header("Conteúdo")]
    public string[] dialogueLines;

    private void Start()
    {
        textCanvasGroup.alpha = 0;
        panelCanvasGroup.alpha = 1;
        
        StartCoroutine(PlayDialogueSequence());
    }

    private IEnumerator PlayDialogueSequence()
    {
        int index = 0;

        while (index < dialogueLines.Length)
        {
            textElement.text = dialogueLines[index];

            yield return StartCoroutine(FadeCanvas(textCanvasGroup, 0, 1));

            float timer = 0;
            bool skipped = false;

            // Enquanto o tempo não acabar e o jogador não clicar...
            while (timer < displayDuration && !skipped)
            {
                timer += Time.deltaTime;

                if (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
                {
                    skipped = true;
                }
                yield return null;
            }

            yield return StartCoroutine(FadeCanvas(textCanvasGroup, 1, 0));
            yield return new WaitForSeconds(0.3f);

            index++;
        }

        yield return StartCoroutine(FadeCanvas(panelCanvasGroup, 1, 0));

        gameObject.SetActive(false);
    }

    private IEnumerator FadeCanvas(CanvasGroup cg, float start, float end)
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsedTime / fadeDuration);
            yield return null;
        }

        cg.alpha = end;
    }
}