using UnityEngine;
using System.Collections;

public class InteractableItem : MonoBehaviour
{
    [Header("Configurações")]
    public float interactionTime = 2.0f;
    public float originalAnimDuration = 1.0f;

    [Header("Referências")]
    public Animator loadingBallAnim;
    public GameObject loadingObject;

    private bool isPlayerNearby = false;
    private bool isInteracting = false;
    private Coroutine interactionCoroutine;
    private Animator playerAnim;
    private Animator itemAnim;
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
        }
    }

    public void StartInteracting()
    {
        if (isPlayerNearby && !isInteracting)
        {
            interactionCoroutine = StartCoroutine(InteractionRoutine());
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
                playerAnim.SetBool("isVasculhando", false);
                itemAnim.SetBool("isVasculhando", false);
            }
        }
    }

    private IEnumerator InteractionRoutine()
    {
        isInteracting = true;
        loadingObject.SetActive(true);

        loadingBallAnim.speed = originalAnimDuration / interactionTime;
        loadingBallAnim.Play("Loading", -1, 0f);

        if(playerAnim != null)
        {
            playerAnim.SetBool("isVasculhando", true);
            itemAnim.SetBool("isVasculhando", true);
        }

        yield return new WaitForSeconds(interactionTime);

        isInteracting = false;
        loadingObject.SetActive(false);
        
        if(playerAnim != null) 
            playerAnim.SetBool("isVasculhando", false);
            itemAnim.SetBool("isVasculhando", false);

        ConseguirAlgo();
    }

    private void ConseguirAlgo()
    {
        Debug.Log("Interação concluída com sucesso!");
    }
}