using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TimeBarManager : MonoBehaviour
{
    [Header("Configurações de UI")]
    public Slider timeSlider; 
    
    [Header("Configurações de Tempo")]
    public float maxTime = 60f;
    private float currentTime;
    
    [Header("Eventos por Porcentagem")]
    public UnityEvent onHalfTime; // Evento em 50%
    public UnityEvent onCriticalTime; // Evento em 20%
    public UnityEvent onTimeOut; // Evento em 0%

    private bool triggeredHalf = false;
    private bool triggeredCritical = false;

    void Start()
    {
        currentTime = maxTime;
        timeSlider.maxValue = maxTime;
        timeSlider.value = maxTime;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
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

        if (pct <= 0)
        {
            onTimeOut.Invoke();
        }
    }

    public void AddTime(float amount)
    {
        currentTime = Mathf.Clamp(currentTime + amount, 0, maxTime);
        
        if ((currentTime / maxTime) * 100f > 50f) triggeredHalf = false;
        if ((currentTime / maxTime) * 100f > 20f) triggeredCritical = false;
    }
}