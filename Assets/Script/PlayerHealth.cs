using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;                  

    [Header("Damage Effect")]
    [SerializeField] private GameObject damageEffectPrefab; 
    [SerializeField] private Transform effectPosition; 

    private Animator animator;
    private bool isDead = false;

    private PlayerUIManager uiManager;  

    void Start()
    {
        currentHealth = maxHealth; 
        animator = GetComponent<Animator>();

        uiManager = FindObjectOfType<PlayerUIManager>();
        if (uiManager != null)
        {
            uiManager.TakeDamage(0);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; 

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log($"플레이어가 {damage} 데미지를 입었습니다! 현재 HP: {currentHealth}");

        ShowDamageEffect();

        if (uiManager != null)
        {
            uiManager.TakeDamage(damage);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("플레이어 사망!");

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        if (uiManager != null)
        {
            uiManager.TakeDamage(maxHealth); 
        }

        GetComponent<PlayerController>().enabled = false;
    }

    private void ShowDamageEffect()
    {
        if (damageEffectPrefab != null && effectPosition != null)
        {
            GameObject effect = Instantiate(damageEffectPrefab, effectPosition.position, Quaternion.identity);
            Destroy(effect, 1.0f); 
        }
    }
}
