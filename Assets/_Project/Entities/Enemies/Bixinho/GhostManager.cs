using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    [HideInInspector] public TimeBarManager hungerSystem;

    public GameObject ghostPrefab;
    public int maxGhosts = 15;

    [Header("Spawn Points")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Range(0f, 1f)]
    public float startSpawningAt = 0.8f;

    private List<GameObject> activeGhosts = new List<GameObject>();

    void Start()
    {
        hungerSystem = FindFirstObjectByType<TimeBarManager>();

        if (hungerSystem == null)
        {
            Debug.LogError("GhostManager n�o encontrou o TimeBarManager na cena!");
        }
    }

    void Update()
    {
        if (hungerSystem == null || Time.frameCount < 5) return;

        if (hungerSystem.maxTime <= 0) return;

        float hungerPct = hungerSystem.currentTime / hungerSystem.maxTime;

        int targetGhostCount = 0;

        if (hungerPct < startSpawningAt)
        {
            float t = Mathf.InverseLerp(startSpawningAt, 0f, hungerPct);
            targetGhostCount = Mathf.RoundToInt(t * maxGhosts);
        }

        if (activeGhosts.Count < targetGhostCount)
        {
            SpawnGhost();
        }
        else if (activeGhosts.Count > targetGhostCount)
        {
            RemoveGhost();
        }
    }

    void SpawnGhost()
    {
        Vector3 spawnPos;
        if (spawnPoints != null && spawnPoints.Count > 0)
        {
            int idx = Random.Range(0, spawnPoints.Count);
            spawnPos = spawnPoints[idx].position;
        }
        else
        {
            Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            spawnPos = playerTransform.position + (Vector3)(Random.insideUnitCircle * 8f);
        }
        spawnPos.z = 0;

        GameObject g = Instantiate(ghostPrefab, spawnPos, Quaternion.identity);
        activeGhosts.Add(g);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(SoundManager.Instance.somEsconder, 0.3f);
    }

    void RemoveGhost()
    {
        if (activeGhosts.Count > 0)
        {
            GameObject g = activeGhosts[activeGhosts.Count - 1];
            activeGhosts.RemoveAt(activeGhosts.Count - 1);
            Destroy(g);
        }
    }
}