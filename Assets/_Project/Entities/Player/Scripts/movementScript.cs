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
    private Vector2 moveInput;
    private bool isJumping = false;
    private float verticalVelocity = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    void Update()
    {
        Debug.Log($"Move Input: {moveInput}, Vertical Velocity: {verticalVelocity}, Is Jumping: {isJumping}");
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

    void FixedUpdate()
    {
        Vector2 velocity = new Vector2(
            moveInput.x * moveSpeed,
            moveInput.y * (moveSpeed * depthMultiplier)
        );

        rb.linearVelocity = velocity;
    }

    private IEnumerator JumpRoutine()
    {
        isJumping = true;
        verticalVelocity = jumpForce;
        float currentY = 0;

        while (currentY > 0 || verticalVelocity > 0)
        {
            verticalVelocity += gravityValue * Time.deltaTime;
            
            currentY += verticalVelocity * Time.deltaTime;

            if (currentY < 0)
            {
                currentY = 0;
            }

            visualTransform.localPosition = new Vector3(0, currentY, 0);

            yield return null;
        }

        isJumping = false;
        verticalVelocity = 0;
    }
}