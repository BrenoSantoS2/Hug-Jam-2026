using UnityEngine;
using TMPro;

public class FoodCounter : MonoBehaviour
{
    [Header("Referências")]
    public TextMeshProUGUI foodText;

    [Header("Formatação")]
    public string displayFormat = "Comidas: {0}";

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        
        if (gameManager == null)
        {
            Debug.LogError("GameManager não encontrado na cena!");
            return;
        }

        if (foodText == null)
        {
            foodText = GetComponent<TextMeshProUGUI>();
            if (foodText == null)
                Debug.LogError("TextMeshProUGUI não encontrado no objeto!");
        }

        UpdateDisplay();
    }

    void Update()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (gameManager != null && foodText != null)
        {
            foodText.text = string.Format(displayFormat, gameManager.foodCollected);
        }
    }
}
