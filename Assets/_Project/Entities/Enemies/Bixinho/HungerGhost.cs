using UnityEngine;
using System.Collections;

public class HungerGhost : MonoBehaviour
{
    private Transform player;
    private BeatEmUpController playerController;
    public float followSpeed = 2f;
    public float stoppingDistance = 1.5f;
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1f;
    [Header("Orbit")]
    public float orbitSpeed = 1f;
    public float orbitRadius = 2f;
    private float orbitAngle;
    private Animator anim;
    private Rigidbody2D rb;

    private Vector3 randomOffset;
    private SpriteRenderer sr;
    private Vector3 prevPosition;
    private float originalAlpha;
    private bool hasBeenSeen = false;
    
    [Header("Sons")]
    public float runSoundInterval = 0.3f;
    private float lastRunSoundTime = 0f;

    private void Start()
    {
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null)
        {
            player = pObj.transform;
            playerController = pObj.GetComponent<BeatEmUpController>();
        }
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        float speedVariation = Random.Range(0.85f, 1.15f);

        followSpeed *= speedVariation;
        orbitSpeed *= speedVariation;

        orbitAngle = Random.Range(0f, Mathf.PI * 2f);
        randomOffset = Vector3.zero;

        prevPosition = transform.position;

        originalAlpha = sr != null ? sr.color.a : 1f;
        if (sr != null)
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
        StartCoroutine(FadeInSprite());
    }


    private IEnumerator FadeInSprite()
    {
        if (sr == null) yield break;
        float elapsed = 0f;
        float duration = 0.5f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(0f, originalAlpha, elapsed / duration);
            Color c = sr.color;
            c.a = a;
            sr.color = c;
            yield return null;
        }
        Color cc = sr.color;
        cc.a = originalAlpha;
        sr.color = cc;
        
        if (!hasBeenSeen && DialogueSystem.Instance != null)
        {
            DialogueSystem.Instance.ShowDialogue(DialogueType.SawGhostFirst);
            hasBeenSeen = true;
        }
    }

    private void Update()
    {
        float speed = (transform.position - prevPosition).magnitude / Time.deltaTime;
        prevPosition = transform.position;
        anim.SetFloat("Speed", speed);

        if (player == null) return;

        if (playerController != null && !playerController.enabled)
            return;

        orbitAngle += orbitSpeed * Time.deltaTime;
        Vector3 offset = new Vector3(Mathf.Cos(orbitAngle), Mathf.Sin(orbitAngle), 0) * orbitRadius;
        Vector3 targetPos = player.position + offset;
        float distance = Vector3.Distance(transform.position, targetPos);

        Vector3 moveDirection = (targetPos - transform.position).normalized;

        if (distance > stoppingDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, followSpeed * Time.deltaTime);
            
            // Som de corrida do ghost
            if (Time.time - lastRunSoundTime >= runSoundInterval)
            {
                if (SoundManager.Instance != null && SoundManager.Instance.somGhostCorrendo != null)
                    SoundManager.Instance.PlaySFX(SoundManager.Instance.somGhostCorrendo, 0.3f);
                lastRunSoundTime = Time.time;
            }
        }

        // Virar sprite baseado na direção (sempre atualiza)
        if (sr != null && Mathf.Abs(moveDirection.x) > 0.01f)
            sr.flipX = moveDirection.x > 0;

        float vOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position += new Vector3(0, vOffset * Time.deltaTime, 0);


        Color c = sr.color;
        c.a = 0.3f + (Mathf.PingPong(Time.time, 0.4f));
        sr.color = c;
    }
}
