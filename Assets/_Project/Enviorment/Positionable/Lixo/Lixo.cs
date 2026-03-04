using UnityEngine;

[RequireComponent(typeof(InteractableItem))]
public class Lixo : MonoBehaviour
{
    [Header("Conteúdo")]
    public bool hasFood = false;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Color searchedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private InteractableItem interactable;
    private bool wasSearched = false;

    void Awake()
    {
        interactable = GetComponent<InteractableItem>();
        if (interactable != null)
        {
            interactable.reusable = false;
            interactable.onInteractionStart.AddListener(OnInteractionStart);
            interactable.onInteractionCanceled.AddListener(OnInteractionCanceled);
            interactable.onInteractionComplete.AddListener(OnFirstInteraction);
            interactable.onSubsequentInteraction.AddListener(OnSubsequentInteraction);
        }

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        // Se needs setup, faz aqui
    }

    public void OnFirstInteraction()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.StopInteractionLoopSFX();

        if (hasFood && !wasSearched)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.somLixo);
            
            wasSearched = true;
            GameManager.Instance.AddFood();
            GameManager.Instance.IncrementExploredCount();
            
            SoundManager.Instance.PlaySFX(SoundManager.Instance.somComidaEncontrada);
            DialogueSystem.Instance.ShowDialogue(DialogueType.TrashFoundFood);

            if (spriteRenderer != null)
                spriteRenderer.color = searchedColor;
        }
        else if (!hasFood && !wasSearched)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.somLixo);
            
            wasSearched = true;
            GameManager.Instance.IncrementExploredCount();
            
            DialogueSystem.Instance.ShowDialogue(DialogueType.TrashEmpty);

            if (spriteRenderer != null)
                spriteRenderer.color = searchedColor;
        }
    }

    public void OnSubsequentInteraction()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.StopInteractionLoopSFX();

        DialogueSystem.Instance.ShowDialogue(DialogueType.TrashEmpty);
    }

    private void OnInteractionStart()
    {
        if (wasSearched)
            return;

        if (SoundManager.Instance != null && SoundManager.Instance.somRevirandoLixo != null)
            SoundManager.Instance.PlayInteractionLoopSFX(SoundManager.Instance.somRevirandoLixo, 0.55f);
    }

    private void OnInteractionCanceled()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.StopInteractionLoopSFX();
    }
}
