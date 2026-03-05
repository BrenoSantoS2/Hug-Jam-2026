using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Configura��es de Movimento")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Configura��es essenciais via c�digo para evitar erros no Inspector
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (PauseManager.IsGamePaused)
        {
            moveInput = Vector2.zero;
            return;
        }

        // Captura o input (WASD ou Setas)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Normaliza para que o movimento diagonal n�o seja mais r�pido
        if (moveInput.magnitude > 1)
        {
            moveInput.Normalize();
        }
    }

    void FixedUpdate()
    {
        if (PauseManager.IsGamePaused)
            return;

        // Aplica o movimento usando a f�sica para respeitar colis�es
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}