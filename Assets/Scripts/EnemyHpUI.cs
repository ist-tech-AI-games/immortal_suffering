using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpUI : MonoBehaviour
{
    [SerializeField] private EnemyHitTrigger source;
    [SerializeField] private TextMeshProUGUI textComp;
    [SerializeField] private string placeholder = "{0:0}/{1:0}";
    [SerializeField] private Image slider;
    [SerializeField] private Gradient colorOverHp;
    private float maxHealth = -1f;

    public void OnHealthChanged(float remainingHealth)
    {
        if (maxHealth < 0f) // not initialized yet
            maxHealth = remainingHealth;

        textComp.SetText(placeholder, remainingHealth, maxHealth);
        float progress = Mathf.Clamp01(remainingHealth / maxHealth);
        slider.color = colorOverHp.Evaluate(progress);
        slider.fillAmount = progress;
    }
}
