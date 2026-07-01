using UnityEngine;

public class PlayerCollectible : MonoBehaviour
{
    public static PlayerCollectible Instance { get; private set; }

    // Mevalar soni
    public int AppleCount { get; private set; }
    public int BananaCount { get; private set; }
    public int StrawberryCount { get; private set; }

    [Header("Collect Settings")]
    [SerializeField] private float destroyDelay = 0.5f;
    [SerializeField] private AudioClip collectSound;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool collected = false;

        if (other.CompareTag("Apple"))
        {
            AppleCount++;
            collected = true;
        }
        else if (other.CompareTag("Coin"))
        {
            BananaCount++;
            collected = true;
        }
        else if (other.CompareTag("Diamond"))
        {
            StrawberryCount++;
            collected = true;
        }

        if (!collected)
            return;

        // UI ni yangilash
        UpdateUI();

        // Tovush
        if (collectSound != null && SoundManager.instance != null)
        {
            SoundManager.instance.PlaySound(collectSound);
        }

        // Animatsiya
        Animator animator = other.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("collected");
        }

        // Colliderni o'chirish
        Collider2D col = other.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Rigidbody bo'lsa to'xtatish
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        // Obyektni yo'q qilish
        Destroy(other.gameObject, destroyDelay);
    }

    private void UpdateUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateInGameFruits(
                AppleCount,
                BananaCount,
                StrawberryCount
            );
        }
    }

    // Faqat o'yin boshidan qayta boshlanganda chaqiring
    public void ResetCounters()
    {
        AppleCount = 0;
        BananaCount = 0;
        StrawberryCount = 0;

        UpdateUI();
    }
}