using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BeatEmUpController : MonoBehaviour
{
    [Header("Movimentação de Base")]
    public float moveSpeed = 5f;
    public float depthMultiplier = 0.6f;

    [Header("Configurações do Pulo")]
    public Transform visualTransform;
    public float jumpForce = 10f;
    public float gravityValue = -25f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private bool isJumping = false;
    private float verticalVelocity = 0;
    private float lastDirectionX = 1f;
    private InteractableItem currentItem;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        anim = GetComponent<Animator>();
        spriteRenderer = visualTransform.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float speed = moveInput.magnitude; 
        anim.SetFloat("Speed", speed);

        if (moveInput.x > 0.1f)
        {
            lastDirectionX = 1f;
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < -0.1f)
        {
            lastDirectionX = -1f;
            spriteRenderer.flipX = true;
        }

        anim.SetBool("isJumping", isJumping);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && !isJumping)
        {
            StartCoroutine(JumpRoutine());
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && currentItem != null)
        {
            currentItem.StartInteracting();
        }
        
        if (context.canceled && currentItem != null)
        {
            currentItem.CancelInteraction();
        }
    }

    void FixedUpdate()
    {
        Vector2 velocity = new Vector2(
            moveInput.x * moveSpeed,
            moveInput.y * (moveSpeed * depthMultiplier)
        );

        rb.linearVelocity = velocity;
    }

    public void SetCurrentItem(InteractableItem item)
    {
        currentItem = item;
    }

    private IEnumerator JumpRoutine()
    {
        isJumping = true;
        verticalVelocity = jumpForce;
        float startY = visualTransform.localPosition.y;
        float currentY = startY;

        while (currentY > startY || verticalVelocity > 0)
        {
            verticalVelocity += gravityValue * Time.deltaTime;
            
            currentY += verticalVelocity * Time.deltaTime;

            if (currentY < startY)
            {
                currentY = startY;
            }

            Vector3 newPos = visualTransform.localPosition;
            newPos.y = currentY;
            visualTransform.localPosition = newPos;

            yield return null;
        }

        Vector3 finalPos = visualTransform.localPosition;
        finalPos.y = startY;
        visualTransform.localPosition = finalPos;

        isJumping = false;
        verticalVelocity = 0;
    }
}