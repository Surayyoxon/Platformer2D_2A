using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;

    [Header("iFrames (Zarardan keyingi daxlsizlik)")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;
    private bool invulnerable;

    [Header("Components (O'lganda o'chadigan skriptlar)")]
    [SerializeField] private Behaviour[] components;

    [Header("Audio")]
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
        // Agar dushman daxlsiz bo'lsa yoki allaqachon o'lgan bo'lsa, kodni to'xtatamiz
        if (invulnerable || dead) return;

        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            // Tirik bo'lsa - og'riq animatsiyasi va daxlsizlik korutini
            if (anim != null) anim.SetTrigger("hurt");

            StartCoroutine(Invulnerability());

            if (SoundManager.instance != null && hurtSound != null)
                SoundManager.instance.PlaySound(hurtSound);
        }
        else
        {
            // O'lgan bo'lsa - O'lim funksiyasini chaqiramiz
            Die();
        }
    }

    private void Die()
    {
        dead = true;

        // Dushmanning harakatlanish yoki hujum qilish skriptlarini o'chiramiz
        foreach (Behaviour component in components)
        {
            if (component != null)
                component.enabled = false;
        }

        // O'lim animatsiyasini yoqamiz
        if (anim != null)
        {
            anim.SetBool("grounded", true);
            anim.SetTrigger("die");
        }

        // O'lim ovozi
        if (SoundManager.instance != null && deathSound != null)
            SoundManager.instance.PlaySound(deathSound);

        // Dushman o'lim animatsiyasi tugashi uchun 1.5 soniya kutib, keyin butunlay yo'qoladi
        Invoke("Deactivate", 1.5f);
    }

    private IEnumerator Invulnerability()
    {
        invulnerable = true;

        // 10 va 11-layerlar dushman va player bo'lsa, ular bir-biriga tegib ketmasligi uchun
        Physics2D.IgnoreLayerCollision(10, 11, true);

        for (int i = 0; i < numberOfFlashes; i++)
        {
            if (spriteRend != null) spriteRend.color = new Color(1, 0, 0, 0.5f); // Qizil flesh
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));

            if (spriteRend != null) spriteRend.color = Color.white; // Oddiy holat
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }

        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}