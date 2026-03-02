using UnityEngine;

public class HungerGhost : MonoBehaviour
{
    private Transform player;
    public float followSpeed = 2f;
    public float stoppingDistance = 1.5f; // Distância que ele para do player
    public float floatAmplitude = 0.5f;   // O quanto ele balança para cima/baixo
    public float floatFrequency = 1f;

    private Vector3 randomOffset;
    private SpriteRenderer sr;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        sr = GetComponent<SpriteRenderer>();
        randomOffset = new Vector3(Random.Range(-2f, 2f), Random.Range(-1f, 1f), 0);
    }

    private void Update()
    {
        if(player == null) return;

        Vector3 targetPos = player.position + randomOffset;
        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance > stoppingDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, followSpeed * Time.deltaTime);
        }

        float vOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position += new Vector3(0, vOffset * Time.deltaTime, 0);

        sr.flipX = player.position.x < transform.position.x;

        Color c = sr.color;
        c.a = 0.3f + (Mathf.PingPong(Time.time, 0.4f));
        sr.color = c;
    }
}
