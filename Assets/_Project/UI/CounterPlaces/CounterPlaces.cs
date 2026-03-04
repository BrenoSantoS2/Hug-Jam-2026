using UnityEngine;
using TMPro;

public class CounterPlaces : MonoBehaviour
{
    [Header("Referências")]
    public TextMeshProUGUI counterText;

    [Header("Formatação")]
    public string displayFormat = "Locais: {0}/{1}";
    public string completedMessage = "volte para o beco";

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            Debug.LogError("GameManager não encontrado na cena!");
            return;
        }

        if (counterText == null)
        {
            counterText = GetComponent<TextMeshProUGUI>();
            if (counterText == null)
            {
                Debug.LogError("TextMeshProUGUI não encontrado no objeto!");
                return;
            }
        }

        UpdateDisplay();
    }

    void Update()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (gameManager == null || counterText == null)
            return;

        int explored = gameManager.GetExploredItemsCount();
        int total = gameManager.GetTotalItemsCount();

        if (total > 0 && explored >= total)
        {
            counterText.text = completedMessage;
            return;
        }

        counterText.text = string.Format(displayFormat, explored, total);
    }
}
