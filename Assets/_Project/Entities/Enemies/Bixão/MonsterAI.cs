using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterAI : MonoBehaviour
{
    [Header("Configuracoes de patrulha")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    private Vector3 targetDestination;
    private Vector3 originalWorldScale;

    [Header("Deteccao do jogador")]
    public Transform player;
    public float pulseDistance = 5f;
    public float maxPulseFactor = 0.2f;
    public Color alertColor = Color.red;
    public Color originalColor;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        originalWorldScale = transform.localScale;
        targetDestination = pointA.position;

        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        MovePatrol();
        HandleVisualFeedback();
    }

    void MovePatrol()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetDestination, speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, targetDestination) < 0.1f)
        {
            targetDestination = targetDestination == pointA.position ? pointB.position : pointA.position;
        }
    }

    void HandleVisualFeedback()
    {
        float lookDirection = (targetDestination.x > transform.position.x) ? 1f : -1f;

        float distance = Vector3.Distance(transform.position, player.position);
        float scalePulseFactor = 0f;

        if(distance < pulseDistance)
        {
            float pulseValue = Mathf.PingPong(Time.time * 5f, 1f);
            spriteRenderer.color = Color.Lerp(originalColor, alertColor, pulseValue);
            scalePulseFactor = pulseValue * maxPulseFactor;
        }
        else
        {
            spriteRenderer.color = originalColor;
        }

        float finalScaleX = originalWorldScale.x * lookDirection;
        float finalScaleY = originalWorldScale.y;
        float finalScaleZ = originalWorldScale.z;

        transform.localScale = new Vector3(
            finalScaleX * (1f + scalePulseFactor),
            finalScaleY * (1f + scalePulseFactor),
            finalScaleZ
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("O Monstro te pegou!");
            RestartLevel();
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
