using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxHealth = 10f;
    private float currentHealth;

    private bool isDead;
    void Start()
    {
        currentHealth = maxHealth;
    }


    public void TakeDamage(float damage)
    {
        if (!isDead)
        {
            currentHealth -= damage;
            Debug.Log(currentHealth);

        }


        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
    }
    
}
