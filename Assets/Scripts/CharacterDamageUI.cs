using TMPro;
using UnityEngine;

public class CharacterDamageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComp;
    [SerializeField] private string placeholder = "{0:0.0}%";
    [SerializeField] private Gradient colorOverDamage;
    [SerializeField] private float maxGradientDamage = 999;

    public void OnDamageChanged(float damage)
    {
        textComp.SetText(placeholder, damage);
        textComp.color = colorOverDamage.Evaluate(damage / maxGradientDamage);
    }
}