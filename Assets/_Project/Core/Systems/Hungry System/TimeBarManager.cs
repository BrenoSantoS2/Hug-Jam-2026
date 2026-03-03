using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

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

    [Header("Monólogos")]
    public float monologueInterval = 30f;
    private float monologueTimer = 0f;

    private bool triggeredHalf = false;
    private bool triggeredCritical = false;
    private bool triggeredMin = false;
    
    private bool triggered70 = false;
    private bool triggered50 = false;
    private bool triggered30 = false;
    private bool triggered10 = false;
    
    private const float minPercent = 0.1f;

    void Awake()
    {
        currentTime = maxTime;
        timeSlider.maxValue = maxTime;
        timeSlider.value = maxTime;
    }

    void Update()
    {
        monologueTimer += Time.deltaTime;
        if (monologueTimer >= monologueInterval)
        {
            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowDialogue(DialogueType.MonologueRandom);
            monologueTimer = 0f;
        }

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
        if (pct <= 70f && !triggered70)
        {
            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowDialogue(DialogueType.Hunger70);
            triggered70 = true;
        }
        if (pct > 70f) triggered70 = false;

        if (pct <= 50f && !triggered50)
        {
            Debug.Log("Metade do tempo atingida!");
            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowDialogue(DialogueType.Hunger50);
            onHalfTime.Invoke();
            triggered50 = true;
        }
        if (pct > 50f) triggered50 = false;

        if (pct <= 30f && !triggered30)
        {
            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowDialogue(DialogueType.Hunger30);
            triggered30 = true;
        }
        if (pct > 30f) triggered30 = false;

        if (pct <= 20f && !triggeredCritical)
        {
            Debug.Log("Tempo crítico!");
            onCriticalTime.Invoke();
            triggeredCritical = true;
        }
        if (pct > 20f) triggeredCritical = false;

        if (pct <= minPercent * 100f && !triggered10)
        {
            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowDialogue(DialogueType.Hunger10);
            onTimeOut.Invoke();
            triggered10 = true;
        }
    }

    public void AddTime(float amount)
    {
        currentTime = Mathf.Clamp(currentTime + amount, maxTime * minPercent, maxTime);
        
        float pct = (currentTime / maxTime) * 100f;
        if (pct > 70f) triggered70 = false;
        if (pct > 50f) triggered50 = false;
        if (pct > 30f) triggered30 = false;
        if (pct > 20f) triggeredCritical = false;
        if (pct > minPercent * 100f) triggered10 = false;
    }
}