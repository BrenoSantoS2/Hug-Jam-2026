using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class InteractableItem : MonoBehaviour
{
    [Header("Configurações")]
    public float interactionTime = 2.0f;
    public float originalAnimDuration = 1.0f;

    [Header("Referências")]
    public Animator loadingBallAnim;
    public GameObject loadingObject;
    
    [Header("UI de Interação")]
    public GameObject keyPromptPrefab;
    private GameObject keyPromptInstance;

    [Header("Eventos")]
    public UnityEvent onInteractionComplete;
    public UnityEvent onSubsequentInteraction;

    [Header("Comportamento")]
    public bool reusable = true;

    private bool isPlayerNearby = false;
    private bool isInteracting = false;
    private Coroutine interactionCoroutine;
    private Animator playerAnim;
    private Animator itemAnim;

    private int interactionCount = 0;

    private void Awake()
    {
        itemAnim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            
            BeatEmUpController player = other.GetComponent<BeatEmUpController>();
            
            if (player != null)
            {
                player.SetCurrentItem(this);
                playerAnim = other.GetComponent<Animator>();
            }

            if (keyPromptPrefab != null && keyPromptInstance == null)
            {
                keyPromptInstance = Instantiate(keyPromptPrefab, transform);
            }
            ToggleKeyPrompt(true);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            BeatEmUpController player = other.GetComponent<BeatEmUpController>();
            if (player != null)
                player.SetCurrentItem(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            
            BeatEmUpController player = other.GetComponent<BeatEmUpController>();
            if (player != null)
            {
                player.SetCurrentItem(null);
            }
            
            CancelInteraction();
            ToggleKeyPrompt(false);
        }
    }

    public void StartInteracting()
    {
        if (!isInteracting)
        {
            interactionCoroutine = StartCoroutine(InteractionRoutine());
            ToggleKeyPrompt(false);
        }
    }
    public void CancelInteraction()
    {
        if (isInteracting)
        {
            StopCoroutine(interactionCoroutine);
            isInteracting = false;
            loadingObject.SetActive(false);
            
            loadingBallAnim.Play("Loading", -1, 0f); 
            loadingBallAnim.speed = 0;

            if(playerAnim != null)
            {
                playerAnim.SetBool("isInteracting", false);
                itemAnim.SetBool("isInteracting", false);
            }
        }
        ToggleKeyPrompt(false);
    }

    private void ToggleKeyPrompt(bool show)
    {
        if (keyPromptInstance == null)
            return;
        keyPromptInstance.SetActive(show);
    }

    private IEnumerator InteractionRoutine()
    {
        isInteracting = true;
        loadingObject.SetActive(true);
        ToggleKeyPrompt(false);

        loadingBallAnim.speed = originalAnimDuration / interactionTime;
        loadingBallAnim.Play("Loading", -1, 0f);

        if(playerAnim != null)
        {
            playerAnim.SetBool("isInteracting", true);
            itemAnim.SetBool("isInteracting", true);
        }

        yield return new WaitForSeconds(interactionTime);

        isInteracting = false;
        loadingObject.SetActive(false);
        
        if(playerAnim != null) 
            playerAnim.SetBool("isInteracting", false);
            itemAnim.SetBool("isInteracting", false);

        interactionCount++;

        if (isPlayerNearby)
            ToggleKeyPrompt(true);

        if (interactionCount == 1 || reusable)
        {
            if(onInteractionComplete != null)
                onInteractionComplete.Invoke();
        }
        else
        {
            if(onSubsequentInteraction != null)
                onSubsequentInteraction.Invoke();
        }
    }
}