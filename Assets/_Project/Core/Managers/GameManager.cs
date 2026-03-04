using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Recursos")]
    public int foodCollected = 0;

    [Header("Progressão")]
    public int currentPhase = 1;

    // Rastreamento de exploração por contadores
    private int totalLocations = 0;      // Total de Lixo + Porta na cena
    private int exploredCount = 0;       // Quantas foram exploradas

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
        exploredCount = 0;
        
        // Contar total de locais (Lixo + Porta) na cena
        int lixoCount = FindObjectsByType<Lixo>(FindObjectsSortMode.None).Length;
        int portaCount = FindObjectsByType<Porta>(FindObjectsSortMode.None).Length;
        totalLocations = lixoCount + portaCount;
        
        Debug.Log($"Cena carregada: {scene.name} - Total de locais: {totalLocations} ({lixoCount} lixos, {portaCount} portas)");
    }

    public void AddFood(int amount = 1)
    {
        foodCollected += amount;
        Debug.Log($"Comida coletada: {foodCollected}");
    }

    public void IncrementExploredCount()
    {
        exploredCount++;
        Debug.Log($"Local explorado: {exploredCount}/{totalLocations}");
    }

    public bool AreAllItemsExplored()
    {
        if (totalLocations == 0)
            return false; // Sem locais registrados, não pode passar
        return exploredCount >= totalLocations;
    }

    public int GetExploredItemsCount()
    {
        return exploredCount;
    }

    public int GetTotalItemsCount()
    {
        return totalLocations;
    }
}