using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    public float maxHealth = 10f;
    private float currentHealth;
    [SerializeField] private Animator bloodAnimator;
    [SerializeField] private AudioSource hurt1, hurt2;

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
            bloodAnimator.SetTrigger("hurt");
            Debug.Log(currentHealth);
        }


        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
    }
    
    public void PlayHurtSFX()
    {
        int _randSFX = Random.Range(0, 2);

        if (_randSFX == 0)
            hurt1.Play();
        else
            hurt2.Play();
    }
}
