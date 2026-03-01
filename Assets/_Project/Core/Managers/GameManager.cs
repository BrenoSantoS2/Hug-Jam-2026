using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Recursos")]
    public int foodCollected = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddFood(int amount = 1)
    {
        foodCollected += amount;
        Debug.Log($"Comida coletada: {foodCollected}");
    }
}