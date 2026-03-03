using UnityEngine;
using UnityEngine.Rendering.Universal; 
using System.Collections;

public class LightningFlasher : MonoBehaviour
{
    [Header("Configurações de Luz")]
    public Light2D targetLight;
    public float flashIntensity = 5.0f;
    public float flashDuration = 0.1f;
    public float defaultIntensity = 0.1f;

    [Header("Intervalo entre Trovões")]
    public float minTime = 5.0f;
    public float maxTime = 15.0f;

    private void Start()
    {
        if (targetLight != null)
        {
            targetLight.intensity = defaultIntensity;
            
            if (SoundManager.Instance != null && SoundManager.Instance.somChuva != null)
                SoundManager.Instance.PlayLoopSFX(SoundManager.Instance.somChuva, 0.4f);
            
            StartCoroutine(LightningRoutine());
        }
    }

    private IEnumerator LightningRoutine()
    {
        while (true)
        {
            float timeToWait = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(timeToWait);

            StartCoroutine(FlashRoutine());
        }
    }

    private IEnumerator FlashRoutine()
    {
        if (SoundManager.Instance != null && SoundManager.Instance.somTrovao != null)
            SoundManager.Instance.PlaySFX(SoundManager.Instance.somTrovao, 0.8f);
        
        targetLight.intensity = flashIntensity;

        yield return new WaitForSeconds(flashDuration);

        float elapsedTime = 0;
        while (elapsedTime < flashDuration * 2.0f)
        {
            elapsedTime += Time.deltaTime;
            targetLight.intensity = Mathf.Lerp(flashIntensity, defaultIntensity, elapsedTime / (flashDuration * 2.0f));
            yield return null;
        }

        targetLight.intensity = defaultIntensity;
    }
}