using UnityEngine;
using UnityEngine.UI;

public class MinionUIManager : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Unit unit;
    
    void Start()
    {
        unit.OnHealthChanged += OnHealthChanged;
    }

    private void LateUpdate()
    {
        healthBar.transform.parent.LookAt(Camera.main.transform);
    }

    private void OnHealthChanged(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = Mathf.Max(currentHealth, 0) / maxHealth;
    }
}
