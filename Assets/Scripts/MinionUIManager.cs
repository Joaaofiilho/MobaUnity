using UnityEngine;
using UnityEngine.UI;

public class MinionUIManager : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Unit unit;
    
    void Start()
    {
        unit.OnHealthChangedCallback += OnHealthChangedCallback;
    }

    private void LateUpdate()
    {
        healthBar.transform.parent.LookAt(Camera.main.transform);
    }

    private void OnHealthChangedCallback(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = Mathf.Max(currentHealth, 0) / maxHealth;
    }
}