using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;

    [Header("Components")]
    [SerializeField] private Behaviour[] components;
    private bool invulnerable;

    [Header("Death Sound")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float _damage)
    {
        if (invulnerable) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            StartCoroutine(Invunerability());
            SoundManager.instance.PlaySound(hurtSound);
        }
        else
        {
            if (!dead)
            {
                // Deactivate all attached component classes
                foreach (Behaviour component in components)
                    component.enabled = false;

                anim.SetBool("grounded", true);
                anim.SetTrigger("die");

                dead = true;
                SoundManager.instance.PlaySound(deathSound);

                // --- UI BILAN BOG'LASH VA DUSHMANLARNI O'CHIRISH ---
                if (gameObject.CompareTag("Player"))
                {
                    // Agar o'lgan obyekt Player bo'lsa, Game Over panelini ochamiz
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.OnGameOver();
                    }
                    else
                    {
                        Debug.LogError("Sahnada UIManager topilmadi!");
                    }
                }
                else
                {
                    // Agar bu ob'ekt Player bo'lmasa (ya'ni dushman bo'lsa), uni o'chiramiz
                    // Dushman o'lim animatsiyasi tugashi uchun 1-2 soniya kutib, keyin o'chadi
                    Invoke("Deactivate", 1.5f);
                }
            }
        }
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    private IEnumerator Invunerability()
    {
        invulnerable = true;
        Physics2D.IgnoreLayerCollision(10, 11, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    // Respawn
    public void Respawn()
    {
        dead = false;

        // --- YANGI QO'SHILGAN QISM ---
        // Korutin chala qolib ketgan bo'lsa, uni to'xtatamiz va fizika to'qnashuvini majburan tiklaymiz
        StopAllCoroutines();
        invulnerable = false;
        Physics2D.IgnoreLayerCollision(10, 11, false); // Layerlarni qayta yoqamiz
        spriteRend.color = Color.white; // Rangni joyiga qaytaramiz
                                        // -----------------------------

        AddHealth(startingHealth);
        anim.ResetTrigger("die");
        anim.Play("Idle");
        StartCoroutine(Invunerability());

        // Activate all attached component classes
        foreach (Behaviour component in components)
            component.enabled = true;
    }
}