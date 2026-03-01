using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterAI : MonoBehaviour
{
    private enum MonsterState { Patrolling, Chasing }
    [SerializeField] private MonsterState currentState = MonsterState.Patrolling;

    [Header("Configuracoes de patrulha")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    private Vector3 targetDestination;
    private Vector3 originalWorldScale;

    [Header("Configurações de Perseguição")]
    public Transform player;
    public float chaseSpeed = 3.5f;
    public float visionRange = 5f;
    public float stopChasingRange = 7f;

    [Header("Deteccao do jogador")]
    public float pulseDistance = 5f;
    public float maxPulseFactor = 0.2f;
    public Color alertColor = Color.red;
    public Color originalColor;
    private float lookDirection = 1f;

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
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch(currentState)
        {
            case MonsterState.Patrolling:
                MovePatrol();
                if (distanceToPlayer < visionRange)
                {
                    currentState = MonsterState.Chasing;
                }
                break;

            case MonsterState.Chasing:
                MoveChase();
                if (distanceToPlayer > stopChasingRange)
                {
                    currentState = MonsterState.Patrolling;
                }
                break;
        }
        HandleVisualFeedback(10);
    }

    void MovePatrol()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetDestination, speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, targetDestination) < 0.1f)
        {
            targetDestination = targetDestination == pointA.position ? pointB.position : pointA.position;
        }
    }

    void MoveChase()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }

    void HandleVisualFeedback(float distance)
    {
        Vector3 moveTarget = (currentState == MonsterState.Chasing) ? player.position : targetDestination;

        float xDiff = moveTarget.x - transform.position.x;
        if (Mathf.Abs(xDiff) > 0.01f)
        {
            lookDirection = (xDiff > 0) ? 1f : -1f;
        }

        // Lógica de Pulsação
        float pulseValue = 0f;
        if (distance < visionRange || currentState == MonsterState.Chasing)
        {
            pulseValue = Mathf.PingPong(Time.time * 5f, 1f);
            spriteRenderer.color = Color.Lerp(originalColor, alertColor, pulseValue);
        }
        else
        {
            spriteRenderer.color = originalColor;
        }

        float scaleFactor = 1f + (pulseValue * maxPulseFactor);

        transform.localScale = new Vector3(
            originalWorldScale.x * lookDirection * scaleFactor,
            originalWorldScale.y * scaleFactor,
            originalWorldScale.z
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


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopChasingRange);
    }
}
