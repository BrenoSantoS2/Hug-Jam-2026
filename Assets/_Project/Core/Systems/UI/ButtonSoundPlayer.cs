using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundPlayer : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlayButtonSound);
        }
    }

    private void PlayButtonSound()
    {
        if (SoundManager.Instance != null && SoundManager.Instance.somBotao != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.somBotao, 0.8f);
        }
    }
}
