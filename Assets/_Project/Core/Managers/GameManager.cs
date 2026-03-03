using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Recursos")]
    public int foodCollected = 0;

    [Header("Progressão")]
    public int currentPhase = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foodCollected = 0;
        Debug.Log($"Cena carregada: {scene.name} - Comida resetada para 0");
    }

    public void AddFood(int amount = 1)
    {
        foodCollected += amount;
        Debug.Log($"Comida coletada: {foodCollected}");
    }
}