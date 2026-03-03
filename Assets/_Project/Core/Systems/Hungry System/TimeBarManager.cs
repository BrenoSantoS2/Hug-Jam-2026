using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TimeBarManager : MonoBehaviour
{
    [Header("Configurações de UI")]
    public Slider timeSlider; 
    
    [Header("Configurações de Tempo")]
    public float maxTime = 60f;
    public float currentTime;
    
    [Header("Eventos por Porcentagem")]
    public UnityEvent onHalfTime;
    public UnityEvent onCriticalTime;
    public UnityEvent onTimeOut;

    private bool triggeredHalf = false;
    private bool triggeredCritical = false;
    private bool triggeredMin = false;
    
    private const float minPercent = 0.1f;

    void Awake()
    {
        currentTime = maxTime;
        timeSlider.maxValue = maxTime;
        timeSlider.value = maxTime;
    }

    void Update()
    {
        if (currentTime > maxTime * minPercent)
        {
            currentTime -= Time.deltaTime;
            if (currentTime < maxTime * minPercent)
                currentTime = maxTime * minPercent;

            timeSlider.value = currentTime;

            float percentage = (currentTime / maxTime) * 100f;

            CheckThresholds(percentage);
        }
    }

    private void CheckThresholds(float pct)
    {
        if (pct <= 50f && !triggeredHalf)
        {
            Debug.Log("Metade do tempo atingida!");
            onHalfTime.Invoke();
            triggeredHalf = true;
        }

        if (pct <= 20f && !triggeredCritical)
        {
            Debug.Log("Tempo crítico!");
            onCriticalTime.Invoke();
            triggeredCritical = true;
        }

        if (pct <= minPercent * 100f && !triggeredMin)
        {
            onTimeOut.Invoke();
            triggeredMin = true;
        }
    }

    public void AddTime(float amount)
    {
        currentTime = Mathf.Clamp(currentTime + amount, maxTime * minPercent, maxTime);
        
        float pct = (currentTime / maxTime) * 100f;
        if (pct > 50f) triggeredHalf = false;
        if (pct > 20f) triggeredCritical = false;
        if (pct > minPercent * 100f) triggeredMin = false;
    }
}