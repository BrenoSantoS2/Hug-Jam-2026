using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class esconderijos : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite emptySprite;
    public Sprite occupiedSprite;

    [Header("Player")]
    public Vector3 exitOffset = Vector3.zero;

    private SpriteRenderer sr;
    private bool isPlayerInside = false;
    private GameObject player;
    private SpriteRenderer[] playerRenderers;
    private Behaviour playerController;
    private Rigidbody2D playerRigidbody;
    private Coroutine hideLoopCoroutine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = gameObject.AddComponent<SpriteRenderer>();

        if (emptySprite != null)
            sr.sprite = emptySprite;
    }

    public void OnInteractionComplete()
    {
        ToggleHide();
    }

    private void ToggleHide()
    {
        if (isPlayerInside)
            Exit();
        else
            Enter();
    }

    private void Enter()
    {
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null)
                playerRigidbody.linearVelocity = Vector2.zero;

            player.transform.position = transform.position + exitOffset;

            playerRenderers = player.GetComponentsInChildren<SpriteRenderer>();
            foreach (var r in playerRenderers)
                r.enabled = false;

            playerController = player.GetComponent<BeatEmUpController>();
            if (playerController != null)
                playerController.enabled = false;
        }

        isPlayerInside = true;
        if (occupiedSprite != null)
            sr.sprite = occupiedSprite;

        if (DialogueSystem.Instance != null)
            DialogueSystem.Instance.ShowDialogue(DialogueType.HidingEnter);

        SoundManager.Instance.PlaySFX(SoundManager.Instance.somEsconder);
        
        if (hideLoopCoroutine != null)
            StopCoroutine(hideLoopCoroutine);
        hideLoopCoroutine = StartCoroutine(StartHideLoopAfterDelay());
    }

    private IEnumerator StartHideLoopAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);
        if (isPlayerInside && SoundManager.Instance != null && SoundManager.Instance.somEstáEscondido != null)
        {
            SoundManager.Instance.PlayLoopSFX(SoundManager.Instance.somEstáEscondido, 0.5f);
        }
    }

    private void Exit()
    {
        if (player != null)
        {
            if (playerRenderers != null)
            {
                foreach (var r in playerRenderers)
                    r.enabled = true;
            }
            if (playerController != null)
                playerController.enabled = true;

            player.transform.position = transform.position + exitOffset;
        }

        isPlayerInside = false;
        if (emptySprite != null)
            sr.sprite = emptySprite;

        if (DialogueSystem.Instance != null)
            DialogueSystem.Instance.ShowDialogue(DialogueType.HidingExit);
        
        if (hideLoopCoroutine != null)
            StopCoroutine(hideLoopCoroutine);
        SoundManager.Instance.StopLoopSFX();
    }
}
