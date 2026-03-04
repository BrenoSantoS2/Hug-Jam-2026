using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LevelStarter : MonoBehaviour
{
    [Header("Visual")]
    public CanvasGroup fadeCanvasGroup;
    
    [Header("Flashback (Opcional)")]
    public bool hasFlashback = false;
    public GameObject flashbackPrefab;
    
    [Header("Timing")]
    public float fadeOutDuration = 2f;

    // Armazena quais flashbacks já foram vistos nesta sessão
    private static HashSet<string> flashbacksViewed = new HashSet<string>();

    private void Start()
    {
        if (fadeCanvasGroup == null)
            fadeCanvasGroup = FindFirstObjectByType<CanvasGroup>();

        StartCoroutine(StartLevelSequence());
    }

    /// <summary>
    /// Chame este método quando voltar do menu principal para resetar os flashbacks
    /// </summary>
    public static void ResetFlashbacksFromMenu()
    {
        flashbacksViewed.Clear();
    }

    private IEnumerator StartLevelSequence()
    {
        Time.timeScale = 0f;

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 1f;

        // Identifica a cena atual
        string currentScene = SceneManager.GetActiveScene().name;
        
        // Verifica se deve tocar o flashback
        bool shouldPlayFlashback = hasFlashback && 
                                   flashbackPrefab != null && 
                                   !flashbacksViewed.Contains(currentScene);

        if (shouldPlayFlashback)
        {
            // Marca como visto
            flashbacksViewed.Add(currentScene);
            
            GameObject flashbackInstance = Instantiate(flashbackPrefab);
            Canvas flashbackCanvas = flashbackInstance.GetComponentInChildren<Canvas>();
            if (flashbackCanvas != null)
                flashbackCanvas.sortingOrder = 10000;
            
            yield return new WaitUntil(() => flashbackInstance == null || !flashbackInstance.activeInHierarchy);
        }

        yield return FadeOut();
        
        Time.timeScale = 1f;
    }

    private IEnumerator FadeOut()
    {
        if (fadeCanvasGroup == null)
            yield break;

        float elapsed = 0f;
        float realStartTime = Time.realtimeSinceStartup;
        
        while (elapsed < fadeOutDuration)
        {
            elapsed = Time.realtimeSinceStartup - realStartTime;
            fadeCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeOutDuration);
            yield return null;
        }
        
        fadeCanvasGroup.alpha = 0f;
    }
}
