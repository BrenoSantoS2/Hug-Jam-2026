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
            interactable.onInteractionComplete.AddListener(OnFirstInteraction);
            interactable.onSubsequentInteraction.AddListener(OnSubsequentInteraction);
        }

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnFirstInteraction()
    {
        if (hasFood && !wasSearched)
        {
            wasSearched = true;
            GameManager.Instance.AddFood();
            DialogueSystem.Instance.ShowDialogue(DialogueType.TrashFoundFood);

            if (spriteRenderer != null)
                spriteRenderer.color = searchedColor;
        }
        else if (!hasFood && !wasSearched)
        {
            wasSearched = true;
            DialogueSystem.Instance.ShowDialogue(DialogueType.TrashEmpty);

            if (spriteRenderer != null)
                spriteRenderer.color = searchedColor;
        }
    }

    public void OnSubsequentInteraction()
    {
        DialogueSystem.Instance.ShowDialogue(DialogueType.TrashEmpty);
    }
}
