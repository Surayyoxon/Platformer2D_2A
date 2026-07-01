using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Agar triggerga tekkan ob'ekt Player bo'lsa
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Playerning joriy barcha jonini olib tashlaymiz, shunda o'lim tizimi ishlaydi
                playerHealth.TakeDamage(playerHealth.currentHealth);
            }
        }
    }
}